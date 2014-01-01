using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.WebPages.Html;
using System.Web.Mvc.Html;
using System.Web.Mvc;

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
            return string.Concat(key, " (", System.Threading.Thread.CurrentThread.CurrentUICulture.Name, ")");
        }

        public static MvcHtmlString _Get(string key)
        {
            return new MvcHtmlString(Get(key));
        }
    }
}
