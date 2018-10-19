using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace n1mm2web
{
    class N1mmRadioInfo
    {
        /// <summary>
        /// Computer name
        /// </summary>
        public string StationName { get; set; }

        /// <summary>
        /// the radio number associated with a specific XML packet - in other words, the source of the information in that packet. When in SO2V or SO2R mode, N1MM+ sends two packets every ten seconds - one packet each from RadioNr1 and RadioNr2
        /// </summary>
        public int RadioNr { get; set; }

        /// <summary>
        /// *10 for hz, /10000 for MHz
        /// </summary>
        public int Freq { get; set; }

        /// <summary>
        /// *10 for hz, /10000 for MHz
        /// </summary>
        public int TXFreq { get; set; }

        /// <summary>
        /// could be any one of the following: CW, USB, LSB, RTTY, PSK31, PSK63, PSK125, PSK250, QPSK31, QPSK63, QPSK125, QPSK250, FSK, GMSK, DOMINO, HELL, FMHELL, HELL80, MT63, THOR, THRB, THRBX, OLIVIA, MFSK8, MFSK16
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// is the callsign entered by the operator after OPON (or Ctl-O). Defaults to the station call
        /// </summary>
        public string OpCall { get; set; }

        internal static bool TryParse(byte[] datagram, out N1mmRadioInfo ri)
        {
            string str;
            try
            {
                str = Encoding.UTF8.GetString(datagram);
            }
            catch (Exception ex)
            {
                Program.Log("Exception: {0}", ex);
                ri = null;
                return false;
            }

            try
            {
                var serialiser = new XmlSerializer(typeof(N1mmRadioInfo));
                using (var reader = new StringReader(str))
                {
                    ri = (N1mmRadioInfo)serialiser.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                Program.Log("Exception: {0}", ex);
                ri = null;
                return false;
            }

            return true;
        }
    }
}