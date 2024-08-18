using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transporter.Common.Models
{
    public class RolePageMapping
    {
        public int RolePageMappingID { get; set; }

        public int RoleID { get; set; } = 0;

        public int PageID { get; set; } = 0;
    }
}
