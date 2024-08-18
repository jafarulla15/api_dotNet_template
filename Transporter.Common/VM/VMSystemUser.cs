using Transporter.Common.VM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transporter.Common.Models
{
    [NotMapped]
    public class VMSystemUser : BaseClass
    {
        public int SystemUserID { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Role { get; set; } = 0;
        public bool IsApproved { get; set; }
        public int StatusOfUser { get; set; } = 0;
        public string RoleName { get; set; }
        public string StatusName { get; set; }
        public string? ColorCode { get; set; }
    }
}
