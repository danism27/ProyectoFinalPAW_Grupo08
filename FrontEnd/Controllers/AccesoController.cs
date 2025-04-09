using FrontEnd.Models;
using FrontEnd.Models.modelsDataParameterMethods;
using FrontEnd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FrontEnd.Controllers
{
    public class AccesoController : Controller
    {
        private readonly ProyectoVeterinariaContext _context;
        private readonly CorreoController _correoController;

        public AccesoController(ProyectoVeterinariaContext context, CorreoController correoController)
        {
            _context = context;
            _correoController = correoController;
        }

        private void VerificarSesion()
        {
            var lastActivity = HttpContext.Session.GetString("LastActivity");
            if (lastActivity != null)
            {
                var lastActivityTime = DateTime.Parse(lastActivity);
                if (lastActivityTime.AddMinutes(5) < DateTime.Now) // 5 minutos de inactividad
                {
                    HttpContext.Session.Clear();
                    TempData["MensajeSesion"] = "Sesión cerrada por inactividad.";
                }
            }
            HttpContext.Session.SetString("LastActivity", DateTime.Now.ToString()); // Actualizar la última actividad
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
                // Validar que el usuario no exista previamente
                if (_context.Personal.Any(u => u.Correo == modelo.Correo))
                {
                    ModelState.AddModelError("", "El correo de usuario ya existe.");
                    return View(modelo);
                }
                // Crear un nuevo usuario con la información proporcionada
                var nuevoUsuario = new Personal
                {
                    Nombre = modelo.Nombre,
                    Apellido = modelo.PrimerApellido + " " + modelo.SegundoApellido,
                    Correo = modelo.Correo,
                    Contrasenna = modelo.Contrasena,
                    Telefono = modelo.TelefonoPrincipal,
                    IdRol = modelo.IdRol,
                    IdClinica = modelo.IdSucursal
                };
                // Guardar el usuario en la base de datos
                _context.Personal.Add(nuevoUsuario);
                _context.SaveChanges();
                TempData["MensajeRegistroCorrecto"] = "Usuario registrado correctamente, ahora puedes iniciar sesión.";
                return RedirectToAction("InicioSesionUsuario");
            }
            else
            {
                return View(modelo);
            }
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
                // Buscar el usuario en la base de datos
                var usuario = _context.Personal
                    .FirstOrDefault(u => u.Correo == modelo.Correo && u.Contrasenna == modelo.Contrasena);
                if (usuario != null)
                {
                    // Guardar información del usuario en la sesión
                    HttpContext.Session.SetString("UsuarioId", usuario.IdPersonal.ToString()); // Guardar el ID del usuario
                    HttpContext.Session.SetString("UsuarioNombre", usuario.Nombre);
                    HttpContext.Session.SetString("UsuarioRol", usuario.IdRol.ToString()); // Guardar el rol del usuario
                    TempData["Mensaje"] = $"Bienvenido {usuario.Nombre}";
                    // Verificar el rol del usuario
                    // Redirigir a otra vista
                    return RedirectToAction("Index", "Home");
                }
                // Si no coincide usuario o contraseña
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
                // Validar que el cliente no exista previamente
                if (_context.Propietarios.Any(c => c.Correo == modelo.Correo))
                {
                    ModelState.AddModelError("", "El correo ya existe.");
                    return View(modelo);
                }
                // Crear un nuevo cliente con la información proporcionada
                var nuevoCliente = new Propietario
                {
                    Nombre = modelo.Nombre,
                    Apellido = modelo.PrimerApellido + " " + modelo.SegundoApellido,
                    Correo = modelo.Correo,
                    Contrasenna = modelo.Contrasena,
                    Telefono = modelo.TelefonoPrincipal,
                    IdRol = modelo.IdRol
                };
                // Guardar el cliente en la base de datos
                _context.Propietarios.Add(nuevoCliente);
                _context.SaveChanges();
                TempData["MensajeRegistroCorrecto"] = "Cliente registrado correctamente, ahora puedes iniciar sesión.";
                return RedirectToAction("InicioSesionCliente");
            }
            else
            {
                return View(modelo);
            }
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
        public IActionResult InicioSesionCliente(LoginModeloUsuarioCliente modelo)
        {
            VerificarSesion();
            if (ModelState.IsValid)
            {
                // Buscar el cliente en la base de datos
                var cliente = _context.Propietarios
                    .FirstOrDefault(u => u.Correo == modelo.Correo && u.Contrasenna == modelo.Contrasena);
                if (cliente != null)
                {
                    HttpContext.Session.SetString("ClienteId", cliente.IdPropietario.ToString()); // Guardar el ID del cliente
                    HttpContext.Session.SetString("ClienteNombre", cliente.Nombre);
                    HttpContext.Session.SetString("ClienteRol", cliente.IdRol.ToString()); // Guardar el rol del cliente
                    TempData["Mensaje"] = $"Bienvenido {cliente.Nombre}";
                    // se envia correo indicando que se inicio sesion y la fecha de inicio al usuario por seguridad 
                    Task<ActionResult> taskSendEmail = _correoController.EnviarCorreoInicioSesionPropietario(cliente);
                    return RedirectToAction("Index", "Home");
                }
                // Si no coincide usuario o contraseña
                TempData["MensajeInicioFallido"] = "Correo o contraseña incorrectos.";
                return RedirectToAction("InicioSesionCliente");
            }
            return View();
        }

        // Cerrar sesión
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Limpiar la sesión
            TempData["MensajeSession"] = "Sesión cerrada correctamente.";
            return RedirectToAction("Index", "Home");
        }

        //// Recuperar contrasena metodos 
        [AllowAnonymous]
        [HttpGet]
        public IActionResult EnviarCodigoSeguridad()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult EnviarCodigoSeguridad(CorreoParameter correoUsuario)
        {
            // Buscar el cliente en la base de datos
            var cliente = _context.Propietarios
                .FirstOrDefault(u => u.Correo == correoUsuario.Correo);
            if (cliente != null)
            {
                // se envia correo indicando alguien solicito recuperar la contrasena y el codigo de seguridad
                Task<ActionResult> taskSendEmail = _correoController.EnviarCorreoCodigoSeguridadPropietario(cliente);
                return RedirectToAction("EnviarContrasenaNueva", cliente);
            }
            else
            {
                // Si no coincide usuario o contraseña
                TempData["MensajeCorreoUsuarioNoEncontrado"] = "Correo no registrado en el sistema";
                return RedirectToAction("InicioSesionCliente");
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult EnviarContrasenaNueva(Propietario cliente)
        {
            return View(cliente);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult EnviarContrasenaAleatoria(Propietario clienteRecibido)
        {
            // Buscar el cliente en la base de datos
            var cliente = _context.Propietarios
                .FirstOrDefault(u => u.IdPropietario == clienteRecibido.IdPropietario);
            // Verificar el código de seguridad
            if (cliente != null && cliente.Contrasenna == clienteRecibido.Contrasenna)
            {
                // se envia correo indicando la contrasena nueva para que el usuario pueda iniciar sesion.
                Task<ActionResult> taskSendEmail = _correoController.EnviarCorreoContrasenaTemporalPropietario(cliente);
                // Actualizar la contraseña en la base de datos
                cliente.Contrasenna = clienteRecibido.Contrasenna; // Asumiendo que clienteRecibido también tiene la nueva contraseña
                _context.SaveChanges();
                TempData["MensajeContrasenaTemporalEnviada"] = "Contaseña temporal enviada a su correo, favor iniciar sesión con la contraseña nueva";
                return RedirectToAction("InicioSesionCliente");
            }
            else
            {
                // Si no coincide usuario o contraseña
                TempData["MensajeCodigoDeSeguridadIncorrecto"] = "El Código de seguridad es incorrecto, favor intente de nuevo el proceso";
                return RedirectToAction("InicioSesionCliente");
            }
        }
    }
}