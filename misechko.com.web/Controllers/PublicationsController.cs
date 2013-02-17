using System.Web.Mvc;
using RadaCode.Web.Application.MVC;
using misechko.com.Content;
using misechko.com.Models;

namespace misechko.com.Controllers
{
    public class PublicationsController : RadaCodeBaseController
    {
        //
        // GET: /Publications/

        public ActionResult Index(string publication)
        {
            var model = new PublicationViewModel(); 

            if(!string.IsNullOrEmpty(publication))
            {
                model.PublicationMarkUp = Publications.ResourceManager.GetString(publication);
            }
            return View(model);
        }

    }
}
