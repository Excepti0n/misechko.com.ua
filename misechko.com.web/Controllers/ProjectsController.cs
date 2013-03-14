using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RadaCode.Web.Application.MVC;
using misechko.com.Areas.Admin.Models;
using misechko.com.Models;
using misechko.com.data.EF;

namespace misechko.com.Controllers
{
    public class ProjectsController : RadaCodeBaseController
    {
        private readonly MPDataContext _context;

        public ProjectsController(MPDataContext context)
        {
            _context = context;
        }

        public JsonResult SearchProjects(string practiceId, string industryId, string keyword)
        {
            if (string.IsNullOrEmpty(practiceId) && string.IsNullOrEmpty(industryId) && string.IsNullOrEmpty(keyword))
                return Json(new {status = "SPCD: ERROR. NO SEARCH CRITERIA PROVIDED"});

            var res = new List<ProjectModel>();

            var practiceGuid = new Guid();
            bool checkPractice = false;
            var industryGuid = new Guid();
            bool checkIndustry = false;

            bool checkBoth = false;

            if(!string.IsNullOrEmpty(practiceId))
            {
                practiceGuid = Guid.Parse(practiceId);
                checkPractice = true;
            }
            if(!string.IsNullOrEmpty(industryId))
            {
                industryGuid = Guid.Parse(industryId);
                checkIndustry = true;
            }

            if(checkIndustry && checkPractice) checkBoth = true;

            if(checkBoth)
            {
                var projects = from indProject in _context.Industries.First(ind => ind.Id == industryGuid).Projects
                    join pracProject in _context.Practicies.First(pr => pr.Id == practiceGuid).Projects on indProject.Id equals pracProject.Id
                select indProject;

                foreach (var project in projects)
                {
                    if(!string.IsNullOrEmpty(keyword))
                    {
                        if(!project.Headline.Contains(keyword)) continue;
                    }

                    res.Add(new ProjectModel
                                {
                                    Id = project.Id.ToString(),
                                    Headline = project.Headline,
                                    Index = project.ListWeight,
                                    LinkPath = project.LinkPath,
                                    PublishDate = project.PublishDate.ToString("yyyy-MM-dd"),
                                    RelatesToPaths = project.RelatesToPaths
                                });
                }
            } else if (checkPractice)
            {
                var practiceProjects = _context.Practicies.First(pr => pr.Id == practiceGuid).Projects.ToList();

                foreach (var project in practiceProjects)
                {
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        if (!project.Headline.Contains(keyword)) continue;
                    }

                    res.Add(new ProjectModel
                    {
                        Id = project.Id.ToString(),
                        Headline = project.Headline,
                        Index = project.ListWeight,
                        LinkPath = project.LinkPath,
                        PublishDate = project.PublishDate.ToString("yyyy-MM-dd"),
                        RelatesToPaths = project.RelatesToPaths
                    });
                }
            } else if (checkIndustry)
            {
                var industryProjects = _context.Industries.First(ind => ind.Id == industryGuid).Projects.ToList();

                foreach (var project in industryProjects)
                {
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        if (!project.Headline.Contains(keyword)) continue;
                    }

                    res.Add(new ProjectModel
                    {
                        Id = project.Id.ToString(),
                        Headline = project.Headline,
                        Index = project.ListWeight,
                        LinkPath = project.LinkPath,
                        PublishDate = project.PublishDate.ToString("yyyy-MM-dd"),
                        RelatesToPaths = project.RelatesToPaths
                    });
                }
            }
            else
            {
                var keywordProjects = _context.Projects.Where(pr => pr.Headline.Contains(keyword)).ToList();

                foreach (var project in keywordProjects)
                {
                    res.Add(new ProjectModel
                    {
                        Id = project.Id.ToString(),
                        Headline = project.Headline,
                        Index = project.ListWeight,
                        LinkPath = project.LinkPath,
                        PublishDate = project.PublishDate.ToString("yyyy-MM-dd"),
                        RelatesToPaths = project.RelatesToPaths
                    });
                }
            }

            return Json(new {status = "SPCD: OK", projects = res});
        }

        public ActionResult Index()
        {
            var model = new ProjectsViewViewModel
                            {
                                Industries = new List<IndustryModel>(),
                                Practicies = new List<PracticeModel>()
                            };


            foreach (var practice in _context.Practicies.Where(i => i.Culture == _curCult || String.IsNullOrEmpty(i.Culture)).ToList())
            {
                var practiceModel = new PracticeModel
                {
                    Headline = practice.Headline,
                    LinkPath = practice.LinkPath,
                    PublishDate = practice.PublishDate.ToString("yyyy-MM-dd"),
                    Id = practice.Id.ToString(),
                    Index = practice.ListWeight
                };

                var projectsForPractice = new List<string>();
                var publicationsForPractice = new List<string>();

                if (practice.Publications != null)
                    foreach (var publication in practice.Publications.Where(i => i.Culture == _curCult || String.IsNullOrEmpty(i.Culture)).ToList())
                    {
                        publicationsForPractice.Add(publication.Headline);
                    }

                if (practice.Projects != null)
                    foreach (var project in practice.Projects.Where(i => i.Culture == _curCult || String.IsNullOrEmpty(i.Culture)).ToList())
                    {
                        projectsForPractice.Add(project.Headline);
                    }

                practiceModel.Projects = projectsForPractice;
                practiceModel.Publications = publicationsForPractice;

                model.Practicies.Add(practiceModel);
            }

            foreach (var industry in _context.Industries.Where(i => i.Culture == _curCult || String.IsNullOrEmpty(i.Culture)).ToList())
            {
                var industryModel = new IndustryModel
                {
                    Headline = industry.Headline,
                    LinkPath = industry.LinkPath,
                    PublishDate = industry.PublishDate.ToString("yyyy-MM-dd"),
                    Id = industry.Id.ToString(),
                    Index = industry.ListWeight
                };

                var projectsForIndustry = new List<String>();
                var publicationsForIndustry = new List<String>();

                if (industry.Publications != null)
                    foreach (var publication in industry.Publications.ToList())
                    {
                        publicationsForIndustry.Add(publication.Headline);
                    }

                if (industry.Projects != null)
                    foreach (var project in industry.Projects.ToList())
                    {
                        projectsForIndustry.Add(project.Headline);
                    }

                industryModel.Projects = projectsForIndustry;
                industryModel.Publications = publicationsForIndustry;

                model.Industries.Add(industryModel);
            }

            model.SearchedKeyword = string.Empty;
            model.SelectedPractice = null;
            model.SelectedProject = null;
            
            var projectsModels = new List<ProjectModel>();

            foreach (var projectItem in _context.Projects.Where(i => i.Culture == _curCult || String.IsNullOrEmpty(i.Culture)).ToList())
            {
                projectsModels.Add(new ProjectModel
                {
                    Headline = projectItem.Headline,
                    LinkPath = projectItem.LinkPath,
                    PublishDate = projectItem.PublishDate.ToString("yyyy-MM-dd"),
                    Id = projectItem.Id.ToString()
                });
            }

            model.DisplayedProjects = projectsModels;
            
            return View(model);
        }

    }
}
