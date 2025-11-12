using Microsoft.AspNetCore.Mvc;
using hotel_web_final.Servicios;
using biblioteca_hotel.Clases;

namespace hotel_web_final.Controllers
{
    public class RecepcionController : Controller
    {
        private readonly RecepcionService _recepcionService;
        private readonly ReservaService _reservaService;
        private readonly ILogger<RecepcionController> _logger;

        public RecepcionController(
            RecepcionService recepcionService,
            ReservaService reservaService,
            ILogger<RecepcionController> logger)
        {
            _recepcionService = recepcionService;
            _reservaService = reservaService;
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
    }
}

