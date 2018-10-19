using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
        }

        public IDbConnection GetOpenConnection()
        {
            var conn = new SqlConnection(cs);
            return conn;
        }
    }
}
