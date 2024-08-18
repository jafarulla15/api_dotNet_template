using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transporter.Common.DTO
{
    public class TripSearch
    {
        public object? BasicSearch { get; set; }
        public DateTime? FromDatePost { get; set; }
        public DateTime? ToDatePost { get; set; }
        public DateTime? FromDateTrip { get; set; }
        public DateTime? ToDateTrip { get; set; }

        public DateTime? FromDateDeadline { get; set; }
        public DateTime? ToDateDeadline { get; set; }

        public string? SearchText { get; set; }
        public string? SourcePlace { get; set; }
        public string? DestinationPlace { get; set; }
        public List<string>? LstColumnName { get; set; }
        public List<FilterModel>? FilterModel { get; set; }
        public List<SortModel>? SortModel { get; set; }

        public int CompanyID { get; set; }

        public int? BidStatus { get; private set; }
    }
}
