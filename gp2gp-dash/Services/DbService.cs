using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
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
        readonly string databasePath;

        public DbService(IConfiguration config)
        {
            databasePath = config.GetValue<string>("SqliteDb");
        }

        private static IDbConnection conn;

        public IDbConnection GetOpenConnection()
        {
            if (conn == null)
            {
                string db;
                if (Path.IsPathRooted(databasePath))
                {
                    db = databasePath;
                }
                else
                {
                    db = Path.Combine(Directory.GetCurrentDirectory(), databasePath);
                }

                Console.WriteLine($"Database location: {db}");

                conn = new SQLiteConnection($"Data Source={db};");
                bool exists = File.Exists(db);
                conn.Open();

                if (!exists)
                {
                    Console.WriteLine("Creating a new database");
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "CREATE TABLE \"contacts\" ( \"id\" TEXT NOT NULL, \"pinLat\" NUMERIC, \"pinLon\" NUMERIC, \"utcTime\" TEXT, \"theirCall\" TEXT, \"ourStation\" INTEGER, \"sentReport\" TEXT, \"receivedReport\" TEXT, \"theiroperator\" TEXT, \"theirgroup\" TEXT, \"theirlocation\" TEXT, \"freqMhz\" NUMERIC, \"mode\" TEXT, \"country\" TEXT, PRIMARY KEY(\"id\") )";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "CREATE TABLE \"badcall\" ( \"call\" TEXT, PRIMARY KEY(\"call\") )";
                    cmd.ExecuteNonQuery();
                }
            }

            return conn;

            /*var conn = new SqlConnection(cs);
            return conn;*/
        }
    }
}
