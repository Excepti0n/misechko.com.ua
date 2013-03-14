using System.Collections.Generic;
using misechko.com.Areas.Admin.Models;

namespace misechko.com.Models
{
    public class ProjectViewModel
    {
        public bool HasSupportMaterials { get; set; }
        public string ProjectMarkup { get; set; }
        public string CurrentProjectName { get; set; }
    }
}