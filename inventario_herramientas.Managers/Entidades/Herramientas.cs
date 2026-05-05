using inventario_herramientas.Managers.Entidades.Audit;
using System;
using System.ComponentModel.DataAnnotations; // IMPORTANTE: Agregamos esto

namespace inventario_herramientas.Managers.Entidades
{
    public class Herramientas : Auditoria
    {
        public int id_herramienta { get; set; }

        [Required(ErrorMessage = "El nombre de la herramienta es obligatorio.")]
        public string nombre { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria para identificar la herramienta.")]
        public string descripcion { get; set; }

        [Required(ErrorMessage = "Debes ingresar una cantidad.")]
        [Range(0, 10000, ErrorMessage = "La cantidad no puede ser un número negativo.")]
        public int cantidad { get; set; }

        [Required(ErrorMessage = "Por favor, seleccione una categoría.")]
        public int categoria_id { get; set; }

        [Required(ErrorMessage = "Por favor, seleccione una ubicación.")]
        public int ubicacion_id { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria.")]
        public DateTime fecha_modificacion { get; set; }

        [Required(ErrorMessage = "El estado actual es obligatorio.")]
        public int estado_id { get; set; }

        public bool eliminada { get; set; }

        public string? NombreUsuarioAlta { get; set; }
        public string? NombreUsuarioModificacion { get; set; }
        public string? NombreUsuarioBaja { get; set; }
    }
}