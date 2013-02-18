using System.Collections.Generic;
using misechko.com.data.Entities;

namespace misechko.com.Areas.Admin.Models
{
    public class PublicationsModel
    {
        public List<PublicationModel> Publications { get; set; }
    }

    public class PublicationModel
    {
        public string Id { get; set; }
        public string Headline { get; set; }
        public string LinkPath { get; set; }
        public List<string> RelatesToPaths { get; set; }
        public string PublishDate { get; set; }
    }
}