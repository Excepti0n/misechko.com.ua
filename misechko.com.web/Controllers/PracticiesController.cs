using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using RadaCode.Web.Application.MVC;
using misechko.com.Areas.Admin.Models;
using misechko.com.Content;
using misechko.com.Models;
using misechko.com.data.EF;

namespace misechko.com.Controllers
{
    public class PracticiesController : RadaCodeBaseController
    {
        private readonly MPDataContext _context;

        public PracticiesController(MPDataContext context)
        {
            _context = context;
        }
        //
        // GET: /Practicies/

        public ActionResult Index(string practice)
        {
            var model = new PracticeViewModel
                            {
                                AllPracticies =
                                    _context.Practicies.Where(
                                        i => i.Culture == _curCult || String.IsNullOrEmpty(i.Culture)).
                                    ToList().Select(pr => new PracticeMenuViewModel()
                                                            {
                                                                DisplayText = pr.Headline,
                                                                Slug = pr.LinkPath,
                                                                Index = pr.ListWeight
                                                            }).ToList()
                            };

            model.AllPracticies.Sort((a, b) => a.Index.CompareTo(b.Index));

            var key = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName + "/Practicies";

            if (!string.IsNullOrEmpty(practice))
            {
                key += "/" + practice;
            }

            key += "#main-content";

            model.CurrentPracticeName = practice;

            var practiceItem = _context.Practicies.SingleOrDefault(pr => pr.LinkPath == "/Practicies/" + practice && pr.Culture == _curCult);
            if(practiceItem != null && (practiceItem.Projects.Count > 0 || practiceItem.Publications.Count > 0))
            {
                model.HasSupportMaterials = true;

                model.Projects = new List<ProjectModel>();
                model.Publications= new List<PublicationModel>();

                model.Projects = practiceItem.Projects.Select(prj => new ProjectModel
                                                                         {
                                                                             Id = prj.Id.ToString(),
                                                                             Index = prj.ListWeight,
                                                                             Headline = prj.Headline,
                                                                             LinkPath = "/Read" + prj.LinkPath,
                                                                             PublishDate =
                                                                                 prj.PublishDate.ToString("yyyy-MM-dd"),
                                                                             RelatesToPaths = prj.RelatesToPaths

                                                                         }).Take(3).ToList();

                model.Publications = practiceItem.Publications.Select(pub => new PublicationModel
                {
                    Id = pub.Id.ToString(),
                    Index = pub.ListWeight,
                    Headline = pub.Headline,
                    LinkPath = "/Read" + pub.LinkPath,
                    PublishDate =
                        pub.PublishDate.ToString("yyyy-MM-dd"),
                    RelatesToPaths = pub.RelatesToPaths

                }).Take(3).ToList();

                model.Projects.Sort((a, b) => b.PublishDate.CompareTo(a.PublishDate));
                model.Publications.Sort((a, b) => b.PublishDate.CompareTo(a.PublishDate));
            }

            var firstOrDefault = _context.ContentElements.FirstOrDefault(c => c.ContentKey == key);
            if (firstOrDefault != null)
            {
                model.PracticeMarkup = firstOrDefault.ContentMarkup;
            }

            return View(model);
        }

    }
}
