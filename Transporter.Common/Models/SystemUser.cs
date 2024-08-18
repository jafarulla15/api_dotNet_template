using System.ComponentModel.DataAnnotations.Schema;

namespace Transporter.Common.Models
{
    public class SystemUser : BaseClass
    {
        public int SystemUserID { get; set; }

        public int ReferenceTypeID { get; set; }  // VendorUser / Employee/Distributor
        public int ReferenceID { get; set; }  // VendorID/EmployeeID

        public string FirstName { get; set; } = "";

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
        [NotMapped]
        public string? ConfirmPassword { get; set; }

        public int Role { get; set; } = 0;

        public bool IsApproved { get; set; }

        public int StatusOfUser { get; set; } = 0;

        [NotMapped]
        public string RoleName { get; set; }

    }

}
