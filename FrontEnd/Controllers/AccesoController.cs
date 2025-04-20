using FrontEnd.Models;
using FrontEnd.Models.modelsDataParameterMethods; // Assuming CorreoParameter is defined here
using FrontEnd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
// No additional using statements added, as requested.
using System; // Needed for DateTime, Random, Convert
using System.Linq; // Needed for FirstOrDefault, Any, etc.
using System.Text; // Needed for StringBuilder in password generation
using System.Threading.Tasks; // Needed for async methods

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

        // Helper method to generate a random password
        private string GenerarPasswordAleatoria(int length = 8)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890#$%^&+=!";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(validChars[rnd.Next(validChars.Length)]);
            }
            // Ensure complexity requirements are likely met (add checks if needed)
            // For simplicity here, just returning the random string.
            // A robust implementation would ensure at least one of each required character type.
            return res.ToString();
        }


        private void VerificarSesion()
        {
            var lastActivity = HttpContext.Session.GetString("LastActivity");
            if (lastActivity != null)
            {
                var lastActivityTime = DateTime.Parse(lastActivity);
                // Using 5 minutes inactivity timeout as per original code
                if (lastActivityTime.AddMinutes(5) < DateTime.Now)
                {
                    HttpContext.Session.Clear();
                    TempData["MensajeSesion"] = "Sesión cerrada por inactividad.";
                    // Consider RedirectToAction("InicioSesionUsuario") or appropriate login page
                }
            }
            HttpContext.Session.SetString("LastActivity", DateTime.Now.ToString());
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult RegistroUsuario()
        {
            // VerificarSesion(); // Often not needed on public registration pages
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken] // Good practice to add this
        public IActionResult RegistroUsuario(RegistroModeloUsuario modelo)
        {
            // VerificarSesion();
            if (ModelState.IsValid)
            {
                if (_context.Personal.Any(u => u.Correo == modelo.Correo))
                {
                    ModelState.AddModelError("Correo", "El correo de usuario ya existe."); // More specific error
                    return View(modelo);
                }

                var nuevoUsuario = new Personal
                {
                    Nombre = modelo.Nombre,
                    // Handle potential null SegundoApellido gracefully
                    Apellido = modelo.PrimerApellido + (!string.IsNullOrWhiteSpace(modelo.SegundoApellido) ? " " + modelo.SegundoApellido : ""),
                    Correo = modelo.Correo,
                    Contrasena = modelo.Contrasena, // SECURITY RISK: Store hashed password instead
                    Telefono = modelo.TelefonoPrincipal,
                    IdRol = modelo.IdRol, // Relies on default or future input
                    IdClinica = modelo.IdClinica, // Relies on default or future input
                    FotoPerfil = modelo.FotoPerfil // Added mapping for FotoPerfil
                };

                _context.Personal.Add(nuevoUsuario);
                _context.SaveChanges();
                TempData["MensajeRegistroCorrecto"] = "Usuario registrado correctamente, ahora puedes iniciar sesión.";
                // Redirect to the correct login page for users (Personal)
                return RedirectToAction("InicioSesionUsuario");
            }

            // If model state is invalid, return view with validation errors
            return View(modelo);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult InicioSesionUsuario()
        {
            // VerificarSesion(); // Prevent access if already logged in? Optional.
            // Clear session just in case?
            // HttpContext.Session.Clear();
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken] // Good practice
        public IActionResult InicioSesionUsuario(LoginModeloUsuario modelo)
        {
            // VerificarSesion(); // Update activity time if login fails?
            if (ModelState.IsValid)
            {
                // SECURITY RISK: Comparing plain text passwords
                var usuario = _context.Personal.FirstOrDefault(u => u.Correo == modelo.Correo && u.Contrasena == modelo.Contrasena);
                if (usuario != null)
                {
                    // Set session state
                    HttpContext.Session.SetString("UsuarioId", usuario.IdPersonal.ToString());
                    HttpContext.Session.SetString("UsuarioNombre", usuario.Nombre ?? "Usuario"); // Handle potential null name
                    HttpContext.Session.SetString("UsuarioRol", usuario.IdRol.ToString() ?? "0"); // Handle potential null role
                    HttpContext.Session.SetString("LastActivity", DateTime.Now.ToString()); // Start session timer

                    TempData["Mensaje"] = $"Bienvenido {usuario.Nombre}";
                    return RedirectToAction("Index", "Home"); // Or admin dashboard
                }

                // Use TempData for redirect messages
                TempData["MensajeInicioFallido"] = "Correo o contraseña incorrectos.";
                // Redirect back to the GET action to display the message
                return RedirectToAction("InicioSesionUsuario");
            }

            // If model state is invalid, return the view with the model to show validation errors
            // Make sure the view can display ModelState errors
            return View(modelo); // Pass model back to view
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult RegistroCliente()
        {
            // VerificarSesion();
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken] // Good practice
        public IActionResult RegistroCliente(RegistroModeloCliente modelo)
        {
            // VerificarSesion();
            if (ModelState.IsValid)
            {
                if (_context.Propietarios.Any(c => c.Correo == modelo.Correo))
                {
                    ModelState.AddModelError("Correo", "El correo ya existe."); // More specific
                    return View(modelo);
                }

                var nuevoCliente = new Propietario
                {
                    Nombre = modelo.Nombre,
                    Apellido = modelo.PrimerApellido + (!string.IsNullOrWhiteSpace(modelo.SegundoApellido) ? " " + modelo.SegundoApellido : ""),
                    Correo = modelo.Correo,
                    Contrasena = modelo.Contrasena, // SECURITY RISK: Store hashed password
                    Telefono = modelo.TelefonoPrincipal,
                    IdRol = modelo.IdRol // Relies on default or future input
                    // Address fields from RegistroModeloCliente are not mapped to Propietario entity here
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
            // VerificarSesion();
            // HttpContext.Session.Clear(); // Optional: Clear session on accessing login page
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken] // Good practice
        public async Task<IActionResult> InicioSesionCliente(LoginModeloUsuarioCliente modelo)
        {
            // VerificarSesion(); // Update activity on failure?
            if (ModelState.IsValid)
            {
                // SECURITY RISK: Plain text password comparison
                var cliente = _context.Propietarios.FirstOrDefault(u => u.Correo == modelo.Correo && u.Contrasena == modelo.Contrasena);
                if (cliente != null)
                {
                    HttpContext.Session.SetString("ClienteId", cliente.IdPropietario.ToString());
                    HttpContext.Session.SetString("ClienteNombre", cliente.Nombre ?? "Cliente");
                    HttpContext.Session.SetString("ClienteRol", cliente.IdRol.ToString() ?? "0");
                    HttpContext.Session.SetString("LastActivity", DateTime.Now.ToString()); // Start session timer

                    TempData["Mensaje"] = $"Bienvenido {cliente.Nombre}";

                    // Send login notification email
                    try
                    {
                        await _correoService.EnviarCorreo(
                            cliente.Correo,
                            "Inicio de sesión exitoso",
                            $"Hola {cliente.Nombre}, has iniciado sesión el {DateTime.Now}."
                        );
                    }
                    catch (Exception ex)
                    {
                        // Log the email sending error (using a logging framework is recommended)
                        Console.WriteLine($"Error sending login email: {ex.Message}");
                        // Don't block login if email fails, but maybe notify admin or log prominently
                    }

                    return RedirectToAction("Index", "Home"); // Or client dashboard
                }

                TempData["MensajeInicioFallido"] = "Correo o contraseña incorrectos.";
                return RedirectToAction("InicioSesionCliente");
            }

            return View(modelo); // Return view with model state errors
        }

        [HttpGet] // Should ideally be POST if it modifies state (clears session)
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["MensajeSession"] = "Sesión cerrada correctamente."; // Consistent key "MensajeSesion"?
            return RedirectToAction("Index", "Home"); // Redirect to public home
        }

        // ----- Password Recovery Flow -----

        [AllowAnonymous]
        [HttpGet]
        public IActionResult EnviarCodigoSeguridad()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnviarCodigoSeguridad(CorreoParameter correoUsuario)
        {
            // Basic validation on the model if needed
            if (string.IsNullOrWhiteSpace(correoUsuario?.Correo))
            {
                TempData["MensajeCorreoUsuarioNoEncontrado"] = "Por favor, ingrese un correo.";
                return RedirectToAction("EnviarCodigoSeguridad"); // Redirect back to the GET action
            }

            var cliente = _context.Propietarios.FirstOrDefault(u => u.Correo == correoUsuario.Correo);
            if (cliente != null)
            {
                // SECURITY RISK: Sending the actual password as the code.
                // Should generate and send a temporary, unique token.
                try
                {
                    await _correoService.EnviarCorreo(
                        cliente.Correo,
                        "Código de seguridad",
                        $"Hola {cliente.Nombre}, este es tu código de seguridad: {cliente.Contrasena}" // EXTREMELY INSECURE
                    );

                    // Pass only necessary ID to the next step, not the whole object potentially
                    // Using TempData or RouteData is generally safer than passing complex objects in GET requests
                    TempData["RecoveryClientId"] = cliente.IdPropietario; // Store ID temporarily
                    TempData["RecoveryClientEmail"] = cliente.Correo; // Store email for display
                    return RedirectToAction("EnviarContrasenaNueva"); // Redirect to GET action
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending recovery code email: {ex.Message}");
                    TempData["MensajeCorreoUsuarioNoEncontrado"] = "Error al enviar el código. Intente de nuevo más tarde.";
                    // Don't reveal if the email exists or not on error
                    return RedirectToAction("InicioSesionCliente");
                }
            }

            // IMPORTANT: Do not reveal if the email exists or not for security reasons.
            // Show a generic message even if the user is not found.
            TempData["MensajeCorreoUsuarioNoEncontrado"] = "Si el correo está registrado, recibirás un código de seguridad.";
            // Redirect to a confirmation page or back to login, not revealing failure specifics.
            return RedirectToAction("InicioSesionCliente");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult EnviarContrasenaNueva()
        {
            // Retrieve data needed for the view from TempData
            if (TempData["RecoveryClientId"] == null || TempData["RecoveryClientEmail"] == null)
            {
                // If data is missing (e.g., user navigated directly), redirect appropriately
                TempData["MensajeErrorRecuperacion"] = "Proceso de recuperación inválido. Por favor, solicite un código nuevamente.";
                return RedirectToAction("EnviarCodigoSeguridad");
            }

            // Pass necessary data to the view using ViewData or a simple ViewModel
            ViewData["ClientId"] = TempData["RecoveryClientId"];
            ViewData["ClientEmail"] = TempData["RecoveryClientEmail"]; // Keep email for display

            // Keep TempData alive if returning the view directly (though redirect is used above)
            // TempData.Keep("RecoveryClientId");
            // TempData.Keep("RecoveryClientEmail");

            return View(); // Return the view that collects the code
        }


        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Corrected signature to accept form field names
        public async Task<IActionResult> EnviarContrasenaAleatoria(int IdPropietario, string ClaveRecupera)
        {
            if (string.IsNullOrWhiteSpace(ClaveRecupera))
            {
                TempData["MensajeCodigoDeSeguridadIncorrecto"] = "Por favor ingrese el código de seguridad.";
                // Need to pass back the necessary IDs to the GET action if redirecting
                TempData["RecoveryClientId"] = IdPropietario;
                var ownerEmail = _context.Propietarios.Where(p => p.IdPropietario == IdPropietario).Select(p => p.Correo).FirstOrDefault();
                TempData["RecoveryClientEmail"] = ownerEmail;
                return RedirectToAction("EnviarContrasenaNueva"); // Redirect back to GET
            }

            // Refetch the client from DB using the ID
            var cliente = _context.Propietarios.FirstOrDefault(u => u.IdPropietario == IdPropietario);

            // SECURITY RISK: Comparing submitted code (which is the original plain password) with the stored plain password.
            if (cliente != null && cliente.Contrasena == ClaveRecupera)
            {
                // Generate a NEW random password
                string nuevaContrasenaTemporal = GenerarPasswordAleatoria();

                try
                {
                    // Send the NEW temporary password via email
                    await _correoService.EnviarCorreo(
                        cliente.Correo,
                        "Contraseña temporal",
                        $"Hola {cliente.Nombre}, tu nueva contraseña temporal es: {nuevaContrasenaTemporal}" // SECURITY: Still sending plain text
                    );

                    // Update the password in the database
                    // SECURITY RISK: Storing the new password in plain text. Should be hashed.
                    cliente.Contrasena = nuevaContrasenaTemporal;
                    _context.SaveChanges(); // Save the updated client with the new password

                    TempData["MensajeContrasenaTemporalEnviada"] = "Una nueva contraseña temporal ha sido enviada a tu correo.";
                    return RedirectToAction("InicioSesionCliente");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending new password email: {ex.Message}");
                    TempData["MensajeCodigoDeSeguridadIncorrecto"] = "Error al procesar la solicitud. Intente de nuevo más tarde.";
                    // Redirect back to the start of the recovery process or login
                    return RedirectToAction("InicioSesionCliente");
                }
            }

            // If client not found or code is incorrect
            TempData["MensajeCodigoDeSeguridadIncorrecto"] = "El código de seguridad es incorrecto o inválido.";
            // Need to repopulate TempData for the redirect if necessary or just redirect to login
            TempData["RecoveryClientId"] = IdPropietario;
            var email = _context.Propietarios.Where(p => p.IdPropietario == IdPropietario).Select(p => p.Correo).FirstOrDefault();
            TempData["RecoveryClientEmail"] = email;
            return RedirectToAction("EnviarContrasenaNueva"); // Redirect back to GET action to allow re-entry
        }
    }
}