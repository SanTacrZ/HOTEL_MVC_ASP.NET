using biblioteca_hotel.Aspectos;
using biblioteca_hotel.Clases;
using biblioteca_hotel.Interfaces;

namespace hotel_web_final.Servicios
{
    /// <summary>
    /// Servicio para enviar notificaciones a clientes.
    /// Implementa el aspecto de Notificación de la biblioteca.
    /// </summary>
    public class NotificacionService
    {
        private readonly AuditoriaService _auditoriaService;

        public NotificacionService(AuditoriaService auditoriaService)
        {
            _auditoriaService = auditoriaService;
        }

        /// <summary>
        /// Envía notificación de confirmación de reserva.
        /// </summary>
        public void EnviarConfirmacionReserva(string destinatario, int reservaId, DateTime fechaEntrada, DateTime fechaSalida)
        {
            var mensaje = $"Su reserva #{reservaId} ha sido confirmada. Entrada: {fechaEntrada:dd/MM/yyyy}, Salida: {fechaSalida:dd/MM/yyyy}";
            EnviarNotificacion(destinatario, mensaje, "Confirmación de Reserva");
        }

        /// <summary>
        /// Envía notificación de check-in.
        /// </summary>
        public void EnviarNotificacionCheckIn(string destinatario, string numeroHabitacion)
        {
            var mensaje = $"Bienvenido! Su check-in ha sido realizado. Habitación: {numeroHabitacion}. Que disfrute su estadía!";
            EnviarNotificacion(destinatario, mensaje, "Check-In Realizado");
        }

        /// <summary>
        /// Envía notificación de check-out con factura.
        /// </summary>
        public void EnviarNotificacionCheckOut(string destinatario, string numeroFactura, decimal montoTotal)
        {
            var mensaje = $"Gracias por su estadía! Su check-out ha sido procesado. Factura: {numeroFactura}, Total: {montoTotal:C}. Esperamos verle pronto!";
            EnviarNotificacion(destinatario, mensaje, "Check-Out y Factura");
        }

        /// <summary>
        /// Envía notificación de cancelación de reserva.
        /// </summary>
        public void EnviarNotificacionCancelacion(string destinatario, int reservaId)
        {
            var mensaje = $"Su reserva #{reservaId} ha sido cancelada exitosamente.";
            EnviarNotificacion(destinatario, mensaje, "Cancelación de Reserva");
        }

        /// <summary>
        /// Método privado para enviar notificaciones.
        /// Simula el envío ya que no tenemos servidor de correo real.
        /// </summary>
        private void EnviarNotificacion(string destinatario, string mensaje, string tipo)
        {
            try
            {
                // Crear email usando la clase de la biblioteca
                var email = new Email();

                // Usar el aspecto de Notificacion
                var notificacion = new Notificacion(email);

                // Como no tenemos servidor real de correo, esto simulará el envío
                // En producción aquí se enviaría el correo real
                notificacion.EnviarNotificacion(mensaje, destinatario);

                // Registrar en auditoría
                _auditoriaService.RegistrarAccion(
                    $"NOTIFICACIÓN enviada",
                    "Sistema",
                    $"{tipo} - Destinatario: {destinatario}"
                );
            }
            catch (Exception ex)
            {
                // Log del error pero no fallar la operación principal
                _auditoriaService.RegistrarAccion(
                    $"ERROR al enviar notificación",
                    "Sistema",
                    $"{tipo} - Destinatario: {destinatario} - Error: {ex.Message}"
                );
            }
        }

        /// <summary>
        /// Envía una notificación personalizada.
        /// </summary>
        public void EnviarNotificacionPersonalizada(string destinatario, string asunto, string mensaje)
        {
            EnviarNotificacion(destinatario, mensaje, asunto);
        }

        /// <summary>
        /// Envía la factura por correo electrónico al cliente.
        /// </summary>
        public void EnviarFacturaPorCorreo(string destinatario, string numeroFactura, decimal montoTotal, DateTime fechaEmision, string nombreCliente)
        {
            var mensaje = $@"
Estimado/a {nombreCliente},

Gracias por hospedarse en Hotel Santa. Adjuntamos los detalles de su factura:

Factura: #{numeroFactura}
Fecha: {fechaEmision:dd/MM/yyyy HH:mm}
Monto Total: {montoTotal:C}

Esperamos que haya disfrutado su estadía y esperamos verle nuevamente pronto.

Atentamente,
Hotel Santa
Sistema de Gestión Hotelera
            ";

            EnviarNotificacion(destinatario, mensaje, "Factura de Hotel Santa");
        }
    }
}
