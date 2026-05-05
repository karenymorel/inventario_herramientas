using inventario_herramientas.Managers.Entidades;
using inventario_herramientas.Managers.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace inventario_herramientas.Managers.Managers
{
    public interface IHerramientasManager
    {
        IEnumerable<Herramientas> GetHerramientas(bool? soloActivas = true);
        Herramientas GetHerramienta(int idHerramienta);
        int CrearHerramienta(Herramientas herramienta, int idUsuarioAlta);
        bool ModificarHerramienta(int idHerramienta, Herramientas herramienta, int idUsuarioModificacion);
        bool EliminarHerramienta(int idHerramienta, int idUsuarioBaja);
    }

    public class HerramientasManager : IHerramientasManager
    {
        private IHerramientasRepositorio _repo;

        public HerramientasManager(IHerramientasRepositorio repo)
        {
            _repo = repo;
        }

        public Herramientas GetHerramienta(int idHerramienta)
        {
            return _repo.GetHerramienta(idHerramienta);
        }

        public IEnumerable<Herramientas> GetHerramientas(bool? soloActivas = true)
        {
            return _repo.GetHerramientas(soloActivas);
        }

        public int CrearHerramienta(Herramientas herramienta, int idUsuarioAlta)
        {
            herramienta.fecha_modificacion = DateTime.Now;
            return _repo.CrearHerramienta(herramienta);
        }

        public bool ModificarHerramienta(int idHerramienta, Herramientas herramienta, int idUsuarioModificacion)
        {
            herramienta.fecha_modificacion = DateTime.Now;
            return _repo.ModificarHerramienta(idHerramienta, herramienta);
        }

        public bool EliminarHerramienta(int idHerramienta, int idUsuarioBaja)
        {
            return _repo.EliminarHerramienta(idHerramienta, idUsuarioBaja); // lo cambie por borrado lógico
        }
    }
}
