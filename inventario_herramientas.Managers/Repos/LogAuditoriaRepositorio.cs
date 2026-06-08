using Dapper;
using inventario_herramientas.Managers.Entidades.Audit;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inventario_herramientas.Managers.Repos
{
    public class LogAuditoriaRepositorio
    {
        private readonly string _connectionString;
        public LogAuditoriaRepositorio(string connectionString) => _connectionString = connectionString;

        public void GuardarLog(LogAuditoria log)
        {
            using (IDbConnection db = new
NpgsqlConnection(_connectionString))
            {
                string query = @"INSERT INTO
""LogAuditoria"" (""usuario"", ""accion"", ""fecha"",
""detalles"")
                                 VALUES (@usuario,
@accion, @fecha, @detalles)";
                db.Execute(query, log);
            }
        }
    }
}
