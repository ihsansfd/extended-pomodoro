using ExtendedPomodoro.Models.DbConfigs;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Models.DbConnections
{
    public class SqliteDbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;
        public SqliteDbConnectionFactory(DbConfig config)
        {
            _connectionString = config.ConnectionString;
        
        }

        public IDbConnection Connect()
        {
            return new SqliteConnection(_connectionString);
        }
    }
}
