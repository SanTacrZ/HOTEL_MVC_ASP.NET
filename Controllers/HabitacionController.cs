using Microsoft.AspNetCore.Mvc;
using hotel_web_final.Servicios;
using biblioteca_hotel.Clases;

namespace hotel_web_final.Controllers
{
    public class HabitacionController : Controller
    {
        private readonly HabitacionService _habitacionService;
        private readonly ClienteService _clienteService;
        private readonly ReservaService _reservaService;
        private readonly ILogger<HabitacionController> _logger;

        public HabitacionController(
            HabitacionService habitacionService,
            ClienteService clienteService,
            ReservaService reservaService,
            ILogger<HabitacionController> logger)
        {
            _habitacionService = habitacionService;
            _clienteService = clienteService;
            _reservaService = reservaService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var habitaciones = _habitacionService.ObtenerTodas();
            ViewBag.HayClientes = _clienteService.ObtenerTodos().Any();
            return View(habitaciones);
        }

        public IActionResult Disponibles()
        {
            var habitaciones = _habitacionService.ObtenerDisponibles();
            ViewBag.HayClientes = _clienteService.ObtenerTodos().Any();
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

        public IActionResult ReservarHabitacion(int id)
        {
            var habitacion = _habitacionService.BuscarPorId(id);
            if (habitacion == null)
                return NotFound();

            if (habitacion.Estado != "Disponible")
            {
                TempData["Error"] = "Esta habitación no está disponible para reservar";
                return RedirectToAction(nameof(Disponibles));
            }

            // Verificar que haya clientes registrados
            var clientes = _clienteService.ObtenerTodos().ToList();
            if (!clientes.Any())
            {
                TempData["Error"] = "No hay clientes registrados. Por favor, agregue al menos un cliente antes de realizar una reserva.";
                return RedirectToAction(nameof(Disponibles));
            }

            ViewBag.Habitacion = habitacion;
            ViewBag.Clientes = clientes;
            return View();
        }

        [HttpPost]
        public IActionResult ReservarHabitacion(int habitacionId, string opcionCliente, int? clienteExistenteId,
            string? nuevoNombre, string? nuevoApellido, string? nuevoTipoDocumento, string? nuevoNumeroDocumento,
            string? nuevoTelefono, string? nuevoEmail, DateTime fechaEntrada, DateTime fechaSalida, int numHuespedes)
        {
            try
            {
                var habitacion = _habitacionService.BuscarPorId(habitacionId);
                if (habitacion == null)
                    return NotFound();

                int clienteId;

                // Opción 1: Cliente existente
                if (opcionCliente == "existente")
                {
                    if (!clienteExistenteId.HasValue)
                    {
                        ModelState.AddModelError("", "Debe seleccionar un cliente existente");
                        ViewBag.Habitacion = habitacion;
                        ViewBag.Clientes = _clienteService.ObtenerTodos().ToList();
                        return View();
                    }
                    clienteId = clienteExistenteId.Value;
                }
                // Opción 2: Cliente nuevo
                else
                {
                    if (string.IsNullOrWhiteSpace(nuevoNombre) || string.IsNullOrWhiteSpace(nuevoApellido) ||
                        string.IsNullOrWhiteSpace(nuevoTipoDocumento) || string.IsNullOrWhiteSpace(nuevoNumeroDocumento) ||
                        string.IsNullOrWhiteSpace(nuevoTelefono))
                    {
                        ModelState.AddModelError("", "Debe completar todos los campos del nuevo cliente");
                        ViewBag.Habitacion = habitacion;
                        ViewBag.Clientes = _clienteService.ObtenerTodos().ToList();
                        return View();
                    }

                    // Crear el nuevo cliente
                    var nuevoCliente = new Cliente
                    {
                        TipoDocumento = new TipoDocumento { Nombre = nuevoTipoDocumento },
                        NumeroDocumento = nuevoNumeroDocumento,
                        Nombre = nuevoNombre,
                        Apellido = nuevoApellido,
                        Telefono = nuevoTelefono,
                        Email = nuevoEmail ?? ""
                    };

                    nuevoCliente.RegistrarCliente();
                    _clienteService.Agregar(nuevoCliente);
                    clienteId = nuevoCliente.IdCliente;
                }

                // Crear la reserva con la habitación seleccionada
                var reserva = _reservaService.CrearReserva(
                    clienteId,
                    fechaEntrada,
                    fechaSalida,
                    numHuespedes,
                    new List<int> { habitacionId },
                    null
                );

                TempData["Success"] = "Reserva creada exitosamente";
                return RedirectToAction("Detalles", "Reserva", new { id = reserva.Id });
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                _logger.LogWarning(ex, "Error de validación al crear reserva rápida");
                var habitacion = _habitacionService.BuscarPorId(habitacionId);
                ViewBag.Habitacion = habitacion;
                ViewBag.Clientes = _clienteService.ObtenerTodos().ToList();
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _logger.LogError(ex, "Error al crear reserva rápida");
                var habitacion = _habitacionService.BuscarPorId(habitacionId);
                ViewBag.Habitacion = habitacion;
                ViewBag.Clientes = _clienteService.ObtenerTodos().ToList();
                return View();
            }
        }
    }
}

