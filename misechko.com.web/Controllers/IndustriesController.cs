using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using misechko.com.Content;
using misechko.com.Models;

namespace misechko.com.Controllers
{
    public class IndustriesController : Controller
    {
        //
        // GET: /Industries/

        public ActionResult Index(string industry)
        {
            IndustryViewModel model;

            if (string.IsNullOrEmpty(industry))
            {
                model = new IndustryViewModel
                {
                    HasSupportMaterials = false,
                    IndustryMarkup = Industries.defaultText
                };
                return View(model);
            }

            model = new IndustryViewModel
            {
                HasSupportMaterials = false,
                IndustryMarkup = Industries.ResourceManager.GetString(industry),
                CurrentIndustryName = industry
            };

            var supportMarkup = Industries.ResourceManager.GetString(industry + "_side");

            if (!string.IsNullOrEmpty(supportMarkup))
            {
                model.HasSupportMaterials = true;
                model.SupportMaterialsMarkup = supportMarkup;
            }

            return View(model);
        }

    }
}
