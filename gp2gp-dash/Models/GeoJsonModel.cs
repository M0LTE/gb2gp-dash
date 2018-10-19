namespace gp2gp_dash.Models
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class GeoJsonModel
    {
        public static GeoJsonModel FromLatLons(IEnumerable<LatLonPair> lines)
        {
            var model = new GeoJsonModel
            {
                Type = "FeatureCollection",
                Features = new List<Feature>()
            };

            foreach (var item in lines)
            {
                model.Features.Add(new Feature
                {
                    Type = "Feature",
                    Properties = new Properties { Name=item.End.Longitude.ToString() },
                    Geometry = new Geometry
                    {
                        Type = "LineString",
                        Coordinates = new List<List<double>> {
                            new List<double>{ item.Start.Longitude, item.Start.Latitude },
                            new List<double>{ item.End.Longitude, item.End.Latitude },
                        }
                    }
                });
            }

            return model;
        }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("features")]
        public List<Feature> Features { get; set; }
    }

    public partial class Feature
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public Properties Properties { get; set; }

        [JsonProperty("geometry")]
        public Geometry Geometry { get; set; }
    }

    public partial class Geometry
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("coordinates")]
        public List<List<double>> Coordinates { get; set; }
    }

    public partial class Properties
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class LatLonPair
    {
        public LatLonPair(LatLon start, LatLon end)
        {
            this.Start = start;
            this.End = end;
        }

        public LatLon Start { get; set; }
        public LatLon End { get; set; }
    }

    public class LatLon
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lat">Degrees above the equator</param>
        /// <param name="lon">Degrees east of Greenwich</param>
        public LatLon(double lat, double lon)
        {
            this.Latitude = lat;
            this.Longitude = lon;
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
