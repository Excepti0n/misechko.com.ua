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
    }

    public class ProductionMPSettings : IMPSettings
    {
        public List<string> ImplementedCultures
        {
            get
            {
                return new List<string> { "ru", "uk", "en" };
            }
        }

        public bool CreateContentOnAllLanguages { get { return true; } }
    }
}
