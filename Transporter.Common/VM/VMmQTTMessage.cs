using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transporter.Common.VM
{
    public  class VMmQTTMessage
    {
        public string MessageType { get; set; }

        public string Title { get; set; }
        public string Message { get; set; }
    }
}
