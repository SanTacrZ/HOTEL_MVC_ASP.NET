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
                InicializarMinibar(ejecutiva);
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
                InicializarMinibar(suite);
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

        /// <summary>
        /// Inicializa el minibar de una habitación con productos estándar.
        /// </summary>
        /// <param name="habitacion">La habitación a la que se le inicializará el minibar</param>
        private void InicializarMinibar(Habitacion habitacion)
        {
            if (habitacion?.Minibar == null)
                return;

            // Productos básicos para todos los minibares
            var productos = new List<Producto>
            {
                new Agua { Id = 1, Nombre = "Agua Mineral", Precio = 3000, Stock = 4 },
                new Agua { Id = 2, Nombre = "Agua con Gas", Precio = 3500, Stock = 2 },
                new Gaseosa { Id = 3, Nombre = "Coca Cola", Precio = 4000, Stock = 3 },
                new Gaseosa { Id = 4, Nombre = "Sprite", Precio = 4000, Stock = 3 },
                new Jugo { Id = 5, Nombre = "Jugo de Naranja", Precio = 5000, Stock = 2 },
                new Jugo { Id = 6, Nombre = "Jugo de Manzana", Precio = 5000, Stock = 2 },
                new Snack { Id = 7, Nombre = "Papas Fritas", Precio = 6000, Stock = 3 },
                new Snack { Id = 8, Nombre = "Maní", Precio = 5000, Stock = 3 },
                new Snack { Id = 9, Nombre = "Chocolate", Precio = 7000, Stock = 2 }
            };

            // Si es una Suite, agregar productos premium
            if (habitacion is Suite)
            {
                productos.AddRange(new List<Producto>
                {
                    new Vino { Id = 10, Nombre = "Vino Tinto", Precio = 45000, Stock = 2 },
                    new Vino { Id = 11, Nombre = "Vino Blanco", Precio = 45000, Stock = 2 },
                    new Licor { Id = 12, Nombre = "Whisky", Precio = 80000, Stock = 1 },
                    new Licor { Id = 13, Nombre = "Vodka", Precio = 70000, Stock = 1 },
                    new Bebida { Id = 14, Nombre = "Champagne", Precio = 120000, Stock = 1 }
                });
            }

            // Agregar los productos al minibar
            foreach (var producto in productos)
            {
                habitacion.Minibar.AgregarProducto(producto);
            }
        }
    }
}

