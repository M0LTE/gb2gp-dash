using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace gp2gp_dash.Services
{
    public interface IDbService
    {
        IDbConnection GetOpenConnection();
    }

    public class DbService : IDbService
    {
        string cs;

        public DbService(IConfiguration config)
        {
            //cs = config["connectionString"];
            //string conString = ConfigurationExtensions.GetConnectionString(config, "DefaultConnection");
            cs = config.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(cs))
            {
                cs = config["DefaultConnection"];
            }

            if (string.IsNullOrWhiteSpace(cs))
            {
                cs = ConfigurationManager.AppSettings["DefaultConnection"];
            }
        }

        private static IDbConnection conn;

        public IDbConnection GetOpenConnection()
        {
            if (conn == null)
            {
                conn = new SQLiteConnection("Data Source=C:\\Users\\ARSAS\\Desktop\\gb2gp.db;");
                conn.Open();
            }

            return conn;

            /*var conn = new SqlConnection(cs);
            return conn;*/
        }
    }
}
