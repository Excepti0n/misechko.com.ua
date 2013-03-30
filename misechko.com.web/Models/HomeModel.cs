using System.Collections.Generic;
using misechko.com.Areas.Admin.Models;

namespace misechko.com.Models
{
    public class HomeModel
    {
        public List<ProjectModel> Projects { get; set; }
        public List<NewModel> News { get; set; } 
    }
}