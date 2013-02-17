using System.Linq;
using System.Threading;
using System.Web.Mvc;
using RadaCode.Web.Application.MVC;
using misechko.com.Models;
using misechko.com.data.EF;

namespace misechko.com.Controllers
{
    public class AboutController : RadaCodeBaseController
    {
        private readonly MPDataContext _context;

        public AboutController(MPDataContext context)
        {
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

            key += "#about-main";
            
            var model = new AboutViewModel
                            {
                                CurrentMenuItemName = submenu
                            };
            
            
            var firstOrDefault = _context.ContentElements.FirstOrDefault(c => c.ContentKey == key);
            if (firstOrDefault != null)
            {
                model.MainMarkup = firstOrDefault.ContentMarkup;
            }
            return View(model);
        }

    }
}
