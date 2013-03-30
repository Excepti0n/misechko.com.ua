using System;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using RadaCode.Web.Application.MVC;
using misechko.com.Models;
using misechko.com.core;
using misechko.com.data.EF;

namespace misechko.com.Controllers
{
    public class AboutController : RadaCodeBaseController
    {
        private readonly MPDataContext _context;
        private readonly IMPSettings _settings;

        public AboutController(MPDataContext context, IMPSettings settings)
        {
            _settings = settings;
            _context = context;
        }

        //
        // GET: /About/

        public ActionResult Index(string submenu)
        {
            var key = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName +  "/About";

            if(!string.IsNullOrEmpty(submenu))
            {
                key += "/" + submenu;
            }

            key += "#main-content";
            
            var model = new AboutViewModel
                            {
                                CurrentMenuItemName = submenu,
                                AboutMenus = _context.AboutMenus.Where(
                                        i => i.Culture == _curCult || String.IsNullOrEmpty(i.Culture)).
                                    ToList().Select(pr => new AboutMenuViewModel()
                                    {
                                        DisplayText = pr.Headline,
                                        Slug = pr.LinkPath,
                                        Index = pr.ListWeight
                                    }).ToList()
                            };

            model.AboutMenus.Sort((a, b) => a.Index.CompareTo(b.Index));

            if (_settings.ShouldGoToFirstMenuItem && model.AboutMenus.Count > 0 && string.IsNullOrEmpty(submenu))
            {
                return Redirect(model.AboutMenus.First().Slug);
            }
            
            
            var firstOrDefault = _context.ContentElements.FirstOrDefault(c => c.ContentKey == key);
            if (firstOrDefault != null)
            {
                model.MainMarkup = firstOrDefault.ContentMarkup;
            }
            return View(model);
        }

    }
}
