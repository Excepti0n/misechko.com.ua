using System.Collections.Generic;
using misechko.com.Areas.Admin.Models;

namespace misechko.com.Models
{
    public class TeamViewModel
    {
        public string MainMarkup { get; set; }
        public List<ProjectModel> Projects { get; set; }
        public List<PublicationModel> Publications { get; set; }
        public List<EmployeeMenuViewModel> AllEmployees { get; set; } 
        public string CurrentEmployeeName { get; set; }
    }

    public class EmployeeMenuViewModel
    {
        public string Slug { get; set; }
        public string DisplayText { get; set; }
        public int Index { get; set; }
    }


}