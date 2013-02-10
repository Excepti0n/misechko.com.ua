using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using misechko.com.Models;

namespace misechko.com.Controllers
{
    public class AboutController : Controller
    {
        //
        // GET: /About/

        public ActionResult Index(string submenu)
        {
            var model = new AboutViewModel
                            {
                                CurrentMenuItemName = submenu,

                            };
            return View();
        }

    }
}
