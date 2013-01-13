using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using misechko.com.Content;
using misechko.com.Models;

namespace misechko.com.Controllers
{
    public class PracticiesController : Controller
    {
        //
        // GET: /Practicies/

        public ActionResult Index()
        {
            var model = new PracticeViewModel
                            {
                                HasSupportMaterials = false,
                                PracticeMarkup = Practices.DefaultText
                            };
            return View(model);
        }

    }
}
