using Microsoft.AspNetCore.Mvc;
using hotel_web_final.Servicios;

namespace hotel_web_final.Controllers
{
    public class AuditoriaController : Controller
    {
        private readonly AuditoriaService _auditoriaService;
        private readonly ILogger<AuditoriaController> _logger;

        public AuditoriaController(AuditoriaService auditoriaService, ILogger<AuditoriaController> logger)
        {
            _auditoriaService = auditoriaService;
            _logger = logger;
        }

        /// <summary>
        /// Muestra todos los registros de auditoría.
        /// </summary>
        public IActionResult Index(int cantidad = 100)
        {
            var registros = _auditoriaService.ObtenerUltimosRegistros(cantidad);
            ViewBag.TotalRegistros = _auditoriaService.ObtenerRegistros().Count;
            ViewBag.CantidadMostrada = cantidad;
            return View(registros);
        }

        /// <summary>
        /// Limpia todos los registros de auditoría.
        /// </summary>
        [HttpPost]
        public IActionResult Limpiar()
        {
            try
            {
                _auditoriaService.LimpiarRegistros();
                TempData["Mensaje"] = "Registros de auditoría limpiados exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                _logger.LogError(ex, "Error al limpiar auditoría");
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
