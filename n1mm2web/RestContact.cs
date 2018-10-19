using System;
using System.Collections.Generic;
using System.Text;

namespace n1mm2web
{
    public class RestContact
    {
        public Guid ID { get; set; }
        public DateTime UtcTime { get; set; }
        public string TheirCall { get; set; }
        public int OurStation { get; set; }
        public string SentReport { get; set; }
        public string ReceivedReport { get; set; }
        public double PinLat { get; set; }
        public double PinLon { get; set; }
        public string TheirOperator { get; set; }
        public string TheirGroup { get; set; }
        public string TheirLocation { get; set; }
        public double FreqMhz { get; set; }
        public string Mode { get; set; }
        public string Country { get; set; }
    }
}
