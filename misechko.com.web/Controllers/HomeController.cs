using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RadaCode.Web.Application.MVC;

namespace misechko.com.Controllers
{
    public class HomeController : RadaCodeBaseController
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

    }
}
