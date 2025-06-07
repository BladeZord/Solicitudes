using es_solicitudes.Constans;
using es_solicitudes.exception;
using Microsoft.AspNetCore.Mvc;

namespace es_solicitudes.utils
{
    public static class DataValidator
    {
        public static ObjectResult ValidarResultadoExcepcion(ServiceException ServiceException)
        {
            ObjectResult Resultado;

            switch (ServiceException.Codigo)
            {
                case TipoError.NO_ENCONTRADO:
                    Resultado = new ObjectResult(ServiceException.Message)
                    {
                        StatusCode = StatusCodes.Status404NotFound
                    };
                    break;

                case TipoError.SERVICIO_INACCESIBLE:
                    Resultado = new ObjectResult(ServiceException.Message)
                    {
                        StatusCode = StatusCodes.Status502BadGateway
                    };
                    break;

                case TipoError.LOGICA_DE_NEGOCIO:
                    Resultado = new ObjectResult(ServiceException.Message)
                    {
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                    break;

                default:
                    Resultado = new ObjectResult(MensajesDelSistema.ERROR_INTERNO_SERVIDOR)
                    {
                        StatusCode = StatusCodes.Status502BadGateway
                    };
                    break;
            }
            return Resultado;
        }

        public static DateTime ValidarFecha(string fechaIngresada)
        {
            if (string.IsNullOrWhiteSpace(fechaIngresada))
            {
                throw new ArgumentException("La fecha ingresada no puede estar vacía.");
            }

            // Formatos válidos de entrada
            string[] formatosValidos = { "yyyy-MM-dd", "dd/MM/yyyy", "MM/dd/yyyy", "dd-MM-yyyy", "MM-dd-yyyy" };

            if (!DateTime.TryParseExact(fechaIngresada, formatosValidos, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime fechaConvertida))
            {
                throw new FormatException($"El formato de la fecha ingresada '{fechaIngresada}' no es válido.");
            }

            return fechaConvertida;
        }


    }
}
