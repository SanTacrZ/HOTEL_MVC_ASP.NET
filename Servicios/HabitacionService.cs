using biblioteca_hotel.Clases;

namespace hotel_web_final.Servicios
{
    public class HabitacionService
    {
        private readonly List<Habitacion> _habitaciones = new();
        private int _nextId = 1;

        public void Agregar(Habitacion habitacion)
        {
            if (habitacion == null)
                throw new ArgumentNullException(nameof(habitacion));

            habitacion.Id = _nextId++;
            _habitaciones.Add(habitacion);
        }

        public void InicializarHabitaciones()
        {
            var tipoCamaDoble = new TipoCama { Id = 1, Nombre = "Doble" };
            var tipoCamaSencilla = new TipoCama { Id = 2, Nombre = "Sencilla" };
            var tipoCamaQueen = new TipoCama { Id = 3, Nombre = "Queen" };
            var tipoCamaKing = new TipoCama { Id = 4, Nombre = "King" };
            var tipoCamaSemidoble = new TipoCama { Id = 5, Nombre = "Semidoble" };

            int numeroHabitacion = 1;

            for (int piso = 2; piso <= 4; piso++)
            {
                for (int i = 1; i <= 10; i++)
                {
                    var sencilla = new Sencilla
                    {
                        Numero = $"{piso}{i:D2}",
                        PrecioPorNoche = 200000,
                        Estado = "Disponible",
                        TipoCama = i % 2 == 0 ? tipoCamaDoble : tipoCamaSencilla,
                        NumeroCamas = i % 2 == 0 ? 1 : 2,
                        Descripcion = $"Habitación sencilla en piso {piso}"
                    };
                    Agregar(sencilla);
                }
            }

            for (int i = 1; i <= 10; i++)
            {
                var ejecutiva = new Ejecutiva
                {
                    Numero = $"5{i:D2}",
                    PrecioPorNoche = 350000,
                    Estado = "Disponible",
                    TipoCama = i % 2 == 0 ? tipoCamaQueen : tipoCamaSemidoble,
                    NumeroCamas = i % 2 == 0 ? 1 : 2,
                    Descripcion = "Habitación ejecutiva con minibar"
                };
                Agregar(ejecutiva);
            }

            for (int i = 1; i <= 5; i++)
            {
                var suite = new Suite
                {
                    Numero = $"6{i:D2}",
                    PrecioPorNoche = 500000,
                    Estado = "Disponible",
                    TipoCama = i % 2 == 0 ? tipoCamaKing : tipoCamaQueen,
                    NumeroCamas = i % 2 == 0 ? 1 : 2,
                    Descripcion = "Suite de lujo con minibar completo"
                };
                Agregar(suite);
            }
        }

        public IEnumerable<Habitacion> ObtenerTodas()
        {
            return _habitaciones;
        }

        public Habitacion? BuscarPorId(int id)
        {
            return _habitaciones.FirstOrDefault(h => h.Id == id);
        }

        public Habitacion? BuscarPorNumero(string numero)
        {
            return _habitaciones.FirstOrDefault(h => h.Numero == numero);
        }

        public IEnumerable<Habitacion> ObtenerDisponibles()
        {
            return _habitaciones.Where(h => h.Estado == "Disponible");
        }

        public IEnumerable<Habitacion> ObtenerPorTipo(string tipo)
        {
            return _habitaciones.Where(h => h.Tipo == tipo);
        }

        public void Actualizar(Habitacion habitacion)
        {
            var existente = BuscarPorId(habitacion.Id);
            if (existente == null)
                throw new Exception($"Habitación con ID {habitacion.Id} no encontrada");

            var index = _habitaciones.IndexOf(existente);
            _habitaciones[index] = habitacion;
        }
    }
}

