using Microsoft.AspNetCore.Mvc;
using hotel_web_final.Servicios;
using biblioteca_hotel.Clases;

namespace hotel_web_final.Controllers
{
    public class RecepcionController : Controller
    {
        private readonly RecepcionService _recepcionService;
        private readonly ReservaService _reservaService;
        private readonly MinibarService _minibarService;
        private readonly NotificacionService _notificacionService;
        private readonly ILogger<RecepcionController> _logger;

        public RecepcionController(
            RecepcionService recepcionService,
            ReservaService reservaService,
            MinibarService minibarService,
            NotificacionService notificacionService,
            ILogger<RecepcionController> logger)
        {
            _recepcionService = recepcionService;
            _reservaService = reservaService;
            _minibarService = minibarService;
            _notificacionService = notificacionService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var reservas = _reservaService.ObtenerTodas();
            return View(reservas);
        }

        public IActionResult CheckIn(int id)
        {
            var reserva = _reservaService.BuscarPorId(id);
            if (reserva == null)
                return NotFound();

            return View(reserva);
        }

        [HttpPost]
        [ActionName("CheckIn")]
        public IActionResult CheckInPost(int id)
        {
            try
            {
                _recepcionService.RealizarCheckIn(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                _logger.LogError(ex, "Error al realizar check-in");
                var reserva = _reservaService.BuscarPorId(id);
                return View(reserva);
            }
        }

        public IActionResult CheckOut(int id)
        {
            var reserva = _reservaService.BuscarPorId(id);
            if (reserva == null)
                return NotFound();

            return View(reserva);
        }

        [HttpPost]
        [ActionName("CheckOut")]
        public IActionResult CheckOutPost(int id, string metodoPago)
        {
            try
            {
                if (string.IsNullOrEmpty(metodoPago))
                {
                    ViewBag.Error = "Debe seleccionar un método de pago";
                    var reserva = _reservaService.BuscarPorId(id);
                    return View(reserva);
                }

                var factura = _recepcionService.RealizarCheckOut(id, metodoPago);
                return RedirectToAction(nameof(VerFactura), new { id = factura.Id });
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                _logger.LogError(ex, "Error al realizar check-out");
                var reserva = _reservaService.BuscarPorId(id);
                return View(reserva);
            }
        }

        public IActionResult VerFactura(int id)
        {
            var factura = _recepcionService.BuscarFacturaPorId(id);
            if (factura == null)
                return NotFound();

            return View(factura);
        }

        public IActionResult Facturas()
        {
            var facturas = _recepcionService.ObtenerTodasFacturas();
            return View(facturas);
        }

        // ===== GESTIÓN DE MINIBAR =====

        /// <summary>
        /// Muestra el minibar de una habitación para registrar consumos.
        /// </summary>
        /// <param name="habitacionId">ID de la habitación</param>
        public IActionResult GestionarMinibar(int habitacionId)
        {
            try
            {
                if (!_minibarService.TieneMinibar(habitacionId))
                {
                    ViewBag.Error = "Esta habitación no tiene minibar";
                    return RedirectToAction(nameof(Index));
                }

                var minibar = _minibarService.ObtenerMinibarPorHabitacion(habitacionId);
                ViewBag.HabitacionId = habitacionId;
                ViewBag.CostoTotal = _minibarService.CalcularCostoTotal(habitacionId);
                return View(minibar);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                _logger.LogError(ex, "Error al gestionar minibar");
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Registra un consumo del minibar.
        /// </summary>
        [HttpPost]
        public IActionResult RegistrarConsumo(int habitacionId, int productoId, int cantidad)
        {
            try
            {
                if (cantidad <= 0)
                {
                    TempData["Error"] = "La cantidad debe ser mayor a 0";
                    return RedirectToAction(nameof(GestionarMinibar), new { habitacionId });
                }

                _minibarService.RegistrarConsumo(habitacionId, productoId, cantidad);
                TempData["Mensaje"] = "Consumo registrado exitosamente";
                return RedirectToAction(nameof(GestionarMinibar), new { habitacionId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                _logger.LogError(ex, "Error al registrar consumo");
                return RedirectToAction(nameof(GestionarMinibar), new { habitacionId });
            }
        }

        /// <summary>
        /// Ver los consumos realizados en una habitación.
        /// </summary>
        public IActionResult VerConsumos(int habitacionId)
        {
            try
            {
                var consumos = _minibarService.ObtenerConsumosPorHabitacion(habitacionId);
                ViewBag.HabitacionId = habitacionId;
                ViewBag.CostoTotal = _minibarService.CalcularCostoTotal(habitacionId);
                return View(consumos);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                _logger.LogError(ex, "Error al ver consumos");
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Envía la factura por correo electrónico al cliente.
        /// </summary>
        [HttpPost]
        public IActionResult EnviarFacturaPorCorreo(int id)
        {
            try
            {
                var factura = _recepcionService.BuscarFacturaPorId(id);
                if (factura == null)
                {
                    TempData["Error"] = "Factura no encontrada";
                    return RedirectToAction(nameof(Facturas));
                }

                // Verificar que el cliente tenga email
                if (string.IsNullOrEmpty(factura.Cliente?.Email))
                {
                    TempData["Error"] = "El cliente no tiene un email registrado";
                    return RedirectToAction(nameof(VerFactura), new { id });
                }

                // Enviar la factura por correo usando el NotificacionService
                _notificacionService.EnviarFacturaPorCorreo(
                    factura.Cliente.Email,
                    factura.NumeroFactura,
                    factura.MontoTotal,
                    factura.FechaEmision,
                    factura.Cliente.Nombre + " " + factura.Cliente.Apellido
                );

                TempData["Mensaje"] = $"Factura enviada exitosamente a {factura.Cliente.Email}";
                return RedirectToAction(nameof(VerFactura), new { id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al enviar la factura: {ex.Message}";
                _logger.LogError(ex, "Error al enviar factura por correo");
                return RedirectToAction(nameof(VerFactura), new { id });
            }
        }
    }
}

