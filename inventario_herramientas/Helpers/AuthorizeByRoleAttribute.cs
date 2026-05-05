using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace inventario_herramientas.Web.Helpers
{
    public class AuthorizeByRoleAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _claimsRequeridas;

        public AuthorizeByRoleAttribute(params string[] claimsRequeridas)
        {
            this._claimsRequeridas = claimsRequeridas;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                // Si el usuario no está autenticado, denegamos el acceso
                context.Result = new UnauthorizedResult();
                return;
            }

            // Filtramos los claims para encontrar el claim "admin"
            var claims = user.Claims.Where(x => x.Type == "admin").ToList();

            if (!claims.Any())
            {
                // Si no tiene ningún claim "admin", denegamos el acceso
                context.Result = new UnauthorizedResult();
                return;
            }

            // Verificamos si el usuario tiene alguno de los roles requeridos
            bool existe = claims.Any(claim => _claimsRequeridas.Contains(claim.Value));

            if (!existe)
            {
                // Si el claim "admin" no tiene el valor adecuado, denegamos el acceso
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
