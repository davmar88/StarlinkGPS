using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMoreClient.DTO
{
    public class TrackRequest
    {
        public string trackSource {get;set;}
        public string trackSourceType { get; set; }
        public string trackNo { get; set; }
        public int trackType { get; set; }
        public string affiliation { get; set; }
        public string[] lla { get; set; }
        public double speed { get; set; }
        public Classification classification { get; set; }
        public int accuracy { get; set; }
        public string timestamp { get; set; }

    }
}
