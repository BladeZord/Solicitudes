namespace es_solicitudes.Constans
{
    public static class ApiConstants
    {
        public static class Routes
        {
            public const string BaseRoute = "v1";
            public const string Type = "es";
            public const string ControllerName = "solicitudes";
            public const string PathSeparator = "/";
            public const string BasePath = $"{BaseRoute}{PathSeparator}{Type}{PathSeparator}{ControllerName}";
        }

        public static class ErrorMessages
        {
            public const string InvalidInput = "Los datos de entrada no son válidos";
            public const string NotFound = "El recurso solicitado no fue encontrado";
            public const string InternalError = "Error interno del servidor";
            public const string InsertError = "Error al intentar guardar";  
        }

        public static class LogMessages
        {
            public const string OperationStart = "Iniciando operación {Operation}";
            public const string OperationEnd = "Finalizando operación {Operation}";
            public const string OperationError = "Error en operación {Operation}: {Error}";
            public const string UpdateError = "Error en operación {Operation}: {Error}";            
        }
    }
} 