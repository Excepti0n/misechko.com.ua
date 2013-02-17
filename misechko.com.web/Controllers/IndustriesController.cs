using System.Linq;
using System.Threading;
using System.Web.Mvc;
using misechko.com.Content;
using misechko.com.Models;
using misechko.com.data.EF;

namespace misechko.com.Controllers
{
    public class IndustriesController : Controller
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
            IndustryViewModel model;

            var key = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName + "/Industries";

            if (!string.IsNullOrEmpty(industry))
            {
                key += "/" + industry;
            }

            key += "#industry-main";

            model = new IndustryViewModel
            {
                CurrentIndustryName = industry
            };

            var firstOrDefault = _context.ContentElements.FirstOrDefault(c => c.ContentKey == key);
            if (firstOrDefault != null)
            {
                model.IndustryMarkup = firstOrDefault.ContentMarkup;
            }

            return View(model);
        }

    }
}
