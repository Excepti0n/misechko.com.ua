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
        public DateTime PublishDate { get; set; }
        public string Culture { get; set; }
        public int ListWeight { get; set; }
        public bool Hidden { get; set; }
    }

    public class AboutMenuItem: SinglePageInformationEntity {}
    public class TeamMember: SinglePageInformationEntity {}

    public class Project: SinglePageInformationEntity {}
    public class Award: SinglePageInformationEntity {}
    public class Publication: SinglePageInformationEntity {}
    public class Brochure: SinglePageInformationEntity {}
    public class New : SinglePageInformationEntity { }
    public class LawNew : SinglePageInformationEntity { }
    
    public class Practice: SinglePageInformationEntity
    {
        public virtual List<Publication> Publications { get; set; }
        public virtual List<Project> Projects { get; set; } 
    }
    public class Industry: SinglePageInformationEntity
    {
        public virtual List<Publication> Publications { get; set; }
        public virtual List<Project> Projects { get; set; } 
    }

}
