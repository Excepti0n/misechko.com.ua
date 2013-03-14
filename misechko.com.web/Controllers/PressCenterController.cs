using System;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using RadaCode.Web.Application.MVC;
using misechko.com.Models;
using misechko.com.data.EF;

namespace misechko.com.Controllers
{
    public class PressCenterController : RadaCodeBaseController
    {
        private readonly MPDataContext _context;

        public PressCenterController(MPDataContext context)
        {
            _context = context;
        }

        //
        // GET: /PressCenter/

        public ActionResult Index(string submenu)
        {
            var model = new PressCenterViewModel
            {
                CurrentMenuItemName = submenu
            };

            switch (submenu)
            {
                case "publications":
                    model.MainMarkup = RenderRazorViewToString("_PublicationsFull", _context.Publications.Where(i => i.Culture == _curCult || String.IsNullOrEmpty(i.Culture)).ToList());
                    break;
                case "brochures":
                    model.MainMarkup = RenderRazorViewToString("_BrochuresFull", _context.Brochures.Where(i => i.Culture == _curCult || String.IsNullOrEmpty(i.Culture)).ToList());
                    break;
                case "lawnews":
                    model.MainMarkup = RenderRazorViewToString("_LawNewsFull", _context.LawNews.Where(i => i.Culture == _curCult || String.IsNullOrEmpty(i.Culture)).ToList());
                    break;
                default:
                    model.MainMarkup = RenderRazorViewToString("_NewsFull", _context.News.Where(i => i.Culture == _curCult || String.IsNullOrEmpty(i.Culture)).ToList());
                    break;
            }
            
            return View(model);
        }

    }
}
