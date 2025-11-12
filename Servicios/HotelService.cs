using biblioteca_hotel.Clases;

namespace hotel_web_final.Servicios
{
    public class HotelService
    {
        private Hotel? _hotel;
        private readonly HabitacionService _habitacionService;

        public HotelService(HabitacionService habitacionService)
        {
            _habitacionService = habitacionService;
        }

        public Hotel InicializarHotel()
        {
            if (_hotel != null)
                return _hotel;

            _hotel = new Hotel
            {
                Id = 1,
                Nombre = "Hotel Premium",
                Direccion = "Calle Principal 123",
                Telefono = "+57 300 123 4567",
                Email = "contacto@hotelpremium.com",
                Estrellas = 5
            };

            var oficinaReserva = new OficinaReserva
            {
                Id = 1,
                Nombre = "Oficina de Reservas",
                Direccion = _hotel.Direccion,
                Telefono = _hotel.Telefono,
                Email = "reservas@hotelpremium.com",
                Hotel = _hotel
            };

            var recepcion = new Recepcion
            {
                Id = 1,
                Nombre = "Recepción Principal",
                Ubicacion = "Lobby Principal",
                Hotel = _hotel
            };

            _hotel.OficinaReserva = oficinaReserva;
            _hotel.Recepcion = recepcion;

            var habitaciones = _habitacionService.ObtenerTodas().ToList();
            foreach (var habitacion in habitaciones)
            {
                habitacion.Hotel = _hotel;
            }
            _hotel.Habitaciones = habitaciones;

            return _hotel;
        }

        public Hotel? ObtenerHotel()
        {
            return _hotel;
        }

        public List<Habitacion> ObtenerHabitacionesDisponibles()
        {
            return _hotel?.ObtenerHabitacionesDisponibles() ?? new List<Habitacion>();
        }
    }
}

