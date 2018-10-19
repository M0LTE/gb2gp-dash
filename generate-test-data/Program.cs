using Dapper;
using gp2gp_dash.Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace generate_test_data
{
    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> PostAsJsonAsync<T>(
            this HttpClient httpClient, string url, T data)
        {
            var dataAsString = JsonConvert.SerializeObject(data);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return httpClient.PostAsync(url, content);
        }

        public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
        {
            var dataAsString = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(dataAsString);
        }
    }

    class Program
    {
        static Random rnd = new Random();

        static void Main(string[] args)
        {
            var cli = new HttpClient();

            

            while (true)
            {
                var rs = new List<RadioState>
                {
                    new RadioState{ ID = "HF1", Frequency = rnd.NextDouble() * 30.0},
                };

                var response = cli.PostAsJsonAsync("http://localhost:55229/api/radios", rs).Result;

                Thread.Sleep(200);
            }
        }

        static void Main1(string[] args)
        {
            var cr = new ContactRow
            {
                ID = Guid.NewGuid(),
                OurStation = 2,
                PinLat = 50.123,
                PinLon = -3.456,
                ReceivedReport = "59",
                SentReport = "57",
                TheirCall = "2E1EPQ",
                TheirGroup = "group",
                TheirLocation = "reading",
                TheirOperator = "Tom",
                UtcTime = new DateTime(2018, 10, 19, 14, 4, 1)
            };

            var cli = new HttpClient();

            try
            {
                var response = cli.PostAsJsonAsync("http://localhost:55229/api/contact", cr).Result;

            }
            catch (Exception ex)
            {
                Debugger.Break();
            }
        }

        static void _Main(string[] args)
        {
            Random rnd = new Random();

            using (var conn = new SqlConnection("server=gp-dash.database.windows.net;database=gp-dash;user id=gp-dash;password=sflgjhdlfkstB2£;"))
            {
                conn.Open();
                var objs = new List<dynamic>();
                for (int i = 0; i < 200; i++)
                {
                    double lat, lon;
                    lat = rnd.NextDouble() * 180.0 - 90;
                    lon = rnd.NextDouble() * 360 - 180;
                    objs.Add(new { pinLat = lat, pinLon = lon });
                }

                conn.Execute("insert into contacts (pinLat, pinLon) values (@pinLat, @pinLon)", objs);
            }
        }
    }
}
