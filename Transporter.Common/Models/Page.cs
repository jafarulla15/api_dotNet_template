using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transporter.Common.Models
{
    public class Page: BaseClass
    {
        public int PageID { get; set; }
        public string PageName { get; set; }
        public string DisplayName { get; set; }
        public int Sequence { get; set; }

    }
}
