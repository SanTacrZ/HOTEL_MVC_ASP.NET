using biblioteca_hotel.Clases;
using biblioteca_hotel.Interfaces;

namespace hotel_web_final.Servicios
{
    public class RecepcionService
    {
        private readonly List<Factura> _facturas = new();
        private int _nextFacturaId = 1;
        private readonly ReservaService _reservaService;
        private readonly HotelService _hotelService;
        private readonly AuditoriaService _auditoriaService;

        public RecepcionService(ReservaService reservaService, HotelService hotelService, AuditoriaService auditoriaService)
        {
            _reservaService = reservaService;
            _hotelService = hotelService;
            _auditoriaService = auditoriaService;
        }

        public void RealizarCheckIn(int reservaId)
        {
            var reserva = _reservaService.BuscarPorId(reservaId);
            if (reserva == null)
                throw new Exception($"Reserva con ID {reservaId} no encontrada");

            var hotel = _hotelService.ObtenerHotel();
            if (hotel?.Recepcion == null)
                throw new Exception("Recepción no inicializada");

            hotel.Recepcion.RealizarCheckIn(reserva, reserva.Huespedes);

            // Registrar en auditoría
            var habitaciones = reserva.Habitaciones.Select(h => h.Numero).ToList();
            var clienteNombre = $"{reserva.Cliente?.Nombre} {reserva.Cliente?.Apellido}";
            _auditoriaService.RegistrarCheckIn(reservaId, clienteNombre, habitaciones);
        }

        public Factura RealizarCheckOut(int reservaId, string metodoPago)
        {
            var reserva = _reservaService.BuscarPorId(reservaId);
            if (reserva == null)
                throw new Exception($"Reserva con ID {reservaId} no encontrada");

            var hotel = _hotelService.ObtenerHotel();
            if (hotel?.Recepcion == null)
                throw new Exception("Recepción no inicializada");

            var factura = GenerarFactura(reserva);
            hotel.Recepcion.RealizarCheckOut(reserva);
            hotel.Recepcion.GestionarPagos(factura, metodoPago);

            _facturas.Add(factura);

            // Registrar en auditoría
            var clienteNombre = $"{reserva.Cliente?.Nombre} {reserva.Cliente?.Apellido}";
            _auditoriaService.RegistrarCheckOut(reservaId, clienteNombre, factura.NumeroFactura, factura.MontoTotal);

            return factura;
        }

        private Factura GenerarFactura(Reserva reserva)
        {
            var hotel = _hotelService.ObtenerHotel();
            var factura = new Factura
            {
                Id = _nextFacturaId++,
                NumeroFactura = $"FAC-{DateTime.Now:yyyyMMdd}-{_nextFacturaId - 1:D4}",
                Cliente = reserva.Cliente,
                Reserva = reserva,
                Recepcion = hotel?.Recepcion
            };

            int noches = (reserva.FechaSalida - reserva.FechaEntrada).Days;
            decimal subtotal = 0;
            var detalleObservaciones = new System.Text.StringBuilder();

            // DETALLE DE ESTADÍA
            detalleObservaciones.AppendLine("=== DETALLE DE FACTURA ===");
            detalleObservaciones.AppendLine($"Estadía: {reserva.FechaEntrada:dd/MM/yyyy} al {reserva.FechaSalida:dd/MM/yyyy}");
            detalleObservaciones.AppendLine($"Total de noches: {noches}");
            detalleObservaciones.AppendLine();

            // DETALLE DE HABITACIONES
            detalleObservaciones.AppendLine("--- HABITACIONES ---");
            foreach (var habitacion in reserva.Habitaciones)
            {
                decimal costoHabitacion = habitacion.PrecioPorNoche * noches;
                subtotal += costoHabitacion;
                detalleObservaciones.AppendLine($"• Habitación {habitacion.Numero} ({habitacion.Tipo}): {habitacion.PrecioPorNoche:C} x {noches} noches = {costoHabitacion:C}");
            }
            detalleObservaciones.AppendLine($"Subtotal Habitaciones: {subtotal:C}");
            detalleObservaciones.AppendLine();

            // DETALLE DE MINIBAR
            decimal costoMinibar = 0;
            var hayConsumos = false;

            detalleObservaciones.AppendLine("--- CONSUMO DE MINIBAR ---");
            foreach (var habitacion in reserva.Habitaciones)
            {
                if (habitacion is IMinibar habitacionConMinibar && habitacion.Minibar?.ConsumosRealizados != null && habitacion.Minibar.ConsumosRealizados.Any())
                {
                    hayConsumos = true;
                    detalleObservaciones.AppendLine($"Habitación {habitacion.Numero}:");

                    foreach (var consumo in habitacion.Minibar.ConsumosRealizados)
                    {
                        decimal subtotalConsumo = consumo.CalcularSubtotal();
                        costoMinibar += subtotalConsumo;
                        detalleObservaciones.AppendLine($"  • {consumo.ProductoConsumido?.Nombre} x{consumo.Cantidad} = {subtotalConsumo:C}");
                    }
                }
            }

            if (hayConsumos)
            {
                detalleObservaciones.AppendLine($"Subtotal Minibar: {costoMinibar:C}");
            }
            else
            {
                detalleObservaciones.AppendLine("No se registraron consumos");
            }
            detalleObservaciones.AppendLine();

            // CARGOS ADICIONALES
            decimal seguro = subtotal * 0.025m;
            decimal iva = 0;

            var esColombiano = reserva.Huespedes.Any(h => h.Nacionalidad?.ToLower() == "colombia" ||
                                                          h.Nacionalidad?.ToLower() == "colombiano");

            detalleObservaciones.AppendLine("--- CARGOS ADICIONALES ---");
            detalleObservaciones.AppendLine($"Seguro hotelero (2.5%): {seguro:C}");

            if (esColombiano)
            {
                iva = subtotal * 0.19m;
                detalleObservaciones.AppendLine($"IVA (19%): {iva:C}");
            }
            else
            {
                detalleObservaciones.AppendLine("IVA (19%): No aplica (huésped extranjero)");
            }
            detalleObservaciones.AppendLine();

            // TOTAL
            factura.MontoTotal = subtotal + seguro + iva + costoMinibar;
            detalleObservaciones.AppendLine("=========================");
            detalleObservaciones.AppendLine($"TOTAL A PAGAR: {factura.MontoTotal:C}");

            factura.Observaciones = detalleObservaciones.ToString();

            return factura;
        }

        public IEnumerable<Factura> ObtenerTodasFacturas()
        {
            return _facturas;
        }

        public Factura? BuscarFacturaPorId(int id)
        {
            return _facturas.FirstOrDefault(f => f.Id == id);
        }

        public Factura? BuscarFacturaPorReserva(int reservaId)
        {
            return _facturas.FirstOrDefault(f => f.Reserva?.Id == reservaId);
        }
    }
}

