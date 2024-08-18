using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transporter.Common.Models
{
    public class Company : BaseClass
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }=string.Empty;
        public string Alias { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string ContactNo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsParentCompany { get; set; } = false;
        //public string CompanyCode { get; set; } = CommonHelper.RandomString(6);
        public string CompanyCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Logo { get; set; } = string.Empty;
        public string LogoExtension { get; set; } = string.Empty;
    }
}
