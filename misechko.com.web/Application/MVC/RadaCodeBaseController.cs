using System.Globalization;
using System.IO;
using System.Threading;
using System.Web.Mvc;

namespace RadaCode.Web.Application.MVC
{
    public class RadaCodeBaseController : Controller
    {
        protected string ActionerIP
        {
            get
            {
                var ip = HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ip)) ip = HttpContext.Request.UserHostAddress;

                return ip;
            }
        } 

        protected string ActionerID
        {
            get
            {
                if (!User.Identity.IsAuthenticated) return string.Empty;
                return User.Identity.Name;
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext ctx)
        {
            base.OnActionExecuting(ctx);
            var request = ctx.HttpContext.Request;

            if (request.Cookies["language"] != null)
            {
                Thread.CurrentThread.CurrentCulture =
                    Thread.CurrentThread.CurrentUICulture =
                    new CultureInfo(request.Cookies["language"].Value);
            }

            if (request.QueryString["lang"] != null)
            {
                Thread.CurrentThread.CurrentCulture =
                    Thread.CurrentThread.CurrentUICulture =
                    new CultureInfo(request.QueryString["lang"]);
            }
        }

        protected string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}