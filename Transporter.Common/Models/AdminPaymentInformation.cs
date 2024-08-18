using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transporter.Common.Models
{
    public class AdminPaymentInformation : BaseClass
    {
        public long AdminPaymentInformationID { get; set; }
        public long VendorPaymentInformationID { get; set; }  // in description
        public int TransportRequisitionID { get; set; }
        public int VendorID { get; set; }
        public int PaidAmount { get; set; }
        public DateTime PaymentReleaseDate { get; set; }
        public string BankInformation { get; set; }
        public string Comment { get; set; }
        public Int16 PaymentStatus { get; set; }
    }
}
