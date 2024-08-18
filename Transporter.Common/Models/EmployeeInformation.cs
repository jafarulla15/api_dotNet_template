using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transporter.Common.Models
{
    public class EmployeeInformation : BaseClass
    {
        public int EmployeeInformationID { get; set; }

        //public int CompanyID { get; set; } // there is a CompanyEmployeeMapping table for one to many
        public string Alias { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string Phone { get; set; }
        public string EmployeeCode { get; set; }
        public string PhoneAlter { get; set; }
        public string EmployeeJobID { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        [NotMapped]
        public string Password { get; set; }
        [NotMapped]
        public string ConfirmPassword { get; set; }
        [NotMapped]
        public List<Company> ListCompany { get; set; } = new List<Company>();
        [NotMapped]
        public int RoleId { get; set; }
        [NotMapped]
        public string  RoleName { get; set; }
        [NotMapped]
        public string  StatusName { get; set; }
    }
}
