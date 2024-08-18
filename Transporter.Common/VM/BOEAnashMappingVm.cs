using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transporter.Common.VM
{
    public class BOEAnashMappingVm
    {
        public int UserId { get; set; }
        public long BOEAnashMappingID { get; set; }
        public long BOEID { get; set; }
        public long AnashID { get; set; }


        public BOEAnashMappingVm(int userId, long boeAnashMappingID, long boeID, long anashID)
        {
            UserId = userId;
            BOEAnashMappingID = boeAnashMappingID;
            BOEID = boeID;
            AnashID = anashID;
        }
    }
}
