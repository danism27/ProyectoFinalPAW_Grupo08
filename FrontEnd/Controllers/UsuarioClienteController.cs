using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FrontEnd.Models; // Asegúrate que este sea el namespace correcto para tus modelos y DbContext
using System.Text.RegularExpressions; // Para validación de contraseña


// ---- INICIO: Placeholder para Password Hashing ----
// ¡¡¡IMPORTANTE!!! Reemplaza esto con tu implementación real usando una librería segura
// como BCrypt.Net o la integrada en ASP.NET Core Identity.
public static class PasswordHelper
{
    public static string HashPassword(string password)
    {
        // Placeholder: ¡NO USAR EN PRODUCCIÓN!
        Console.WriteLine("ADVERTENCIA: Usando hashing de contraseña inseguro (placeholder).");
        return password; // Devuelve la contraseña en texto plano (¡Muy inseguro!)
        // Ejemplo con BCrypt.Net: return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public static bool VerifyPasswordHash(string password, string storedHash)
    {
        // Placeholder: ¡NO USAR EN PRODUCCIÓN!
        Console.WriteLine("ADVERTENCIA: Usando verificación de contraseña insegura (placeholder).");
        return password == storedHash; // Comparación insegura en texto plano
        // Ejemplo con BCrypt.Net: return BCrypt.Net.BCrypt.Verify(password, storedHash);
    }
}
// ---- FIN: Placeholder para Password Hashing ----


namespace FrontEnd.Controllers
{
    // Asegúrate de aplicar la autorización adecuada si tienes un sistema de login
    // [Authorize(Roles = "Propietario")] // Ejemplo: Solo usuarios con rol Propietario
    // [Authorize] // Ejemplo: Cualquier usuario logueado
    public class PropietarioController : Controller
    {
        // Reemplaza 'VeterinariaContext' con el nombre real de tu DbContext
        private readonly ProyectoVeterinariaContext _context;
        private readonly ILogger<PropietarioController> _logger;

        // Inyecta tu DbContext y Logger
        public PropietarioController(ProyectoVeterinariaContext context, ILogger<PropietarioController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // --- Función Helper para obtener el ID del Propietario Logueado ---
        // ¡¡¡IMPORTANTE!!! Debes implementar esta función según tu sistema de autenticación.
        private int? GetCurrentPropietarioId()
        {
            // Ejemplo 1: Usando Claims (común con ASP.NET Core Identity)
            /*
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // O el Claim que uses para el ID
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int propietarioId))
            {
                return propietarioId;
            }
            */

            // Ejemplo 2: Usando Sesión (si almacenas el ID en sesión)
            /*
            if (HttpContext.Session.TryGetValue("PropietarioId", out byte[] idBytes) && idBytes != null)
            {
                 if (int.TryParse(System.Text.Encoding.UTF8.GetString(idBytes), out int propietarioId))
                 {
                     return propietarioId;
                 }
            }
            */

            // Placeholder si no hay autenticación implementada (SOLO PARA DESARROLLO)
            // Deberías lanzar una excepción o retornar null en producción si no se encuentra el ID.
            // Asumamos que el propietario con ID 1 está logueado para pruebas:
            _logger.LogWarning("GetCurrentPropietarioId() está usando un valor placeholder (ID=1). Implementar lógica de autenticación real.");
            // Intenta obtenerlo de la sesión primero, si no, usa el placeholder
            if (HttpContext.Session.TryGetValue("PropietarioId", out byte[] idBytes) && idBytes != null)
            {
                if (int.TryParse(System.Text.Encoding.UTF8.GetString(idBytes), out int propietarioId))
                {
                    return propietarioId;
                }
            }
            // Como fallback MUY TEMPORAL para desarrollo si no hay sesión:
            // return 1;

            // En producción, si no se encuentra el ID, retorna null o lanza excepción
            _logger.LogError("No se pudo obtener el ID del propietario autenticado.");
            return null; // Retorna null si no se puede obtener el ID
        }

        // --- Acción para la vista Citas (GET /Propietario/Citas) ---
        public async Task<IActionResult> Citas(string buscar)
        {
            int? propietarioId = GetCurrentPropietarioId();
            if (propietarioId == null)
            {
                // Si no se puede obtener el ID, redirige a Login o muestra error.
                TempData["Error"] = "Debe iniciar sesión para ver sus citas.";
                // return RedirectToAction("Login", "Account"); // Asume que tienes un AccountController con acción Login
                return RedirectToAction("Index", "Home"); // O redirige a la página principal
            }

            var query = _context.Citas
                                .Include(c => c.IdClinicaDestinoNavigation) // Para mostrar nombre de clínica
                                .Include(c => c.IdVeterinarioAsignadoNavigation) // Para mostrar nombre de veterinario
                                                                                 // Include PropietarioNavigation si lo necesitas explícitamente en la vista principal o modal.
                                                                                 // .Include(c => c.IdPropietarioNavigation)
                                .Where(c => c.IdPropietario == propietarioId);

            if (!string.IsNullOrEmpty(buscar))
            {
                if (int.TryParse(buscar, out int citaIdBuscada))
                {
                    // Busca por ID exacto si es un número
                    query = query.Where(c => c.IdCita == citaIdBuscada);
                }
                else
                {
                    // Busca por texto en el motivo (case-insensitive)
                    string lowerBuscar = buscar.ToLower();
                    query = query.Where(c => c.MotivoConsulta != null && c.MotivoConsulta.ToLower().Contains(lowerBuscar));
                }
            }

            // Ordena las citas, por ejemplo, por fecha descendente
            var citas = await query.OrderByDescending(c => c.FechaHoraCita).ToListAsync();

            return View(citas); // Pasa la lista de citas a la vista Citas.cshtml
        }

        // --- Acción para la vista CitasEnProceso (GET /Propietario/CitasActivas) ---
        // Nota: El enlace en Citas.cshtml apunta a 'CitasActivas', pero la vista se llama 'CitasEnProceso.cshtml'
        public async Task<IActionResult> CitasActivas(string buscarCitaId)
        {
            int? propietarioId = GetCurrentPropietarioId();
            if (propietarioId == null)
            {
                TempData["Error"] = "Debe iniciar sesión para ver sus citas activas.";
                // return RedirectToAction("Login", "Account");
                return RedirectToAction("Index", "Home");
            }

            var query = _context.Citas
                                .Include(c => c.IdClinicaDestinoNavigation)
                                .Include(c => c.IdPropietarioNavigation) // Necesario para mostrar nombre propietario en tabla/modal
                                .Include(c => c.IdVeterinarioAsignadoNavigation)
                                .Include(c => c.Pagos) // ¡¡ESENCIAL para calcular 'Estado Pago' en la vista!!
                                .Where(c => c.IdPropietario == propietarioId &&
                                            (c.EstadoCita == "Pendiente" || c.EstadoCita == "Confirmada")); // Filtra por estados activos

            if (!string.IsNullOrEmpty(buscarCitaId) && int.TryParse(buscarCitaId, out int citaIdBuscada))
            {
                query = query.Where(c => c.IdCita == citaIdBuscada);
            }

            // Ordena por fecha ascendente para ver las próximas primero
            var citasActivas = await query.OrderBy(c => c.FechaHoraCita).ToListAsync();

            // Retorna la vista CitasEnProceso.cshtml con la lista filtrada
            return View("CitasEnProceso", citasActivas);
        }


        // --- Acción para la vista EstadoCitas (GET /Propietario/EstadoCitas) ---
        // Muestra citas 'Completada' o 'Cancelada'
        public async Task<IActionResult> EstadoCitas(string buscarCitaId)
        {
            int? propietarioId = GetCurrentPropietarioId();
            if (propietarioId == null)
            {
                TempData["Error"] = "Debe iniciar sesión para ver el historial de citas.";
                // return RedirectToAction("Login", "Account");
                return RedirectToAction("Index", "Home");
            }

            // Fetching 'Closed' appointments (Completed or Cancelled)
            var query = _context.Citas
                                .Include(c => c.IdClinicaDestinoNavigation)
                                .Include(c => c.IdPropietarioNavigation)
                                .Include(c => c.IdClinicaOrigenNavigation) // Incluir para modal si es necesario
                                .Include(c => c.IdVeterinarioAsignadoNavigation)
                                .Include(c => c.Pagos) // Incluir para modal si es necesario
                                .Include(c => c.HistorialCitas).ThenInclude(h => h.IdPersonalModificacionNavigation) // Incluir para modal
                                .Where(c => c.IdPropietario == propietarioId &&
                                            (c.EstadoCita == "Completada" || c.EstadoCita == "Cancelada"));

            if (!string.IsNullOrEmpty(buscarCitaId) && int.TryParse(buscarCitaId, out int citaIdBuscada))
            {
                query = query.Where(c => c.IdCita == citaIdBuscada);
            }

            var citasCerradas = await query.OrderByDescending(c => c.FechaHoraCita).ToListAsync();

            // --- IMPORTANTE ---
            // La vista EstadoCitas.cshtml originalmente esperaba @model IEnumerable<CitaPersonalClinica>.
            // Se ha cambiado la lógica para pasar @model IEnumerable<Cita> por consistencia.
            // Debes ASEGURARTE de modificar EstadoCitas.cshtml para que funcione con IEnumerable<Cita>.
            // Ej: Cambiar @item.IdCitaNavigation.IdCita por @item.IdCita
            // Ej: Cambiar @item.IdCitaNavigation.MotivoConsulta por @item.MotivoConsulta
            // Ej: Cambiar @item.IdPersonalNavigation por @item.IdVeterinarioAsignadoNavigation (o decidir qué mostrar)
            return View(citasCerradas); // Pasa la lista de Cita objects
        }


        // --- Acción para Cancelar una Cita (POST /Propietario/CancelarCita) ---
        [HttpPost]
        [ValidateAntiForgeryToken] // Protege contra ataques CSRF
        public async Task<IActionResult> CancelarCita(int idCita)
        {
            int? propietarioId = GetCurrentPropietarioId();
            if (propietarioId == null)
            {
                TempData["Error"] = "Acción no permitida. Debe iniciar sesión.";
                // return RedirectToAction("Login", "Account");
                return RedirectToAction(nameof(CitasActivas)); // Redirige a la lista de activas
            }

            // Busca la cita asegurándose que pertenece al propietario logueado
            var cita = await _context.Citas.FirstOrDefaultAsync(c => c.IdCita == idCita && c.IdPropietario == propietarioId);

            if (cita == null)
            {
                TempData["Error"] = "Cita no encontrada o no tiene permiso para cancelarla.";
                return RedirectToAction(nameof(CitasActivas));
            }

            // Solo permite cancelar si está 'Pendiente' o 'Confirmada'
            if (cita.EstadoCita == "Pendiente" || cita.EstadoCita == "Confirmada")
            {
                string estadoAnterior = cita.EstadoCita; // Guarda estado para historial
                cita.EstadoCita = "Cancelada";

                // Crea un registro en el historial
                var historial = new HistorialCita
                {
                    IdCita = idCita,
                    FechaCambio = DateTime.Now, // Hora actual del servidor
                    EstadoAnterior = estadoAnterior,
                    EstadoNuevo = "Cancelada",
                    IdPersonalModificacion = null, // El propietario canceló, no un personal
                    UbicacionActual = "Cancelada por Propietario" // Opcional: Añadir nota
                };
                _context.HistorialCitas.Add(historial);
                _context.Update(cita); // Marca la cita como modificada

                try
                {
                    await _context.SaveChangesAsync(); // Guarda ambos cambios (Cita y Historial)
                    TempData["Success"] = "Cita cancelada exitosamente.";
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Error al guardar cancelación de cita {CitaId} para propietario {PropietarioId}", idCita, propietarioId);
                    TempData["Error"] = "Error al guardar los cambios en la base de datos.";
                }
            }
            else
            {
                TempData["Warning"] = $"La cita ya se encuentra en estado '{cita.EstadoCita}' y no puede ser cancelada.";
            }

            return RedirectToAction(nameof(CitasActivas)); // Vuelve a la lista de citas activas
        }

        // --- Acción para ver el Historial de una Cita (GET /Propietario/HistorialCita/{idCita}) ---
        // Unificado para manejar enlaces de Citas.cshtml, CitasEnProceso.cshtml y EstadoCitas.cshtml
        // Asegúrate que los forms/links usen asp-action="HistorialCita" y pasen 'idCita'
        public async Task<IActionResult> HistorialCita(int idCita)
        {
            int? propietarioId = GetCurrentPropietarioId();
            if (propietarioId == null)
            {
                TempData["Error"] = "Debe iniciar sesión para ver el historial.";
                // return RedirectToAction("Login", "Account");
                return RedirectToAction("Index", "Home");
            }

            // Paso de Seguridad Clave: Verifica que la cita cuyo historial se pide, PERTENECE al propietario logueado.
            var citaPerteneceAlUsuario = await _context.Citas.AnyAsync(c => c.IdCita == idCita && c.IdPropietario == propietarioId);

            if (!citaPerteneceAlUsuario)
            {
                _logger.LogWarning("Intento de acceso no autorizado al historial de CitaId {CitaId} por PropietarioId {PropietarioId_Attempt}", idCita, propietarioId);
                TempData["Error"] = "No tiene permiso para ver el historial de esta cita.";
                // Puedes redirigir a una vista de error o a la lista general de citas
                return RedirectToAction(nameof(Citas));
                // Alternativas: return Forbid(); o return NotFound();
            }

            // Obtiene el historial de la cita específica
            var historial = await _context.HistorialCitas
                                      .Include(h => h.IdPersonalModificacionNavigation) // Carga datos del personal que modificó (si aplica)
                                      .Where(h => h.IdCita == idCita)
                                      .OrderByDescending(h => h.FechaCambio) // Muestra los cambios más recientes primero
                                      .ToListAsync();

            ViewBag.CitaId = idCita; // Pasa el ID de la cita a la vista para mostrarlo si es necesario

            // Retorna la vista Historial.cshtml con la lista de entradas de historial
            return View("Historial", historial);
        }

        // --- Acción para la vista Configuracion (GET /Propietario/Configuracion) ---
        public async Task<IActionResult> Configuracion()
        {
            int? propietarioId = GetCurrentPropietarioId();
            if (propietarioId == null)
            {
                TempData["Error"] = "Debe iniciar sesión para acceder a la configuración.";
                // return RedirectToAction("Login", "Account");
                return RedirectToAction("Index", "Home");
            }

            // Busca los datos del propietario actual
            var propietario = await _context.Propietarios
                                            // .Include(p => p.Rol) // Incluye el Rol si necesitas mostrar info del rol en la vista
                                            .FirstOrDefaultAsync(p => p.IdPropietario == propietarioId);

            if (propietario == null)
            {
                _logger.LogError("No se encontró el Propietario con ID {PropietarioId} que debería estar logueado.", propietarioId);
                // Esto no debería pasar si GetCurrentPropietarioId() funciona bien
                return NotFound($"Error: No se encontró un propietario con ID {propietarioId}.");
            }

            // Pasa el objeto Propietario a la vista Configuracion.cshtml
            return View(propietario);
        }

        // --- Acción para Editar Información Personal (POST /Propietario/EditarInformacion) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Usamos [Bind] para evitar overposting. Solo permite actualizar estos campos.
        public async Task<IActionResult> EditarInformacion([Bind("IdPropietario,Nombre,Apellido,Correo,Telefono")] Propietario propietarioModel)
        {
            int? currentPropietarioId = GetCurrentPropietarioId();
            if (currentPropietarioId == null)
            {
                TempData["Error"] = "Sesión expirada o inválida.";
                // return RedirectToAction("Login", "Account");
                return RedirectToAction("Index", "Home");
            }

            // Seguridad: Asegura que el ID del modelo enviado coincida con el usuario logueado
            if (propietarioModel.IdPropietario != currentPropietarioId)
            {
                _logger.LogWarning("Intento de overposting o edición no autorizada para PropietarioId {TargetId} por usuario {CurrentUserId}", propietarioModel.IdPropietario, currentPropietarioId);
                TempData["Error"] = "Intento de modificación no autorizado.";
                return RedirectToAction(nameof(Configuracion));
                // Alternativa: return Forbid();
            }

            // Quitamos del ModelState campos que NO se editan aquí y podrían causar fallos de validación
            ModelState.Remove("Contrasena"); // La contraseña no se edita en este form
            ModelState.Remove("Rol");        // El rol (objeto de navegación) no se edita aquí
            ModelState.Remove("Citas");      // La colección de citas no se edita aquí
            ModelState.Remove("IdRol");      // El IdRol no se edita aquí (asumimos que no)


            if (ModelState.IsValid)
            {
                // Busca la entidad existente en la BD
                var propietarioToUpdate = await _context.Propietarios.FindAsync(currentPropietarioId);
                if (propietarioToUpdate == null)
                {
                    // Esto sería raro si el ID viene del usuario logueado
                    _logger.LogError("Propietario {PropietarioId} no encontrado durante edición, aunque estaba logueado.", currentPropietarioId);
                    return NotFound();
                }

                // Actualiza solo los campos permitidos del objeto existente con los valores del modelo enviado
                propietarioToUpdate.Nombre = propietarioModel.Nombre;
                propietarioToUpdate.Apellido = propietarioModel.Apellido;
                propietarioToUpdate.Correo = propietarioModel.Correo;
                propietarioToUpdate.Telefono = propietarioModel.Telefono;
                // NO actualices Contrasena, IdRol, etc. aquí.

                try
                {
                    _context.Update(propietarioToUpdate); // Marca la entidad como modificada
                    await _context.SaveChangesAsync();    // Guarda los cambios en la BD
                    TempData["Success"] = "Información actualizada correctamente.";
                    return RedirectToAction(nameof(Configuracion)); // Redirige de vuelta a la config
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    // Manejo de errores de concurrencia (si otro usuario modificó mientras tanto)
                    _logger.LogError(ex, "Error de concurrencia al editar propietario {PropietarioId}", propietarioModel.IdPropietario);
                    TempData["Error"] = "Error al guardar los cambios. El registro pudo haber sido modificado por otro usuario.";
                    ModelState.AddModelError(string.Empty, "Los datos han cambiado desde que cargó la página. Por favor, inténtelo de nuevo.");
                }
                catch (DbUpdateException ex)
                {
                    // Manejo de otros errores de BD (ej. Correo duplicado)
                    _logger.LogError(ex, "Error de BD al editar propietario {PropietarioId}", propietarioModel.IdPropietario);
                    TempData["Error"] = "Error al guardar los cambios en la base de datos.";
                    // Intenta dar un mensaje más específico si es posible (requiere inspeccionar InnerException)
                    if (ex.InnerException?.Message.Contains("UNIQUE constraint failed: Propietario.correo") ?? false)
                    {
                        ModelState.AddModelError("Correo", "Este correo electrónico ya está en uso por otro propietario.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "No se pudieron guardar los cambios. Verifique sus datos.");
                    }
                }
            }
            else
            {
                // Si el ModelState no es válido, registra los errores
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _logger.LogWarning("ModelState inválido al editar información para PropietarioId {PropietarioId}: {Errors}",
                                     propietarioModel.IdPropietario, string.Join("; ", errors));
            }

            // Si ModelState es inválido o hubo un error al guardar, volvemos a mostrar el formulario
            // con los datos que el usuario envió (propietarioModel) para que corrija los errores.
            TempData["ShowEditForm"] = true; // Indica a la vista que mantenga el form de edición abierto
            return View("Configuracion", propietarioModel); // Pasa el modelo con errores de vuelta
        }


        // --- Acción para Cambiar Contraseña (POST /Propietario/CambiarContrasena) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarContrasena(int IdPropietario, string ContrasenaActual, string NuevaContrasena, string ConfirmarContrasena)
        {
            int? currentPropietarioId = GetCurrentPropietarioId();
            if (currentPropietarioId == null)
            {
                TempData["ErrorContrasena"] = "Sesión expirada o inválida."; // Usa la key de TempData de la vista
                                                                             // return RedirectToAction("Login", "Account");
                return RedirectToAction("Index", "Home");
            }

            // Seguridad: Verifica que el ID coincida
            if (IdPropietario != currentPropietarioId)
            {
                TempData["ErrorContrasena"] = "Intento de modificación no autorizado.";
                return RedirectToAction(nameof(Configuracion));
            }

            // Validación básica de campos no vacíos
            if (string.IsNullOrWhiteSpace(ContrasenaActual) || string.IsNullOrWhiteSpace(NuevaContrasena) || string.IsNullOrWhiteSpace(ConfirmarContrasena))
            {
                TempData["ErrorContrasena"] = "Todos los campos de contraseña son requeridos.";
                TempData["ShowPasswordForm"] = true; // Mantiene form abierto
                return RedirectToAction(nameof(Configuracion));
            }

            // Verifica que la nueva contraseña y la confirmación coincidan
            if (NuevaContrasena != ConfirmarContrasena)
            {
                TempData["ErrorContrasena"] = "La nueva contraseña y su confirmación no coinciden.";
                TempData["ShowPasswordForm"] = true;
                return RedirectToAction(nameof(Configuracion));
            }

            // Validación de complejidad (debe coincidir con la validación JS en la vista)
            string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$%^&+=!]).{8,}$";
            if (!Regex.IsMatch(NuevaContrasena, passwordPattern))
            {
                TempData["ErrorContrasena"] = "La nueva contraseña no cumple los requisitos de complejidad (mín. 8 caracteres, mayúscula, minúscula, número, símbolo #$%^&+=!).";
                TempData["ShowPasswordForm"] = true;
                return RedirectToAction(nameof(Configuracion));
            }


            // Busca al propietario
            var propietario = await _context.Propietarios.FindAsync(currentPropietarioId);
            if (propietario == null)
            {
                // Caso muy improbable
                return NotFound();
            }

            // --- ¡¡¡VERIFICACIÓN DE CONTRASEÑA ACTUAL!!! ---
            // Usa tu helper de hashing real aquí.
            bool passwordVerified = PasswordHelper.VerifyPasswordHash(ContrasenaActual, propietario.Contrasena);

            if (!passwordVerified)
            {
                TempData["ErrorContrasena"] = "La contraseña actual es incorrecta.";
                TempData["ShowPasswordForm"] = true;
                return RedirectToAction(nameof(Configuracion));
            }

            // --- ¡¡¡HASHING DE NUEVA CONTRASEÑA!!! ---
            // Usa tu helper de hashing real aquí.
            propietario.Contrasena = PasswordHelper.HashPassword(NuevaContrasena);

            try
            {
                _context.Update(propietario);
                await _context.SaveChangesAsync();
                TempData["ExitoContrasena"] = "Contraseña actualizada correctamente."; // Usa la key de TempData de la vista
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de BD al cambiar contraseña para PropietarioId {PropietarioId}", IdPropietario);
                TempData["ErrorContrasena"] = "Error al guardar la nueva contraseña.";
                TempData["ShowPasswordForm"] = true;
            }

            // Redirige de vuelta a la configuración, mostrando el mensaje de éxito o error
            return RedirectToAction(nameof(Configuracion));
        }

        // --- Acción para la vista DetallesDeLaCita (GET /Propietario/DetallesDeLaCita) ---
        // Esta vista carga datos vía AJAX, por lo que la acción solo necesita retornar la vista.
        public IActionResult DetallesDeLaCita()
        {
            // Verifica si el usuario está logueado (puede ser necesario para el _Layout)
            int? propietarioId = GetCurrentPropietarioId();
            if (propietarioId == null)
            {
                TempData["Error"] = "Debe iniciar sesión para consultar citas.";
                // return RedirectToAction("Login", "Account");
                return RedirectToAction("Index", "Home");
            }

            // No se necesita pasar modelo aquí, la vista usa JS para llamar a ConsultaCitaJson
            return View();
        }


        // --- Endpoint AJAX para DetallesDeLaCita (GET /Propietario/ConsultaCitaJson) ---
        [HttpGet]
        public async Task<IActionResult> ConsultaCitaJson(int idCita)
        {
            int? propietarioId = GetCurrentPropietarioId();
            if (propietarioId == null)
            {
                // Para AJAX, no redirigir, devolver error HTTP
                return Unauthorized(new { message = "Usuario no autenticado." });
            }

            // Busca la cita y datos relacionados, asegurando que pertenece al propietario
            var cita = await _context.Citas
                                 .Include(c => c.IdPropietarioNavigation)
                                 .Include(c => c.IdClinicaDestinoNavigation)
                                 .Include(c => c.IdClinicaOrigenNavigation) // Necesario para el modal
                                 .Include(c => c.IdVeterinarioAsignadoNavigation)
                                 .Include(c => c.HistorialCitas)
                                     .ThenInclude(h => h.IdPersonalModificacionNavigation) // Carga quién modificó en el historial
                                 .Where(c => c.IdCita == idCita && c.IdPropietario == propietarioId)
                                 .FirstOrDefaultAsync();

            if (cita == null)
            {
                return NotFound(new { message = "Cita no encontrada o no pertenece a usted." });
            }

            // Crea un objeto anónimo o DTO (Data Transfer Object) para enviar solo los datos necesarios
            var result = new
            {
                // Datos directos de la Cita
                idCita = cita.IdCita,
                motivoConsulta = cita.MotivoConsulta,
                estadoCita = cita.EstadoCita,
                fechaHoraCita = cita.FechaHoraCita, // Enviar como string ISO 8601 o DateTime, JS lo formateará
                pesoMascota = cita.PesoMascota,
                detallesMascota = cita.DetallesMascota,
                fechaCreacion = cita.FechaCreacion,

                // Datos del Propietario (si IdPropietarioNavigation no es null)
                propietario = cita.IdPropietarioNavigation == null ? null : new
                {
                    nombreCompleto = $"{cita.IdPropietarioNavigation.Nombre} {cita.IdPropietarioNavigation.Apellido}",
                    correo = cita.IdPropietarioNavigation.Correo
                },

                // Datos de Clínicas (si las navegaciones no son null)
                clinicaDestino = cita.IdClinicaDestinoNavigation == null ? null : new { nombre = cita.IdClinicaDestinoNavigation.Nombre },
                clinicaOrigen = cita.IdClinicaOrigenNavigation == null ? null : new { nombre = cita.IdClinicaOrigenNavigation.Nombre },

                // Datos del Veterinario (si IdVeterinarioAsignadoNavigation no es null)
                veterinarioAsignado = cita.IdVeterinarioAsignadoNavigation == null ? null : new
                {
                    nombreCompleto = $"{cita.IdVeterinarioAsignadoNavigation.Nombre} {cita.IdVeterinarioAsignadoNavigation.Apellido}",
                    telefono = cita.IdVeterinarioAsignadoNavigation.Telefono
                },

                // Historial de la Cita (mapeado a objetos más simples)
                historial = cita.HistorialCitas
                                .OrderByDescending(h => h.FechaCambio) // Ordena para JS
                                .Select(h => new {
                                    fechaCambio = h.FechaCambio, // Enviar como DateTime o ISO string
                                    estadoAnterior = h.EstadoAnterior,
                                    estadoNuevo = h.EstadoNuevo,
                                    modificadoPor = h.IdPersonalModificacionNavigation == null ? "Sistema/Propietario" : $"{h.IdPersonalModificacionNavigation.Nombre} {h.IdPersonalModificacionNavigation.Apellido}",
                                    ubicacionActual = h.UbicacionActual
                                }).ToList()

                // Puedes añadir aquí el conteo de notificaciones si esa lógica existe
                // numeroNotificaciones = await _context.Notificaciones.CountAsync(n => n.IdPropietario == propietarioId && !n.Leida),
            };

            // Devuelve los datos como JSON con estado 200 OK
            return Ok(result);
        }


        // --- Acción para la vista Notificaciones (GET /Propietario/Notificaciones) ---
        public IActionResult Notificaciones()
        {
            int? propietarioId = GetCurrentPropietarioId();
            if (propietarioId == null)
            {
                TempData["Error"] = "Debe iniciar sesión para ver notificaciones.";
                // return RedirectToAction("Login", "Account");
                return RedirectToAction("Index", "Home");
            }

            // --- Lógica de Notificaciones ---
            // La vista proporcionada tiene HTML estático y JS para marcar como leído (solo client-side).
            // Para una funcionalidad real, necesitarías:
            // 1. Un modelo `Notificacion` en FrontEnd.Models.
            // 2. Una tabla `Notificacion` en la base de datos.
            // 3. Lógica para crear notificaciones (ej. cambio de estado de cita, mensajes admin).
            // 4. Obtener las notificaciones del usuario actual aquí.
            // 5. Probablemente una acción POST para marcar notificaciones como leídas en la BD.

            // Ejemplo hipotético si tuvieras el modelo Notificacion:
            /*
            var notifications = await _context.Notificaciones
                                         .Where(n => n.IdPropietario == propietarioId) // O IdUsuario si es general
                                         .OrderByDescending(n => n.FechaCreacion)
                                         .Take(20) // Limita el número de notificaciones mostradas
                                         .ToListAsync();
            return View(notifications); // Pasa la lista a la vista
            */

            // Como no hay modelo/lógica definida, solo retornamos la vista estática.
            return View();
        }


        // --- Acción para la vista Pagos (GET /Propietario/Pagos) ---
        public IActionResult Pagos()
        {
            int? propietarioId = GetCurrentPropietarioId();
            if (propietarioId == null)
            {
                TempData["Error"] = "Debe iniciar sesión para ver su historial de pagos.";
                // return RedirectToAction("Login", "Account");
                return RedirectToAction("Index", "Home");
            }
            // La vista carga los datos mediante AJAX llamando a ObtenerPagosPorUsuario.
            // No se necesita pasar un modelo inicialmente.
            return View();
        }

        // --- Endpoint AJAX para la vista Pagos (GET /Propietario/ObtenerPagosPorUsuario) ---
        [HttpGet]
        public async Task<IActionResult> ObtenerPagosPorUsuario()
        {
            int? propietarioId = GetCurrentPropietarioId();
            if (propietarioId == null)
            {
                return Unauthorized(new { message = "Usuario no autenticado." });
            }

            var pagos = await _context.Pagos
                                    // ¡Importante! Filtra los pagos basándose en el IdPropietario de la Cita asociada.
                                    .Where(p => p.IdCitaNavigation != null && p.IdCitaNavigation.IdPropietario == propietarioId)
                                    .OrderByDescending(p => p.FechaPago) // Muestra los más recientes primero
                                    .Select(p => new {
                                        // Selecciona solo los campos necesarios para la tabla en la vista Pagos.cshtml
                                        idCita = p.IdCita,
                                        fechaPago = p.FechaPago, // Enviar como DateTime o ISO String
                                        monto = p.Monto,
                                        // --- MetodoPago ---
                                        // El modelo Pago no tiene MetodoPago. Añádelo o elimina esta columna de la vista/JS.
                                        // Si lo añades al modelo Pago:
                                        // metodoPago = p.MetodoPago ?? "No especificado"
                                        // Si NO lo añades, el JS fallará o mostrará 'undefined'. Deberías quitarlo del select y del JS.
                                        // Por ahora, asumiremos que no existe y lo comentamos o ponemos un placeholder:
                                        metodoPago = "N/D" // Placeholder ya que no está en el modelo Pago
                                    })
                                    .ToListAsync();

            return Ok(pagos); // Devuelve la lista de pagos como JSON
        }

        // --- Opcional: Acción para subir foto de perfil (si implementas esa funcionalidad) ---
        // Necesitarías añadir la propiedad FotoPerfil al modelo Propietario y lógica para guardar el archivo.
        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarFotoPerfil(IFormFile ProfileImage)
        {
            int? propietarioId = GetCurrentPropietarioId();
            if (propietarioId == null) return RedirectToAction("Login", "Account");

            if (ProfileImage != null && ProfileImage.Length > 0)
            {
                // 1. Validar tipo de archivo (jpg/png) y tamaño.
                // 2. Generar un nombre de archivo único.
                // 3. Definir la ruta de guardado (ej. wwwroot/imagenesperfil). Asegúrate que la carpeta exista.
                // 4. Guardar el archivo en el servidor.
                // 5. Actualizar la propiedad FotoPerfil (ruta o nombre de archivo) en el registro del Propietario en la BD.
                // 6. Guardar cambios en la BD.
                // 7. Añadir TempData de éxito/error.

                // Ejemplo (simplificado, necesita validación y manejo de errores robusto):
                // var fileName = $"{propietarioId}_{Guid.NewGuid()}{Path.GetExtension(ProfileImage.FileName)}";
                // var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagenesperfil", fileName);
                // using (var stream = new FileStream(filePath, FileMode.Create))
                // {
                //     await ProfileImage.CopyToAsync(stream);
                // }
                // var propietario = await _context.Propietarios.FindAsync(propietarioId);
                // if (propietario != null) {
                //     propietario.FotoPerfil = $"/imagenesperfil/{fileName}"; // Guarda la ruta relativa
                //     _context.Update(propietario);
                //     await _context.SaveChangesAsync();
                //     TempData["Success"] = "Foto de perfil actualizada.";
                // }
            }
            else
            {
                TempData["Error"] = "No se seleccionó ningún archivo.";
            }

            return RedirectToAction(nameof(Configuracion));
        }
        */

    }
}