using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transporter.Common.Models
{
    public class ActionsWithApi : BaseClass
    {
        public int ActionsWithApiID { get; set; }
        public int ActionID { get; set; }
        public string ApiURL { get; set; }
    }
}
