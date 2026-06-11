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
using Microsoft.Extensions.Localization; // NUEVO: Para usar traducciones
using System.Linq; // NUEVO: Para procesar los bytes del CSV

namespace inventario_herramientas.Web.Controllers
{
    public class HerramientasController : Controller
    {
        private readonly IHerramientasManager _herramientasManager;
        private readonly GeminiService _geminiService;
        private readonly IStringLocalizer<SharedResource> _localizer; // NUEVO

        // recordatorio: constructor inyecta manager y localizador
        public HerramientasController(
            IHerramientasManager herramientasManager,
            GeminiService geminiService,
            IStringLocalizer<SharedResource> localizer) // NUEVO
        {
            _herramientasManager = herramientasManager;
            _geminiService = geminiService;
            _localizer = localizer; // NUEVO
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
        public async Task<IActionResult> SugerirDescripcion(string nombre)
        {
            var prompt = string.Format(_localizer["Prompt_AI"], nombre);

            var descripcionSugerida = await _geminiService.GenerarDescripcion(prompt);

            return Json(new { descripcion = descripcionSugerida });
        }

        [Authorize]
        [HttpGet]
        public IActionResult ExportarCSV()
        {
            var herramientas = _herramientasManager.GetHerramientas();
            var builder = new System.Text.StringBuilder();

            // 1. Encabezados del CSV Traducidos
            builder.AppendLine($"ID,{_localizer["Header_Name"]},{_localizer["Header_Description"]},{_localizer["Header_Stock"]},Categoria_ID,Estado_ID,{_localizer["Header_Modification"]}");

            foreach (var h in herramientas)
            {
                // 2. Traducir Nombre y Descripción "al vuelo"
                var nombreTraducido = _localizer[h.nombre ?? ""];
                var descripcionTraducida = _localizer[h.descripcion ?? ""];

                // Limpiamos la descripción traducida por si tiene comas o saltos de línea que rompan el CSV
                var descLimpia = string.IsNullOrEmpty(descripcionTraducida)
                    ? ""
                    : descripcionTraducida.Value.Replace("\"", "\"\"").Replace("\n", " ");

                builder.AppendLine($"{h.id_herramienta},\"{nombreTraducido}\",\"{descLimpia}\",{h.cantidad},{h.categoria_id},{h.estado_id},{h.fecha_modificacion:yyyy-MM-dd}");
            }

            // 3. Agregar BOM para que Excel lea bien las tildes y las ñ
            var bom = System.Text.Encoding.UTF8.GetPreamble();
            var fileBytes = System.Text.Encoding.UTF8.GetBytes(builder.ToString());
            var resultBytes = bom.Concat(fileBytes).ToArray();

            // 4. Cambiar el nombre del archivo según el idioma
            var currentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            var fileName = currentCulture == "en-US" ? "Inventory_Data_Report.csv" : "Reporte_Inventario_Data.csv";

            return File(resultBytes, "text/csv", fileName);
        }

        private void CargarDatosDropdown()
        {
            // Reutilizamos las claves que ya existen para los Estados
            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = _localizer["Status_Available"] },
                new SelectListItem { Value = "2", Text = _localizer["Status_InUse"] },
                new SelectListItem { Value = "3", Text = _localizer["Status_Damaged"] },
                new SelectListItem { Value = "4", Text = _localizer["Status_InRepair"] }
            };

            // Envueltos en el localizador para poder traducirlos en el archivo .resx
            ViewBag.Ubicaciones = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = _localizer["Almacén Central"] },
                new SelectListItem { Value = "2", Text = _localizer["Taller"] },
                new SelectListItem { Value = "3", Text = _localizer["Obra 1"] },
                new SelectListItem { Value = "4", Text = _localizer["Obra 2"] }
            };

            // Envueltos en el localizador
            ViewBag.Categorias = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = _localizer["Eléctricas - Herramientas eléctricas y equipos motorizados"] },
                new SelectListItem { Value = "2", Text = _localizer["Manuales - Herramientas manuales, sin motor"] },
                new SelectListItem { Value = "3", Text = _localizer["Protección Personal - Equipos de protección y seguridad personal"] }
            };
        }
    }
}