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

    public class LawNewsModel
    {
        public List<LawNewModel> LawNews { get; set; }
    }

    public class LawNewModel
    {
        public string Id { get; set; }
        public string Headline { get; set; }
        public string LinkPath { get; set; }
        public List<string> RelatesToPaths { get; set; }
        public string PublishDate { get; set; }
    }

    public class NewsModel
    {
        public List<NewModel> News { get; set; }
    }

    public class NewModel
    {
        public string Id { get; set; }
        public string Headline { get; set; }
        public string LinkPath { get; set; }
        public List<string> RelatesToPaths { get; set; }
        public string PublishDate { get; set; }
    }

    public class BrochuresModel
    {
        public List<BrochureModel> Brochures { get; set; }
    }

    public class BrochureModel
    {
        public string Id { get; set; }
        public string Headline { get; set; }
        public string LinkPath { get; set; }
        public List<string> RelatesToPaths { get; set; }
        public string PublishDate { get; set; }
    }

    public class ProjectsModel
    {
        public List<ProjectModel> Projects { get; set; }
    }

    public class ProjectModel
    {
        public string Id { get; set; }
        public string Headline { get; set; }
        public string LinkPath { get; set; }
        public List<string> RelatesToPaths { get; set; }
        public string PublishDate { get; set; }
    }

    public class AwardsModel
    {
        public List<AwardModel> Awards { get; set; }
    }

    public class AwardModel
    {
        public string Id { get; set; }
        public string Headline { get; set; }
        public string LinkPath { get; set; }
        public List<string> RelatesToPaths { get; set; }
        public string PublishDate { get; set; }
    }
}