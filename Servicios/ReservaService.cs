using biblioteca_hotel.Clases;

namespace hotel_web_final.Servicios
{
    public class ReservaService
    {
        private readonly List<Reserva> _reservas = new();
        private int _nextId = 1;
        private readonly HabitacionService _habitacionService;
        private readonly ClienteService _clienteService;
        private readonly HuespedService _huespedService;

        public ReservaService(HabitacionService habitacionService, ClienteService clienteService, HuespedService huespedService)
        {
            _habitacionService = habitacionService;
            _clienteService = clienteService;
            _huespedService = huespedService;
        }

        public Reserva CrearReserva(int clienteId, DateTime fechaEntrada, DateTime fechaSalida, int numHuespedes, List<int> habitacionIds, List<int>? huespedIds = null)
        {
            var cliente = _clienteService.BuscarPorId(clienteId);
            if (cliente == null)
                throw new Exception($"Cliente con ID {clienteId} no encontrado");

            var fechaHoy = DateTime.Today;

            if (fechaEntrada.Date < fechaHoy)
                throw new Exception("La fecha de entrada no puede ser anterior a hoy");

            if (fechaEntrada.Date >= fechaSalida.Date)
                throw new Exception("La fecha de entrada debe ser anterior a la fecha de salida");

            var reserva = new Reserva
            {
                Id = _nextId++,
                Cliente = cliente,
                FechaEntrada = fechaEntrada,
                FechaSalida = fechaSalida,
                NumHuespedes = numHuespedes,
                Estado = new EstadoReserva { Nombre = "Pendiente" },
                FechaCreacion = DateTime.Now,
                FechaModificacion = DateTime.Now
            };

            foreach (var habitacionId in habitacionIds)
            {
                var habitacion = _habitacionService.BuscarPorId(habitacionId);
                if (habitacion == null)
                    throw new Exception($"Habitación con ID {habitacionId} no encontrada");

                if (habitacion.Estado != "Disponible")
                    throw new Exception($"La habitación {habitacion.Numero} no está disponible");

                reserva.AsignarHabitacion(habitacion);
                habitacion.Reservar();
            }

            if (huespedIds != null)
            {
                foreach (var huespedId in huespedIds)
                {
                    var huesped = _huespedService.BuscarPorId(huespedId);
                    if (huesped != null)
                    {
                        reserva.Huespedes.Add(huesped);
                    }
                }
            }

            reserva.CalcularPrecioTotal();
            _reservas.Add(reserva);
            return reserva;
        }

        public IEnumerable<Reserva> ObtenerTodas()
        {
            return _reservas;
        }

        public Reserva? BuscarPorId(int id)
        {
            return _reservas.FirstOrDefault(r => r.Id == id);
        }

        public IEnumerable<Reserva> ObtenerPorCliente(int clienteId)
        {
            return _reservas.Where(r => r.Cliente?.IdCliente == clienteId);
        }

        public IEnumerable<Reserva> ObtenerPorFecha(DateTime fecha)
        {
            return _reservas.Where(r => r.FechaEntrada.Date <= fecha.Date && r.FechaSalida.Date >= fecha.Date);
        }

        public void ConfirmarReserva(int id)
        {
            var reserva = BuscarPorId(id);
            if (reserva == null)
                throw new Exception($"Reserva con ID {id} no encontrada");

            reserva.ConfirmarReserva();
        }

        public void CancelarReserva(int id)
        {
            var reserva = BuscarPorId(id);
            if (reserva == null)
                throw new Exception($"Reserva con ID {id} no encontrada");

            reserva.CancelarReserva();
            foreach (var habitacion in reserva.Habitaciones)
            {
                habitacion.Liberar();
            }
        }

        public void ActualizarReserva(Reserva reserva)
        {
            var existente = BuscarPorId(reserva.Id);
            if (existente == null)
                throw new Exception($"Reserva con ID {reserva.Id} no encontrada");

            var index = _reservas.IndexOf(existente);
            reserva.FechaModificacion = DateTime.Now;
            _reservas[index] = reserva;
        }
    }
}

