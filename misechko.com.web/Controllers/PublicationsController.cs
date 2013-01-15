using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using misechko.com.Content;
using misechko.com.Models;

namespace misechko.com.Controllers
{
    public class PublicationsController : Controller
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
