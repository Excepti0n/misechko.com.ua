using System;
using System.Threading;
using System.Web.Mvc;
using misechko.com.Models;

namespace misechko.com.Controllers
{
    public class NavigationController : Controller
    {
        public PartialViewResult RenderTopMenu()
        {
            return PartialView("_TopMenu");
        }

        public PartialViewResult RenderLanguages()
        {
            var model = new LanguageBlockModel();

            string langCookie = null;

            try
            {
                langCookie = Request.Cookies["language"].Value;
            }
            catch (Exception)
            { }


            model.CurrentLang = !string.IsNullOrEmpty(langCookie) ? langCookie : Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

            return PartialView("_LangBlock", model);
        }

    }
}
