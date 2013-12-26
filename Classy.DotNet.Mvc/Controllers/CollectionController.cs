using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using System.Web.Mvc;
using Classy.DotNet.Mvc.ViewModels.Collection;

namespace Classy.DotNet.Mvc.Controllers
{
    public class CollectionController  : BaseController
    {
        public CollectionController() : base() { }
        public CollectionController(string ns) : base(ns) { }

        /// <summary>
        /// register routes within host app's route collection
        /// </summary>
        public override void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                name: "AddListingToCollection",
                url: "collection/new",
                defaults: new { controller = "Collection", action = "AddListingToCollection" },
                namespaces: new string[] { Namespace }
            );
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [Authorize]
        public ActionResult AddListingToCollection()
        {
            var model = new AddToCollectionViewModel();
            return PartialView("AddListingToCollectionModal", model);
        }
    }
}
