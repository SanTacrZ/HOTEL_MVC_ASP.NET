using biblioteca_hotel.Aspectos;

namespace hotel_web_final.Servicios
{
    /// <summary>
    /// Servicio para gestionar la auditoría del sistema.
    /// Registra todas las acciones importantes realizadas en el hotel.
    /// </summary>
    public class AuditoriaService
    {
        private readonly Auditoria _auditoria;

        public AuditoriaService()
        {
            _auditoria = new Auditoria();
        }

        /// <summary>
        /// Registra una acción en el sistema de auditoría.
        /// </summary>
        /// <param name="accion">Descripción de la acción realizada</param>
        /// <param name="usuario">Usuario que realizó la acción (opcional)</param>
        /// <param name="detalles">Detalles adicionales (opcional)</param>
        public void RegistrarAccion(string accion, string usuario = "Sistema", string detalles = "")
        {
            var mensaje = $"[{usuario}] {accion}";
            if (!string.IsNullOrEmpty(detalles))
            {
                mensaje += $" - {detalles}";
            }
            _auditoria.RegistrarEvento(mensaje);
        }

        /// <summary>
        /// Registra una acción de check-in.
        /// </summary>
        public void RegistrarCheckIn(int reservaId, string clienteNombre, List<string> habitaciones)
        {
            var habsStr = string.Join(", ", habitaciones);
            RegistrarAccion(
                $"CHECK-IN realizado",
                "Recepción",
                $"Reserva #{reservaId} - Cliente: {clienteNombre} - Habitaciones: {habsStr}"
            );
        }

        /// <summary>
        /// Registra una acción de check-out.
        /// </summary>
        public void RegistrarCheckOut(int reservaId, string clienteNombre, string numeroFactura, decimal monto)
        {
            RegistrarAccion(
                $"CHECK-OUT realizado",
                "Recepción",
                $"Reserva #{reservaId} - Cliente: {clienteNombre} - Factura: {numeroFactura} - Monto: {monto:C}"
            );
        }

        /// <summary>
        /// Registra la creación de una reserva.
        /// </summary>
        public void RegistrarCreacionReserva(int reservaId, string clienteNombre, DateTime fechaEntrada, DateTime fechaSalida)
        {
            RegistrarAccion(
                $"RESERVA creada",
                "Sistema",
                $"Reserva #{reservaId} - Cliente: {clienteNombre} - Entrada: {fechaEntrada:dd/MM/yyyy} - Salida: {fechaSalida:dd/MM/yyyy}"
            );
        }

        /// <summary>
        /// Registra la confirmación de una reserva.
        /// </summary>
        public void RegistrarConfirmacionReserva(int reservaId)
        {
            RegistrarAccion(
                $"RESERVA confirmada",
                "Sistema",
                $"Reserva #{reservaId}"
            );
        }

        /// <summary>
        /// Registra la cancelación de una reserva.
        /// </summary>
        public void RegistrarCancelacionReserva(int reservaId, string motivo = "")
        {
            var detalles = $"Reserva #{reservaId}";
            if (!string.IsNullOrEmpty(motivo))
            {
                detalles += $" - Motivo: {motivo}";
            }
            RegistrarAccion("RESERVA cancelada", "Sistema", detalles);
        }

        /// <summary>
        /// Registra un consumo de minibar.
        /// </summary>
        public void RegistrarConsumoMinibar(int habitacionId, string producto, int cantidad, decimal precio)
        {
            RegistrarAccion(
                $"CONSUMO MINIBAR",
                "Recepción",
                $"Habitación #{habitacionId} - {producto} x{cantidad} - Total: {precio:C}"
            );
        }

        /// <summary>
        /// Registra el registro de un nuevo cliente.
        /// </summary>
        public void RegistrarNuevoCliente(string nombreCompleto, string documento)
        {
            RegistrarAccion(
                $"CLIENTE registrado",
                "Recepción",
                $"{nombreCompleto} - Doc: {documento}"
            );
        }

        /// <summary>
        /// Registra el registro de un nuevo huésped.
        /// </summary>
        public void RegistrarNuevoHuesped(string nombreCompleto, string nacionalidad)
        {
            RegistrarAccion(
                $"HUÉSPED registrado",
                "Recepción",
                $"{nombreCompleto} - Nacionalidad: {nacionalidad}"
            );
        }

        /// <summary>
        /// Registra un inicio de sesión.
        /// </summary>
        public void RegistrarLogin(string usuario, bool exitoso)
        {
            var resultado = exitoso ? "EXITOSO" : "FALLIDO";
            RegistrarAccion($"LOGIN {resultado}", usuario);
        }

        /// <summary>
        /// Obtiene todos los registros de auditoría.
        /// </summary>
        /// <returns>Lista de registros</returns>
        public List<string> ObtenerRegistros()
        {
            return _auditoria.RegistroEventos;
        }

        /// <summary>
        /// Obtiene los últimos N registros de auditoría.
        /// </summary>
        public List<string> ObtenerUltimosRegistros(int cantidad = 50)
        {
            var registros = _auditoria.RegistroEventos;
            return registros.Skip(Math.Max(0, registros.Count - cantidad)).ToList();
        }

        /// <summary>
        /// Limpia todos los registros de auditoría.
        /// </summary>
        public void LimpiarRegistros()
        {
            _auditoria.RegistroEventos.Clear();
            RegistrarAccion("AUDITORÍA limpiada", "Administrador");
        }
    }
}
