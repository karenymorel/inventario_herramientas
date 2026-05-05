using inventario_herramientas.Managers.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using Npgsql;

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
                string query = @"
                    SELECT 
                        h.*, 
                        ua.""nombre"" AS ""NombreUsuarioAlta"",
                        um.""nombre"" AS ""NombreUsuarioModificacion"",
                        ub.""nombre"" AS ""NombreUsuarioBaja""
                    FROM ""Herramientas"" h
                    LEFT JOIN ""Usuarios"" ua ON h.""idUsuarioAlta"" = ua.""id_usuario""
                    LEFT JOIN ""Usuarios"" um ON h.""idUsuarioModificacion"" = um.""id_usuario""
                    LEFT JOIN ""Usuarios"" ub ON h.""idUsuarioBaja"" = ub.""id_usuario""
                    WHERE h.""id_herramienta"" = @Id AND h.""eliminada"" = FALSE";

                return conn.QuerySingleOrDefault<Herramientas>(query, new { Id = idHerramienta });
            }
        }

        public IEnumerable<Herramientas> GetHerramientas(bool? soloActivas = true)
        {
            using (IDbConnection conn = new NpgsqlConnection(_connectionString))
            {
                string query = @"
                    SELECT 
                        h.*, 
                        ua.""nombre"" AS ""NombreUsuarioAlta"",
                        um.""nombre"" AS ""NombreUsuarioModificacion"",
                        ub.""nombre"" AS ""NombreUsuarioBaja""
                    FROM ""Herramientas"" h
                    LEFT JOIN ""Usuarios"" ua ON h.""idUsuarioAlta"" = ua.""id_usuario""
                    LEFT JOIN ""Usuarios"" um ON h.""idUsuarioModificacion"" = um.""id_usuario""
                    LEFT JOIN ""Usuarios"" ub ON h.""idUsuarioBaja"" = ub.""id_usuario""
                    WHERE h.""eliminada"" = FALSE";

                return conn.Query<Herramientas>(query);
            }
        }

        public int CrearHerramienta(Herramientas herramienta)
        {
            using (IDbConnection db = new NpgsqlConnection(_connectionString))
            {
                herramienta.fechaAlta = DateTime.Now;

                string query = @"INSERT INTO ""Herramientas"" (
                                    ""nombre"", ""descripcion"", ""cantidad"", ""categoria_id"", ""ubicacion_id"", 
                                    ""fecha_modificacion"", ""estado_id"", ""eliminada"", 
                                    ""idUsuarioAlta"", ""fechaAlta""
                                 ) 
                                 VALUES (
                                    @nombre, @descripcion, @cantidad, @categoria_id, @ubicacion_id, 
                                    @fecha_modificacion, @estado_id, false, 
                                    @idUsuarioAlta, @fechaAlta
                                 )
                                 RETURNING ""id_herramienta"";";

                int nuevoId = db.QuerySingle<int>(query, herramienta);
                return nuevoId;
            }
        }

        public bool ModificarHerramienta(int idHerramienta, Herramientas herramienta)
        {
            using (IDbConnection db = new NpgsqlConnection(_connectionString))
            {
                herramienta.fechaModificacion = DateTime.Now;
                herramienta.id_herramienta = idHerramienta;

                string query = @"UPDATE ""Herramientas"" 
                                 SET ""nombre"" = @nombre, ""descripcion"" = @descripcion, ""cantidad"" = @cantidad, 
                                     ""categoria_id"" = @categoria_id, ""ubicacion_id"" = @ubicacion_id, 
                                     ""fecha_modificacion"" = @fecha_modificacion, ""estado_id"" = @estado_id,
                                     ""idUsuarioModificacion"" = @idUsuarioModificacion,
                                     ""fechaModificacion"" = @fechaModificacion
                                 WHERE ""id_herramienta"" = @id_herramienta";

                return db.Execute(query, herramienta) == 1;
            }
        }

        public bool EliminarHerramienta(int idHerramienta, int idUsuarioBaja)
        {
            using (IDbConnection db = new NpgsqlConnection(_connectionString))
            {
                string query = @"UPDATE ""Herramientas"" 
                                 SET ""eliminada"" = true,
                                     ""idUsuarioBaja"" = @IdBaja,
                                     ""fechaBaja"" = @FechaBaja
                                 WHERE ""id_herramienta"" = @IdHerramienta";

                return db.Execute(query, new
                {
                    IdHerramienta = idHerramienta,
                    IdBaja = idUsuarioBaja,
                    FechaBaja = DateTime.Now
                }) == 1;
            }
        }
    }
}