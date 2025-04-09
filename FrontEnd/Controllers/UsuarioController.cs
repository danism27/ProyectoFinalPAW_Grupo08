using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FrontEnd.Models; // Namespace de modelos y Context
using Microsoft.Extensions.Logging; // Para logging

namespace FrontEnd.Controllers
{
    // Este controlador maneja Personal
    public class UsuarioController : Controller
    {
        private readonly ProyectoVeterinariaContext _context;
        private readonly ILogger<UsuarioController> _logger; // Añadido Logger

        public UsuarioController(ProyectoVeterinariaContext context, ILogger<UsuarioController> logger)
        {
            _context = context;
            _logger = logger; // Inyectar Logger
        }

        // GET: Usuario (Lista de Personal)
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Cargando índice de Personal.");
            // *** CORREGIDO: Usa propiedades de navegación correctas ***
            var personalList = await _context.Personal
                                             .Include(p => p.IdRolNavigation)
                                             .Include(p => p.IdClinicaNavigation)
                                             .ToListAsync();
            return View(personalList);
        }

        // GET: Usuario/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) { /* NotFound */ }

            _logger.LogInformation("Buscando Details para Personal ID {PersonalId}.", id);
            // *** CORREGIDO: Usa propiedades de navegación correctas y PK ***
            // *** CORREGIDO: ThenInclude usa IdCitaNavigation ***
            var personal = await _context.Personal
                .Include(p => p.IdRolNavigation)
                .Include(p => p.IdClinicaNavigation)
                .Include(p => p.CitaPersonalClinicas) // Colección de asignaciones
                    .ThenInclude(cpc => cpc.IdCitaNavigation) // Navegación de asignación a Cita
                                                              // Incluir otras colecciones si se muestran en Details
                .Include(p => p.HistorialCitas) // Donde este personal modificó
                .Include(p => p.Notificaciones) // Si aplica
                                                // *** CORREGIDO: Usa IdPersonal como PK ***
                .FirstOrDefaultAsync(m => m.IdPersonal == id);

            if (personal == null) { /* NotFound */ }

            return View(personal);
        }

        // GET: Usuario/Create
        public IActionResult Create()
        {
            _logger.LogInformation("Mostrando formulario de creación de Personal.");
            // *** CORREGIDO: Usa Nombre para Clinica ***
            ViewData["IdRol"] = new SelectList(_context.Roles, "IdRol", "NombreRol"); // Asume NombreRol
            ViewData["IdClinica"] = new SelectList(_context.Clinicas, "IdClinica", "Nombre");
            return View();
        }

        // POST: Usuario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // *** CORREGIDO: Ajusta Bind a propiedades existentes. Quita Contraseña, Cedula, Direccion, Especialidad ***
        public async Task<IActionResult> Create([Bind("IdPersonal,Nombre,Apellido,Correo,Telefono,IdRol,IdClinica")] Personal personal)
        // Nota: Se asume que 'Contrasenna' se establece en un proceso de registro dedicado.
        {
            // Quitar propiedades de navegación y contraseña
            ModelState.Remove("IdRolNavigation");
            ModelState.Remove("IdClinicaNavigation");
            ModelState.Remove("HistorialCitas");
            ModelState.Remove("Notificaciones");
            ModelState.Remove("CitaIdVeterinarioAsignadoNavigations");
            ModelState.Remove("CitaPersonalClinicas");
            ModelState.Remove("Contrasenna");

            if (ModelState.IsValid)
            {
                try
                {
                    // Lógica de Hashing de contraseña iría aquí si aplica
                    // personal.Contrasenna = HashPassword(contraseñaDelFormulario);

                    _context.Add(personal);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Personal creado con ID {PersonalId}.", personal.IdPersonal);
                    TempData["SuccessMessage"] = "Personal creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Error DbUpdateException al crear Personal.");
                    ModelState.AddModelError("", "No se pudo guardar. Verifique correo no duplicado y Rol/Clínica válidos.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error inesperado al crear Personal.");
                    ModelState.AddModelError("", "Ocurrió un error inesperado.");
                }
            }
            else { _logger.LogWarning("ModelState inválido al crear Personal."); }

            // Recargar SelectLists si hay error
            ViewData["IdRol"] = new SelectList(_context.Roles, "IdRol", "NombreRol", personal.IdRol);
            ViewData["IdClinica"] = new SelectList(_context.Clinicas, "IdClinica", "Nombre", personal.IdClinica);
            return View(personal);
        }

        // GET: Usuario/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) { /* NotFound */ }

            // *** CORREGIDO: Usa IdPersonal como PK ***
            var personal = await _context.Personal.FindAsync(id);
            if (personal == null) { /* NotFound */ }

            _logger.LogInformation("Mostrando formulario Edit para Personal ID {PersonalId}.", id);
            // *** CORREGIDO: Usa Nombre para Clinica ***
            ViewData["IdRol"] = new SelectList(_context.Roles, "IdRol", "NombreRol", personal.IdRol);
            ViewData["IdClinica"] = new SelectList(_context.Clinicas, "IdClinica", "Nombre", personal.IdClinica);
            return View(personal);
        }

        // POST: Usuario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        // *** CORREGIDO: Ajusta Bind a propiedades existentes. Quita Contraseña, Cedula, Direccion, Especialidad ***
        public async Task<IActionResult> Edit(int id, [Bind("IdPersonal,Nombre,Apellido,Correo,Telefono,IdRol,IdClinica")] Personal personal)
        {
            // *** CORREGIDO: Usa IdPersonal como PK ***
            if (id != personal.IdPersonal) { return NotFound(); }

            // *** CORREGIDO: Usa IdPersonal como PK ***
            var personalExistente = await _context.Personal.FindAsync(id);
            if (personalExistente == null) { /* NotFound */ }

            // Quitar navegaciones y contraseña
            ModelState.Remove("IdRolNavigation");
            ModelState.Remove("IdClinicaNavigation");
            ModelState.Remove("HistorialCitas");
            ModelState.Remove("Notificaciones");
            ModelState.Remove("CitaIdVeterinarioAsignadoNavigations");
            ModelState.Remove("CitaPersonalClinicas");
            ModelState.Remove("Contrasenna");

            if (ModelState.IsValid)
            {
                try
                {
                    // Copiar valores modificados, manteniendo contraseña original
                    _context.Entry(personalExistente).CurrentValues.SetValues(personal);
                    // Asegurar FKs si estaban en Bind
                    personalExistente.IdRol = personal.IdRol;
                    personalExistente.IdClinica = personal.IdClinica;

                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Personal ID {PersonalId} actualizado.", id);
                    TempData["SuccessMessage"] = "Personal actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Error Concurrencia al actualizar Personal ID {PersonalId}.", id);
                    // *** CORREGIDO: Usa IdPersonal como PK ***
                    if (!PersonalExists(personal.IdPersonal)) { return NotFound(); }
                    else { ModelState.AddModelError("", "Registro modificado. Recargue."); }
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Error DbUpdateException al actualizar Personal ID {PersonalId}.", id);
                    ModelState.AddModelError("", "No se pudo guardar. Verifique correo y FKs.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error inesperado al actualizar Personal ID {PersonalId}.", id);
                    ModelState.AddModelError("", "Error inesperado.");
                }

                if (ModelState.ErrorCount == 0)
                    return RedirectToAction(nameof(Index));
            }
            else { _logger.LogWarning("ModelState inválido al actualizar Personal ID {PersonalId}.", id); }

            // Recargar SelectLists si hay error
            ViewData["IdRol"] = new SelectList(_context.Roles, "IdRol", "NombreRol", personal.IdRol);
            ViewData["IdClinica"] = new SelectList(_context.Clinicas, "IdClinica", "Nombre", personal.IdClinica);
            return View(personal); // Devolver modelo con errores
        }

        // GET: Usuario/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) { /* NotFound */ }

            _logger.LogInformation("Mostrando confirmación Delete para Personal ID {PersonalId}.", id);
            // *** CORREGIDO: Usa propiedades de navegación y PK correctas ***
            var personal = await _context.Personal
                .Include(p => p.IdRolNavigation)
                .Include(p => p.IdClinicaNavigation)
                .FirstOrDefaultAsync(m => m.IdPersonal == id); // PK IdPersonal

            if (personal == null) { /* NotFound */ }

            return View(personal);
        }

        // POST: Usuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation("Confirmado Delete para Personal ID {PersonalId}.", id);
            // *** CORREGIDO: Usa IdPersonal como PK ***
            // Cargar Personal con dependencias a verificar/modificar
            var personal = await _context.Personal
                                         .Include(p => p.CitaPersonalClinicas) // Asignaciones a citas
                                         .Include(p => p.HistorialCitas)     // Modificaciones hechas
                                         .Include(p => p.CitaIdVeterinarioAsignadoNavigations) // Citas donde es vet. principal
                                         .Include(p => p.Notificaciones)    // Notificaciones
                                         .FirstOrDefaultAsync(p => p.IdPersonal == id);

            if (personal != null)
            {
                // --- VERIFICAR DEPENDENCIAS ---
                // 1. ¿Está asignado como personal en CitaPersonalClinica?
                if (personal.CitaPersonalClinicas.Any())
                {
                    TempData["ErrorMessage"] = "No se puede eliminar. Personal asignado a citas activas.";
                    return RedirectToAction(nameof(Delete), new { id = id });
                }
                // 2. ¿Es el veterinario principal en alguna Cita?
                if (personal.CitaIdVeterinarioAsignadoNavigations.Any())
                {
                    TempData["ErrorMessage"] = "No se puede eliminar. Veterinario asignado principal en citas.";
                    // Podrías poner a null esta FK en las citas afectadas si es nullable
                    // foreach(var citaAfectada in personal.CitaIdVeterinarioAsignadoNavigations) { citaAfectada.IdVeterinarioAsignado = null; }
                    // _context.UpdateRange(personal.CitaIdVeterinarioAsignadoNavigations);
                    return RedirectToAction(nameof(Delete), new { id = id });
                }
                // Considera otras dependencias (Notificaciones, Pagos si están ligados a Personal, etc.)

                // --- SI NO HAY BLOQUEOS, PROCEDER ---
                try
                {
                    // 3. Poner a NULL referencias en HistorialCita
                    // *** CORREGIDO: Usa IdPersonalModificacion ***
                    // No necesitamos cargar explícitamente los historiales aquí si ya los incluimos arriba
                    if (personal.HistorialCitas.Any())
                    {
                        _logger.LogInformation("Desasociando Personal ID {PersonalId} de {Count} registros de HistorialCita.", id, personal.HistorialCitas.Count);
                        foreach (var h in personal.HistorialCitas)
                        {
                            h.IdPersonalModificacion = null; // Poner FK a null
                        }
                        _context.UpdateRange(personal.HistorialCitas); // Marcar historiales como modificados
                    }

                    // 4. Borrar Notificaciones asociadas (si aplica)
                    if (personal.Notificaciones.Any())
                    {
                        _logger.LogInformation("Eliminando {Count} Notificaciones asociadas a Personal ID {PersonalId}.", personal.Notificaciones.Count, id);
                        _context.Notificaciones.RemoveRange(personal.Notificaciones);
                    }

                    // 5. Eliminar el registro de Personal
                    _context.Personal.Remove(personal);

                    await _context.SaveChangesAsync(); // Guardar todos los cambios
                    _logger.LogInformation("Personal ID {PersonalId} eliminado.", id);
                    TempData["SuccessMessage"] = "Personal eliminado exitosamente.";
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Error DbUpdateException al eliminar Personal ID {PersonalId}.", id);
                    TempData["ErrorMessage"] = "Error al eliminar. Verifique dependencias restantes.";
                    return RedirectToAction(nameof(Delete), new { id = id });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error inesperado al eliminar Personal ID {PersonalId}.", id);
                    TempData["ErrorMessage"] = "Error inesperado al eliminar.";
                    return RedirectToAction(nameof(Delete), new { id = id });
                }
            }
            else
            {
                _logger.LogWarning("No se encontró Personal ID {PersonalId} para DeleteConfirmed.", id);
                TempData["ErrorMessage"] = "Personal no encontrado.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PersonalExists(int id)
        {
            // *** CORREGIDO: Usa IdPersonal como PK ***
            return _context.Personal.Any(e => e.IdPersonal == id);
        }
    }
}