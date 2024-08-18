using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transporter.Common.Models
{
    public class Roles : BaseClass
    {
        public int RoleID { get; set; }

        public string? RoleName { get; set; }

        public string? RoleDetails { get; set; }
    }
}
