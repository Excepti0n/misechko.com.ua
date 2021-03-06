﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace misechko.com.Areas.Admin.Models
{
    public class PracticiesModel
    {
        public List<PracticeModel> Practicies { get; set; }
        public List<ProjectModel> AllProjects { get; set; }
        public List<PublicationModel> AllPublications { get; set; } 
    }

    public class PracticeModel
    {
        public string Id { get; set; }
        public string Headline { get; set; }
        public string LinkPath { get; set; }
        public List<String> Projects { get; set; }
        public List<String> Publications { get; set; }
        public string PublishDate { get; set; }
        public int Index { get; set; }
    }

    public class IndustriesModel
    {
        public List<IndustryModel> Industries { get; set; }
        public List<ProjectModel> AllProjects { get; set; }
        public List<PublicationModel> AllPublications { get; set; } 
    }

    public class IndustryModel
    {
        public string Id { get; set; }
        public string Headline { get; set; }
        public string LinkPath { get; set; }
        public List<String> Projects { get; set; }
        public List<String> Publications { get; set; }
        public string PublishDate { get; set; }
        public int Index { get; set; }
    }
}