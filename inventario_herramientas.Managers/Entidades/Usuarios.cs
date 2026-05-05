using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inventario_herramientas.Managers.Entidades
{
    public class Usuarios
    {
        public int id_usuario { get; set; }
        public string nombre { get; set; }
        public string email { get; set; }
        public string google_identificador { get; set; }
        public bool admin_rol { get; set; }
    }
}
