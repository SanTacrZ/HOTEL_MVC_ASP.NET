using biblioteca_hotel.Clases;
using biblioteca_hotel.Interfaces;

namespace hotel_web_final.Servicios
{
    /// <summary>
    /// Servicio para gestionar servicios adicionales (batas, lavandería, restaurante) en las reservas.
    /// </summary>
    public class ServicioAdicionalService
    {
        private readonly ReservaService _reservaService;
        private readonly HabitacionService _habitacionService;
        private readonly AuditoriaService _auditoriaService;

        public ServicioAdicionalService(
            ReservaService reservaService,
            HabitacionService habitacionService,
            AuditoriaService auditoriaService)
        {
            _reservaService = reservaService;
            _habitacionService = habitacionService;
            _auditoriaService = auditoriaService;
        }

        /// <summary>
        /// Agrega un servicio de lavandería a una reserva.
        /// </summary>
        public void AgregarServicioLavanderia(int reservaId, string descripcion, int cantidadPrendas, decimal precioPorPrenda)
        {
            var reserva = _reservaService.BuscarPorId(reservaId);
            if (reserva == null)
                throw new Exception($"Reserva con ID {reservaId} no encontrada");

            var servicio = new ServicioLavanderia
            {
                Descripcion = descripcion,
                CantidadPrendas = cantidadPrendas,
                PrecioPorPrenda = precioPorPrenda,
                Fecha = DateTime.Now
            };

            reserva.ServiciosAdicionales.Add(servicio);

            // Registrar en auditoría
            _auditoriaService.RegistrarAccion(
                "ServicioAdicional",
                $"Lavandería agregada a reserva {reservaId}: {cantidadPrendas} prendas - {servicio.CalcularCosto():C}"
            );
        }

        /// <summary>
        /// Agrega un servicio de restaurante a una reserva.
        /// </summary>
        public void AgregarServicioRestaurante(int reservaId, TipoComida tipoComida, int cantidad, decimal precioUnitario)
        {
            var reserva = _reservaService.BuscarPorId(reservaId);
            if (reserva == null)
                throw new Exception($"Reserva con ID {reservaId} no encontrada");

            var servicio = new ServicioRestaurante
            {
                TipoComida = tipoComida,
                Cantidad = cantidad,
                PrecioUnitario = precioUnitario,
                Fecha = DateTime.Now
            };

            reserva.ServiciosAdicionales.Add(servicio);

            // Registrar en auditoría
            _auditoriaService.RegistrarAccion(
                "ServicioAdicional",
                $"Restaurante agregado a reserva {reservaId}: {tipoComida?.Nombre} x{cantidad} - {servicio.CalcularCosto():C}"
            );
        }

        /// <summary>
        /// Agrega venta de bata(s) a una reserva (solo para Suite y Ejecutiva).
        /// </summary>
        public void AgregarVentaBata(int reservaId, int habitacionId, string talla, int cantidad)
        {
            var reserva = _reservaService.BuscarPorId(reservaId);
            if (reserva == null)
                throw new Exception($"Reserva con ID {reservaId} no encontrada");

            var habitacion = _habitacionService.BuscarPorId(habitacionId);
            if (habitacion == null)
                throw new Exception($"Habitación con ID {habitacionId} no encontrada");

            // Verificar que la habitación sea Suite o Ejecutiva
            if (habitacion is not IVentaBata habitacionConBata)
                throw new Exception($"La habitación {habitacion.Numero} no tiene servicio de venta de batas");

            // Vender la bata usando el método de la habitación
            var bata = habitacionConBata.VenderBata(talla, cantidad);

            reserva.ServiciosAdicionales.Add(bata);

            // Registrar en auditoría
            _auditoriaService.RegistrarAccion(
                "ServicioAdicional",
                $"Bata vendida en reserva {reservaId}, habitación {habitacion.Numero}: Talla {talla} x{cantidad} - {bata.CalcularCosto():C}"
            );
        }

        /// <summary>
        /// Obtiene todos los servicios adicionales de una reserva.
        /// </summary>
        public List<IFacturable> ObtenerServiciosPorReserva(int reservaId)
        {
            var reserva = _reservaService.BuscarPorId(reservaId);
            return reserva?.ServiciosAdicionales ?? new List<IFacturable>();
        }

        /// <summary>
        /// Calcula el costo total de servicios adicionales de una reserva.
        /// </summary>
        public decimal CalcularCostoTotalServicios(int reservaId)
        {
            var reserva = _reservaService.BuscarPorId(reservaId);
            if (reserva?.ServiciosAdicionales == null)
                return 0m;

            return reserva.ServiciosAdicionales.Sum(s => s.CalcularCosto());
        }

        /// <summary>
        /// Elimina un servicio adicional de una reserva.
        /// </summary>
        public void EliminarServicio(int reservaId, int indiceServicio)
        {
            var reserva = _reservaService.BuscarPorId(reservaId);
            if (reserva == null)
                throw new Exception($"Reserva con ID {reservaId} no encontrada");

            if (indiceServicio < 0 || indiceServicio >= reserva.ServiciosAdicionales.Count)
                throw new Exception("Índice de servicio inválido");

            var servicio = reserva.ServiciosAdicionales[indiceServicio];
            reserva.ServiciosAdicionales.RemoveAt(indiceServicio);

            // Registrar en auditoría
            _auditoriaService.RegistrarAccion(
                "ServicioAdicional",
                $"Servicio eliminado de reserva {reservaId}: {servicio.ObtenerDescripcion()}"
            );
        }

        /// <summary>
        /// Obtiene las habitaciones de una reserva que pueden vender batas.
        /// </summary>
        public List<Habitacion> ObtenerHabitacionesConVentaBata(int reservaId)
        {
            var reserva = _reservaService.BuscarPorId(reservaId);
            if (reserva?.Habitaciones == null)
                return new List<Habitacion>();

            return reserva.Habitaciones
                .Where(h => h is IVentaBata)
                .ToList();
        }
    }
}
