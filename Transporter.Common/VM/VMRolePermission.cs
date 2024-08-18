using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transporter.Common.VM
{
    [NotMapped]
    public class VMRolePermission
    {
        public int PageID { get; set; }
        public string PageName { get; set; }
        public string DisplayName { get; set; }
        public int Sequence { get; set; }
        public int ActionID { get; set; }
        public string ActionName { get; set; }
        public int Status { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
    }
}
