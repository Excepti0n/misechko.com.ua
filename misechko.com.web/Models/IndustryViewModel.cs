using System.Collections.Generic;
using misechko.com.Areas.Admin.Models;

namespace misechko.com.Models
{
    public class IndustryViewModel
    {
        public bool HasSupportMaterials { get; set; }
        public string IndustryMarkup { get; set; }
        public List<ProjectModel> Projects { get; set; }
        public List<PublicationModel> Publications { get; set; }
        public List<IndustryMenuViewModel> AllIndustries { get; set; }
        public string CurrentIndustryName { get; set; }
    }

    public class IndustryMenuViewModel
    {
        public string Slug { get; set; }
        public string DisplayText { get; set; }
        public int Index { get; set; }
    }
}