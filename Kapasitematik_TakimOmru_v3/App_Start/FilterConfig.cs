using System.Web;
using System.Web.Mvc;

namespace Kapasitematik_TakimOmru_v3
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
