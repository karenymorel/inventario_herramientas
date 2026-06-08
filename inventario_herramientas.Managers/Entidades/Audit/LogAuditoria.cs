using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inventario_herramientas.Managers.Entidades.Audit
{
    public class LogAuditoria
    {
        public int id_log { get; set; }
        public string usuario { get; set; }
        public string accion { get; set; }
        public DateTime fecha { get; set; }
        public string? detalles { get; set; }
    }
}
