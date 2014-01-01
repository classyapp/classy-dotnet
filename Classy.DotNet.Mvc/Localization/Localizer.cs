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

namespace Classy.DotNet.Mvc.Localization
{
    public static class Localizer
    {
        static Localizer()
        {
            // init db
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
    }
}
