using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Classy.DotNet.Mvc.Localization;
using System.Web.Mvc;

namespace Classy.DotNet.Mvc.Controllers
{
    public class DefaultController : BaseController
    {
        public override void RegisterRoutes(System.Web.Routing.RouteCollection routes)
        {
            routes.MapRouteForSupportedLocales(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Default", action = "Index", id = UrlParameter.Optional },
                namespaces: null
            );
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}
