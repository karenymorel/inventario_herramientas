using inventario_herramientas.Managers.Entidades.Audit;
using inventario_herramientas.Managers.Repos;
using Microsoft.Extensions.Hosting;

namespace inventario_herramientas.Web.Helpers
{
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;

        public AuditMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext
context, LogAuditoriaRepositorio logRepo)
        {
            if (context.Request.Method == "POST" ||
context.Request.Method == "PUT" ||
context.Request.Method == "DELETE")
            {
                var usuario =
context.User.Identity?.Name ?? "Anónimo";
                var accion =
$"{context.Request.Method} {context.Request.Path}";

                var log = new LogAuditoria
                {
                    usuario = usuario,
                    accion = accion,
                    fecha = DateTime.Now,
                    detalles = $"IP: { context.Connection.RemoteIpAddress }"
                };

                logRepo.GuardarLog(log);
            }

            await _next(context);
        }
    }
}