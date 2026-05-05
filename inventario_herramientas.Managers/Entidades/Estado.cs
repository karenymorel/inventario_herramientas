using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using inventario_herramientas.Managers.Entidades.Audit;

namespace inventario_herramientas.Managers.Entidades
{
    public class Estado : Auditoria
    {
        public int IdEstadoHerramienta { get; set; }
        public string Descripcion { get; set; }
    }
}
