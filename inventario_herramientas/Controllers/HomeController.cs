using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using inventario_herramientas.Models;
using inventario_herramientas.Managers.Managers;
using Microsoft.AspNetCore.Authorization;

namespace inventario_herramientas.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHerramientasManager _herramientasManager;

        public HomeController(ILogger<HomeController> logger, IHerramientasManager herramientasManager)
        {
            _logger = logger;
            _herramientasManager = herramientasManager;
        }

        [Authorize]
        public IActionResult Index()
        {
            var herramientas = _herramientasManager.GetHerramientas(); 
            return View(herramientas);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
