using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transporter.Common.VM
{
    public class FileFormatInfo
    {
       public  int index { get; set; }
        public string cellHeaderKeyName { get; set; }
        public Type cellContentType { get; set; }
    }
}
