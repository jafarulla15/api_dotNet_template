using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transporter.Common.Models
{
    [NotMapped]
    public class AccessToken
    {
        public int AccessTokenID { get; set; }
        public int SystemUserID { get; set; }
        public int VendorOrEmployeeId { get; set; }
        public int EmployeeId { get; set; }
        public int VendorId { get; set; }
        public int? RoleId { get; set; } = 0;
        public DateTime? IssuedOn { get; set; }
        public DateTime? ExpiredOn { get; set; }
        public string? Token { get; set; }
    }

    public class JwtClaim
    {
        public const string VendorOrEmployeeId = "VendorOrEmployeeId";
        public const string EmployeeId = "EmployeeId";
        public const string VendorId = "VendorId";
        public const string UserId = "SystemUserID";
        public const string UserType = "UserType";
        public const string ExpiresOn = "ExpiresOn";
        public const string IssuedOn = "IssuedOn";
    }

}
