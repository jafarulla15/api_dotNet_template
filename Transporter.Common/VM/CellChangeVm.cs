using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transporter.Common.VM
{
    public class CellChangeVm
    {
        public int HistoryId { get; set; }
        public string MismatchCellName { get; set; }
        public int Status { get; set; }
    }
}
