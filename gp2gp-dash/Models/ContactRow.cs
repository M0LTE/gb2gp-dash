using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gp2gp_dash.Models
{
    public class ContactRow
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
    }
}
