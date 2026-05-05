using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inventario_herramientas.Managers.Entidades.Audit
{
    // NO ESTA IMPLEMENTADO
    public class Auditoria
    {
        public int idUsuarioAlta { get; set; }
        public DateTime fechaAlta { get; set; }
        public int? idUsuarioModificacion { get; set; }
        public DateTime? fechaModificacion { get; set; }
        public int? idUsuarioBaja { get; set; }
        public DateTime? fechaBaja { get; set; }
    }
}
