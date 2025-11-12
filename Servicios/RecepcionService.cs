using biblioteca_hotel.Clases;

namespace hotel_web_final.Servicios
{
    public class RecepcionService
    {
        private readonly List<Factura> _facturas = new();
        private int _nextFacturaId = 1;
        private readonly ReservaService _reservaService;
        private readonly HotelService _hotelService;

        public RecepcionService(ReservaService reservaService, HotelService hotelService)
        {
            _reservaService = reservaService;
            _hotelService = hotelService;
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

            foreach (var habitacion in reserva.Habitaciones)
            {
                subtotal += habitacion.PrecioPorNoche * noches;
            }

            decimal seguro = subtotal * 0.025m;
            decimal iva = 0;

            var esColombiano = reserva.Huespedes.Any(h => h.Nacionalidad?.ToLower() == "colombia" || 
                                                          h.Nacionalidad?.ToLower() == "colombiano");
            if (esColombiano)
            {
                iva = subtotal * 0.19m;
            }

            factura.MontoTotal = subtotal + seguro + iva;
            factura.Observaciones = $"Noches: {noches}, Seguro: {seguro:C}, IVA: {iva:C}";

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

