using Microsoft.AspNetCore.Mvc;
using inventario_herramientas.Managers;
using inventario_herramientas.Managers.Entidades;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using inventario_herramientas.Managers.Managers;
using inventario_herramientas.Managers.Repos;
using Dapper;
using inventario_herramientas.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace inventario_herramientas.Web.Controllers
{
    public class HerramientasController : Controller
    {
        private readonly IHerramientasManager _herramientasManager;
        private readonly GeminiService _geminiService;

        // recordar: constructor inyecta manager
        public HerramientasController(IHerramientasManager
herramientasManager, GeminiService geminiService)
        {
            _herramientasManager = herramientasManager;
            _geminiService = geminiService;
        }

        [Authorize]
        public IActionResult Index()
        {
            CargarDatosDropdown();
            var herramientas = _herramientasManager.GetHerramientas();
            return View(herramientas);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            CargarDatosDropdown();
            return View("/Views/Herramientas/Create.cshtml");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Herramientas herramienta)
        {
            if (ModelState.IsValid)
            {
                var idUsuarioActual = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                _herramientasManager.CrearHerramienta(herramienta, idUsuarioActual);
                return RedirectToAction("Index", "Home");
            }
            CargarDatosDropdown(); 
            return View(herramienta);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var herramienta = _herramientasManager.GetHerramienta(id);
            if (herramienta == null)
            {
                return NotFound();
            }
            CargarDatosDropdown();
            return View(herramienta);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Herramientas herramienta)
        {
            if (ModelState.IsValid)
            {
                var idUsuarioActual = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                if (_herramientasManager.ModificarHerramienta(id, herramienta, idUsuarioActual))
                {
                    return RedirectToAction("Index", "Home");
                }
                return NotFound();
            }
            CargarDatosDropdown(); 
            return View(herramienta);
        }

        public IActionResult DeleteConfirmed(int id)
        {
            var idUsuarioActual = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            _herramientasManager.EliminarHerramienta(id, idUsuarioActual);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> SugerirDescripcion(string
        nombre)
        {
            var descripcionSugerida = await _geminiService.GenerarDescripcion(nombre);
            return Json(new { descripcion = descripcionSugerida });
        }

        private void CargarDatosDropdown()
        {
            // viewbag es mostrador de vista dinamico!
            ViewBag.Estados = new List<SelectListItem>
            {
                // esto es lo que esta en la bbdd Estados
                new SelectListItem { Value = "1", Text = "Disponible" },
                new SelectListItem { Value = "2", Text = "En Uso" },
                new SelectListItem { Value = "3", Text = "Dañada" },
                new SelectListItem { Value = "4", Text = "En Reparación" }
            };

            ViewBag.Ubicaciones = new List<SelectListItem>
            {
                // idem para tabla Ubicaciones
                new SelectListItem { Value = "1", Text = "Almacén Central" },
                new SelectListItem { Value = "2", Text = "Taller" },
                new SelectListItem { Value = "3", Text = "Obra 1" },
                new SelectListItem { Value = "4", Text = "Obra 2" }
            };

            ViewBag.Categorias = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Eléctricas - Herramientas eléctricas y equipos motorizados" },
                new SelectListItem { Value = "2", Text = "Manuales - Herramientas manuales, sin motor" },
                new SelectListItem { Value = "3", Text = "Protección Personal - Equipos de protección y seguridad personal" }
            };
        }
    }
}
