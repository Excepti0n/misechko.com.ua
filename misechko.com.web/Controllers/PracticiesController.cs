using System.Web.Mvc;
using misechko.com.Content;
using misechko.com.Models;

namespace misechko.com.Controllers
{
    public class PracticiesController : Controller
    {
        //
        // GET: /Practicies/

        public ActionResult Index(string practice)
        {
            PracticeViewModel model;

            if(string.IsNullOrEmpty(practice))
            {
                model = new PracticeViewModel
                {
                    HasSupportMaterials = false,
                    PracticeMarkup = Practices.DefaultText
                };
                return View(model);
            }

            model = new PracticeViewModel
            {
                HasSupportMaterials = false,
                PracticeMarkup = Practices.ResourceManager.GetString(practice),
                CurrentPracticeName = practice
            };
            return View(model);
        }

    }
}
