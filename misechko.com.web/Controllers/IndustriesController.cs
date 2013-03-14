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
    public class IndustriesController : RadaCodeBaseController
    {
        private readonly MPDataContext _context;

        public IndustriesController(MPDataContext context)
        {
            _context = context;
        }
        //
        // GET: /Industries/

        public ActionResult Index(string industry)
        {
            var model = new IndustryViewModel
                            {
                                AllIndustries =
                                    _context.Industries.Where(
                                        i => i.Culture == _curCult || String.IsNullOrEmpty(i.Culture)).
                                    ToList().Select(pr => new IndustryMenuViewModel()
                                                              {
                                                                  DisplayText = pr.Headline,
                                                                  Slug = pr.LinkPath,
                                                                  Index = pr.ListWeight
                                                              }).ToList()
                            };

            model.AllIndustries.Sort((a, b) => a.Index.CompareTo(b.Index));

            var key = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName + "/Industries";

            if (!string.IsNullOrEmpty(industry))
            {
                key += "/" + industry;
            }

            key += "#main-content";

            model.CurrentIndustryName = industry;

            var firstOrDefault = _context.ContentElements.FirstOrDefault(c => c.ContentKey == key);
            if (firstOrDefault != null)
            {
                model.IndustryMarkup = firstOrDefault.ContentMarkup;
            }

            return View(model);
        }

    }
}
