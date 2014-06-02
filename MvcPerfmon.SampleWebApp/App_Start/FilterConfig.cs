using System.Web;
using System.Web.Mvc;

namespace MvcPerfmon.SampleWebApp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}