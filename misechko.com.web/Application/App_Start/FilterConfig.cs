using System.Web;
using System.Web.Mvc;
using misechko.com.Application.Filters;

namespace misechko.com
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new GlobalSecurityActionFilter());
        }
    }
}