using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace n1mm2web
{
    class Match
    {
        public override string ToString()
        {
            return $"{CountryName} ({PrimaryPrefix})";
        }

        [JsonProperty("ctry")]
        public string CountryName { get; set; }

        [JsonProperty("pre")]
        public string PrimaryPrefix { get; set; }

        [JsonProperty("tz")]
        public double UtcOffset { get; internal set; }

        [JsonProperty("lon")]
        public double Longitude { get; internal set; }

        [JsonProperty("lat")]
        public double Latitude { get; internal set; }

        [JsonProperty("cnt")]
        public string Continent { get; internal set; }

        [JsonProperty("itu")]
        public int ItuZone { get; internal set; }

        [JsonProperty("cq")]
        public int CqZone { get; internal set; }

        [JsonProperty("nb")]
        public Flags Flags { get; set; }
    }

    public class Flags
    {
        [JsonProperty("xact")]
        public bool? TreatAsExact { get; internal set; }

        [JsonProperty("ituo")]
        public bool? ItuZoneOverride { get; internal set; }

        [JsonProperty("cqo")]
        public bool? CqZoneOverride { get; internal set; }

        [JsonProperty("poso")]
        public bool? LatlonOverride { get; internal set; }

        [JsonProperty("tzo")]
        public bool? UtcOffsetOverride { get; internal set; }

        [JsonProperty("conto")]
        public bool? ContinentOverride { get; internal set; }
    }
}
