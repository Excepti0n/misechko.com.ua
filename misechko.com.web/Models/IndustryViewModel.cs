using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace misechko.com.Models
{
    public class IndustryViewModel
    {
        public bool HasSupportMaterials { get; set; }
        public string IndustryMarkup { get; set; }
        public string SupportMaterialsMarkup { get; set; }
        public string CurrentIndustryName { get; set; }
    }
}