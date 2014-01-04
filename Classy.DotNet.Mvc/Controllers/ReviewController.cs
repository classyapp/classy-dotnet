using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using System.Web.Mvc;
using Classy.DotNet.Security;
using Classy.DotNet.Mvc.ViewModels.Profiles;
using Classy.DotNet.Services;
using System.Net;
using Classy.DotNet.Mvc.ViewModels.Reviews;
using Classy.Models.Response;
using ServiceStack.Text;
using System.Web;
using Classy.DotNet.Mvc.Localization; 

namespace Classy.DotNet.Mvc.Controllers
{
    public class ReviewController<TProMetadata, TReviewSubCriteria> : BaseController
        where TProMetadata : IMetadata<TProMetadata>, new()
        where TReviewSubCriteria : IReviewSubCriteria<TReviewSubCriteria>, new()
    {
        public ReviewController() : base() { }
        public ReviewController(string ns) : base(ns) { }

        public event EventHandler<ReviewPostedArgs> OnReviewPosted;

        /// <summary>
        /// register routes within host app's route collection
        /// </summary>
        public override void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRouteWithName(
                name: "FindProfileToReview",
                url: "profile/reviews/new",
                defaults: new { controller = "Review", action = "FindProfileToReview" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRoute(
                name: "PostProfileReview",
                url: "profile/{profileId}/reviews/new",
                defaults: new { controller = "Review", action = "PostProfileReview" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRouteWithName(
                name: "ReviewThanks",
                url: "thanks",
                defaults: new { controller = "Review", action = "ReviewThanks" },
                namespaces: new string[] { Namespace }
            );
        }

        #region // actions

        //
        // GET: /profile/reviews/new
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult FindProfileToReview()
        {
            return View();
        }

        //
        // GET: /profile/{profileId}/reviews/new
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult PostProfileReview(string profileId)
        {
            try
            {
                var service = new ProfileService();
                var profile = service.GetProfileById(profileId);
                var model = new ProfileReviewViewModel<TProMetadata, TReviewSubCriteria>
                {
                    IsProxy = profile.IsProxy
                };
                return View(model);
            }
            catch(ClassyException cex)
            {
                return new HttpStatusCodeResult(cex.StatusCode, cex.Message);
            }
        }

        //
        // POST: post a review for an agent
        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize]
        public ActionResult PostProfileReview(ProfileReviewViewModel<TProMetadata, TReviewSubCriteria> model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var service = new ReviewService();
                    var metadata = model.Metadata != null ? model.Metadata.ToDictionary() : null;
                    var response = service.SubmitProfileReview(
                        model.ProfileId,
                        model.SubCriteria.CalculateScore(),
                        model.Comments,
                        model.SubCriteria.ToDictionary(),
                        model.IsProxy ? model.ContactInfo : null,
                        model.IsProxy ? metadata : null);
                    if (OnReviewPosted != null)
                        OnReviewPosted(this, new ReviewPostedArgs
                        {
                            ReviewResponse = response
                        });
                    return RedirectToAction("ReviewThanks");
                }
                catch(ClassyException cvx)
                {
                    if (cvx.IsValidationError())
                    {
                        AddModelErrors(cvx);
                    }
                    else return new HttpStatusCodeResult(cvx.StatusCode, cvx.Message);
                }
            }
            return View(model);
        }

        //
        // GET: thank you page after submitting an agent review
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ReviewThanks()
        {
            return View();
        }

        #endregion
    }
}