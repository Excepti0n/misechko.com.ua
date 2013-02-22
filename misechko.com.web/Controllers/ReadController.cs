using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using RadaCode.Web.Application.MVC;
using misechko.com.Models;
using misechko.com.data.EF;

namespace misechko.com.Controllers
{
    public class ReadController : RadaCodeBaseController
    {
        private readonly MPDataContext _context;

        public ReadController(MPDataContext context)
        {
            _context = context;
        }

        public ActionResult Index(string contentKey)
        {
            var model = new SingleContentViewModel();
            
            if (!string.IsNullOrEmpty(contentKey))
            {
                //var toEncode = contentKey.Substring(contentKey.LastIndexOf("/", System.StringComparison.Ordinal) + 1);
                //var toLeaveAsIs = contentKey.Substring(0, contentKey.LastIndexOf("/", System.StringComparison.Ordinal) + 1);
                var key = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName + "/Read/" + contentKey + "#single-holder";
                var firstOrDefault = _context.ContentElements.FirstOrDefault(ct => ct.ContentKey == key);
                if (firstOrDefault != null)
                    model.MarkUp = firstOrDefault.ContentMarkup;
            }
            return View(model);
        }
    }
}
