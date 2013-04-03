using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace misechko.com.core
{
    public interface IMPSettings
    {
        List<string> ImplementedCultures { get; }
        bool CreateContentOnAllLanguages { get; }
        bool ShouldGoToFirstMenuItem { get; }
        bool AllProtected { get; }
    }

    public class ProductionMPSettings : IMPSettings
    {
        private bool _allProtected;

        public ProductionMPSettings()
        {
            var rootWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~/");

            var allProtected = rootWebConfig.AppSettings.Settings["Locked"];
            _allProtected = allProtected != null && bool.Parse(allProtected.Value);
        }

        public List<string> ImplementedCultures
        {
            get
            {
                return new List<string> { "ru", "uk", "en" };
            }
        }

        public bool CreateContentOnAllLanguages { get { return true; } }
        public bool ShouldGoToFirstMenuItem { get { return true; } }
        public bool AllProtected { get; private set; }
    }
}
