using FrontEnd.Models;
using FrontEnd.Models.modelsDataParameterMethods;
using FrontEnd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FrontEnd.Controllers
{
    public class AccesoController : Controller
    {
        private readonly ProyectoVeterinariaContext _context;
        private readonly ICorreoService _correoService;

        public AccesoController(ProyectoVeterinariaContext context, ICorreoService correoService)
        {
            _context = context;
            _correoService = correoService;
        }

        private void VerificarSesion()
        {
            var lastActivity = HttpContext.Session.GetString("LastActivity");
            if (lastActivity != null)
            {
                var lastActivityTime = DateTime.Parse(lastActivity);
                if (lastActivityTime.AddMinutes(5) < DateTime.Now)
                {
                    HttpContext.Session.Clear();
                    TempData["MensajeSesion"] = "Sesión cerrada por inactividad.";
                }
            }
            HttpContext.Session.SetString("LastActivity", DateTime.Now.ToString());
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult RegistroUsuario()
        {
            VerificarSesion();
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult RegistroUsuario(RegistroModeloUsuario modelo)
        {
            VerificarSesion();
            if (ModelState.IsValid)
            {
                if (_context.Personal.Any(u => u.Correo == modelo.Correo))
                {
                    ModelState.AddModelError("", "El correo de usuario ya existe.");
                    return View(modelo);
                }

                var nuevoUsuario = new Personal
                {
                    Nombre = modelo.Nombre,
                    Apellido = modelo.PrimerApellido + " " + modelo.SegundoApellido,
                    Correo = modelo.Correo,
                    Contrasena = modelo.Contrasena,
                    Telefono = modelo.TelefonoPrincipal,
                    IdRol = modelo.IdRol,
                    IdClinica = modelo.IdClinica
                };

                _context.Personal.Add(nuevoUsuario);
                _context.SaveChanges();
                TempData["MensajeRegistroCorrecto"] = "Usuario registrado correctamente, ahora puedes iniciar sesión.";
                return RedirectToAction("InicioSesionUsuario");
            }

            return View(modelo);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult InicioSesionUsuario()
        {
            VerificarSesion();
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult InicioSesionUsuario(LoginModeloUsuario modelo)
        {
            VerificarSesion();
            if (ModelState.IsValid)
            {
                var usuario = _context.Personal.FirstOrDefault(u => u.Correo == modelo.Correo && u.Contrasena == modelo.Contrasena);
                if (usuario != null)
                {
                    HttpContext.Session.SetString("UsuarioId", usuario.IdPersonal.ToString());
                    HttpContext.Session.SetString("UsuarioNombre", usuario.Nombre);
                    HttpContext.Session.SetString("UsuarioRol", usuario.IdRol.ToString());
                    TempData["Mensaje"] = $"Bienvenido {usuario.Nombre}";
                    return RedirectToAction("Index", "Home");
                }

                TempData["MensajeInicioFallido"] = "Correo o contraseña incorrectos.";
                return RedirectToAction("InicioSesionUsuario");
            }

            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult RegistroCliente()
        {
            VerificarSesion();
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult RegistroCliente(RegistroModeloCliente modelo)
        {
            VerificarSesion();
            if (ModelState.IsValid)
            {
                if (_context.Propietarios.Any(c => c.Correo == modelo.Correo))
                {
                    ModelState.AddModelError("", "El correo ya existe.");
                    return View(modelo);
                }

                var nuevoCliente = new Propietario
                {
                    Nombre = modelo.Nombre,
                    Apellido = modelo.PrimerApellido + " " + modelo.SegundoApellido,
                    Correo = modelo.Correo,
                    Contrasena = modelo.Contrasena,
                    Telefono = modelo.TelefonoPrincipal,
                    IdRol = modelo.IdRol
                };

                _context.Propietarios.Add(nuevoCliente);
                _context.SaveChanges();
                TempData["MensajeRegistroCorrecto"] = "Cliente registrado correctamente, ahora puedes iniciar sesión.";
                return RedirectToAction("InicioSesionCliente");
            }

            return View(modelo);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult InicioSesionCliente()
        {
            VerificarSesion();
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> InicioSesionCliente(LoginModeloUsuarioCliente modelo)
        {
            VerificarSesion();
            if (ModelState.IsValid)
            {
                var cliente = _context.Propietarios.FirstOrDefault(u => u.Correo == modelo.Correo && u.Contrasena == modelo.Contrasena);
                if (cliente != null)
                {
                    HttpContext.Session.SetString("ClienteId", cliente.IdPropietario.ToString());
                    HttpContext.Session.SetString("ClienteNombre", cliente.Nombre);
                    HttpContext.Session.SetString("ClienteRol", cliente.IdRol.ToString());
                    TempData["Mensaje"] = $"Bienvenido {cliente.Nombre}";

                    await _correoService.EnviarCorreo(
                        cliente.Correo,
                        "Inicio de sesión exitoso",
                        $"Hola {cliente.Nombre}, has iniciado sesión el {DateTime.Now}."
                    );

                    return RedirectToAction("Index", "Home");
                }

                TempData["MensajeInicioFallido"] = "Correo o contraseña incorrectos.";
                return RedirectToAction("InicioSesionCliente");
            }

            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["MensajeSession"] = "Sesión cerrada correctamente.";
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult EnviarCodigoSeguridad()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> EnviarCodigoSeguridad(CorreoParameter correoUsuario)
        {
            var cliente = _context.Propietarios.FirstOrDefault(u => u.Correo == correoUsuario.Correo);
            if (cliente != null)
            {
                await _correoService.EnviarCorreo(
                    cliente.Correo,
                    "Código de seguridad",
                    $"Hola {cliente.Nombre}, este es tu código de seguridad: {cliente.Contrasena}"
                );
                return RedirectToAction("EnviarContrasenaNueva", cliente);
            }

            TempData["MensajeCorreoUsuarioNoEncontrado"] = "Correo no registrado en el sistema";
            return RedirectToAction("InicioSesionCliente");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult EnviarContrasenaNueva(Propietario cliente)
        {
            return View(cliente);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> EnviarContrasenaAleatoria(Propietario clienteRecibido)
        {
            var cliente = _context.Propietarios.FirstOrDefault(u => u.IdPropietario == clienteRecibido.IdPropietario);
            if (cliente != null && cliente.Contrasena == clienteRecibido.Contrasena)
            {
                await _correoService.EnviarCorreo(
                    cliente.Correo,
                    "Contraseña temporal",
                    $"Hola {cliente.Nombre}, tu nueva contraseña temporal es: {clienteRecibido.Contrasena}"
                );

                cliente.Contrasena = clienteRecibido.Contrasena;
                _context.SaveChanges();

                TempData["MensajeContrasenaTemporalEnviada"] = "Contraseña temporal enviada a su correo.";
                return RedirectToAction("InicioSesionCliente");
            }

            TempData["MensajeCodigoDeSeguridadIncorrecto"] = "El código de seguridad es incorrecto.";
            return RedirectToAction("InicioSesionCliente");
        }
    }
}