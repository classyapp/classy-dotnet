using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.WebPages.Html;
using System.Web.Mvc.Html;
using System.Web.Mvc;
using Classy.DotNet.Services;
using Classy.Models.Response;
using System.Globalization;
using System.Web.Routing;

namespace Classy.DotNet.Mvc.Localization
{
    public static class Localizer
    {
        public static string CULTURE_COOKIE_NAME = "classy.env.culture";
        public static string SUPPORTED_CULTURES_CACHE_KEY = "classy.cache.supported-languages";
        public static string SUPPORTED_COUNTRIES_CACHE_KEY = "classy.cache.supported-countries";
        public static string SUPPORTED_CURRENCIES_CACHE_KEY = "classy.cache.supported-currencies";
        public static string ROUTE_LOCALE_DATA_TOKEN_KEY = "classy.routetoken.locale";

        static Localizer()
        {
            // init db
        }

        public static void Initialize()
        {
            Initialize(null);
        }
        public static void Initialize(string forceCulture)
        {
            var cookie = HttpContext.Current.Request.Cookies[CULTURE_COOKIE_NAME];
            string cultureName = forceCulture ?? (cookie != null ? cookie.Value : null);
            if (cultureName != null)
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(cultureName);
            }
        }

        public static IEnumerable<CultureInfo> SupportedCultures
        {
            get
            {
                var languages = HttpRuntime.Cache[SUPPORTED_CULTURES_CACHE_KEY] as CultureInfo[];
                if (languages == null)
                {
                    languages = new CultureInfo[]
                {
                    CultureInfo.CreateSpecificCulture("en-US"),
                    CultureInfo.CreateSpecificCulture("fr-BE"),
                    CultureInfo.CreateSpecificCulture("nl-BE"),
                    CultureInfo.CreateSpecificCulture("he-IL")
                };
                }
                return languages;
            }
        }

        public static string Get(string key)
        {
            LocalizationResourceView resource = HttpRuntime.Cache[key] as LocalizationResourceView;
            if (resource == null)
            {
                var service = new LocalizationService();
                resource = service.GetResourceByKey(key);
                if (resource != null) HttpRuntime.Cache[key] = resource;
            }
            if (resource != null)
            {
                var value = resource.Values.SingleOrDefault(x => x.Key == System.Threading.Thread.CurrentThread.CurrentUICulture.Name);
                return value.Value;
            }
            return string.Concat(key, "_", System.Threading.Thread.CurrentThread.CurrentUICulture.Name);
        }

        // RouteCollection extension to map a route for all supported cultures
        public static void MapRouteForSupportedLocales(
            this RouteCollection routes, 
            string name, 
            string url, 
            object defaults, 
            string[] namespaces)
        {
            Route route;
            // then add another route for each supported culture
            foreach (var culture in SupportedCultures)
            {
                route = routes.MapRoute(
                    GetRouteNameForLocale(name, culture.Name),
                    string.Concat(culture.Name, "/", url), 
                    defaults, 
                    namespaces);
                route.DataTokens = new RouteValueDictionary();
                route.DataTokens.Add("RouteName", name);
                route.DataTokens.Add(ROUTE_LOCALE_DATA_TOKEN_KEY, culture.Name);
            }

            // map the route directly
            route = routes.MapRoute(name, url, defaults, namespaces);
            route.DataTokens = new RouteValueDictionary();
            route.DataTokens.Add("RouteName", name);

        }

        // Url extension to get a link to a route in a specific culture
        public static string RouteUrlForLocale(this System.Web.Mvc.UrlHelper url, string routeName, string cultureCode)
        {
            return url.RouteUrl(GetRouteNameForLocale(routeName, cultureCode));
        }

        // Html extension to get a link to a route for the current culture
        public static MvcHtmlString RouteLinkForCurrentLocale(this System.Web.Mvc.HtmlHelper html, string linkText, string routeName)
        {
            return RouteLinkForCurrentLocale(html, linkText, routeName, null);
        }

        // Html extension to get a link to a route for the current culture
        public static MvcHtmlString RouteLinkForCurrentLocale(this System.Web.Mvc.HtmlHelper html, string linkText, string routeName, object routeValues)
        {
            return html.RouteLink(linkText, GetRouteNameForLocale(routeName, System.Threading.Thread.CurrentThread.CurrentUICulture.Name), routeValues);
        }

        // Html extension to get a link to a route in a specific culture
        public static MvcHtmlString RouteLinkForLocale(this System.Web.Mvc.HtmlHelper html, string linkText, string routeName, string cultureName)
        {
            return html.RouteLink(linkText, GetRouteNameForLocale(routeName, cultureName));
        }

        private static string GetRouteNameForLocale(string name, string cultureCode)
        {
            return string.Concat(name, "_", cultureCode);
        }

    }
}
