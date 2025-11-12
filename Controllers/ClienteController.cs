using Microsoft.AspNetCore.Mvc;
using hotel_web_final.Servicios;
using biblioteca_hotel.Clases;

namespace hotel_web_final.Controllers
{
    public class ClienteController : Controller
    {
        private readonly ClienteService _clienteService;
        private readonly ILogger<ClienteController> _logger;

        public ClienteController(ClienteService clienteService, ILogger<ClienteController> logger)
        {
            _clienteService = clienteService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var clientes = _clienteService.ObtenerTodos();
            return View(clientes);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Crear(Cliente cliente)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _clienteService.Agregar(cliente);
                    TempData["Success"] = "Cliente registrado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                _logger.LogWarning(ex, "Error de validación al crear cliente");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _logger.LogError(ex, "Error al crear cliente");
            }
            return View(cliente);
        }

        public IActionResult Editar(int id)
        {
            var cliente = _clienteService.BuscarPorId(id);
            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

        [HttpPost]
        public IActionResult Editar(Cliente cliente)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _clienteService.Actualizar(cliente);
                    TempData["Success"] = "Cliente actualizado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                _logger.LogWarning(ex, "Error de validación al actualizar cliente");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _logger.LogError(ex, "Error al actualizar cliente");
            }
            return View(cliente);
        }

        public IActionResult Eliminar(int id)
        {
            var cliente = _clienteService.BuscarPorId(id);
            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

        [HttpPost, ActionName("Eliminar")]
        public IActionResult EliminarConfirmado(int id)
        {
            try
            {
                _clienteService.Eliminar(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                _logger.LogError(ex, "Error al eliminar cliente");
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Detalles(int id)
        {
            var cliente = _clienteService.BuscarPorId(id);
            if (cliente == null)
                return NotFound();

            return View(cliente);
        }
    }
}

