using Microsoft.AspNetCore.Mvc;
using hotel_web_final.Servicios;
using biblioteca_hotel.Clases;

namespace hotel_web_final.Controllers
{
    public class ReservaController : Controller
    {
        private readonly ReservaService _reservaService;
        private readonly ClienteService _clienteService;
        private readonly HabitacionService _habitacionService;
        private readonly HuespedService _huespedService;
        private readonly ILogger<ReservaController> _logger;

        public ReservaController(
            ReservaService reservaService,
            ClienteService clienteService,
            HabitacionService habitacionService,
            HuespedService huespedService,
            ILogger<ReservaController> logger)
        {
            _reservaService = reservaService;
            _clienteService = clienteService;
            _habitacionService = habitacionService;
            _huespedService = huespedService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var reservas = _reservaService.ObtenerTodas();
            return View(reservas);
        }

        public IActionResult Crear()
        {
            ViewBag.Clientes = _clienteService.ObtenerTodos().ToList();
            ViewBag.Habitaciones = _habitacionService.ObtenerDisponibles().ToList();
            ViewBag.Huespedes = _huespedService.ObtenerTodos().ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Crear(int clienteId, DateTime fechaEntrada, DateTime fechaSalida, 
            int numHuespedes, int[] habitacionIds, int[]? huespedIds)
        {
            try
            {
                if (habitacionIds == null || habitacionIds.Length == 0)
                {
                    ModelState.AddModelError("", "Debe seleccionar al menos una habitación");
                    ViewBag.Clientes = _clienteService.ObtenerTodos().ToList();
                    ViewBag.Habitaciones = _habitacionService.ObtenerDisponibles().ToList();
                    ViewBag.Huespedes = _huespedService.ObtenerTodos().ToList();
                    return View();
                }

                var reserva = _reservaService.CrearReserva(
                    clienteId,
                    fechaEntrada,
                    fechaSalida,
                    numHuespedes,
                    habitacionIds.ToList(),
                    huespedIds?.ToList()
                );

                TempData["Success"] = "Reserva creada exitosamente";
                return RedirectToAction(nameof(Detalles), new { id = reserva.Id });
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                _logger.LogWarning(ex, "Error de validación al crear reserva");
                ViewBag.Clientes = _clienteService.ObtenerTodos().ToList();
                ViewBag.Habitaciones = _habitacionService.ObtenerDisponibles().ToList();
                ViewBag.Huespedes = _huespedService.ObtenerTodos().ToList();
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _logger.LogError(ex, "Error al crear reserva");
                ViewBag.Clientes = _clienteService.ObtenerTodos().ToList();
                ViewBag.Habitaciones = _habitacionService.ObtenerDisponibles().ToList();
                ViewBag.Huespedes = _huespedService.ObtenerTodos().ToList();
                return View();
            }
        }

        public IActionResult Detalles(int id)
        {
            var reserva = _reservaService.BuscarPorId(id);
            if (reserva == null)
                return NotFound();

            return View(reserva);
        }

        [HttpPost]
        public IActionResult Confirmar(int id)
        {
            try
            {
                _reservaService.ConfirmarReserva(id);
                return RedirectToAction(nameof(Detalles), new { id });
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                _logger.LogError(ex, "Error al confirmar reserva");
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public IActionResult Cancelar(int id)
        {
            try
            {
                _reservaService.CancelarReserva(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                _logger.LogError(ex, "Error al cancelar reserva");
                return RedirectToAction(nameof(Index));
            }
        }
    }
}

