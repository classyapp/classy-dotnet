﻿using System;
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
using Classy.Models.Response;
using ServiceStack.Text;
using Classy.DotNet.Mvc.Attributes;
using Classy.DotNet.Mvc.ActionFilters;

namespace Classy.DotNet.Mvc.Controllers
{

    public class ProfileController<TProMetadata> : BaseController
        where TProMetadata : IMetadata<TProMetadata>, new()
    {
        public ProfileController() : base() { }
        public ProfileController(string ns) : base(ns) { }

        private readonly string PROFESSIONAL_PROFILE_METADATA_KEY = "ProfessionalProfile";

        public EventHandler<ContactProfessionalArgs<TProMetadata>> OnContactProfessional; 

        /// <summary>
        /// register routes within host app's route collection
        /// </summary>
        public override void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                name: "MyProfile",
                url: "profile/me",
                defaults: new { controller = "Profile", action = "MyProfile" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRoute(
                name: "ClaimProxyProfile",
                url: "profile/{profileId}/claim",
                defaults: new { controller = "Profile", action = "ClaimProxyProfile" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRoute(
                name: "SearchProfiles",
                url: "profile/search/{category}",
                defaults: new { controller = "Profile", action = "Search", category = "" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRoute(
                name: "CreateProfessionalProfile",
                url: "profile/me/gopro",
                defaults: new { controller = "Profile", action = "CreateProfessionalProfile" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRoute(
                name: "FollowProfile",
                url: "profile/{username}/follow",
                defaults: new { controller = "Profile", action = "FollowProfile" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRoute(
                name: "ContactProfessional",
                url: "profile/{ProfessionalProfileId}/contact",
                defaults: new { controller = "Profile", action = "ContactProfessional" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRoute(
                name: "PublicProfile",
                url: "profile/{profileId}/{slug}",
                defaults: new { controller = "Profile", action = "PublicProfile", slug = "public" },
                namespaces: new string[] { Namespace }
            );
        }

        #region // actions

        //
        // GET: /profile/me
        // 
        [AuthorizeWithRedirect("Index")]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult MyProfile()
        {
            return View(AuthenticatedUserProfile);
        }

        //
        // GET: /profile/{ProfileId}/{Slug}
        //
        [AcceptVerbs(HttpVerbs.Get)]
        [ImportModelStateFromTempData]
        public ActionResult PublicProfile(string profileId)
        {
            var service = new ProfileService();
            var profile = service.GetProfileById(profileId, true, true, true, true);
            var metadata = new TProMetadata();
            metadata.FromCustomAttributeList(profile.Metadata);
            var model = new PublicProfileViewModel<TProMetadata>
            {
                Profile = profile,
                TypedMetadata = metadata
            };

            return View(profile.IsSeller ? "PublicProfessionalProfile" : "PublicProfile", model);
        }

        //
        // GET: /profile/{ProfileId}/claim
        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ClaimProxyProfile(string profileId)
        {
            if (User.Identity.IsAuthenticated)
            {
                var profile = AuthenticatedUserProfile;
                var metadata = new TProMetadata();
                metadata.FromCustomAttributeList(profile.Metadata);
                var model = new ClaimProfileViewModel<TProMetadata>
                {
                    ProfileId = profileId,
                    SellerInfo = profile.SellerInfo,
                    Metadata = metadata
                };
                return View(model);
            }
            else return View();
        }

        //
        // POST: /profile/{proxyId}/claim
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ClaimProxyProfile(ClaimProfileViewModel<TProMetadata> request)
        {
            try
            {
                var service = new ProfileService();
                var claim = service.ClaimProfileProxy(
                    request.ProfileId, 
                    request.SellerInfo, 
                    request.Metadata.ToCustomAttributeList());
                service.ApproveProxyClaim(claim.Id);
                return RedirectToRoute("MyProfile");
            }
            catch (WebException wex)
            {
                throw wex;
            }
            catch (UnauthorizedAccessException uex)
            {
                throw uex;
            }
        }

        // 
        // GET: /profile/search
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Search(SearchViewModel<TProMetadata> model)
        {
            var service = new ProfileService();
            var metadata = model.Metadata != null ? model.Metadata.ToCustomAttributeList() : null;
            var profiles = service.SearchProfiles(model.Name, model.Category, model.Location, metadata);
            if (Request.AcceptTypes.Contains("application/json"))
            {
                return Json(profiles, JsonRequestBehavior.AllowGet);
            }
            else
            {
                model.Results = profiles;
                return View(model);
            }
        }

        // 
        // POST: /profile/search
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Search(SearchViewModel<TProMetadata> model, object dummyforpost)
        {
            return Search(model);
        }

        // 
        // GET: /profile/me/gopro
        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult CreateProfessionalProfile()
        {
            var service = new ProfileService();
            var profile = service.GetProfileById(AuthenticatedUserProfile.Id);
            var metadata = new TProMetadata();
            metadata.FromCustomAttributeList(profile.Metadata);
            var model = new CreateProfessionalProfileViewModel<TProMetadata>
            {
                SellerInfo = profile.SellerInfo,
                Metadata = metadata
            };

            return View(model);
        }

        // 
        // POST: /profile/me/gopro
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateProfessionalProfile(CreateProfessionalProfileViewModel<TProMetadata> model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                var service = new ProfileService();
                var profile = service.UpdateProfile(
                    AuthenticatedUserProfile.Id, 
                    model.SellerInfo, 
                    model.Metadata.ToCustomAttributeList(), 
                    "CreateProfessionalProfile");
                return RedirectToRoute("MyProfile");
            }
            catch(ClassyValidationException cve)
            {
                AddModelErrors(cve);
                return View(model);
            }
        }

        //
        // POST: /profile/{ProfessionalProfileId}/contact
        [AcceptVerbs((HttpVerbs.Post))]
        [ExportModelStateToTempData]
        public ActionResult ContactProfessional(ContactProfessionalViewModel model)
        {
            var service = new ProfileService();
            var profile = service.GetProfileById(model.ProfessionalProfileId);
            if (profile == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "invalid profile id");

            // when user is not logged-in, ReplyToEmail is required
            if (!Request.IsAuthenticated && string.IsNullOrEmpty(model.ReplyToEmail))
            {
                ModelState.AddModelError("ReplyToEmail", "Please enter reply-to email");
            }
            
            if (ModelState.IsValid)
            {
                var metadata = new TProMetadata();
                metadata.FromCustomAttributeList(profile.Metadata);
                var args = new ContactProfessionalArgs<TProMetadata>
                {
                    ReplyToEmail = (Request.IsAuthenticated) ? (User.Identity as ClassyIdentity).Profile.ContactInfo.Email : model.ReplyToEmail,
                    Subject = model.Subject,
                    Content = model.Content,
                    ProfessionalProfile = profile,
                    TypedMetadata = metadata
                };

                if (OnContactProfessional != null)
                    OnContactProfessional(this, args);

                var analytics = new AnalyticsService();
                analytics.LogActivity(Request.IsAuthenticated ? (User.Identity as ClassyIdentity).Profile.Id : "guest", ActivityPredicate.ProContact, model.ProfessionalProfileId);

                TempData["ContactSuccess"] = "ההודעה נשלחה. בעל המקצוע יצור עמך קשר בכתובת האימייל שהזנת בטופס הפניה";
            }

            return RedirectToRoute("PublicProfile", new { profileId = model.ProfessionalProfileId });
        }

        //
        // POST: /profile/{username}/follow
        //
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FollowProfile(string username)
        {
            try
            {
                var service = new ProfileService();
                service.FollowProfile(username);
            }
            catch (ClassyValidationException cvx)
            {
                AddModelErrors(cvx);
            }

            return Json(new { IsValid = true });
        }

        #endregion
    }
}