using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gp2gp_dash.Models
{
    public class RadioState
    {
        public RadioState()
        {
            LastUpdated = DateTime.Now;
        }

        public DateTime LastUpdated { get; set; }
        public string ID { get; set; }
        public double Frequency { get; set; }
    }
}
