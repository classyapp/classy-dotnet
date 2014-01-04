using Classy.DotNet.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Routing;
using System.Collections.Specialized;
using Classy.Models.Response;
using Classy.DotNet.Mvc.ViewModels.Security;
using Classy.DotNet.Services;
using ServiceStack.Text;
using Classy.DotNet.Mvc.Localization;

namespace Classy.DotNet.Mvc.Controllers.Security
{
    public class SecurityController<TMetadata> : BaseController
        where TMetadata : IMetadata<TMetadata>, new()
    {
        public event EventHandler<ProfileView> OnProfileRegistered;

        public SecurityController() : base() { }
        public SecurityController(string ns) : base(ns) { }

        private readonly string USER_EXTENDED_METADATA_KEY = "UserExtendedProfile";

        /// <summary>
        /// register routes within host app's route collection
        /// </summary>
        public override void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                name: "Logout",
                url: "logout",
                defaults: new { controller = "Security", action = "Logout" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRouteWithName(
                name: "Login",
                url: "login",
                defaults: new { controller = "Security", action = "Login" },
                namespaces: new string[] { Namespace }
                );

            routes.MapRoute(
                name: "AuthenticateFacebookUser",
                url: "login/fb",
                defaults: new { controller = "Security", action = "AuthenticateWithFacebook" },
                namespaces: new string[] { Namespace }
                );

            routes.MapRouteWithName(
                name: "Register",
                url: "register",
                defaults: new { controller = "Security", action = "Register" },
                namespaces: new string[] { Namespace });

            routes.MapRouteWithName(
                name: "CompleteRegistration",
                url: "register/more",
                defaults: new { controller = "Security", action = "CompleteRegistration" },
                namespaces: new string[] { Namespace });
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Login()
        {
            var model = new LoginViewModel
            {
                RedirectUrl = Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : "~/"
            };

            if (Request.IsAjaxRequest())
            {
                return PartialView("LoginModal", model);
            }
            else
            {
                return View(model);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Login(LoginViewModel model)
        {
            try
            {
                var isValid = ClassyAuth.AuthenticateUser(model.Email, model.Password, model.RememberMe);
                if (!isValid)
                {
                    ModelState.AddModelError("Invalid", Localization.Localizer.Get("Login_InvalidCredentials"));
                    return View(model);
                }
                else
                {
                    var returnUrl = string.IsNullOrEmpty(model.RedirectUrl) ? "~/" : Uri.UnescapeDataString(model.RedirectUrl);
                    return Redirect(returnUrl);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AuthenticateWithFacebook(string token)
        {
            try
            {
                var isValid = ClassyAuth.AuthenticateFacebookUser(token);
                if (isValid) return Json(new { IsValid = true, Profile = (User.Identity as ClassyIdentity).Profile });
                else return Json(new { IsValid = false });
            }
            catch (WebException wex)
            {
                return new HttpStatusCodeResult((wex.Response as HttpWebResponse).StatusCode);
            }
            catch (UnauthorizedAccessException uex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, uex.Message);
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Logout()
        {
            try
            {
                ClassyAuth.Logout();
                var returnUrl = Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : "~/";
                return new RedirectResult(returnUrl);
            }
            catch (WebException wex)
            {
                return new HttpStatusCodeResult((wex.Response as HttpWebResponse).StatusCode);
            }
            catch (UnauthorizedAccessException uex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, uex.Message);
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Register()
        {
            if (Request.IsAuthenticated) return RedirectToRoute("MyProfile");

            return View(new RegistrationViewModel<TMetadata>());
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CompleteRegistration(RegistrationViewModel<TMetadata> model)
        {
            var profile = (User.Identity as ClassyIdentity).Profile;
            if (model.Metadata != null)
            {
                var service = new ProfileService();
                profile = service.UpdateProfile(profile.Id, null, model.Metadata.ToDictionary(), null);
            }

            if (OnProfileRegistered != null)
                OnProfileRegistered(this, profile);

            return RedirectToRoute(
                    model.IsProfessional ? "CreateProfessionalProfile" : "MyProfile"
                );
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Register(RegistrationViewModel<TMetadata> model)
        {
            try
            {
                // TODO: validate CSRF token
                if (!ClassyAuth.Register(model.Username, model.Email, model.Password))
                    throw new Exception("what happened?");

                return CompleteRegistration(model);
            }
            catch (ClassyException eex)
            {
                AddModelErrors(eex);
                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}