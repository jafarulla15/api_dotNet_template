using Transporter.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transporter.Common.VM
{
    public class VMLogin
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public bool SSOLogin { get; set; }
        public string SSOAccessToken { get; set; }
        public string Token { get; set; }
        public int SystemUserID { get; set; } = 0;

        public int VendorOrEmployeeId { get; set; } = 0;
        public int VendorId { get; set; } = 0;
        public int EmployeeId { get; set; } = 0;
        public int RoleID { get; set; } = 0;
        public string RoleName { get; set; }
        public List<Page> lstPermissions { get; set; } = new List<Page>();

    }
}
