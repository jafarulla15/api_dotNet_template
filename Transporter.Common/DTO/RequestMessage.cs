namespace Transporter.Common.DTO
{
    public class RequestMessage
    {
        public object? RequestObj { get; set; }
        public int PageRecordSize { get; set; } = 0;
        public int PageNumber { get; set; } = 0;
        public int UserID { get; set; }
        public int VendorOrEmployeeId { get; set; }
        public int VendorId { get; set; }
        public int EmployeeId { get; set; }
    }
}
