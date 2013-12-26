using Classy.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.WebPages.Html;
using System.Web.Mvc.Html;
using System.Web.Mvc;

namespace Classy.DotNet.Mvc
{
    public static class HtmlHelperExtensions
    {
        //public static RenderComment(this HtmlHelper html, string comment)
        //{
        //    var hashRegex = new Regex(@"\B#\w\w+");
        //    var hashtags = hashRegex.Matches(comment)
        //        .Cast<Match>()
        //        .Select(m => m.Value)
        //        .ToArray();

        //    var userRegex = new Regex(@"\B@\w\w+");
        //    var usernames = userRegex.Matches(comment)
        //        .Cast<Match>()
        //        .Select(m => m.Value)
        //        .ToArray();

        //    return 
        //}

        #region // triger listing actions

        public static MvcHtmlString TriggerListingActionLink(this System.Web.Mvc.HtmlHelper html, string linkText, string actionToTrigger, ListingView listing)
        {
            return TriggerListingActionLink(html, linkText, actionToTrigger, listing, true);
        }

        public static MvcHtmlString TriggerListingActionLink(this System.Web.Mvc.HtmlHelper html, string linkText, string actionToTrigger, ListingView listing, bool requireLogin)
        {
            var needsAuth = requireLogin && !html.ViewContext.HttpContext.User.Identity.IsAuthenticated;
            string link = "<a href=\"#\" trigger-listing-action=\"{0}\" listing-type=\"{1}\" listing-id=\"{2}\" {3}>{4}</a>";
            string output = string.Format(link, 
                actionToTrigger, 
                listing.ListingType.ToLower(), 
                listing.Id,
                requireLogin ? "authorize" : string.Empty,
                linkText);
            return new MvcHtmlString(output);
        }

        #endregion

        #region // triger profile actions

        public static MvcHtmlString TriggerProfileActionLink(this System.Web.Mvc.HtmlHelper html, string linkText, string actionToTrigger, ProfileView profile)
        {
            return TriggerProfileActionLink(html, linkText, actionToTrigger, profile, true);
        }

        public static MvcHtmlString TriggerProfileActionLink(this System.Web.Mvc.HtmlHelper html, string linkText, string actionToTrigger, ProfileView profile, bool requireLogin)
        {
            string link = "<a href=\"#\" trigger-profile-action=\"{0}\" profile-id=\"{1}\" {2}>{3}</a>";
            string output = string.Format(link,
                actionToTrigger,
                profile.UserName,
                requireLogin ? "authorize" : string.Empty,
                linkText);
            return new MvcHtmlString(output);
        }

        #endregion

        public static MvcHtmlString ListingLink(this System.Web.Mvc.HtmlHelper html, string listingType, ListingView listing)
        {
            return html.RouteLink(listing.Title, string.Concat(listingType, "Details"), new { listingId = listing.Id, slug = ToSlug(listing.Title) });
        }

        public static MvcHtmlString ProfileLink(this System.Web.Mvc.HtmlHelper html, ProfileView profile)
        {
            if (profile.ContactInfo == null && !profile.IsProfessional) return new MvcHtmlString("unknown");
            var name = string.Empty;
            if (profile.IsProxy) name = profile.ProfessionalInfo.CompanyName;
            else if (profile.IsProfessional) name = profile.ProfessionalInfo.CompanyName;
            else name = profile.ContactInfo.Name;
            return html.RouteLink(name ?? profile.UserName, "PublicProfile", new { profileId = profile.Id, slug = ToSlug(name) });
        }

        public static MvcHtmlString ToSlug(this System.Web.Mvc.HtmlHelper html, string content)
        {
            return new MvcHtmlString(ToSlug(content));
        }

        private static string ToSlug(string content)
        {
            return content != null ? content.ToLower()
                .Replace("?", string.Empty)
                .Replace("&", "-and-")
                .Replace("  ", " ")
                .Replace(" ", "-") : null;
        }
    }
}