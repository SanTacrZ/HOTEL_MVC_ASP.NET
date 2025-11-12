using System.Text.RegularExpressions;

namespace hotel_web_final.Servicios
{
    public static class ValidacionService
    {
        public static void ValidarNombre(string nombre, string campo = "Nombre")
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException($"{campo} no puede estar vacío");

            if (Regex.IsMatch(nombre, @"\d"))
                throw new ArgumentException($"{campo} no puede contener números");

            if (!Regex.IsMatch(nombre, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
                throw new ArgumentException($"{campo} solo puede contener letras y espacios");
        }

        public static void ValidarDocumento(string documento, string tipoDocumento = "CC")
        {
            if (string.IsNullOrWhiteSpace(documento))
                throw new ArgumentException("El número de documento no puede estar vacío");

            if (tipoDocumento == "CC" || tipoDocumento == "TI" || tipoDocumento == "CE")
            {
                if (documento.Length > 10)
                    throw new ArgumentException("El número de documento no puede tener más de 10 dígitos");

                if (!Regex.IsMatch(documento, @"^\d+$"))
                    throw new ArgumentException("El número de documento solo puede contener dígitos");
            }
        }

        public static void ValidarTelefono(string telefono)
        {
            if (string.IsNullOrWhiteSpace(telefono))
                throw new ArgumentException("El teléfono no puede estar vacío");

            telefono = telefono.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");

            if (telefono.Length != 10)
                throw new ArgumentException("El teléfono debe tener exactamente 10 dígitos");

            if (!Regex.IsMatch(telefono, @"^\d+$"))
                throw new ArgumentException("El teléfono solo puede contener dígitos");
        }

        public static void ValidarNacionalidad(string nacionalidad)
        {
            if (string.IsNullOrWhiteSpace(nacionalidad))
                throw new ArgumentException("La nacionalidad no puede estar vacía");

            if (Regex.IsMatch(nacionalidad, @"\d"))
                throw new ArgumentException("La nacionalidad no puede contener números");

            if (!Regex.IsMatch(nacionalidad, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
                throw new ArgumentException("La nacionalidad solo puede contener letras y espacios");
        }

        public static void ValidarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return;

            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(email))
                throw new ArgumentException("El formato del email no es válido");
        }
    }
}

