using Microsoft.AspNetCore.Mvc;
using FrontEnd.Services;
using FrontEnd.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace FrontEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CorreoController : ControllerBase
    {
        private readonly ProyectoVeterinariaContext _context;
        private readonly ICorreoService _correoService;

        public CorreoController(ICorreoService correoService, ProyectoVeterinariaContext context)
        {
            _correoService = correoService ?? throw new ArgumentNullException(nameof(correoService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpPost("EnviarCorreo")]
        public async Task<ActionResult> EnviarCorreo(string emailReceiver, string subject, string body)
        {
            try
            {
                await _correoService.EnviarCorreo(emailReceiver, subject, body);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al enviar el correo: {ex.Message}");
            }
        }

        [HttpPost("EnviarCorreoRegistroCita")]
        public async Task<ActionResult> EnviarCorreoRegistroCita(Cita cita)
        {
            if (cita == null)
            {
                return BadRequest("La cita no puede ser nula.");
            }

            try
            {
                var propietario = await _context.Propietarios
                    .FirstOrDefaultAsync(p => p.IdPropietario == cita.IdPropietario);

                if (propietario == null)
                {
                    return NotFound($"No se encontró el propietario con ID: {cita.IdPropietario}");
                }

                var clinica = await _context.Clinicas
                    .FirstOrDefaultAsync(c => c.IdClinica == cita.IdClinicaOrigen);

                if (clinica == null)
                {
                    return NotFound($"No se encontró la clínica con ID: {cita.IdClinicaOrigen}");
                }

                string emailReceiver = propietario.Correo;
                string subject = "Nueva cita registrada";

                StringBuilder body = new StringBuilder();
                body.AppendLine("Se ha registrado una nueva cita para su mascota.");
                body.AppendLine($"Fecha y hora: {cita.FechaHoraCita}");
                body.AppendLine($"Motivo de la consulta: {cita.MotivoConsulta}");
                body.AppendLine($"Clínica: {clinica.Nombre}");

                await _correoService.EnviarCorreo(emailReceiver, subject, body.ToString());
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al enviar el correo de registro de cita: {ex.Message}");
            }
        }

        [HttpPost("EnviarCorreoPagoCita")]
        public async Task<ActionResult> EnviarCorreoPagoCita(Pago pago)
        {
            if (pago == null)
            {
                return BadRequest("El pago no puede ser nulo.");
            }

            try
            {
                var propietario = await _context.Propietarios
                    .FirstOrDefaultAsync(p => p.IdPropietario == pago.IdCitaNavigation.IdPropietario);

                if (propietario == null)
                {
                    return NotFound($"No se encontró el propietario para la cita con ID: {pago.IdCita}");
                }

                var cita = await _context.Citas
                    .FirstOrDefaultAsync(c => c.IdCita == pago.IdCita);

                if (cita == null)
                {
                    return NotFound($"No se encontró la cita con ID: {pago.IdCita}");
                }

                string emailReceiver = propietario.Correo;
                string subject = $"Pago registrado para la cita con ID: {pago.IdCita}";

                StringBuilder body = new StringBuilder();
                body.AppendLine("Se ha registrado un pago para su cita.");
                body.AppendLine($"Monto: {pago.Monto}");
                body.AppendLine($"Fecha de pago: {pago.FechaPago}");
                body.AppendLine($"Método de pago: {pago.MetodoPago}");

                await _correoService.EnviarCorreo(emailReceiver, subject, body.ToString());
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al enviar el correo de pago de cita: {ex.Message}");
            }
        }

        [HttpPost("EnviarCorreoActualizarEstadoCita")]
        public async Task<ActionResult> EnviarCorreoActualizarEstadoCita(Cita citaActualizada)
        {
            if (citaActualizada == null)
            {
                return BadRequest("La cita actualizada no puede ser nula.");
            }

            try
            {
                var propietario = await _context.Propietarios
                    .FirstOrDefaultAsync(p => p.IdPropietario == citaActualizada.IdPropietario);

                if (propietario == null)
                {
                    return NotFound($"No se encontró el propietario para la cita con ID: {citaActualizada.IdCita}");
                }

                var clinica = await _context.Clinicas
                    .FirstOrDefaultAsync(c => c.IdClinica == citaActualizada.IdClinicaOrigen);

                if (clinica == null)
                {
                    return NotFound($"No se encontró la clínica con ID: {citaActualizada.IdClinicaOrigen}");
                }

                string emailReceiver = propietario.Correo;
                string subject = $"Actualización de estado de la cita con ID: {citaActualizada.IdCita}";

                StringBuilder body = new StringBuilder();
                body.AppendLine("Se ha actualizado el estado de su cita.");
                body.AppendLine($"Nuevo estado: {citaActualizada.EstadoCita}");
                body.AppendLine($"Motivo de la consulta: {citaActualizada.MotivoConsulta}");
                body.AppendLine($"Clínica: {clinica.Nombre}");

                await _correoService.EnviarCorreo(emailReceiver, subject, body.ToString());
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al enviar el correo de actualización de estado de cita: {ex.Message}");
            }
        }

        [HttpPost("EnviarCorreoInicioSesionCliente")]
        public async Task<ActionResult> EnviarCorreoInicioSesionCliente(Propietario propietario)
        {
            if (propietario == null)
            {
                return BadRequest("El propietario no puede ser nulo.");
            }

            try
            {
                string emailReceiver = propietario.Correo;
                string subject = "Se ha detectado un inicio de sesión en su cuenta";

                StringBuilder body = new StringBuilder();
                body.AppendLine($"Estimado/a {propietario.Nombre},");
                body.AppendLine("Se ha detectado un inicio de sesión en su cuenta.");
                body.AppendLine($"Fecha y hora: {DateTime.Now}");
                body.AppendLine("Si usted no ha iniciado sesión, le recomendamos cambiar su contraseña.");

                await _correoService.EnviarCorreo(emailReceiver, subject, body.ToString());
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al enviar el correo de inicio de sesión: {ex.Message}");
            }
        }

        [HttpPost("EnviarCorreoCambioContrasenaPropietario")]
        public async Task<ActionResult> EnviarCorreoCambioContrasenaPropietario(Propietario propietario)
        {
            if (propietario == null)
            {
                return BadRequest("El propietario no puede ser nulo.");
            }

            try
            {
                string emailReceiver = propietario.Correo;
                string subject = "Su contraseña ha sido cambiada con éxito";

                StringBuilder body = new StringBuilder();
                body.AppendLine($"Estimado/a {propietario.Nombre},");
                body.AppendLine("Su contraseña ha sido cambiada con éxito.");
                body.AppendLine($"Fecha y hora: {DateTime.Now}");
                body.AppendLine("Ahora puede iniciar sesión con su nueva contraseña.");

                await _correoService.EnviarCorreo(emailReceiver, subject, body.ToString());
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al enviar el correo de cambio de contraseña: {ex.Message}");
            }
        }

        [HttpPost("EnviarCorreoCodigoSeguridadPropietario")]
        public async Task<ActionResult> EnviarCorreoCodigoSeguridadPropietario(Propietario propietario)
        {
            if (propietario == null)
            {
                return BadRequest("El propietario no puede ser nulo.");
            }

            try
            {
                Random random = new Random();
                string codigoSeguridad = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 8)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                propietario.Contrasenna = codigoSeguridad;
                await _context.SaveChangesAsync();

                string emailReceiver = propietario.Correo;
                string subject = "Su código de seguridad";

                StringBuilder body = new StringBuilder();
                body.AppendLine($"Estimado/a {propietario.Nombre},");
                body.AppendLine("Ha solicitado recuperar la contraseña de su cuenta.");
                body.AppendLine($"Fecha y hora: {DateTime.Now}");
                body.AppendLine("Su código de seguridad es: ");
                body.AppendLine(codigoSeguridad);
                body.AppendLine("Por favor, no comparta este código con nadie. Nuestro personal nunca se lo pedirá.");

                await _correoService.EnviarCorreo(emailReceiver, subject, body.ToString());
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al enviar el correo de código de seguridad: {ex.Message}");
            }
        }

        [HttpPost("EnviarCorreoContrasenaTemporalPropietario")]
        public async Task<ActionResult> EnviarCorreoContrasenaTemporalPropietario(Propietario propietario)
        {
            if (propietario == null)
            {
                return BadRequest("El propietario no puede ser nulo.");
            }

            try
            {
                Random random = new Random();
                string contrasenaTemporal = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 8)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                propietario.Contrasenna = contrasenaTemporal;
                await _context.SaveChangesAsync();

                string emailReceiver = propietario.Correo;
                string subject = "Contraseña temporal para iniciar sesión";

                StringBuilder body = new StringBuilder();
                body.AppendLine($"Estimado/a {propietario.Nombre},");
                body.AppendLine("Se le ha asignado una contraseña temporal para iniciar sesión.");
                body.AppendLine($"Contraseña temporal: {contrasenaTemporal}");
                body.AppendLine("Por favor, cambie su contraseña después de iniciar sesión.");

                await _correoService.EnviarCorreo(emailReceiver, subject, body.ToString());
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al enviar el correo de contraseña temporal: {ex.Message}");
            }
        }
    }
}