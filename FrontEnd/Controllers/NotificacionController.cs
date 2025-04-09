using Microsoft.AspNetCore.Mvc;
using FrontEnd.Models; // Asegúrate que el namespace sea correcto
using System.Net.Http;
using System.Net.Http.Json; // Para ReadFromJsonAsync y PostAsJsonAsync
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging; // Para logging
using System;
// using Microsoft.AspNetCore.Authorization; // Descomenta si requiere autorización

namespace FrontEnd.Controllers
{
    // [Authorize] // Descomenta si requiere autorización
    public class NotificacionController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly ILogger<NotificacionController> _logger;

        public NotificacionController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<NotificacionController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient"); // Usa el nombre configurado en Program.cs
            _apiBaseUrl = configuration["ApiBaseUrl"] ?? "https://localhost:7001/"; // Obtiene la URL base desde appsettings.json o usa un valor por defecto
            _logger = logger;
        }

        // GET: Notificacion
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}api/Notificaciones"); // Ajusta la ruta de la API si es diferente
                if (response.IsSuccessStatusCode)
                {
                    var notificaciones = await response.Content.ReadFromJsonAsync<List<Notificacion>>();
                    return View(notificaciones ?? new List<Notificacion>());
                }
                else
                {
                    _logger.LogWarning($"Error al obtener notificaciones. Status Code: {response.StatusCode}");
                    TempData["ErrorMessage"] = $"No se pudieron cargar las notificaciones (Error: {response.StatusCode}).";
                    return View(new List<Notificacion>()); // Devuelve vista con lista vacía o manejo de error
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción al obtener notificaciones.");
                TempData["ErrorMessage"] = "Ocurrió un error al conectar con el servidor.";
                return View(new List<Notificacion>());
            }
        }

        // GET: Notificacion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}api/Notificaciones/{id}"); // Ajusta la ruta
                if (response.IsSuccessStatusCode)
                {
                    var notificacion = await response.Content.ReadFromJsonAsync<Notificacion>();
                    if (notificacion == null)
                    {
                        return NotFound();
                    }
                    return View(notificacion);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogWarning($"Error al obtener detalle de notificación {id}. Status Code: {response.StatusCode}");
                    TempData["ErrorMessage"] = $"No se pudo cargar la notificación (Error: {response.StatusCode}).";
                    return RedirectToAction(nameof(Index)); // O mostrar una vista de error
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Excepción al obtener detalle de notificación {id}.");
                TempData["ErrorMessage"] = "Ocurrió un error al conectar con el servidor.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Notificacion/Create
        public IActionResult Create()
        {
            // Aquí podrías pre-cargar datos necesarios para la vista, como listas desplegables
            return View();
        }

        // POST: Notificacion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdNotificacion,IdUsuario,Mensaje,FechaHora,Leida")] Notificacion notificacion) // Ajusta las propiedades según tu modelo
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Podrías necesitar ajustar el objeto antes de enviarlo (ej. FechaHora)
                    // notificacion.FechaHora = DateTime.UtcNow; // O la lógica que necesites

                    var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}api/Notificaciones", notificacion); // Ajusta la ruta

                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation($"Notificación creada exitosamente.");
                        TempData["SuccessMessage"] = "Notificación creada exitosamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        _logger.LogWarning($"Error al crear notificación. Status Code: {response.StatusCode}");
                        string errorContent = await response.Content.ReadAsStringAsync();
                        _logger.LogWarning($"Error Body: {errorContent}");
                        TempData["ErrorMessage"] = $"Error al crear la notificación (Error: {response.StatusCode}). {errorContent}";
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Excepción al crear notificación.");
                    TempData["ErrorMessage"] = "Ocurrió una excepción al procesar la solicitud.";
                }
            }
            else
            {
                _logger.LogWarning("Modelo inválido para crear notificación.");
                TempData["ErrorMessage"] = "Por favor, corrige los errores en el formulario.";
            }
            // Si algo falla, vuelve a mostrar el formulario con los datos ingresados
            return View(notificacion);
        }

        // GET: Notificacion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}api/Notificaciones/{id}"); // Ajusta la ruta
                if (response.IsSuccessStatusCode)
                {
                    var notificacion = await response.Content.ReadFromJsonAsync<Notificacion>();
                    if (notificacion == null)
                    {
                        return NotFound();
                    }
                    // Aquí podrías pre-cargar datos necesarios para la vista (listas desplegables, etc.)
                    return View(notificacion);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogWarning($"Error al obtener notificación {id} para editar. Status Code: {response.StatusCode}");
                    TempData["ErrorMessage"] = $"No se pudo cargar la notificación para editar (Error: {response.StatusCode}).";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Excepción al obtener notificación {id} para editar.");
                TempData["ErrorMessage"] = "Ocurrió un error al conectar con el servidor.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Notificacion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdNotificacion,IdUsuario,Mensaje,FechaHora,Leida")] Notificacion notificacion) // Ajusta las propiedades
        {
            if (id != notificacion.IdNotificacion)
            {
                return BadRequest("ID de notificación no coincide.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}api/Notificaciones/{id}", notificacion); // Ajusta la ruta

                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation($"Notificación {id} actualizada exitosamente.");
                        TempData["SuccessMessage"] = "Notificación actualizada exitosamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogWarning($"Error al actualizar notificación {id}. Status Code: {response.StatusCode}");
                        string errorContent = await response.Content.ReadAsStringAsync();
                        _logger.LogWarning($"Error Body: {errorContent}");
                        TempData["ErrorMessage"] = $"Error al actualizar la notificación (Error: {response.StatusCode}). {errorContent}";
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Excepción al actualizar notificación {id}.");
                    TempData["ErrorMessage"] = "Ocurrió una excepción al procesar la solicitud.";
                    // Podrías querer verificar si la notificación aún existe si hay un error de concurrencia
                }
            }
            else
            {
                _logger.LogWarning($"Modelo inválido para editar notificación {id}.");
                TempData["ErrorMessage"] = "Por favor, corrige los errores en el formulario.";
            }
            // Si algo falla, vuelve a mostrar el formulario con los datos ingresados
            // Podrías necesitar recargar datos para listas desplegables aquí
            return View(notificacion);
        }

        // GET: Notificacion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}api/Notificaciones/{id}"); // Ajusta la ruta
                if (response.IsSuccessStatusCode)
                {
                    var notificacion = await response.Content.ReadFromJsonAsync<Notificacion>();
                    if (notificacion == null)
                    {
                        return NotFound();
                    }
                    return View(notificacion); // Muestra vista de confirmación
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogWarning($"Error al obtener notificación {id} para eliminar. Status Code: {response.StatusCode}");
                    TempData["ErrorMessage"] = $"No se pudo cargar la notificación para eliminar (Error: {response.StatusCode}).";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Excepción al obtener notificación {id} para eliminar.");
                TempData["ErrorMessage"] = "Ocurrió un error al conectar con el servidor.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Notificacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}api/Notificaciones/{id}"); // Ajusta la ruta

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Notificación {id} eliminada exitosamente.");
                    TempData["SuccessMessage"] = "Notificación eliminada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    TempData["ErrorMessage"] = "La notificación que intentas eliminar ya no existe.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogWarning($"Error al eliminar notificación {id}. Status Code: {response.StatusCode}");
                    string errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning($"Error Body: {errorContent}");
                    TempData["ErrorMessage"] = $"Error al eliminar la notificación (Error: {response.StatusCode}). {errorContent}";
                    // Vuelve a la vista de confirmación o al Index
                    // Podrías redirigir a la misma página de eliminación para mostrar el error
                    return RedirectToAction(nameof(Delete), new { id = id });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Excepción al eliminar notificación {id}.");
                TempData["ErrorMessage"] = "Ocurrió una excepción al procesar la solicitud.";
                return RedirectToAction(nameof(Index)); // O a la vista de eliminación
            }
        }

        // Puedes añadir aquí un método auxiliar para verificar si la notificación existe si lo necesitas
        // private async Task<bool> NotificacionExists(int id) { ... }
    }
}