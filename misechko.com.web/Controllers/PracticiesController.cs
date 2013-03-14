using System;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using RadaCode.Web.Application.MVC;
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
                                                                Slug = pr.LinkPath
                                                            }).ToList()
                            };

            var key = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName + "/Practicies";

            if (!string.IsNullOrEmpty(practice))
            {
                key += "/" + practice;
            }

            key += "#main-content";

            model.CurrentPracticeName = practice;

            var firstOrDefault = _context.ContentElements.FirstOrDefault(c => c.ContentKey == key);
            if (firstOrDefault != null)
            {
                model.PracticeMarkup = firstOrDefault.ContentMarkup;
            }

            return View(model);
        }

    }
}
