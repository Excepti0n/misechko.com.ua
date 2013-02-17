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
            PracticeViewModel model;

            var key = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName + "/Practicies";

            if (!string.IsNullOrEmpty(practice))
            {
                key += "/" + practice;
            }

            key += "#practice-main";

            model = new PracticeViewModel
            {
                CurrentPracticeName = practice
            };

            var firstOrDefault = _context.ContentElements.FirstOrDefault(c => c.ContentKey == key);
            if (firstOrDefault != null)
            {
                model.PracticeMarkup = firstOrDefault.ContentMarkup;
            }

            return View(model);
        }

    }
}
