using biblioteca_hotel.Aspectos;
using biblioteca_hotel.Clases;

namespace hotel_web_final.Servicios
{
    /// <summary>
    /// Servicio de autenticación.
    /// Implementa el aspecto de Login de la biblioteca.
    /// </summary>
    public class AuthService
    {
        private readonly Login _login;
        private readonly AuditoriaService _auditoriaService;

        // Usuario actual autenticado
        private string? _usuarioActual;

        // Usuarios predefinidos (en producción esto vendría de una base de datos)
        private readonly Dictionary<string, string> _usuarios = new()
        {
            { "admin", "admin123" },
            { "recepcion", "recepcion123" },
            { "gerente", "gerente123" }
        };

        public AuthService(AuditoriaService auditoriaService)
        {
            _login = new Login();
            _auditoriaService = auditoriaService;
        }

        /// <summary>
        /// Autentica un usuario con nombre de usuario y contraseña.
        /// </summary>
        /// <param name="usuario">Nombre de usuario</param>
        /// <param name="contrasena">Contraseña</param>
        /// <returns>True si la autenticación fue exitosa</returns>
        public bool Autenticar(string usuario, string contrasena)
        {
            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(contrasena))
            {
                _auditoriaService.RegistrarLogin(usuario ?? "Desconocido", false);
                return false;
            }

            // Usar el aspecto de Login
            var autenticado = _login.Autenticar(usuario, contrasena);

            // Validar contra usuarios predefinidos
            if (_usuarios.TryGetValue(usuario, out var contrasenaAlmacenada))
            {
                if (contrasenaAlmacenada == contrasena)
                {
                    _usuarioActual = usuario;
                    _auditoriaService.RegistrarLogin(usuario, true);
                    return true;
                }
            }

            _auditoriaService.RegistrarLogin(usuario, false);
            return false;
        }

        /// <summary>
        /// Cierra la sesión del usuario actual.
        /// </summary>
        public void CerrarSesion()
        {
            if (!string.IsNullOrEmpty(_usuarioActual))
            {
                _auditoriaService.RegistrarAccion($"LOGOUT", _usuarioActual);
                _usuarioActual = null;
            }
        }

        /// <summary>
        /// Obtiene el usuario actualmente autenticado.
        /// </summary>
        public string? ObtenerUsuarioActual()
        {
            return _usuarioActual;
        }

        /// <summary>
        /// Verifica si hay un usuario autenticado.
        /// </summary>
        public bool EstaAutenticado()
        {
            return !string.IsNullOrEmpty(_usuarioActual);
        }

        /// <summary>
        /// Obtiene todos los usuarios disponibles (solo nombres, no contraseñas).
        /// </summary>
        public List<string> ObtenerUsuariosDisponibles()
        {
            return _usuarios.Keys.ToList();
        }

        /// <summary>
        /// Cambia la contraseña de un usuario.
        /// </summary>
        public bool CambiarContrasena(string usuario, string contrasenaActual, string contrasenaNueva)
        {
            if (_usuarios.TryGetValue(usuario, out var contrasenaAlmacenada))
            {
                if (contrasenaAlmacenada == contrasenaActual)
                {
                    _usuarios[usuario] = contrasenaNueva;
                    _auditoriaService.RegistrarAccion($"CONTRASEÑA cambiada", usuario);
                    return true;
                }
            }

            return false;
        }
    }
}
