namespace WebApiAutores.Middleware
{
    public static class LoggearRespuestaHTTPMiddlewareExtensions 
    {
        public static IApplicationBuilder UseLoggearRespuestaHTTP(this IApplicationBuilder app) 
        {
            return app.UseMiddleware<LoggearRespuestaHTTPMiddleware>();
        }
    }
    public class LoggearRespuestaHTTPMiddleware
    {
        private readonly RequestDelegate siguiente;
        private readonly ILogger<LoggearRespuestaHTTPMiddleware> logger;

        public LoggearRespuestaHTTPMiddleware(RequestDelegate siguiente, ILogger<LoggearRespuestaHTTPMiddleware> logger)
        {
            this.siguiente = siguiente;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext contexto) 
        {
            using (var ms = new MemoryStream())
            {
                var cuerpoOriginalRespuesta = contexto.Response.Body;
                contexto.Response.Body = ms;

                await siguiente(contexto);

                ms.Seek(0, SeekOrigin.Begin);
                string respuesta = new StreamReader(ms).ReadToEnd();
                ms.Seek(0, SeekOrigin.Begin);

                await ms.CopyToAsync(cuerpoOriginalRespuesta);
                contexto.Response.Body = cuerpoOriginalRespuesta;
                logger.LogInformation(respuesta);
            }
        }
    }
}
