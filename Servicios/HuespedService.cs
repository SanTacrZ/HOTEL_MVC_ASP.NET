using biblioteca_hotel.Clases;
using System.IO;

namespace hotel_web_final.Servicios
{
    public class HuespedService
    {
        private readonly List<Huesped> _huespedes = new();
        private int _nextId = 1;
        private readonly AuditoriaService _auditoriaService;

        public HuespedService(AuditoriaService auditoriaService)
        {
            _auditoriaService = auditoriaService;
        }

        public void Agregar(Huesped huesped)
        {
            if (huesped == null)
                throw new ArgumentNullException(nameof(huesped));

            ValidarHuesped(huesped);

            if (BuscarPorDocumento(huesped.NumeroDocumento) != null)
                throw new Exception($"Ya existe un huésped con el documento {huesped.NumeroDocumento}");

            huesped.IdHuesped = _nextId++;
            huesped.RegistrarHuesped();
            _huespedes.Add(huesped);

            // Registrar en auditoría
            _auditoriaService.RegistrarNuevoHuesped(
                $"{huesped.Nombre} {huesped.Apellido}",
                huesped.Nacionalidad
            );
        }

        private void ValidarHuesped(Huesped huesped)
        {
            ValidacionService.ValidarNombre(huesped.Nombre, "Nombre");
            ValidacionService.ValidarNombre(huesped.Apellido, "Apellido");
            ValidacionService.ValidarDocumento(huesped.NumeroDocumento, huesped.TipoDocumento?.Nombre);
            ValidacionService.ValidarTelefono(huesped.Telefono);
            ValidacionService.ValidarNacionalidad(huesped.Nacionalidad);
            ValidacionService.ValidarEmail(huesped.Email);
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
                if (datos.Length < 6)
                {
                    throw new Exception($"Línea con formato incorrecto (se esperan al menos 6 campos): {linea}");
                }

                try
                {
                    var tipoDoc = new TipoDocumento
                    {
                        Nombre = datos[0].Trim()
                    };

                    var huesped = new Huesped
                    {
                        TipoDocumento = tipoDoc,
                        NumeroDocumento = datos[1].Trim(),
                        Nombre = datos[2].Trim(),
                        Apellido = datos[3].Trim(),
                        Telefono = datos[4].Trim(),
                        Nacionalidad = datos[5].Trim(),
                        Email = datos.Length > 6 ? datos[6].Trim() : string.Empty
                    };

                    Agregar(huesped);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error procesando línea '{linea}': {ex.Message}");
                }
            }
        }

        public IEnumerable<Huesped> ObtenerTodos()
        {
            return _huespedes;
        }

        public Huesped? BuscarPorId(int id)
        {
            return _huespedes.FirstOrDefault(h => h.IdHuesped == id);
        }

        public Huesped? BuscarPorDocumento(string numeroDocumento)
        {
            return _huespedes.FirstOrDefault(h => h.NumeroDocumento == numeroDocumento);
        }

        public void Actualizar(Huesped huesped)
        {
            if (huesped == null)
                throw new ArgumentNullException(nameof(huesped));

            var existente = BuscarPorId(huesped.IdHuesped);
            if (existente == null)
                throw new Exception($"Huésped con ID {huesped.IdHuesped} no encontrado");

            ValidarHuesped(huesped);

            var otroHuesped = BuscarPorDocumento(huesped.NumeroDocumento);
            if (otroHuesped != null && otroHuesped.IdHuesped != huesped.IdHuesped)
                throw new Exception($"Ya existe otro huésped con el documento {huesped.NumeroDocumento}");

            var index = _huespedes.IndexOf(existente);
            _huespedes[index] = huesped;
        }

        public void Eliminar(int id)
        {
            var huesped = BuscarPorId(id);
            if (huesped != null)
            {
                _huespedes.Remove(huesped);
            }
        }
    }
}

