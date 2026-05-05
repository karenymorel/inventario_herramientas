using inventario_herramientas.Managers.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace inventario_herramientas.Managers.Repos
{
    public interface IHerramientasRepositorio
    {
        Herramientas GetHerramienta(int idHerramienta);
        IEnumerable<Herramientas> GetHerramientas(bool? soloActivas = true);
        int CrearHerramienta(Herramientas herramienta);
        bool ModificarHerramienta(int idHerramienta, Herramientas herramienta);
        bool EliminarHerramienta(int idHerramienta, int idUsuarioBaja);
    }

    public class HerramientasRepositorio : IHerramientasRepositorio
    {
        private readonly string _connectionString;

        public HerramientasRepositorio(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Herramientas GetHerramienta(int idHerramienta)
        {
            using (IDbConnection conn = new NpgsqlConnection(_connectionString))
            {
                //querysingleordefault te permite devolver null
                return conn.QuerySingleOrDefault<Herramientas>("SELECT * FROM \"Herramientas\" WHERE \"id_herramienta\" = @Id AND \"eliminada\" = FALSE", new { Id = idHerramienta });
            }
        }

        public IEnumerable<Herramientas> GetHerramientas(bool? soloActivas = true)
        {
            using (IDbConnection conn = new NpgsqlConnection(_connectionString))
            {
                string query = "SELECT * FROM \"Herramientas\" WHERE \"eliminada\" = FALSE";
                return conn.Query<Herramientas>(query);
            }
        }

        public int CrearHerramienta(Herramientas herramienta)
        {
            using (IDbConnection db = new NpgsqlConnection(_connectionString))
            {
                string query = @"INSERT INTO ""Herramientas"" (""nombre"", ""descripcion"", ""cantidad"", ""categoria_id"", ""ubicacion_id"", ""fecha_modificacion"", ""estado_id"", ""eliminada"") 
                         VALUES (@nombre, @descripcion, @cantidad, @categoria_id, @ubicacion_id, @fecha_modificacion, @estado_id, false)
                         RETURNING ""id_herramienta"";";

                int nuevoId = db.QuerySingle<int>(query, herramienta);
                herramienta.id_herramienta = nuevoId;
                return herramienta.id_herramienta;
            }
        }

        public bool ModificarHerramienta(int idHerramienta, Herramientas herramienta)
        {
            using (IDbConnection db = new NpgsqlConnection(_connectionString))
            {
                string query = @"UPDATE ""Herramientas"" 
                         SET ""nombre"" = @nombre, ""descripcion"" = @descripcion, ""cantidad"" = @cantidad, 
                             ""categoria_id"" = @categoria_id, ""ubicacion_id"" = @ubicacion_id, 
                             ""fecha_modificacion"" = @fecha_modificacion, ""estado_id"" = @estado_id 
                         WHERE ""id_herramienta"" = @id_herramienta";

                herramienta.id_herramienta = idHerramienta;
                return db.Execute(query, herramienta) == 1;
            }
        }

        public bool EliminarHerramienta(int idHerramienta, int idUsuarioBaja)
        {
            using (IDbConnection db = new NpgsqlConnection(_connectionString))
            {
                string query = @"UPDATE ""Herramientas"" 
                         SET ""eliminada"" = true 
                         WHERE ""id_herramienta"" = @Id";
                return db.Execute(query, new { Id = idHerramienta }) == 1;
            }
        }
    }
}

