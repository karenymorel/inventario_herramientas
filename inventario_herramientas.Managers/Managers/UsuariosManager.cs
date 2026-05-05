using inventario_herramientas.Managers.Entidades;
using inventario_herramientas.Managers.Repos;
using System;

namespace inventario_herramientas.Managers.Managers
{
    public interface IUsuariosManager
    {
        int CrearUsuario(Usuarios usuario);
        Usuarios? GetUsuarioPorGoogleIdentificador(string googleIdentificador);
    }

    public class UsuariosManager : IUsuariosManager
    {
        private IUsuariosRepositorio _repo;

        public UsuariosManager(IUsuariosRepositorio repo)
        {
            _repo = repo;
        }

        public int CrearUsuario(Usuarios usuario)
        {
            // Verificar si el usuario ya existe
            var usuarioExistente = _repo.GetUsuarioPorGoogleIdentificador(usuario.google_identificador);
            if (usuarioExistente != null)
            {
                return 0;  // Si el usuario ya existe, no crear uno nuevo
            }

            // Crear el usuario en la base de datos
            return _repo.CrearUsuario(usuario);
        }

        public Usuarios? GetUsuarioPorGoogleIdentificador(string googleIdentificador)
        {
            return _repo.GetUsuarioPorGoogleIdentificador(googleIdentificador);
        }
    }
}
