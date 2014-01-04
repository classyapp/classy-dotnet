using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using System.Web.Mvc;
using Classy.DotNet.Mvc.ViewModels.Localization;
using Classy.DotNet.Services;
using System.Web;

namespace Classy.DotNet.Mvc.Controllers
{
    public class LocalizationController : BaseController
    {
        public LocalizationController() : base() { }
        public LocalizationController(string ns) : base(ns) { }

        /// <summary>
        /// register routes within host app's route collection
        /// </summary>
        public override void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                name: "EnvironmentSettings",
                url: "settings/locale",
                defaults: new { controller = "Localization", action = "EnvironmentSettings" },
                namespaces: new string[] { Namespace }
            );

            routes.MapRoute(
                name: "ManageResources",
                url: "resource/manage",
                defaults: new { controller = "Localization", action = "ManageResources" },
                namespaces: new string[] { Namespace }
            );
        }

        //
        // GET: /resource/manage
        //
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ManageResources()
        {
            return View();
        }

        //
        // POST: /resource/manage
        //
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ManageResources(ManageResourcesViewModel model)
        {
            var service = new LocalizationService();
            service.SetResourceValues(model.ResourceKey, model.Values);
            HttpRuntime.Cache.Remove(model.ResourceKey);
            return View(model);
        }

        //
        // GET: /profile/settings/env
        //
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult EnvironmentSettings()
        {
            var model = GetEnvFromContext();
            //PopulateCultures(model);
            return PartialView(model);
        }

        //
        // POST: /profile/settings/env
        //
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EnvironmentSettings(EnvironmentSettingsViewModel model)
        {
            SetContextEnvFromModel(model);
            //PopulateCultures(model);
            return PartialView(model);
        }

        //private void PopulateCultures(EnvironmentSettingsViewModel model)
        //{
        //    model.SupportedCulturesList = new SelectListItem[]
        //        {
        //            new SelectListItem {
        //                Value = "en-US",
        //                Text = "English (US)"
        //            },
        //            new SelectListItem {
        //                Value = "he-IL",
        //                Text = "עברית"
        //            },
        //            new SelectListItem {
        //                Value = "fr-BE",
        //                Text = "Français"
        //            }
        //        };
        //}

        private EnvironmentSettingsViewModel GetEnvFromContext()
        {
            return new EnvironmentSettingsViewModel
            {
                CultureCode = System.Threading.Thread.CurrentThread.CurrentUICulture.Name,
                CultureName = System.Threading.Thread.CurrentThread.CurrentUICulture.DisplayName,
            };
        }

        private void SetContextEnvFromModel(EnvironmentSettingsViewModel model)
        {
            Response.Cookies.Add(new System.Web.HttpCookie(Localization.Localizer.CULTURE_COOKIE_NAME)
            {
                Value = model.CultureCode,
                Expires = DateTime.UtcNow.AddYears(30)
            });
        }
    }
}
