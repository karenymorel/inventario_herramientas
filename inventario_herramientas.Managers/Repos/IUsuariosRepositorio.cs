using inventario_herramientas.Managers.Entidades;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using Dapper;

namespace inventario_herramientas.Managers.Repos
{
    public interface IUsuariosRepositorio
    {
        int CrearUsuario(Usuarios usuario);
        Usuarios? GetUsuarioPorGoogleIdentificador(string googleIdentificador);
    }

    public class UsuariosRepositorio : IUsuariosRepositorio
    {
        private string _connectionString;

        public UsuariosRepositorio(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Método para verificar si ya existe un usuario por Google ID
        public Usuarios? GetUsuarioPorGoogleIdentificador(string googleIdentificador)
        {
            using (IDbConnection db = new NpgsqlConnection(_connectionString))
            {
                string query = @"SELECT * FROM ""Usuarios"" WHERE ""google_identificador"" = @GoogleId";
                return db.QuerySingleOrDefault<Usuarios>(query, new { GoogleId = googleIdentificador });
            }
        }

        // CREAR ADMIN: modificar el admin_rol a true y ejecutar
        public int CrearUsuario(Usuarios usuario)
        {
            using (IDbConnection db = new NpgsqlConnection(_connectionString))
            {
                string query = @"INSERT INTO ""Usuarios"" (""google_identificador"", ""nombre"", ""email"", ""admin_rol"") 
                                 VALUES (@google_identificador, @nombre, @email, false)
                                 RETURNING ""id_usuario"";";
                return db.QuerySingle<int>(query, usuario);
            }
        }
    }
}
