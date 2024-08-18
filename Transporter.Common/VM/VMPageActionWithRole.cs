using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transporter.Common.VM
{
    public class VMPageActionWithRole
    {
        public int PageID { get; set; } 
        public string PageName { get; set; }
        public string DisplayName { get; set; }
        public int Sequence { get; set; }
        public int ActionID { get; set; }
        public string ActionName { get; set; }

        public string ActionNameDisplay { get; set; }

        public int SequenceAction { get; set; }
        public int RoleID { get; set; }

    }
}
