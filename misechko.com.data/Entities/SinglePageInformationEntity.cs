using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace misechko.com.data.Entities
{
    public abstract class SinglePageInformationEntity: IdableEntity
    {
        public string Headline { get; set; }
        public string LinkPath { get; set; }
        public List<string> RelatesToPaths { get; set; }
    }

    public class Project: SinglePageInformationEntity {}
    public class Award: SinglePageInformationEntity {}
    public class Publication: SinglePageInformationEntity {}
    public class Borchure: SinglePageInformationEntity {}
}
