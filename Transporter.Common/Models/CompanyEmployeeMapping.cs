using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transporter.Common.Models
{
    public class CompanyEmployeeMapping
    {
        //[Key]
        public long CompanyEmployeeMappingID { get; set; }
        public int CompanyID { get; set; }
        public int EmployeeID { get; set; }
    }
}
