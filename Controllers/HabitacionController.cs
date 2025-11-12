using Microsoft.AspNetCore.Mvc;
using hotel_web_final.Servicios;
using biblioteca_hotel.Clases;

namespace hotel_web_final.Controllers
{
    public class HabitacionController : Controller
    {
        private readonly HabitacionService _habitacionService;
        private readonly ILogger<HabitacionController> _logger;

        public HabitacionController(HabitacionService habitacionService, ILogger<HabitacionController> logger)
        {
            _habitacionService = habitacionService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var habitaciones = _habitacionService.ObtenerTodas();
            return View(habitaciones);
        }

        public IActionResult Disponibles()
        {
            var habitaciones = _habitacionService.ObtenerDisponibles();
            return View(habitaciones);
        }

        public IActionResult PorTipo(string tipo)
        {
            if (string.IsNullOrEmpty(tipo))
                return RedirectToAction(nameof(Index));

            var habitaciones = _habitacionService.ObtenerPorTipo(tipo);
            ViewBag.Tipo = tipo;
            return View(habitaciones);
        }

        public IActionResult Detalles(int id)
        {
            var habitacion = _habitacionService.BuscarPorId(id);
            if (habitacion == null)
                return NotFound();

            return View(habitacion);
        }
    }
}

