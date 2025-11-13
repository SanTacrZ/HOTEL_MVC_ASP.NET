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
        private readonly ServicioAdicionalService _servicioAdicionalService;
        private readonly NotificacionService _notificacionService;
        private readonly ILogger<RecepcionController> _logger;

        public RecepcionController(
            RecepcionService recepcionService,
            ReservaService reservaService,
            MinibarService minibarService,
            ServicioAdicionalService servicioAdicionalService,
            NotificacionService notificacionService,
            ILogger<RecepcionController> logger)
        {
            _recepcionService = recepcionService;
            _reservaService = reservaService;
            _minibarService = minibarService;
            _servicioAdicionalService = servicioAdicionalService;
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
        /// Registra un consumo del minibar (POST tradicional - mantener por compatibilidad).
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
        /// Registra un consumo del minibar usando AJAX (sin recarga de página).
        /// </summary>
        [HttpPost]
        public IActionResult RegistrarConsumoAjax(int habitacionId, int productoId, int cantidad)
        {
            try
            {
                if (cantidad <= 0)
                {
                    return Json(new { success = false, message = "La cantidad debe ser mayor a 0" });
                }

                _minibarService.RegistrarConsumo(habitacionId, productoId, cantidad);

                // Obtener datos actualizados del minibar
                var minibar = _minibarService.ObtenerMinibarPorHabitacion(habitacionId);
                var costoTotal = _minibarService.ObtenerCostoTotalConsumos(habitacionId);

                return Json(new {
                    success = true,
                    message = "Consumo registrado exitosamente",
                    costoTotal = costoTotal,
                    productos = minibar?.ProductosDisponibles?.Select(p => new {
                        id = p.Id,
                        stock = p.Stock
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar consumo AJAX");
                return Json(new { success = false, message = ex.Message });
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

        // ===== GESTIÓN DE SERVICIOS ADICIONALES =====

        /// <summary>
        /// Muestra la página para agregar servicios adicionales a una reserva.
        /// </summary>
        public IActionResult GestionarServicios(int reservaId)
        {
            try
            {
                var reserva = _reservaService.BuscarPorId(reservaId);
                if (reserva == null)
                {
                    TempData["Error"] = "Reserva no encontrada";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.ReservaId = reservaId;
                ViewBag.ServiciosAdicionales = _servicioAdicionalService.ObtenerServiciosPorReserva(reservaId);
                ViewBag.CostoTotal = _servicioAdicionalService.CalcularCostoTotalServicios(reservaId);
                ViewBag.HabitacionesConBata = _servicioAdicionalService.ObtenerHabitacionesConVentaBata(reservaId);

                return View(reserva);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                _logger.LogError(ex, "Error al gestionar servicios adicionales");
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Agrega un servicio de lavandería a una reserva.
        /// </summary>
        [HttpPost]
        public IActionResult AgregarLavanderia(int reservaId, string descripcion, int cantidadPrendas, decimal precioPorPrenda)
        {
            try
            {
                _servicioAdicionalService.AgregarServicioLavanderia(reservaId, descripcion, cantidadPrendas, precioPorPrenda);
                TempData["Mensaje"] = "Servicio de lavandería agregado exitosamente";
                return RedirectToAction(nameof(GestionarServicios), new { reservaId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                _logger.LogError(ex, "Error al agregar servicio de lavandería");
                return RedirectToAction(nameof(GestionarServicios), new { reservaId });
            }
        }

        /// <summary>
        /// Agrega un servicio de restaurante a una reserva.
        /// </summary>
        [HttpPost]
        public IActionResult AgregarRestaurante(int reservaId, string tipoComidaNombre, int cantidad, decimal precioUnitario)
        {
            try
            {
                var tipoComida = new TipoComida { Nombre = tipoComidaNombre };
                _servicioAdicionalService.AgregarServicioRestaurante(reservaId, tipoComida, cantidad, precioUnitario);
                TempData["Mensaje"] = "Servicio de restaurante agregado exitosamente";
                return RedirectToAction(nameof(GestionarServicios), new { reservaId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                _logger.LogError(ex, "Error al agregar servicio de restaurante");
                return RedirectToAction(nameof(GestionarServicios), new { reservaId });
            }
        }

        /// <summary>
        /// Agrega venta de bata a una reserva.
        /// </summary>
        [HttpPost]
        public IActionResult AgregarBata(int reservaId, int habitacionId, string talla, int cantidad)
        {
            try
            {
                _servicioAdicionalService.AgregarVentaBata(reservaId, habitacionId, talla, cantidad);
                TempData["Mensaje"] = "Bata agregada exitosamente";
                return RedirectToAction(nameof(GestionarServicios), new { reservaId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                _logger.LogError(ex, "Error al agregar bata");
                return RedirectToAction(nameof(GestionarServicios), new { reservaId });
            }
        }

        /// <summary>
        /// Elimina un servicio adicional de una reserva.
        /// </summary>
        [HttpPost]
        public IActionResult EliminarServicio(int reservaId, int indiceServicio)
        {
            try
            {
                _servicioAdicionalService.EliminarServicio(reservaId, indiceServicio);
                TempData["Mensaje"] = "Servicio eliminado exitosamente";
                return RedirectToAction(nameof(GestionarServicios), new { reservaId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                _logger.LogError(ex, "Error al eliminar servicio");
                return RedirectToAction(nameof(GestionarServicios), new { reservaId });
            }
        }
    }
}

