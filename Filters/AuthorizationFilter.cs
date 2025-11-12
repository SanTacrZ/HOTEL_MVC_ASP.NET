using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace hotel_web_final.Filters
{
    /// <summary>
    /// Filtro de autorización que verifica si el usuario está autenticado.
    /// Redirige a la página de login si no hay sesión activa.
    /// </summary>
    public class AuthorizationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Obtener el controlador y acción actuales
            var controller = context.RouteData.Values["controller"]?.ToString();
            var action = context.RouteData.Values["action"]?.ToString();

            // Permitir acceso a las acciones de autenticación sin verificar sesión
            if (controller == "Auth")
            {
                return;
            }

            // Verificar si hay un usuario en la sesión
            var usuario = context.HttpContext.Session.GetString("Usuario");

            // Si no hay usuario autenticado, redirigir a login
            if (string.IsNullOrEmpty(usuario))
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // No se necesita hacer nada después de ejecutar la acción
        }
    }
}
