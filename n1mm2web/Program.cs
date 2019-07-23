using gp2gp_dash.Models;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace n1mm2web
{
    class Program
    {
        static Thread udpThread = new Thread(new ThreadStart(UdpThread));
        static string url;

        static void Main(string[] args)
        {
            const string error = "Need the URL of a gb2gp-dash instance";

            if (!args.Any())
            {
                Console.WriteLine(error);
                return;
            }

            url = args[0];
            if (!url.StartsWith("http"))
            {
                Console.WriteLine(error);
                return;
            }

            //ResolveLatLon("VO1CAL", out string cn, out double lat, out double lon);
            //Debugger.Break();

            udpThread.Start();
            udpThread.Join();
        }

        static void UdpThread()
        {
            while (true)
            {
                var listener = new UdpClient(new IPEndPoint(IPAddress.Any, 12060));

                while (true)
                {
                    IPEndPoint receivedFrom = new IPEndPoint(IPAddress.Any, 0);
                    byte[] msg = listener.Receive(ref receivedFrom);

                    try
                    {
                        ProcessDatagram(msg);
                    }
                    catch (Exception ex)
                    {
                        Log("Uncaught exception: {0}", ex);
                    }
                }
            }
        }

        static void writedg(byte[] msg)
        {
            try
            {
                string rawFolder = Path.Combine(Environment.CurrentDirectory, "datagrams");
                if (!Directory.Exists(rawFolder))
                {
                    Directory.CreateDirectory(rawFolder);
                }
                string rawFile = Path.Combine(rawFolder, string.Format("{0:yyyyMMdd-HHmmss.fff}.xml", DateTime.Now));
                File.WriteAllBytes(rawFile, msg);
            }
            catch (Exception ex)
            {
                Log("Could not write datagram: {0}", ex);
            }
        }

        static void ProcessDatagram(byte[] msg)
        {
            if (N1mmRadioInfo.TryParse(msg, out N1mmRadioInfo ri))
            {
                ProcessRadioInfo(ri);
                return;
            }

            string dg = Encoding.UTF8.GetString(msg);

            if (N1mmXmlContactInfo.TryParse(msg, out N1mmXmlContactInfo ci))
            {
                ProcessContactAdd(ci);
                writedg(msg);
            }
            else if (N1mmXmlContactReplace.TryParse(msg, out N1mmXmlContactReplace cr))
            {
                ProcessContactReplace(cr);
            }
            else if (ContactDelete.TryParse(msg, out ContactDelete cd))
            {
                ProcessContactDelete(cd);
            }
        }

        private static void ProcessRadioInfo(N1mmRadioInfo ri)
        {
            //Log($"{ri.StationName} {ri.Freq} {ri.TXFreq}");

            RadioState rs = new RadioState
            {
                LastUpdated = DateTime.Now,
                Frequency = Math.Round(ri.TXFreq / 100000.0, 4),
                ID = ri.StationName,
            };

            PostObject(new[] { rs }, "radios");
        }

        static void ProcessContactDelete(ContactDelete cd)
        {
            //throw new NotImplementedException();
        }

        static void PostObject(object cr, string controller)
        {
            var cli = new HttpClient();
            var sw = Stopwatch.StartNew();
            while (sw.Elapsed < TimeSpan.FromSeconds(60))
            {
                HttpResponseMessage response;
                try
                {
                    string u = $"{url}/api/{controller}";
                    Log($"POSTing to {u}:");
                    response = cli.PostAsJsonAsync(u, cr).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        Log(response.StatusCode.ToString());
                        break;
                    }

                    var httpError = response.Content.ReadAsStringAsync().Result;
                    Log(httpError);
                }
                catch (Exception ex)
                {
                    Log(ex.Message);
                }

                Thread.Sleep(1000);
            }
        }

        static void ProcessContactAdd(N1mmXmlContactInfo ci)
        {
            if (Debugger.IsAttached)
            {
                if (ci.Call == "VO1CAL")
                {
                    Debugger.Break();
                }
            }

            ProcessContactAdd1(ci.Rcv, ci.Snt, ci.Call, ci.Timestamp, ci.Mode, ci.Txfreq, ci.Radionr);
        }

        static void ProcessContactAdd1(string rcv, string snt, string call, string dt, string mode, string txf, string rnr)
        {
            var cr = new RestContact
            {
                ID = Guid.NewGuid(),
                ReceivedReport = rcv,
                SentReport = snt,
                TheirCall = call,
                TheirGroup = null,
                TheirLocation = null,
                TheirOperator = null,
                UtcTime = DateTime.Parse(dt, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal),
                Mode = mode
            };

            if (int.TryParse(txf, out int deciHz))
            {
                cr.FreqMhz = deciHz / 100000.0;
            }

            if (int.TryParse(rnr, out int radionr))
            {
                cr.OurStation = radionr;
            }

            if (ResolveLatLon(cr.TheirCall, out string country, out double lat, out double lon))
            {
                cr.Country = country;
                cr.PinLat = lat;
                cr.PinLon = lon;
            }
            else
            {
                Log($"Could not resolve location for contact with {call}");
            }

            if (cr.PinLat == 0)
            {
                return;
            }

            PostObject(cr, "contact");
        }

        static Dictionary<string, List<Match>> dict = JsonConvert.DeserializeObject<Dictionary<string, List<Match>>>(File.ReadAllText("dict.json"));

        static List<Match> GetMatches(string call)
        {
            for (int i = call.Length; i > 0; i--)
            {
                string key = call.Substring(0, i);
                if (dict.TryGetValue(key, out List<Match> values))
                {
                    var matches = values.Where(m => m.Flags != null && m.Flags.TreatAsExact != null && m.Flags.TreatAsExact.Value == false).ToList();

                    return matches;
                }
            }

            return new List<Match>();
        }

        private static bool ResolveLatLon(string call, out string countryName, out double lat, out double lon)
        {
            if (ResolveBigCountry(call, out double lati, out double longi))
            {
                Log($"{call} {lati} {longi}");
                lat = lati;
                lon = longi;
                //countryName = first.CountryName;
                countryName = null;
                return true;
            }

            var matches = GetMatches(call);

            if (!matches.Any())
            {
                lat = lon = 0;
                countryName = null;
                return false;
            }

            var first = matches.First();

            string[] bigCountries = new[] { "United States", "European Russia", "Asiatic Russia", "China", "Australia", "Canada", "Brazil", "India", "Argentina", "United Kingdom", "England", "Wales", "Scotland", "Northern Ireland", "Ireland" };

            /*if (bigCountries.Contains(first.CountryName))
            {
                if (ResolveBigCountry(call, out double lati, out double longi))
                {
                    lat = lati;
                    lon = longi;
                    countryName = first.CountryName;
                    return true;
                }
            }*/

            lat = first.Latitude;
            lon = first.Longitude;
            countryName = first.CountryName;
            return true;
        }

        private static bool ResolveBigCountry(string call, out double lati, out double longi)
        {
            var web = new HtmlWeb();
            HtmlDocument doc;
            try
            {
                doc = web.Load($"https://www.qrzcq.com/call/{call}");
            }
            catch (Exception ex)
            {
                Log(ex.Message);
                lati = longi = 0;
                return false;
            }

            var callDataParaElement = doc.DocumentNode.Descendants("p").SingleOrDefault(n => n.HasClass("calldata"));

            if (callDataParaElement == null)
            {
                Log("calldata p tag not found");
                lati = longi = 0;
                return false;
            }

            var table = callDataParaElement.NextSibling;

            if (table == null)
            {
                lati = longi = 0;
                return false;
            }

            var rows = table.Descendants("tr");

            var latRow = rows.SingleOrDefault(r => r.ChildNodes.First().InnerHtml == "<b>Latitude:</b>");
            var lonRow = rows.SingleOrDefault(r => r.ChildNodes.First().InnerHtml == "<b>Longitude:</b>");

            if (latRow == null || lonRow == null)
            {
                lati = longi = 0;
                return false;
            }

            if (double.TryParse(latRow.ChildNodes[1].InnerText, out lati) && double.TryParse(lonRow.ChildNodes[1].InnerText, out longi))
            {
                return true;
            }

            lati = longi = 0;
            return false;
        }

        static void ProcessContactReplace(N1mmXmlContactReplace cr)
        {
            if (Debugger.IsAttached)
            {
                if (cr.Call == "VO1CAL")
                {
                    Debugger.Break();
                }
            }

            ProcessContactAdd1(cr.Rcv, cr.Snt, cr.Call, cr.Timestamp, cr.Mode, cr.Txfreq, cr.Radionr);
        }

        static object logLockObj = new object();

        internal static void Log(string format, params object[] args)
        {
            lock (logLockObj)
            {
                Console.WriteLine(format, args);
                File.AppendAllText("n1mmlistener.log", String.Concat(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff "), String.Format(format, args), Environment.NewLine));
            }
        }
    }

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
}
