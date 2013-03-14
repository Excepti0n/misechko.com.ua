using System.Collections.Generic;

namespace misechko.com.Models
{
    public class AboutViewModel
    {
        public string MainMarkup { get; set; }
        public string SupportMaterialsMarkup { get; set; }
        public string CurrentMenuItemName { get; set; }
        public List<AboutMenuViewModel> AboutMenus { get; set; }
    }

    public class AboutMenuViewModel
    {
        public string Slug { get; set; }
        public string DisplayText { get; set; }
        public int Index { get; set; }
    }
}