﻿using Classy.DotNet.Security;
using Classy.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Text;
using System.Web;
using System.Net;

namespace Classy.DotNet.Services
{
    public class ListingService : ServiceBase
    {
        // create listing
        private readonly string CREATE_LISTING_URL = ENDPOINT_BASE_URL + "/listings/new";
        private readonly string ADD_EXTERNAL_MEDIA_URL = ENDPOINT_BASE_URL + "/listings/{0}/media";
        private readonly string PUBLISH_LISTING_URL = ENDPOINT_BASE_URL + "/listings/{0}/publish";
        // get listings
        private readonly string GET_LISTING_BY_ID_URL = ENDPOINT_BASE_URL + "/listings/{0}?"; 
        // post comment
        private readonly string POST_COMMENT_URL = ENDPOINT_BASE_URL + "/listings/{0}/comments/new";
        // favorite listing
        private readonly string FAVORITE_LISTING_URL = ENDPOINT_BASE_URL + "/listings/{0}/favorite";

        public ListingView CreateListing(
            string title, 
            string content,
            string listingType,
            IList<CustomAttributeView> metadata,
            HttpFileCollectionBase files)
        {
            var client = ClassyAuth.GetAuthenticatedWebClient();
            var data = new
            {
                Title = title, 
                Content = content,
                ListingType = listingType,
                Metadata = metadata

            }.ToJson();

            // create the listing
            ListingView listing = null;
            try
            {
                var listingJson = client.UploadString(CREATE_LISTING_URL, data);
                listing = listingJson.FromJson<ListingView>();
            }
            catch(WebException wex)
            {
                if (wex.IsBadRequest())
                {
                    throw wex.ToClassyException();
                }
                throw wex;
            }

            // add media files
            var url = string.Format(ADD_EXTERNAL_MEDIA_URL, listing.Id);
            foreach (var f in files.AllKeys)
            {
                var fileContent = new byte[files[f].ContentLength];
                files[f].InputStream.Read(fileContent, 0, files[f].ContentLength);
                var req = ClassyAuth.GetAuthenticatedWebRequest(url);
                HttpUploadFile(req, fileContent, files[f].ContentType);
            }

            // publish
            url = string.Format(PUBLISH_LISTING_URL, listing.Id);
            var updatedJson = client.UploadString(url, "".ToJson());
            listing = updatedJson.FromJson<ListingView>();

            return listing;
        }

        public ListingView GetListingById(
            string listingId,
            bool IncludeComments,
            bool IncludeProfile,
            bool IncludeCommenterProfiles,
            bool IncludeFavoritedByProfiles,
            bool LogImpression)
        {
            var client = ClassyAuth.GetWebClient();
            var url = string.Format(GET_LISTING_BY_ID_URL, listingId);
            if (IncludeComments) url = string.Concat(url, "&includecomments=true");
            if (IncludeProfile) url = string.Concat(url, "&includeprofile=true");
            if (IncludeCommenterProfiles) url = string.Concat(url, "&includecommenterprofiles=true");
            if (IncludeFavoritedByProfiles) url = string.Concat(url, "&includefavoritedbyprofiles=true");
            if (LogImpression) url = string.Concat(url, "&logimperssion=true");
            var listingJson = client.DownloadString(url);
            var listing = listingJson.FromJson<ListingView>();
            return listing;
        }

        public CommentView PostComment(string listingId, string content)
        {
            try
            {
                var client = ClassyAuth.GetAuthenticatedWebClient();
                var url = string.Format(POST_COMMENT_URL, listingId);
                var commentJson = client.UploadString(url, string.Concat("{\"Content\":\"", content, "\"}"));
                var comment = commentJson.FromJson<CommentView>();
                return comment;
            }
            catch(WebException wex)
            {
                if (wex.IsBadRequest())
                {
                    throw wex.ToClassyException();
                }
                throw wex;
            }
        }

        public void FavoriteListing(string listingId)
        {
            try
            {
                var client = ClassyAuth.GetAuthenticatedWebClient();
                var url = string.Format(FAVORITE_LISTING_URL, listingId);
                client.UploadString(url, "{}");
            }
            catch (WebException wex)
            {
                if (wex.IsBadRequest())
                {
                    throw wex.ToClassyException();
                }
                throw wex;
            }
        }

        #region // upload file

        private static void HttpUploadFile(HttpWebRequest wr, byte[] fileContent, string contentType)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;

            var rs = wr.GetRequestStream();
            //header
            rs.Write(boundarybytes, 0, boundarybytes.Length);
            string headerTemplate = "Content-Disposition: form-data; name=\"file\"; filename=\"file\"\r\nContent-Type: {0}\r\n\r\n";
            string header = string.Format(headerTemplate, contentType);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);
            //file
            rs.Write(fileContent, 0, fileContent.Length);
            //footer
            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            try
            {
                var res = wr.GetResponse();
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        #endregion
    }
}
