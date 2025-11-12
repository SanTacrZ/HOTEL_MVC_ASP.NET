using Microsoft.AspNetCore.Mvc;
using hotel_web_final.Servicios;
using biblioteca_hotel.Clases;

namespace hotel_web_final.Controllers
{
    public class HuespedController : Controller
    {
        private readonly HuespedService _huespedService;
        private readonly ILogger<HuespedController> _logger;

        public HuespedController(HuespedService huespedService, ILogger<HuespedController> logger)
        {
            _huespedService = huespedService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var huespedes = _huespedService.ObtenerTodos();
            return View(huespedes);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Crear(Huesped huesped)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _huespedService.Agregar(huesped);
                    TempData["Success"] = "Huésped registrado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                _logger.LogWarning(ex, "Error de validación al crear huésped");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _logger.LogError(ex, "Error al crear huésped");
            }
            return View(huesped);
        }

        public IActionResult Editar(int id)
        {
            var huesped = _huespedService.BuscarPorId(id);
            if (huesped == null)
                return NotFound();

            return View(huesped);
        }

        [HttpPost]
        public IActionResult Editar(Huesped huesped)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _huespedService.Actualizar(huesped);
                    TempData["Success"] = "Huésped actualizado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                _logger.LogWarning(ex, "Error de validación al actualizar huésped");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _logger.LogError(ex, "Error al actualizar huésped");
            }
            return View(huesped);
        }

        public IActionResult Eliminar(int id)
        {
            var huesped = _huespedService.BuscarPorId(id);
            if (huesped == null)
                return NotFound();

            return View(huesped);
        }

        [HttpPost, ActionName("Eliminar")]
        public IActionResult EliminarConfirmado(int id)
        {
            try
            {
                _huespedService.Eliminar(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                _logger.LogError(ex, "Error al eliminar huésped");
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Detalles(int id)
        {
            var huesped = _huespedService.BuscarPorId(id);
            if (huesped == null)
                return NotFound();

            return View(huesped);
        }
    }
}

