using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transporter.Common.Models
{
    public class Actions:BaseClass
    {
        public int ActionID { get; set; }

        public int PageID { get; set; } = 0;

        [NotMapped]
        public string PageName { get; set; }

        public string ActionName { get; set; }

        public string ActionNameDisplay { get; set; }

        public int Sequence { get; set; }   

    }
}