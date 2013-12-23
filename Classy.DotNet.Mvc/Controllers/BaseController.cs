using Classy.DotNet.Security;
using Classy.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace Classy.DotNet.Mvc.Controllers
{
    public abstract class BaseController : Controller
    {

        public string Namespace { get; private set; }
        
        public BaseController()
        {
            Namespace = "Classy.DotNet.Mvc.Controllers";
        }

        public BaseController(string ns)
        {
            Namespace = ns;
        }

        public abstract void RegisterRoutes(RouteCollection routes);

        public ProfileView AuthenticatedUserProfile
        {
            get
            {
                if (User.Identity == null || !User.Identity.IsAuthenticated) return null;
                else return (User.Identity as ClassyIdentity).Profile;
            }
        }

        public void AddModelErrors(ClassyValidationException eex)
        {
            foreach (var e in eex.Errors)
            {
                var key = string.Concat(e.FieldName, e.ErrorCode);
                var message = key; //Localize(key);
                ModelState.AddModelError(e.FieldName ?? key, message);
            }
        }
    }
}
