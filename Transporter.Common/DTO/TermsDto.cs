using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transporter.Common.DTO
{
    public class TermsDto
    {
        public int TermsID { get; set; }
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public int TermsNo { get; set; }
        public string TermsAndCondition { get; set; }
    }
}
