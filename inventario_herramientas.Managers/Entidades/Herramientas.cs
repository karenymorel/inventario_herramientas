using inventario_herramientas.Managers.Entidades.Audit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inventario_herramientas.Managers.Entidades
{
    public class Herramientas : Auditoria
    {
        public int id_herramienta { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public int cantidad { get; set; }
        public int categoria_id { get; set; }
        public int ubicacion_id { get; set; }
        public DateTime fecha_modificacion { get; set; }
        public int estado_id { get; set; }
        public bool eliminada { get; set; }
    }
}
