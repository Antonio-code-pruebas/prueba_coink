using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AntonioL.Api.Filters
{
    /// <summary>
    /// Filtro para urls de origen permitidas
    /// </summary>
    public class ValidateOriginFilter : IActionFilter
    {
        private readonly ILogger<ValidateOriginFilter> _logger;
        private readonly List<string> _allowedOrigins;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="allowedOrigins"></param>
        public ValidateOriginFilter(ILogger<ValidateOriginFilter> logger, List<string> allowedOrigins)
        {
            _logger = logger;
            _allowedOrigins = allowedOrigins;
        }

        /// <summary>
        /// Cuando ejecuté cualquier metodo de cualquier controlador
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Cuando ejecuto cualquier metodo de cualquier controlador
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var origin = context.HttpContext.Request.Headers["Origin"].ToString();

            // Verificar si la solicitud proviene de Swagger (basado en el User-Agent)
            var userAgent = context.HttpContext.Request.Headers["User-Agent"].ToString();
            // Obtener el header "Referer" para identificar solicitudes de Swagger
            var referer = context.HttpContext.Request.Headers["Referer"].ToString();

            if (userAgent.Contains("Swagger", StringComparison.OrdinalIgnoreCase) ||
                referer.Contains("/swagger", StringComparison.OrdinalIgnoreCase) )
            {
                // Bypass para solicitudes desde Swagger
                return;
            }

            // **Excepción para la ruta de confirmación de email**
            if (context.HttpContext.Request.Path.StartsWithSegments("/api/usuario/confirmemail"))
            {
                _logger.LogInformation("Excluyendo la validación del Origin para la ruta de confirmación de email.");
                return; // Permitir que la solicitud continúe
            }

            // Log the origin value
            _logger.LogInformation($"Request Origin: {origin}");


            if (string.IsNullOrEmpty(origin) ||!_allowedOrigins.Contains(origin))
            {
                _logger.LogWarning($"Unauthorized origin attempted access - origin: {origin}");
                _logger.LogWarning($"Unauthorized origin attempted access - useragent: {userAgent}");
                //_logger.LogWarning($"Unauthorized origin attempted access - referer: {referer}");
                //_logger.LogInformation("Unauthorized origin attempted access: {Origin}", origin);
                _logger.LogWarning("Request does not have an Origin header. Request URL: {Url}, Method: {Method}",
                    context.HttpContext.Request.Path, context.HttpContext.Request.Method);
                context.Result = new UnauthorizedResult();
            }
            else
            {
                _logger.LogInformation("Request received from origin: {Origin}", origin);
                _logger.LogInformation("Request received from origin: {userAgent}", userAgent);
                _logger.LogInformation("Request received from origin: {referer}", referer);

            }
        }
    }
}
