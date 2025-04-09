using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FrontEnd.Models; 
using Microsoft.Extensions.Logging; // Para logging

namespace FrontEnd.Controllers
{
    // Este controlador maneja TODO el Personal (puedes llamarlo UsuarioPersonalController si prefieres)
    public class UsuarioPersonalController : Controller // <--- Nombre de clase cambiado
    {
        private readonly ProyectoVeterinariaContext _context;
        private readonly ILogger<UsuarioPersonalController> _logger; // Logger con el nombre de la clase

        // Constructor con el nombre de la clase actualizado
        public UsuarioPersonalController(ProyectoVeterinariaContext context, ILogger<UsuarioPersonalController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: UsuarioPersonal (Lista de TODO el Personal)
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Cargando índice de Personal.");
            // Usa _context.Personal y _context.Roles
            // Usa IdRolNavigation y IdClinicaNavigation
            var personalList = await _context.Personal // DbSet singular
                                             .Include(p => p.IdRolNavigation)
                                             .Include(p => p.IdClinicaNavigation)
                                             .ToListAsync();
            return View(personalList);
        }

        // GET: UsuarioPersonal/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) { /* NotFound */ }

            _logger.LogInformation("Buscando Details para Personal ID {PersonalId}.", id);
            // Usa _context.Personal, PK IdPersonal, y navegaciones correctas
            var personal = await _context.Personal // DbSet singular
                .Include(p => p.IdRolNavigation)
                .Include(p => p.IdClinicaNavigation)
                .Include(p => p.CitaPersonalClinicas)
                    .ThenInclude(cpc => cpc.IdCitaNavigation) // Navegación correcta
                .Include(p => p.HistorialCitas)
                    .ThenInclude(h => h.IdPersonalModificacionNavigation) // Navegación correcta
                .Include(p => p.Notificaciones)
                .Include(p => p.CitaIdVeterinarioAsignadoNavigations) // Si es relevante mostrar
                .FirstOrDefaultAsync(m => m.IdPersonal == id); // PK correcta

            if (personal == null) { /* NotFound */ }

            return View(personal);
        }

        // GET: UsuarioPersonal/Create
        public IActionResult Create()
        {
            _logger.LogInformation("Mostrando formulario de creación de Personal.");
            // Usa _context.Roles y Nombre para Clinica
            ViewData["IdRol"] = new SelectList(_context.Roles, "IdRol", "NombreRol"); // DbSet plural
            ViewData["IdClinica"] = new SelectList(_context.Clinicas, "IdClinica", "Nombre"); // Propiedad Nombre
            return View();
        }

        // POST: UsuarioPersonal/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Bind con propiedades correctas (Apellido singular, sin Cedula, Direccion, Especialidad, Contrasenna)
        public async Task<IActionResult> Create([Bind("IdPersonal,Nombre,Apellido,Correo,Telefono,IdRol,IdClinica")] Personal personal)
        {
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
                    // Lógica Hashing iría aquí
                    _context.Personal.Add(personal); // DbSet singular
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Personal creado con ID {PersonalId}.", personal.IdPersonal);
                    TempData["SuccessMessage"] = "Personal creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex) { /* ... Log y error ... */ }
                catch (Exception ex) { /* ... Log y error ... */ }
            }
            else { _logger.LogWarning("ModelState inválido al crear Personal."); }

            // Recargar SelectLists
            ViewData["IdRol"] = new SelectList(_context.Roles, "IdRol", "NombreRol", personal.IdRol); // DbSet plural
            ViewData["IdClinica"] = new SelectList(_context.Clinicas, "IdClinica", "Nombre", personal.IdClinica); // Propiedad Nombre
            return View(personal);
        }

        // GET: UsuarioPersonal/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) { /* NotFound */ }

            // Usa PK IdPersonal
            var personal = await _context.Personal.FindAsync(id); // DbSet singular
            if (personal == null) { /* NotFound */ }

            _logger.LogInformation("Mostrando formulario Edit para Personal ID {PersonalId}.", id);
            // Usa Nombre para Clinica
            ViewData["IdRol"] = new SelectList(_context.Roles, "IdRol", "NombreRol", personal.IdRol); // DbSet plural
            ViewData["IdClinica"] = new SelectList(_context.Clinicas, "IdClinica", "Nombre", personal.IdClinica); // Propiedad Nombre
            return View(personal);
        }

        // POST: UsuarioPersonal/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Bind con propiedades correctas
        public async Task<IActionResult> Edit(int id, [Bind("IdPersonal,Nombre,Apellido,Correo,Telefono,IdRol,IdClinica")] Personal personal)
        {
            // Usa PK IdPersonal
            if (id != personal.IdPersonal) { return NotFound(); }

            var personalExistente = await _context.Personal.FindAsync(id); // DbSet singular
            if (personalExistente == null) { /* NotFound */ }

            // Quitar navegaciones y contraseña
            ModelState.Remove("IdRolNavigation");
            ModelState.Remove("IdClinicaNavigation");
            // ... (quitar otras navegaciones como en Create) ...
            ModelState.Remove("Contrasenna");

            if (ModelState.IsValid)
            {
                try
                {
                    // Copiar valores, manteniendo contraseña original
                    _context.Entry(personalExistente).CurrentValues.SetValues(personal);
                    personalExistente.IdRol = personal.IdRol;
                    personalExistente.IdClinica = personal.IdClinica;

                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Personal ID {PersonalId} actualizado.", id);
                    TempData["SuccessMessage"] = "Personal actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException ex) { /* ... Log y error concurrencia ... */ }
                catch (DbUpdateException ex) { /* ... Log y error DB ... */ }
                catch (Exception ex) { /* ... Log y error inesperado ... */ }

                if (ModelState.ErrorCount == 0)
                    return RedirectToAction(nameof(Index));
            }
            else { _logger.LogWarning("ModelState inválido al actualizar Personal ID {PersonalId}.", id); }

            // Recargar SelectLists
            ViewData["IdRol"] = new SelectList(_context.Roles, "IdRol", "NombreRol", personal.IdRol); // DbSet plural
            ViewData["IdClinica"] = new SelectList(_context.Clinicas, "IdClinica", "Nombre", personal.IdClinica); // Propiedad Nombre
            return View(personal);
        }

        // GET: UsuarioPersonal/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) { /* NotFound */ }

            _logger.LogInformation("Mostrando confirmación Delete para Personal ID {PersonalId}.", id);
            // Usa PK IdPersonal y navegaciones correctas
            var personal = await _context.Personal // DbSet singular
                .Include(p => p.IdRolNavigation)
                .Include(p => p.IdClinicaNavigation)
                .FirstOrDefaultAsync(m => m.IdPersonal == id); // PK IdPersonal

            if (personal == null) { /* NotFound */ }

            return View(personal);
        }

        // POST: UsuarioPersonal/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation("Confirmado Delete para Personal ID {PersonalId}.", id);
            // Usa PK IdPersonal
            var personal = await _context.Personal // DbSet singular
                                         .Include(p => p.CitaPersonalClinicas)
                                         .Include(p => p.HistorialCitas)
                                         .Include(p => p.CitaIdVeterinarioAsignadoNavigations)
                                         .Include(p => p.Notificaciones)
                                         .FirstOrDefaultAsync(p => p.IdPersonal == id); // PK IdPersonal

            if (personal != null)
            {
                // --- VERIFICAR DEPENDENCIAS ---
                if (personal.CitaPersonalClinicas.Any())
                {
                    TempData["ErrorMessage"] = "No se puede eliminar. Personal asignado a citas.";
                    return RedirectToAction(nameof(Delete), new { id = id });
                }
                if (personal.CitaIdVeterinarioAsignadoNavigations.Any())
                {
                    TempData["ErrorMessage"] = "No se puede eliminar. Veterinario asignado principal en citas.";
                    return RedirectToAction(nameof(Delete), new { id = id });
                }

                try
                {
                    // --- SI NO HAY BLOQUEOS, PROCEDER ---
                    // Poner a NULL referencias en HistorialCita usando IdPersonalModificacion
                    if (personal.HistorialCitas.Any())
                    {
                        _logger.LogInformation("Desasociando Personal ID {PersonalId} de {Count} HistorialCita.", id, personal.HistorialCitas.Count);
                        foreach (var h in personal.HistorialCitas) { h.IdPersonalModificacion = null; }
                        _context.UpdateRange(personal.HistorialCitas);
                    }
                    // Borrar Notificaciones
                    if (personal.Notificaciones.Any())
                    {
                        _logger.LogInformation("Eliminando {Count} Notificaciones asociadas a Personal ID {PersonalId}.", personal.Notificaciones.Count, id);
                        _context.Notificaciones.RemoveRange(personal.Notificaciones);
                    }

                    _context.Personal.Remove(personal); // DbSet singular
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Personal ID {PersonalId} eliminado.", id);
                    TempData["SuccessMessage"] = "Personal eliminado exitosamente.";
                }
                catch (DbUpdateException ex) { /* ... Log y error ... */ }
                catch (Exception ex) { /* ... Log y error ... */ }
            }
            else { /* ... Log y error/mensaje ... */ }

            return RedirectToAction(nameof(Index));
        }

        private bool PersonalExists(int id)
        {
            // Usa PK IdPersonal
            return _context.Personal.Any(e => e.IdPersonal == id); // DbSet singular
        }
    }
}