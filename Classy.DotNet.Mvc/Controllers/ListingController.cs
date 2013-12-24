using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using System.Web.Mvc;
using Classy.DotNet.Mvc.ViewModels.Listing;
using Classy.DotNet.Services;
using Classy.Models.Response;
using ServiceStack.Text;
using Classy.DotNet.Mvc.ActionFilters;


namespace Classy.DotNet.Mvc.Controllers
{
    public class ListingController<TListingMetadata> : BaseController
        where TListingMetadata : new()
    {
        private readonly string LISTING_METADATA_KEY = "ListingMetadata";

        public virtual string ListingTypeName { get { return "Listing"; } }

        public ListingController() : base() { }
        public ListingController(string ns) : base(ns) { }

        /// <summary>
        /// register routes within host app's route collection
        /// </summary>
        public override void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                name: string.Concat("Create", ListingTypeName),
                url: string.Concat(ListingTypeName.ToLower(), "/new"),
                defaults: new { controller = ListingTypeName, action = "CreateListing" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRoute(
                name: string.Concat("PostCommentFor" ,ListingTypeName),
                url: string.Concat(ListingTypeName.ToLower(), "/{listingId}/comments/new"),
                defaults: new { controller = ListingTypeName, action = "PostComment" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRoute(
                name: string.Concat("Favorite", ListingTypeName),
                url: string.Concat(ListingTypeName.ToLower(), "/{listingId}/favorite"),
                defaults: new { controller = ListingTypeName, action = "FavoriteListing" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRoute(
                name: string.Concat(ListingTypeName, "Details"),
                url: string.Concat(ListingTypeName.ToLower(), "/{listingId}/{slug}"),
                defaults: new { controller = ListingTypeName, action = "GetListingById", slug = "show" },
                namespaces: new string[] { Namespace }
            );
        }

        //
        // GET: /{ListingTypeName}/new
        // 
        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult CreateListing()
        {
            return View(string.Concat("Create", ListingTypeName));
        }

        // POST: /{ListingTypeName}/new
        // 
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateListing(CreateListingViewModel<TListingMetadata> model)
        {
            if (!ModelState.IsValid) return View(string.Concat("Create", ListingTypeName), model);

            try
            {
                var service = new ListingService();
                var listing = service.CreateListing(
                    model.Title,
                    model.Content,
                    ListingTypeName,
                    new Dictionary<string, string> {
                         { LISTING_METADATA_KEY, model.Metadata.ToJson() }
                    },
                    Request.Files);

                TempData["CreateListingSuccess"] = listing;

                return View(string.Concat("Create", ListingTypeName));
            }
            catch(ClassyException cvx)
            {
                AddModelErrors(cvx);
                return View(string.Concat("Create", ListingTypeName));
            }
        }

        //
        // GET: /{ListingTypeName}/{listingId}
        //
        [AcceptVerbs(HttpVerbs.Get)]
        [ImportModelStateFromTempData]
        public ActionResult GetListingById(string listingId)
        {
            var service = new ListingService();
            var listing = service.GetListingById(
                listingId,
                true,
                true,
                true,
                true,
                true);
            var listingMetadata = listing.Metadata[LISTING_METADATA_KEY];
            var model = new ListingDetailsViewModel<TListingMetadata>
            {
                Listing = listing,
                Metadata = listingMetadata != null ? listingMetadata.FromJson<TListingMetadata>() : new TListingMetadata()
            };
            return View(string.Concat(ListingTypeName,"Details"), model);
        }

        //
        // POST: /{ListingTypeName}/{listingId}/comments/new
        //
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        [ExportModelStateToTempData]
        public ActionResult PostComment(string listingId, string content)
        {
            try
            {
                var service = new ListingService();
                service.PostComment(listingId, content);
            }
            catch(ClassyException cvx)
            {
                AddModelErrors(cvx);
            }

            return RedirectToAction("GetListingById", new { listingId = listingId });        
        }

        //
        // POST: /{ListingTypeName}/{listingId}/favorite
        //
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FavoriteListing(string listingId)
        {
            try
            {
                var service = new ListingService();
                service.FavoriteListing(listingId);
            }
            catch (ClassyException cvx)
            {
                AddModelErrors(cvx);
            }

            return Json(new { IsValid = true });
        }
    }
}
