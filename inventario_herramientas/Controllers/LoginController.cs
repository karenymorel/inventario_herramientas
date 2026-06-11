using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using inventario_herramientas.Managers.Managers;
using inventario_herramientas.Managers.Entidades;

namespace inventario_herramientas.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUsuariosManager _usuariosManager;

        // Constructor
        public LoginController(IUsuariosManager usuariosManager)
        {
            _usuariosManager = usuariosManager;
        }

        public IActionResult LoginIndex()
        {
            return View();
        }

        public async Task<IActionResult> Login()
        {
            var redirectUri = Url.Action("GoogleResponse", "Login", null, Request.Scheme, Request.Host.Value);

            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties
            {
                RedirectUri = redirectUri
            });
            return new EmptyResult();
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Verifica si la autenticación fue exitosa
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", "Login");
            }

            var google_identificador = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var nombre = result.Principal.FindFirst(ClaimTypes.Name)?.Value;
            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;

            var usuario = _usuariosManager.GetUsuarioPorGoogleIdentificador(google_identificador);

            if (usuario == null)
            {
                usuario = new Usuarios
                {
                    google_identificador = google_identificador,
                    nombre = nombre,
                    email = email,
                    admin_rol = false
                };
                _usuariosManager.CrearUsuario(usuario);
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, google_identificador),
        new Claim(ClaimTypes.Name, nombre),
        new Claim(ClaimTypes.Email, email)
    };

            if (usuario.admin_rol)
            {
                claims.Add(new Claim("admin", "Administradora"));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("LoginIndex");
        }

        // LOGIN ADMIN
        public async Task<IActionResult> LoginAdmin()
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, "Administradora"),
        new Claim("admin", "Administradora")
    };
            var claimsIdentity = new ClaimsIdentity(claims, "LoginAdmin");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
            return RedirectToAction("Index", "Login");
        }
    }
}
