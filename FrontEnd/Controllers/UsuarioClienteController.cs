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
    // Este controlador maneja Propietarios
    public class UsuarioClienteController : Controller
    {
        private readonly ProyectoVeterinariaContext _context;
        private readonly ILogger<UsuarioClienteController> _logger; // Añadido Logger

        public UsuarioClienteController(ProyectoVeterinariaContext context, ILogger<UsuarioClienteController> logger)
        {
            _context = context;
            _logger = logger; // Inyectar Logger
        }

        // GET: UsuarioCliente (Lista de Propietarios)
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Cargando índice de Propietarios.");
            // Incluir Rol
            var propietarios = await _context.Propietarios
                                             .Include(p => p.Rol) // *** AÑADIDO: Incluir Rol ***
                                             .ToListAsync();
            return View(propietarios);
        }

        // GET: UsuarioCliente/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Details Propietario sin ID.");
                return NotFound();
            }

            _logger.LogInformation("Buscando Details para Propietario ID {PropietarioId}.", id);
            var propietario = await _context.Propietarios
                .Include(p => p.Rol) // *** AÑADIDO: Incluir Rol ***
                .Include(p => p.Citas) // OK: Navegación a Citas
                                       // *** CORREGIDO: Comentario para ThenInclude ***
                                       // .ThenInclude(c => c.IdClinicaOrigenNavigation) // Para obtener clínica de origen de cada cita
                                       // .Include(p => p.Mascotas) // Si tuvieras relación con Mascotas
                                       // *** CORREGIDO: Usa IdPropietario ***
                .FirstOrDefaultAsync(m => m.IdPropietario == id);

            if (propietario == null)
            {
                _logger.LogWarning("No se encontró Propietario ID {PropietarioId} para Details.", id);
                return NotFound();
            }

            return View(propietario);
        }

        // GET: UsuarioCliente/Create
        public IActionResult Create()
        {
            _logger.LogInformation("Mostrando formulario de creación de Propietario.");
            // *** AÑADIDO: Pasar lista de Roles ***
            ViewData["IdRol"] = new SelectList(_context.Roles, "IdRol", "NombreRol"); // Asumiendo NombreRol en Rol.cs
            return View();
        }

        // POST: UsuarioCliente/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // *** CORREGIDO: Ajusta Bind a propiedades existentes y Rol. Quita Contraseña, Cedula, Direccion ***
        public async Task<IActionResult> Create([Bind("IdPropietario,Nombre,Apellido,Correo,Telefono,IdRol")] Propietario propietario)
        // Nota: Se asume que 'Contrasenna' se establece en un proceso de registro dedicado, no aquí.
        {
            // Quitar propiedades de navegación para evitar errores de validación
            ModelState.Remove("Rol");
            ModelState.Remove("Citas");
            // Quitar Contrasenna si no se maneja aquí
            ModelState.Remove("Contrasenna");

            if (ModelState.IsValid)
            {
                try
                {
                    // Aquí iría la lógica de Hashing de contraseña si se estableciera en este punto
                    // propietario.Contrasenna = HashPassword(contraseñaDelFormulario);

                    _context.Add(propietario);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Propietario creado con ID {PropietarioId}.", propietario.IdPropietario);
                    TempData["SuccessMessage"] = "Propietario creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Error DbUpdateException al crear Propietario.");
                    ModelState.AddModelError("", "No se pudo guardar. Verifique que el correo no esté duplicado y el Rol seleccionado sea válido.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error inesperado al crear Propietario.");
                    ModelState.AddModelError("", "Ocurrió un error inesperado.");
                }
            }
            else
            {
                _logger.LogWarning("ModelState inválido al crear Propietario.");
            }

            // Si hay error, recargar lista de Roles
            ViewData["IdRol"] = new SelectList(_context.Roles, "IdRol", "NombreRol", propietario.IdRol);
            return View(propietario);
        }

        // GET: UsuarioCliente/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) { /* NotFound */ }

            // *** CORREGIDO: Usa IdPropietario ***
            var propietario = await _context.Propietarios.FindAsync(id);
            if (propietario == null) { /* NotFound */ }

            _logger.LogInformation("Mostrando formulario Edit para Propietario ID {PropietarioId}.", id);
            // *** AÑADIDO: Pasar lista de Roles ***
            ViewData["IdRol"] = new SelectList(_context.Roles, "IdRol", "NombreRol", propietario.IdRol);
            return View(propietario);
        }

        // POST: UsuarioCliente/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        // *** CORREGIDO: Ajusta Bind a propiedades existentes y Rol. Quita Contraseña, Cedula, Direccion ***
        public async Task<IActionResult> Edit(int id, [Bind("IdPropietario,Nombre,Apellido,Correo,Telefono,IdRol")] Propietario propietario)
        {
            // *** CORREGIDO: Usa IdPropietario ***
            if (id != propietario.IdPropietario)
            {
                return NotFound();
            }

            // Obtener entidad existente para actualizarla, manteniendo la contraseña original
            // *** CORREGIDO: Usa IdPropietario ***
            var propietarioExistente = await _context.Propietarios.FindAsync(id);
            if (propietarioExistente == null)
            {
                _logger.LogWarning("No se encontró Propietario ID {PropietarioId} para Edit POST.", id);
                return NotFound();
            }

            // Quitar propiedades de navegación y contraseña del ModelState
            ModelState.Remove("Rol");
            ModelState.Remove("Citas");
            ModelState.Remove("Contrasenna");

            if (ModelState.IsValid)
            {
                try
                {
                    // Copiar solo los valores del Bind al existente, sin tocar la contraseña
                    _context.Entry(propietarioExistente).CurrentValues.SetValues(propietario);
                    // Asegurar que el IdRol se actualice si cambió en el Bind
                    propietarioExistente.IdRol = propietario.IdRol;

                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Propietario ID {PropietarioId} actualizado.", id);
                    TempData["SuccessMessage"] = "Propietario actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Error Concurrencia al actualizar Propietario ID {PropietarioId}.", id);
                    // *** CORREGIDO: Usa IdPropietario ***
                    if (!PropietarioExists(propietario.IdPropietario)) { return NotFound(); }
                    else { ModelState.AddModelError("", "Registro modificado por otro usuario. Recargue."); }
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Error DbUpdateException al actualizar Propietario ID {PropietarioId}.", id);
                    ModelState.AddModelError("", "No se pudo guardar. Verifique correo no duplicado y Rol válido.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error inesperado al actualizar Propietario ID {PropietarioId}.", id);
                    ModelState.AddModelError("", "Ocurrió un error inesperado.");
                }

                // Solo redirigir si no hubo errores que añadieran al ModelState
                if (ModelState.ErrorCount == 0)
                    return RedirectToAction(nameof(Index));
            }
            else
            {
                _logger.LogWarning("ModelState inválido al actualizar Propietario ID {PropietarioId}.", id);
            }

            // Si hay error, recargar SelectList de Roles
            ViewData["IdRol"] = new SelectList(_context.Roles, "IdRol", "NombreRol", propietario.IdRol);
            // Devolver la vista con el modelo original (propietarioExistente) para evitar perder datos no editables
            // o con el modelo 'propietario' que contiene los errores de validación. Usaremos 'propietario'.
            return View(propietario);
        }

        // GET: UsuarioCliente/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) { /* NotFound */ }

            _logger.LogInformation("Mostrando confirmación Delete para Propietario ID {PropietarioId}.", id);
            // Incluir Rol para mostrar info en confirmación
            var propietario = await _context.Propietarios
                .Include(p => p.Rol) // Incluir Rol
                                     // *** CORREGIDO: Usa IdPropietario ***
                .FirstOrDefaultAsync(m => m.IdPropietario == id);

            if (propietario == null) { /* NotFound */ }

            return View(propietario);
        }

        // POST: UsuarioCliente/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation("Confirmado Delete para Propietario ID {PropietarioId}.", id);
            // Cargar propietario CON sus citas para verificar dependencias
            // *** CORREGIDO: Usa IdPropietario ***
            var propietario = await _context.Propietarios
                                             .Include(p => p.Citas) // Incluir citas para verificar
                                             .FirstOrDefaultAsync(p => p.IdPropietario == id);

            if (propietario != null)
            {
                // VERIFICAR DEPENDENCIAS: No eliminar si tiene citas asociadas
                if (propietario.Citas.Any())
                {
                    _logger.LogWarning("Intento de eliminar Propietario ID {PropietarioId} con citas asociadas.", id);
                    TempData["ErrorMessage"] = "No se puede eliminar el propietario porque tiene citas asociadas. Elimine o reasigne las citas primero.";
                    // Redirigir de vuelta a la vista de confirmación de borrado
                    return RedirectToAction(nameof(Delete), new { id = id });
                }

                // Si no hay dependencias (o se manejan de otra forma), proceder a eliminar
                try
                {
                    _context.Propietarios.Remove(propietario);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Propietario ID {PropietarioId} eliminado.", id);
                    TempData["SuccessMessage"] = "Propietario eliminado exitosamente.";
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Error DbUpdateException al eliminar Propietario ID {PropietarioId}.", id);
                    TempData["ErrorMessage"] = "No se pudo eliminar el propietario. Puede tener otras dependencias.";
                    return RedirectToAction(nameof(Delete), new { id = id });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error inesperado al eliminar Propietario ID {PropietarioId}.", id);
                    TempData["ErrorMessage"] = "Ocurrió un error inesperado al eliminar.";
                    return RedirectToAction(nameof(Delete), new { id = id });
                }
            }
            else
            {
                _logger.LogWarning("No se encontró Propietario ID {PropietarioId} para DeleteConfirmed.", id);
                TempData["ErrorMessage"] = "El propietario que intenta eliminar no fue encontrado.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PropietarioExists(int id)
        {
            // *** CORREGIDO: Usa IdPropietario ***
            return _context.Propietarios.Any(e => e.IdPropietario == id);
        }
    }
}