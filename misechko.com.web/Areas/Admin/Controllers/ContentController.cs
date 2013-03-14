using System;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using MP.web.Application.Membership;
using RadaCode.Web.Application.MVC;
using misechko.com.core;
using misechko.com.data.EF;

namespace misechko.com.Areas.Admin.Controllers
{
   public class ContentController : RadaCodeBaseController
    {
        private readonly MPDataContext _context;
       private readonly IMPSettings _settings;

       public ContentController(MPDataContext context, IMPSettings settings)
        {
            _context = context;
            _settings = settings;
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
                key = key.ToLower();

                try
                {
                    return _context.ContentElements.Any(el => el.ContentKey == key)
                            ? _context.ContentElements.First(el => el.ContentKey == key).ContentMarkup
                            : string.Empty;
                }
                catch (Exception)
                {
                    try
                    {
                        return _context.ContentElements.Any(el => el.ContentKey == key)
                            ? _context.ContentElements.First(el => el.ContentKey == key).ContentMarkup
                            : string.Empty;
                    }
                    catch (Exception)
                    {
                        return string.Empty;
                    }
                }

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
                var curCult = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

                key = key.Insert(0, curCult);
                key = key.ToLower();

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
