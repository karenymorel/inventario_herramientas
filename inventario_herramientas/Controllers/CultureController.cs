using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace inventario_herramientas.Web.Controllers
{
    public class CultureController : Controller
    {
        [HttpPost]
        public IActionResult SetCulture(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)));

            return LocalRedirect(returnUrl);
        }
    }
}
 
