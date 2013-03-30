using System.Collections.Generic;
using misechko.com.Areas.Admin.Models;

namespace misechko.com.Models
{
    public class PracticeViewModel
    {
        public bool HasSupportMaterials { get; set; }
        public string PracticeMarkup { get; set; }
        public List<ProjectModel> Projects { get; set; }
        public List<PublicationModel> Publications { get; set; }
        public List<PracticeMenuViewModel> AllPracticies { get; set; } 
        public string CurrentPracticeName { get; set; }
    }

    public class PracticeMenuViewModel
    {
        public string Slug { get; set; }
        public string DisplayText { get; set; }
        public int Index { get; set; }
    }
}