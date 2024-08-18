using Transporter.Common.DTO;

namespace Transporter.Common.Models
{
    public class AggridRequestModel
    {
        public List<FilterModel> FilterModels { get; set; }
        public List<FilterModel> CustomFilters { get; set; }
        public List<SortModel> ShortModels { get; set; }
    }


    public class FilterModel
    {
        public string Key { get; set; }
        public string Type { get; set; }
        public string filter { get; set; }
    }
}
