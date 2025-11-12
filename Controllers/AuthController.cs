using Microsoft.AspNetCore.Mvc;
using hotel_web_final.Servicios;

namespace hotel_web_final.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthController(
            AuthService authService,
            ILogger<AuthController> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _authService = authService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Muestra la página de inicio de sesión.
        /// </summary>
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        /// <summary>
        /// Procesa el inicio de sesión.
        /// </summary>
        [HttpPost]
        public IActionResult Login(string usuario, string contrasena, string? returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(contrasena))
            {
                TempData["Error"] = "Usuario y contraseña son requeridos";
                return View();
            }

            try
            {
                var autenticado = _authService.Autenticar(usuario, contrasena);

                if (autenticado)
                {
                    // Guardar el usuario en la sesión
                    _httpContextAccessor.HttpContext?.Session.SetString("Usuario", usuario);

                    TempData["Mensaje"] = $"Bienvenido, {usuario}!";

                    // Redirigir a la URL de retorno o al inicio
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Error"] = "Usuario o contraseña incorrectos";
                    return View();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el inicio de sesión");
                TempData["Error"] = "Error al procesar el inicio de sesión";
                return View();
            }
        }

        /// <summary>
        /// Cierra la sesión del usuario actual.
        /// </summary>
        [HttpPost]
        public IActionResult Logout()
        {
            try
            {
                var usuario = _httpContextAccessor.HttpContext?.Session.GetString("Usuario");

                if (!string.IsNullOrEmpty(usuario))
                {
                    _authService.CerrarSesion();
                    _httpContextAccessor.HttpContext?.Session.Clear();
                    TempData["Mensaje"] = "Sesión cerrada exitosamente";
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cerrar sesión");
                TempData["Error"] = "Error al cerrar sesión";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Muestra información del usuario actual.
        /// </summary>
        public IActionResult Perfil()
        {
            var usuario = _httpContextAccessor.HttpContext?.Session.GetString("Usuario");

            if (string.IsNullOrEmpty(usuario))
            {
                return RedirectToAction("Login");
            }

            ViewBag.Usuario = usuario;
            ViewBag.EstaAutenticado = _authService.EstaAutenticado();

            return View();
        }

        /// <summary>
        /// Muestra el formulario de cambio de contraseña.
        /// </summary>
        [HttpGet]
        public IActionResult CambiarContrasena()
        {
            var usuario = _httpContextAccessor.HttpContext?.Session.GetString("Usuario");

            if (string.IsNullOrEmpty(usuario))
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        /// <summary>
        /// Procesa el cambio de contraseña.
        /// </summary>
        [HttpPost]
        public IActionResult CambiarContrasena(string contrasenaActual, string contrasenaNueva, string confirmarContrasena)
        {
            var usuario = _httpContextAccessor.HttpContext?.Session.GetString("Usuario");

            if (string.IsNullOrEmpty(usuario))
            {
                return RedirectToAction("Login");
            }

            if (string.IsNullOrWhiteSpace(contrasenaActual) ||
                string.IsNullOrWhiteSpace(contrasenaNueva) ||
                string.IsNullOrWhiteSpace(confirmarContrasena))
            {
                TempData["Error"] = "Todos los campos son requeridos";
                return View();
            }

            if (contrasenaNueva != confirmarContrasena)
            {
                TempData["Error"] = "La nueva contraseña y la confirmación no coinciden";
                return View();
            }

            if (contrasenaNueva.Length < 6)
            {
                TempData["Error"] = "La nueva contraseña debe tener al menos 6 caracteres";
                return View();
            }

            try
            {
                var cambiado = _authService.CambiarContrasena(usuario, contrasenaActual, contrasenaNueva);

                if (cambiado)
                {
                    TempData["Mensaje"] = "Contraseña cambiada exitosamente";
                    return RedirectToAction("Perfil");
                }
                else
                {
                    TempData["Error"] = "La contraseña actual es incorrecta";
                    return View();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar contraseña");
                TempData["Error"] = "Error al cambiar la contraseña";
                return View();
            }
        }
    }
}
