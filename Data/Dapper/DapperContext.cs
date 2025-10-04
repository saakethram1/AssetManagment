using Microsoft.Data.SqlClient;
using System.Data;

namespace AssetManagment.Data.Dapper
{
    public class DapperContext
    {
        private readonly string _connectionString;
        public DapperContext(string cs) => _connectionString = cs;
        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
