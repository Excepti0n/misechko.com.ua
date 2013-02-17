using System.Linq;
using System.Threading;
using System.Web.Mvc;
using MP.web.Application.Membership;
using RadaCode.Web.Application.MVC;
using misechko.com.data.EF;

namespace misechko.com.Areas.Admin.Controllers
{
   public class ContentController : RadaCodeBaseController
    {
        private readonly MPDataContext _context;

        public ContentController(MPDataContext context)
        {
            _context = context;
        }

        public string Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }
            else
            {
                key = key.Insert(0, Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName + "/");

                return _context.ContentElements.Any(el => el.ContentKey == key)
                           ? _context.ContentElements.First(el => el.ContentKey == key).ContentMarkup
                           : string.Empty;
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        [UrlAuthorize(Roles = "Administrator", AuthUrl = "~/Admin/Authorization/Authenticate")]
        public JsonResult SaveContent(string key, string data)
        {
            if(!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(data))
            {
                //Enriching "key" with localization data

                key = key.Insert(0, Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName);

                if(_context.ContentElements.Any(el => el.ContentKey == key))
                {
                    _context.ContentElements.First(el => el.ContentKey == key).ContentMarkup = data;
                }
                else
                {
                    _context.ContentElements.Add(new data.Entities.Content
                    {
                        ContentKey = key,
                        ContentMarkup = data
                    });
                }
                _context.SaveChanges();
                return Json(new {res = "OK"});
            } 
            
            return Json(new {res = "FAIL", message = "Inconsistent data"});
        }
    }
}
