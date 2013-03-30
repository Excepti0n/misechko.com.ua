using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RadaCode.Web.Application.MVC;
using misechko.com.Areas.Admin.Models;
using misechko.com.Models;
using misechko.com.data.EF;

namespace misechko.com.Controllers
{
    public class HomeController : RadaCodeBaseController
    {
        private readonly MPDataContext _context;

        public HomeController(MPDataContext context)
        {
            _context = context;
        }

        public ActionResult Index()
        {
            var projects = _context.Projects.Where(
                                        i => i.Culture == _curCult || String.IsNullOrEmpty(i.Culture)).ToList();

            projects.Sort((a, b) => b.PublishDate.CompareTo(a.PublishDate));

            projects = projects.Take(3).ToList();

            var news = _context.News.Where(
                                        i => i.Culture == _curCult || String.IsNullOrEmpty(i.Culture)).ToList();

            news.Sort((a, b) => b.PublishDate.CompareTo(a.PublishDate));

            news = news.Take(3).ToList();

            var model = new HomeModel
                            {
                                Projects = projects.Select(project => new ProjectModel
                                                                            {
                                                                                Id =
                                                                                    project.Id.ToString(),
                                                                                Headline =
                                                                                    project.Headline,
                                                                                Index =
                                                                                    project.ListWeight,
                                                                                LinkPath = "/Read" +
                                                                                           project.LinkPath,
                                                                                PublishDate =
                                                                                    project.PublishDate.
                                                                                    ToString(
                                                                                        "yyyy-MM-dd"),
                                                                                RelatesToPaths =
                                                                                    project.
                                                                                    RelatesToPaths
                                                                            }).ToList(),

                                News = news.Select(newItem => new NewModel
                                {
                                    Id =
                                        newItem.Id.ToString(),
                                    Headline =
                                        newItem.Headline,
                                    Index =
                                        newItem.ListWeight,
                                    LinkPath = "/Read" +
                                               newItem.LinkPath,
                                    PublishDate =
                                        newItem.PublishDate.
                                        ToString(
                                            "yyyy-MM-dd"),
                                    RelatesToPaths =
                                        newItem.
                                        RelatesToPaths
                                }).ToList()

                            };

            return View(model);
        }

    }
}
