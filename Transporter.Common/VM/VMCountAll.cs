using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transporter.Common.VM
{
    public class VMCountAll
    {
        public int TripNew { get; set; }
        public int TripBids { get; set; }
        public int TripWin { get; set; }
        public int TripRejected { get; set; }
    }
}
