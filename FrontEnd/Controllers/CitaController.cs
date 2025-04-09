using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FrontEnd.Models; // Namespace de tus modelos y Context
using Microsoft.Extensions.Logging;
// using System.Security.Claims; // Para obtener User ID si usas Identity

namespace ProyectoFinal.Controllers
{
    // [Authorize]
    public class CitaController : Controller
    {
        private readonly ProyectoVeterinariaContext _context;
        private readonly ILogger<CitaController> _logger;

        public CitaController(ProyectoVeterinariaContext context, ILogger<CitaController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Helper para obtener ID de usuario (si usas autenticación)
        // private int? GetCurrentUserId()
        // {
        //     var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier); // O el ClaimType que uses
        //     if (int.TryParse(userIdClaim, out int userId))
        //     {
        //         return userId;
        //     }
        //     return null;
        // }

        // GET: Cita
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Cargando índice de Citas.");
            var citas = await _context.Citas
                                      .Include(c => c.IdPropietarioNavigation) // OK
                                      .Include(c => c.IdClinicaOrigenNavigation) // OK
                                      .ToListAsync();
            return View(citas);
        }

        // GET: Cita/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) { /* ... NotFound ... */ }

            _logger.LogInformation("Buscando Details para Cita ID {CitaId}.", id);
            var cita = await _context.Citas
                .Include(c => c.IdPropietarioNavigation) // OK
                .Include(c => c.IdClinicaOrigenNavigation) // OK
                .Include(c => c.IdClinicaDestinoNavigation)
                .Include(c => c.HistorialCitas)
                    // *** CORREGIDO: Usa IdPersonalModificacionNavigation ***
                    .ThenInclude(h => h.IdPersonalModificacionNavigation)
                .Include(c => c.CitaPersonalClinicas)
                    // *** CORREGIDO: Usa IdPersonalNavigation ***
                    .ThenInclude(cpc => cpc.IdPersonalNavigation)
                .Include(c => c.IdVeterinarioAsignadoNavigation)
                .FirstOrDefaultAsync(m => m.IdCita == id);

            if (cita == null) { /* ... NotFound ... */ }

            _logger.LogInformation("Mostrando Details para Cita ID {CitaId}.", id);
            return View(cita);
        }

        // GET: Cita/Create
        public IActionResult Create()
        {
            _logger.LogInformation("Mostrando formulario de creación de Cita.");

            // *** MEJORADO: DisplayMember para Propietario ***
            ViewData["IdPropietario"] = new SelectList(_context.Propietarios
                .Select(p => new { p.IdPropietario, NombreCompleto = p.Nombre + " " + p.Apellido }),
                "IdPropietario", "NombreCompleto");

            ViewData["IdClinicaOrigen"] = new SelectList(_context.Clinicas, "IdClinica", "Nombre");
            // ViewData["IdClinicaDestino"] = new SelectList(_context.Clinicas, "IdClinica", "Nombre");
            // ViewData["IdVeterinarioAsignado"] = new SelectList(_context.Personals.Select(p => new {p.IdPersonal, NombreCompleto = p.Nombre + " " + p.Apellido}), "IdPersonal", "NombreCompleto");

            // *** MEJORADO: DisplayMember para Personal (MultiSelectList) ***
            ViewData["PersonalIds"] = new MultiSelectList(_context.Personal
                .Select(p => new { p.IdPersonal, NombreCompleto = p.Nombre + " " + p.Apellido }),
                "IdPersonal", "NombreCompleto");

            return View();
        }

        // POST: Cita/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCita,FechaHoraCita,MotivoConsulta,EstadoCita,IdPropietario,IdClinicaOrigen,PesoMascota,DetallesMascota,IdClinicaDestino,IdVeterinarioAsignado")] Cita cita, List<int> PersonalIds)
        {
            ModelState.Remove("IdPropietarioNavigation");
            ModelState.Remove("IdClinicaOrigenNavigation");
            ModelState.Remove("IdClinicaDestinoNavigation");
            ModelState.Remove("IdVeterinarioAsignadoNavigation");
            ModelState.Remove("HistorialCitas");
            ModelState.Remove("CitaPersonalClinicas");
            ModelState.Remove("Pagos");
            ModelState.Remove("FechaCreacion");

            if (ModelState.IsValid)
            {
                try
                {
                    cita.FechaCreacion = DateTime.Now;
                    _context.Add(cita);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Cita creada con ID {CitaId}.", cita.IdCita);
                    TempData["SuccessMessage"] = "Cita creada exitosamente.";

                    // --- Crear historial ---
                    var historial = new HistorialCita
                    {
                        IdCita = cita.IdCita,
                        FechaCambio = DateTime.Now,
                        EstadoAnterior = null, // No hay estado anterior al crear
                        EstadoNuevo = cita.EstadoCita,
                        // DescripcionCambio = $"Cita creada. Estado: {cita.EstadoCita ?? "N/A"}", // Opcional si ya tienes los estados
                        // UbicacionActual = // Si aplica, de dónde viene este dato?
                        IdPersonalModificacion = null // GetCurrentUserId() // Asigna el ID del usuario logueado
                    };
                    _context.Add(historial);

                    // --- Asignar personal de CitaPersonalClinica ---
                    if (PersonalIds != null && PersonalIds.Any())
                    {
                        foreach (var personalId in PersonalIds)
                        {
                            var asignacion = new CitaPersonalClinica
                            {
                                IdCita = cita.IdCita,
                                IdPersonal = personalId,
                                IdClinica = cita.IdClinicaOrigen, // O la lógica que necesites
                                FechaAsignacion = DateTime.Now // Asignar fecha de asignación
                            };
                            _context.CitaPersonalClinicas.Add(asignacion);
                        }
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex) { _logger.LogError(ex, "Error DbUpdateException al crear Cita."); ModelState.AddModelError("", "No se pudo guardar. Verifique FKs."); }
                catch (Exception ex) { _logger.LogError(ex, "Error inesperado al crear Cita."); ModelState.AddModelError("", "Error inesperado."); }
            }
            else { _logger.LogWarning("ModelState inválido al crear Cita."); }

            // Recargar datos si falla
            ViewData["IdPropietario"] = new SelectList(_context.Propietarios
               .Select(p => new { p.IdPropietario, NombreCompleto = p.Nombre + " " + p.Apellido }),
               "IdPropietario", "NombreCompleto", cita.IdPropietario);
            ViewData["IdClinicaOrigen"] = new SelectList(_context.Clinicas, "IdClinica", "Nombre", cita.IdClinicaOrigen);
            // ViewData["IdClinicaDestino"] = new SelectList(_context.Clinicas, "IdClinica", "Nombre", cita.IdClinicaDestino);
            // ViewData["IdVeterinarioAsignado"] = new SelectList(_context.Personals.Select(p => new {p.IdPersonal, NombreCompleto = p.Nombre + " " + p.Apellido}), "IdPersonal", "NombreCompleto", cita.IdVeterinarioAsignado);
            ViewData["PersonalIds"] = new MultiSelectList(_context.Personal
                .Select(p => new { p.IdPersonal, NombreCompleto = p.Nombre + " " + p.Apellido }),
                "IdPersonal", "NombreCompleto", PersonalIds);
            return View(cita);
        }

        // GET: Cita/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) { /* ... NotFound ... */ }

            var cita = await _context.Citas
                                     .Include(c => c.CitaPersonalClinicas)
                                     .FirstOrDefaultAsync(c => c.IdCita == id);

            if (cita == null) { /* ... NotFound ... */ }

            _logger.LogInformation("Mostrando formulario Edit para Cita ID {CitaId}.", id);
            ViewData["IdPropietario"] = new SelectList(_context.Propietarios
               .Select(p => new { p.IdPropietario, NombreCompleto = p.Nombre + " " + p.Apellido }),
               "IdPropietario", "NombreCompleto", cita.IdPropietario);
            ViewData["IdClinicaOrigen"] = new SelectList(_context.Clinicas, "IdClinica", "Nombre", cita.IdClinicaOrigen);
            // ViewData["IdClinicaDestino"] = new SelectList(_context.Clinicas, "IdClinica", "Nombre", cita.IdClinicaDestino);
            // ViewData["IdVeterinarioAsignado"] = new SelectList(_context.Personals.Select(p => new {p.IdPersonal, NombreCompleto = p.Nombre + " " + p.Apellido}), "IdPersonal", "NombreCompleto", cita.IdVeterinarioAsignado);

            var personalActualIds = cita.CitaPersonalClinicas.Select(cpc => cpc.IdPersonal).ToList();
            ViewData["PersonalIds"] = new MultiSelectList(_context.Personal
               .Select(p => new { p.IdPersonal, NombreCompleto = p.Nombre + " " + p.Apellido }),
               "IdPersonal", "NombreCompleto", personalActualIds);

            return View(cita);
        }

        // POST: Cita/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCita,FechaHoraCita,MotivoConsulta,EstadoCita,IdPropietario,IdClinicaOrigen,PesoMascota,DetallesMascota,IdClinicaDestino,IdVeterinarioAsignado")] Cita cita, List<int> PersonalIds)
        {
            if (id != cita.IdCita) { /* ... NotFound ... */ }

            ModelState.Remove("IdPropietarioNavigation");
            ModelState.Remove("IdClinicaOrigenNavigation");
            ModelState.Remove("IdClinicaDestinoNavigation");
            ModelState.Remove("IdVeterinarioAsignadoNavigation");
            ModelState.Remove("HistorialCitas");
            ModelState.Remove("CitaPersonalClinicas");
            ModelState.Remove("Pagos");
            ModelState.Remove("FechaCreacion"); // No se debe editar

            if (ModelState.IsValid)
            {
                // Obtener el estado anterior ANTES de hacer el Update
                var estadoAnterior = await _context.Citas.AsNoTracking()
                                                 .Where(c => c.IdCita == id)
                                                 .Select(c => c.EstadoCita)
                                                 .FirstOrDefaultAsync();
                try
                {
                    _context.Update(cita); // Marca la cita como modificada

                    // --- Actualizar CitaPersonalClinica ---
                    var asignacionesActuales = await _context.CitaPersonalClinicas.Where(cpc => cpc.IdCita == id).ToListAsync();
                    var idsPersonalActual = asignacionesActuales.Select(a => a.IdPersonal ?? 0).ToList();
                    var asignacionesAEliminar = asignacionesActuales.Where(a => !PersonalIds.Contains(a.IdPersonal ?? 0)).ToList();
                    if (asignacionesAEliminar.Any()) _context.CitaPersonalClinicas.RemoveRange(asignacionesAEliminar);
                    var idsPersonalAAgregar = PersonalIds.Except(idsPersonalActual).ToList();
                    foreach (var personalId in idsPersonalAAgregar)
                    {
                        var nuevaAsignacion = new CitaPersonalClinica
                        {
                            IdCita = id,
                            IdPersonal = personalId,
                            IdClinica = cita.IdClinicaOrigen, // O lógica necesaria
                            FechaAsignacion = DateTime.Now
                        };
                        _context.CitaPersonalClinicas.Add(nuevaAsignacion);
                    }

                    // --- Crear historial si cambió estado ---
                    if (estadoAnterior != cita.EstadoCita)
                    {
                        var historial = new HistorialCita
                        {
                            IdCita = cita.IdCita,
                            FechaCambio = DateTime.Now,
                            EstadoAnterior = estadoAnterior, // Estado antes del Update
                            EstadoNuevo = cita.EstadoCita,   // Estado después del Update
                                                             // DescripcionCambio = $"Estado cambiado...", // Opcional
                                                             // UbicacionActual = // Si aplica
                            IdPersonalModificacion = null // GetCurrentUserId()
                        };
                        _context.Add(historial);
                    }

                    await _context.SaveChangesAsync(); // Guarda Cita, CitaPersonalClinica, HistorialCita
                    _logger.LogInformation("Cita ID {CitaId} actualizada.", cita.IdCita);
                    TempData["SuccessMessage"] = "Cita actualizada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex) { /* ... Log y error concurrencia ... */ }
                catch (DbUpdateException ex) { /* ... Log y error DB ... */ }
                catch (Exception ex) { /* ... Log y error inesperado ... */ }
            }
            else { _logger.LogWarning("ModelState inválido al actualizar Cita ID {CitaId}.", cita.IdCita); }

            // Recargar datos si falla
            ViewData["IdPropietario"] = new SelectList(_context.Propietarios
               .Select(p => new { p.IdPropietario, NombreCompleto = p.Nombre + " " + p.Apellido }),
               "IdPropietario", "NombreCompleto", cita.IdPropietario);
            ViewData["IdClinicaOrigen"] = new SelectList(_context.Clinicas, "IdClinica", "Nombre", cita.IdClinicaOrigen);
            // ViewData["IdClinicaDestino"] = new SelectList(_context.Clinicas, "IdClinica", "Nombre", cita.IdClinicaDestino);
            // ViewData["IdVeterinarioAsignado"] = new SelectList(_context.Personals.Select(p => new {p.IdPersonal, NombreCompleto = p.Nombre + " " + p.Apellido}), "IdPersonal", "NombreCompleto", cita.IdVeterinarioAsignado);
            var currentPersonalIds = await _context.CitaPersonalClinicas.Where(cpc => cpc.IdCita == id).Select(cpc => cpc.IdPersonal).ToListAsync();
            ViewData["PersonalIds"] = new MultiSelectList(_context.Personal
               .Select(p => new { p.IdPersonal, NombreCompleto = p.Nombre + " " + p.Apellido }),
               "IdPersonal", "NombreCompleto", currentPersonalIds);
            return View(cita);
        }

        // GET: Cita/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) { /* ... NotFound ... */ }

            _logger.LogInformation("Mostrando confirmación Delete para Cita ID {CitaId}.", id);
            var cita = await _context.Citas
                .Include(c => c.IdPropietarioNavigation) // OK
                .Include(c => c.IdClinicaOrigenNavigation) // OK
                .FirstOrDefaultAsync(m => m.IdCita == id);

            if (cita == null) { /* ... NotFound ... */ }

            return View(cita);
        }

        // POST: Cita/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation("Confirmado Delete para Cita ID {CitaId}.", id);
            var cita = await _context.Citas
                                     .Include(c => c.HistorialCitas)
                                     .Include(c => c.CitaPersonalClinicas)
                                     .Include(c => c.Pagos)
                                     .FirstOrDefaultAsync(c => c.IdCita == id);

            if (cita == null) { /* ... Error, redirect ... */ }

            try
            {
                // Borrado explícito de dependencias
                if (cita.HistorialCitas.Any()) _context.HistorialCitas.RemoveRange(cita.HistorialCitas);
                if (cita.CitaPersonalClinicas.Any()) _context.CitaPersonalClinicas.RemoveRange(cita.CitaPersonalClinicas);
                if (cita.Pagos.Any()) _context.Pagos.RemoveRange(cita.Pagos);

                _context.Citas.Remove(cita);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Cita ID {CitaId} eliminada.", id);
                TempData["SuccessMessage"] = "Cita eliminada exitosamente.";
            }
            catch (DbUpdateException ex) { /* ... Log y error DB ... */ }
            catch (Exception ex) { /* ... Log y error inesperado ... */ }

            return RedirectToAction(nameof(Index));
        }

        private bool CitaExists(int id)
        {
            return _context.Citas.Any(e => e.IdCita == id);
        }
    }
}