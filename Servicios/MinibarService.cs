using biblioteca_hotel.Clases;
using biblioteca_hotel.Interfaces;

namespace hotel_web_final.Servicios
{
    public class MinibarService
    {
        private readonly HabitacionService _habitacionService;
        private readonly AuditoriaService _auditoriaService;

        public MinibarService(HabitacionService habitacionService, AuditoriaService auditoriaService)
        {
            _habitacionService = habitacionService;
            _auditoriaService = auditoriaService;
        }

        /// <summary>
        /// Obtiene el minibar de una habitación específica.
        /// </summary>
        /// <param name="habitacionId">ID de la habitación</param>
        /// <returns>El minibar de la habitación o null si no tiene</returns>
        public Minibar? ObtenerMinibarPorHabitacion(int habitacionId)
        {
            var habitacion = _habitacionService.BuscarPorId(habitacionId);
            return habitacion?.Minibar;
        }

        /// <summary>
        /// Registra el consumo de un producto del minibar.
        /// </summary>
        /// <param name="habitacionId">ID de la habitación</param>
        /// <param name="productoId">ID del producto consumido</param>
        /// <param name="cantidad">Cantidad consumida</param>
        public void RegistrarConsumo(int habitacionId, int productoId, int cantidad)
        {
            var habitacion = _habitacionService.BuscarPorId(habitacionId);
            if (habitacion?.Minibar == null)
                throw new Exception($"La habitación {habitacionId} no tiene minibar");

            var producto = habitacion.Minibar.ProductosDisponibles
                .FirstOrDefault(p => p.Id == productoId);

            if (producto == null)
                throw new Exception($"El producto {productoId} no se encontró en el minibar");

            if (producto.Stock < cantidad)
                throw new Exception($"Stock insuficiente. Disponible: {producto.Stock}, Solicitado: {cantidad}");

            habitacion.Minibar.RegistrarConsumo(producto, cantidad);

            // Registrar en auditoría
            _auditoriaService.RegistrarConsumoMinibar(
                habitacionId,
                producto.Nombre,
                cantidad,
                producto.ObtenerPrecio() * cantidad
            );
        }

        /// <summary>
        /// Obtiene todos los consumos realizados en una habitación.
        /// </summary>
        /// <param name="habitacionId">ID de la habitación</param>
        /// <returns>Lista de consumos</returns>
        public List<Consumo> ObtenerConsumosPorHabitacion(int habitacionId)
        {
            var habitacion = _habitacionService.BuscarPorId(habitacionId);
            return habitacion?.Minibar?.ConsumosRealizados ?? new List<Consumo>();
        }

        /// <summary>
        /// Calcula el costo total de consumos de una habitación.
        /// </summary>
        /// <param name="habitacionId">ID de la habitación</param>
        /// <returns>Costo total de los consumos</returns>
        public decimal CalcularCostoTotal(int habitacionId)
        {
            var habitacion = _habitacionService.BuscarPorId(habitacionId);

            if (habitacion is IMinibar habitacionConMinibar)
            {
                return habitacionConMinibar.CalcularCostoConsumo();
            }

            return 0m;
        }

        /// <summary>
        /// Verifica si una habitación tiene minibar.
        /// </summary>
        /// <param name="habitacionId">ID de la habitación</param>
        /// <returns>True si tiene minibar, false en caso contrario</returns>
        public bool TieneMinibar(int habitacionId)
        {
            var habitacion = _habitacionService.BuscarPorId(habitacionId);
            return habitacion is IMinibar && habitacion.Minibar != null;
        }
    }
}
