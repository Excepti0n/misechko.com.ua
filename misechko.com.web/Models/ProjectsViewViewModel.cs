using System.Collections.Generic;
using misechko.com.Areas.Admin.Models;

namespace misechko.com.Models
{
    public class ProjectsViewViewModel
    {
        public List<PracticeModel> Practicies { get; set; }
        public List<IndustryModel> Industries { get; set; }
        public ProjectModel SelectedProject { get; set; }
        public PracticeModel SelectedPractice { get; set; }

        public List<ProjectModel> DisplayedProjects { get; set; }
        public string SearchedKeyword { get; set; }
    }
}