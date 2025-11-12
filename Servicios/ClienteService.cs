using biblioteca_hotel.Clases;
using System.IO;

namespace hotel_web_final.Servicios
{
    public class ClienteService
    {
        private readonly List<Cliente> _clientes = new();
        private int _nextId = 1;

        public void Agregar(Cliente cliente)
        {
            if (cliente == null)
                throw new ArgumentNullException(nameof(cliente));

            ValidarCliente(cliente);

            if (BuscarPorDocumento(cliente.NumeroDocumento) != null)
                throw new Exception($"Ya existe un cliente con el documento {cliente.NumeroDocumento}");

            cliente.IdCliente = _nextId++;
            cliente.RegistrarCliente();
            _clientes.Add(cliente);
        }

        private void ValidarCliente(Cliente cliente)
        {
            ValidacionService.ValidarNombre(cliente.Nombre, "Nombre");
            ValidacionService.ValidarNombre(cliente.Apellido, "Apellido");
            ValidacionService.ValidarDocumento(cliente.NumeroDocumento, cliente.TipoDocumento?.Nombre);
            ValidacionService.ValidarTelefono(cliente.Telefono);
            ValidacionService.ValidarEmail(cliente.Email);
        }

        public void Cargar(string rutaArchivo)
        {
            if (!File.Exists(rutaArchivo))
            {
                throw new FileNotFoundException($"No se encontró el archivo: {rutaArchivo}");
            }

            var lineas = File.ReadAllLines(rutaArchivo);
            foreach (var linea in lineas)
            {
                if (string.IsNullOrWhiteSpace(linea) || linea.StartsWith("#"))
                    continue;

                var datos = linea.Split('|');
                if (datos.Length < 8)
                {
                    throw new Exception($"Línea con formato incorrecto (se esperan al menos 8 campos): {linea}");
                }

                try
                {
                    var tipoDoc = new TipoDocumento
                    {
                        Nombre = datos[0].Trim()
                    };

                    var cliente = new Cliente
                    {
                        TipoDocumento = tipoDoc,
                        NumeroDocumento = datos[1].Trim(),
                        Nombre = datos[2].Trim(),
                        Apellido = datos[3].Trim(),
                        Telefono = datos[4].Trim(),
                        Email = datos[5].Trim(),
                        NumeroTarjetaCredito = datos[6].Trim(),
                        TipoCliente = datos[7].Trim(),
                        Preferencias = datos.Length > 8 ? datos[8].Trim() : string.Empty
                    };

                    Agregar(cliente);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error procesando línea '{linea}': {ex.Message}");
                }
            }
        }

        public IEnumerable<Cliente> ObtenerTodos()
        {
            return _clientes;
        }

        public Cliente? BuscarPorId(int id)
        {
            return _clientes.FirstOrDefault(c => c.IdCliente == id);
        }

        public Cliente? BuscarPorDocumento(string numeroDocumento)
        {
            return _clientes.FirstOrDefault(c => c.NumeroDocumento == numeroDocumento);
        }

        public void Actualizar(Cliente cliente)
        {
            if (cliente == null)
                throw new ArgumentNullException(nameof(cliente));

            var existente = BuscarPorId(cliente.IdCliente);
            if (existente == null)
                throw new Exception($"Cliente con ID {cliente.IdCliente} no encontrado");

            ValidarCliente(cliente);

            var otroCliente = BuscarPorDocumento(cliente.NumeroDocumento);
            if (otroCliente != null && otroCliente.IdCliente != cliente.IdCliente)
                throw new Exception($"Ya existe otro cliente con el documento {cliente.NumeroDocumento}");

            var index = _clientes.IndexOf(existente);
            _clientes[index] = cliente;
        }

        public void Eliminar(int id)
        {
            var cliente = BuscarPorId(id);
            if (cliente != null)
            {
                _clientes.Remove(cliente);
            }
        }
    }
}

