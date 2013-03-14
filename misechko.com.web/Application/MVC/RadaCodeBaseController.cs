using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using misechko.com.Application.Filters;
using misechko.com.Controllers;

namespace RadaCode.Web.Application.MVC
{
    public abstract class RadaCodeBaseController : Controller
    {
        protected string _curCult;

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (!filterContext.IsChildAction)
            {
                var response = filterContext.HttpContext.Response;
                if (string.CompareOrdinal(response.ContentType, "text/html") == 0)
                {
                    if (filterContext.HttpContext.User.IsInRole("Administrator"))
                    {
                        try
                        {
                            response.Filter = new ReplaceTagsFilter(response.Filter);
                        }
                        catch (Exception) {}
                    }
                }
            }
            base.OnResultExecuting(filterContext);
        }

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

            _curCult = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
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

        #region Http404 handling

        protected override void HandleUnknownAction(string actionName)
        {
            // If controller is ErrorController dont 'nest' exceptions
            if (this.GetType() != typeof(ErrorController))
                this.InvokeHttp404(HttpContext);
        }

        public ActionResult InvokeHttp404(HttpContextBase httpContext)
        {
            IController errorController = DependencyResolver.Current.GetService<ErrorController>();
            var errorRoute = new RouteData();
            errorRoute.Values.Add("controller", "Error");
            errorRoute.Values.Add("action", "Http404");
            errorRoute.Values.Add("url", httpContext.Request.Url.OriginalString);
            errorController.Execute(new RequestContext(
                 httpContext, errorRoute));

            return new EmptyResult();
        }

        #endregion
    }
}