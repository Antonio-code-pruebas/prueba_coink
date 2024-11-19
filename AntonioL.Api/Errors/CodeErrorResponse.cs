namespace AntonioL.Api.Errors
{
    /// <summary>
    /// 
    /// </summary>
    public class CodeErrorResponse
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="message"></param>
        public CodeErrorResponse(int statusCode, string? message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageStatusCode(statusCode);
        }

        public int StatusCode { get; set; }
        public string? Message { get; set; }

        private static string? GetDefaultMessageStatusCode(int statusCode)
        {

            return statusCode switch
            {
                400 => "El Request enviado tiene errores",
                401 => "No tienes autorización para este recurso",
                403 => "Error en usuario y/o contraseña",
                404 => "No se encontró el item  buscado",
                405 => "Error Usuario Bloqueado!",
                500 => "Se produjeron errores en el servidor",
                _ => null
            };
        }


    }
}
