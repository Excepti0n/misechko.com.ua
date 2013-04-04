using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using misechko.com.core;

namespace misechko.com.Application.Filters
{
    public class GlobalSecurityActionFilter : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.RequestContext.HttpContext.User.Identity.IsAuthenticated)
            {
                base.OnResultExecuting(context);
                return;
            }

            var settings = DependencyResolver.Current.GetService<IMPSettings>();

            if (!settings.AllProtected) return;

            var authSection =
                (System.Web.Configuration.AuthenticationSection)System.Web.Configuration.WebConfigurationManager.GetSection("system.web/authentication");

            var loginUrl = authSection.Forms.LoginUrl;

            if (loginUrl.StartsWith("~/")) loginUrl = loginUrl.Substring(2);

            var loginPath = GetAppPath(context) + loginUrl;

            if (!context.RequestContext.HttpContext.User.Identity.IsAuthenticated &&
                context.HttpContext.Request.Url.AbsoluteUri != loginPath && !context.HttpContext.Response.IsRequestBeingRedirected)
            {
                var rootWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~/");

                var redirectPathSetting = rootWebConfig.AppSettings.Settings["RedirectOnLock"];

                var redirectPath = string.IsNullOrEmpty(redirectPathSetting.Value)
                                       ? string.Empty
                                       : redirectPathSetting.Value;

                if (!string.IsNullOrEmpty(redirectPath))
                {
                    context.RequestContext.HttpContext.Response.Redirect(redirectPath);
                    base.OnResultExecuting(context);
                    return;
                }

                context.RequestContext.HttpContext.Response.Redirect(authSection.Forms.LoginUrl);

                base.OnResultExecuting(context);
                return;
            }

            base.OnResultExecuting(context);
            return;
        }

        private string GetAppPath(ResultExecutingContext resContext)
        {
            //Return variable declaration
            string appPath = string.Empty;

            //Getting the current context of HTTP request
            var context = resContext.RequestContext.HttpContext;

            //Checking the current context content
            if (context != null)
            {
                //Formatting the fully qualified website url/name
                appPath = string.Format("{0}://{1}{2}{3}",
                                        context.Request.Url.Scheme,
                                        context.Request.Url.Host,
                                        context.Request.Url.Port == 80
                                            ? string.Empty
                                            : ":" + context.Request.Url.Port,
                                        context.Request.ApplicationPath);
            }

            if (!appPath.EndsWith("/"))
                appPath += "/";

            return appPath;
        }
    }
}