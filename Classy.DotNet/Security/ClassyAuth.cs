using Classy.DotNet.Services;
using Classy.Models;
using Classy.Models.Response;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Security;
using Classy.DotNet;

namespace Classy.DotNet.Security
{
    public class ClassyAuth
    {
        // private
        private static string _sApiKey;
        private static string _sEndpointUrl;

        // public
        public static string ApiKey { get { return _sApiKey; } }
        public static string EndpointBaseUrl { get { return _sEndpointUrl;  } }

        // private static
        private static string[] AuthCookieNames = { "ss-id", "ss-pid", "ss-opt", "X-UAId" };
        private static string COOKIE_USER_ID = "X-UAId";
        private static string COOKIE_SESSION_ID = "ss-id";
        private static string COOKIE_PERMANENT_SESSION_ID = "ss-pid";
        private static string CFG_API_KEY = "Classy:AppId";
        private static string CFG_ENDPOINT_BASE_URL = "Classy:EndpointBaseUrl";

        // ctor
        static ClassyAuth()
        {
            _sApiKey = System.Configuration.ConfigurationManager.AppSettings[CFG_API_KEY];
            _sEndpointUrl = System.Configuration.ConfigurationManager.AppSettings[CFG_ENDPOINT_BASE_URL];
        }

        #region // static auth methods

        public static bool AuthenticateUser(string username, string password, bool rememberMe)
        {
            try
            {
                var context = System.Web.HttpContext.Current;
                var creds = new
                {
                    UserName = username,
                    Password = password,
                    RememberMe = rememberMe
                }.ToJson();
                var credsBytes = Encoding.UTF8.GetBytes(creds);
                var client = HttpWebRequest.Create(string.Concat(EndpointBaseUrl, "/auth/credentials")) as HttpWebRequest;
                client.Method = "POST";
                client.ContentType = "application/json";
                client.Accept = "application/json";
                client.Headers.Add("X-Classy-Env", GetEnvHeader());
                var stream = client.GetRequestStream();
                stream.Write(credsBytes, 0, credsBytes.Length);
                stream.Close();
                var response = client.GetResponse() as HttpWebResponse;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    FillInCookiesCollection(context.Response.Cookies, response.Headers[HttpResponseHeader.SetCookie], context.Request.Url);
                    var authJson = (new StreamReader(response.GetResponseStream())).ReadToEnd();
                    var auth = authJson.FromJson<ClassyAuthResponse>();
                    return SetPrincipalInternal(auth.SessionId);
                }
                return false;
            }
            catch(WebException wex)
            {
                return false;
            }
        }

        public static bool AuthenticateFacebookUser(string token)
        {
            try
            {
                var context = System.Web.HttpContext.Current;
                var client = HttpWebRequest.Create(string.Concat(EndpointBaseUrl, "/auth/facebook?format=json&oauth_token=", token)) as HttpWebRequest;
                client.Method = "GET";
                client.ContentType = "application/json";
                client.Accept = "application/json";
                client.Headers.Add("X-Classy-Env", GetEnvHeader());
                var response = client.GetResponse() as HttpWebResponse;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    FillInCookiesCollection(context.Response.Cookies, response.Headers[HttpResponseHeader.SetCookie], context.Request.Url);
                    var authJson = (new StreamReader(response.GetResponseStream())).ReadToEnd();
                    var auth = authJson.FromJson<ClassyAuthResponse>();
                    return SetPrincipalInternal(auth.SessionId);
                }
                return true;
            }
            catch (WebException)
            {
                return false;
            }
        }

        public static void Logout()
        {
            var client = GetWebClient();
            client.DownloadString(string.Concat(EndpointBaseUrl, "/auth/logout"));
            ClearAuthCookies();
        }

        #endregion

        #region // static registration methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="rememberMe"></param>
        /// <returns>The new profile or throws a ClassyValidationException</returns>
        public static bool Register(string username, string email, string password)
        {
            try
            {
                // register user
                var registration = new
                {
                    UserName = username,
                    Email = email,
                    Password = password,
                    AutoLogin = true
                }.ToJson();
                var context = System.Web.HttpContext.Current;
                var regBytes = Encoding.UTF8.GetBytes(registration);
                var client = HttpWebRequest.Create(string.Concat(EndpointBaseUrl, "/register")) as HttpWebRequest;
                client.Method = "POST";
                client.ContentType = "application/json";
                client.Accept = "application/json";
                client.Headers.Add("X-Classy-Env", GetEnvHeader());
                var stream = client.GetRequestStream();
                stream.Write(regBytes, 0, regBytes.Length);
                stream.Close();
                var response = client.GetResponse() as HttpWebResponse;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    FillInCookiesCollection(context.Response.Cookies, response.Headers[HttpResponseHeader.SetCookie], context.Request.Url);
                    var respJson = (new StreamReader(response.GetResponseStream())).ReadToEnd();
                    var reg = respJson.FromJson<ClassyRegistrationResponse>();
                    return SetPrincipalInternal(reg.UserId);
                }
                return false;
            }
            catch (WebException wex)
            {
                if (wex.IsBadRequest())
                {
                    throw wex.ToClassyException();
                }
                return false;
            }
        }

        #endregion

        #region // helpers webclients

        public static WebClient GetWebClient()
        {
            var client = new WebClient();
            client.Encoding = Encoding.UTF8;
            client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            client.Headers.Add(HttpRequestHeader.Accept, "application/json");
            client.Headers.Add("X-Classy-Env", GetEnvHeader());
            return client;
        }

        private static string GetEnvHeader()
        {
            return new
            {
                CultureCode = System.Threading.Thread.CurrentThread.CurrentUICulture.Name,
                CountryCode = "IL",
                CurrencyCode = "ILS",
                AppId = ApiKey
            }.ToJson();
        }

        public static WebClient GetAuthenticatedWebClient(bool throwIfNull = true)
        {
            var context = System.Web.HttpContext.Current;
            if (!IsLoggedIn())
            {
                ClearAuthCookies();
                if (throwIfNull) throw new UnauthorizedAccessException("user not logged in. must be logged in to complete operation.");
                return null;
            }

            var client = new WebClient();
            client.Encoding = Encoding.UTF8;
            client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            client.Headers.Add(HttpRequestHeader.Accept, "application/json");
            client.Headers.Add("X-Classy-Env", GetEnvHeader());
            var cookies = new CookieContainer();
            foreach (var n in AuthCookieNames)
            {
                if (context.Request.Cookies.Get(n) != null)
                    cookies.Add(ToCookie(context.Request.Cookies[n], new Uri(EndpointBaseUrl).Host));
            }
            client.Headers.Add(HttpRequestHeader.Cookie, cookies.GetCookieHeader(new Uri(EndpointBaseUrl)));
            return client;
        }

        public static HttpWebRequest GetAuthenticatedWebRequest(string url, bool throwIfNull = true)
        {
            var context = System.Web.HttpContext.Current;
            if (!IsLoggedIn())
            {
                ClearAuthCookies();
                if (throwIfNull) throw new UnauthorizedAccessException("user not logged in. must be logged in to complete operation.");
                return null;
            }

            var client = WebRequest.Create(url) as HttpWebRequest;
            client.Headers.Add("X-Classy-Env", GetEnvHeader());
            var cookies = new CookieContainer();
            foreach (var n in AuthCookieNames)
            {
                if (context.Request.Cookies.Get(n) != null)
                    cookies.Add(ToCookie(context.Request.Cookies[n], new Uri(EndpointBaseUrl).Host));
            }
            client.Headers.Add(HttpRequestHeader.Cookie, cookies.GetCookieHeader(new Uri(EndpointBaseUrl)));
            return client;
        }

        private static bool IsLoggedIn()
        {
            var context = System.Web.HttpContext.Current;
            return context.Request.Cookies.AllKeys.Contains(COOKIE_USER_ID) &&
                (context.Request.Cookies.AllKeys.Contains(COOKIE_SESSION_ID) ||
                context.Request.Cookies.AllKeys.Contains(COOKIE_PERMANENT_SESSION_ID));

        }

        #endregion

        #region // identity handling

        private static void ClearAuthCookies()
        {
            var context = System.Web.HttpContext.Current;
            foreach (var cookieName in AuthCookieNames)
            {
                if (context.Request.Cookies.AllKeys.Contains(cookieName))
                {
                    var cookie = context.Request.Cookies[cookieName];
                    cookie.Expires = DateTime.Now.AddDays(-1);
                    cookie.Domain = context.Request.Url.Host;
                    context.Response.Cookies.Add(cookie);
                }
            }
        }

        public static void SetPrincipal()
        {
            var context = System.Web.HttpContext.Current;
            if (context.Request.Cookies.AllKeys.Contains(COOKIE_USER_ID))
            {
                var authCookie = context.Request.Cookies[COOKIE_USER_ID];
                SetPrincipalInternal(authCookie.Value);
            }
        }

        private static bool SetPrincipalInternal(string profileId)
        {
            var context = System.Web.HttpContext.Current;
            try
            {
                var service = new ProfileService();
                var profile = service.GetAuthenticatedProfile();
                ClassyIdentity identity = new ClassyIdentity
                {
                    Name = profile.UserName,
                    IsAuthenticated = true,
                    Profile = profile
                };
                ClassyPrincipal principal = new ClassyPrincipal(identity);
                context.User = principal;
                return true;
            }
            catch (ClassyException cex)
            {
                ClearAuthCookies();
                return false;
            }
        }

        #region // cookie collection manipulation methods 

        private static void FillInCookiesCollection(HttpCookieCollection cookieCollection, string header, Uri uri)
        {
            // "ss-id=Xy4xA8CopnmhgvrDuDuW; path=/; HttpOnly,ss-pid=qouowGC6cXVu0Gx714NL; expires=Fri, 02-Sep-2033 19:24:05 GMT; path=/; HttpOnly,X-UAId=2; expires=Fri, 02-Sep-2033 19:24:05 GMT; path=/; HttpOnly"
            var part = new Regex("(.*?)=(.*?);");
            var cookies = header.Split(new string[] { "HttpOnly," }, StringSplitOptions.None);
            foreach (var c in cookies)
            {
                var cookie = new Cookie();
                foreach (Match m in part.Matches(c))
                {
                    var key = m.Groups[1].Value;
                    var value = m.Groups[2].Value;
                    switch (key.ToLower().Trim())
                    {
                        case "expires":
                            cookie.Expires = DateTime.Parse(value);
                            break;
                        case "path":
                            cookie.Path = value;
                            break;
                        default:
                            cookie.Name = key;
                            cookie.Value = value;
                            break;
                    }
                }
                cookie.Domain = uri.Host;
                cookieCollection.Add(ToHttpCookie(cookie));
            }
        }

        private static HttpCookie ToHttpCookie(Cookie cookie)
        {
            return new HttpCookie(cookie.Name, cookie.Value) 
            {
                Domain = cookie.Domain,
                Expires = cookie.Expires,
                Path = cookie.Path,
                Secure = cookie.Secure
            };
        }

        private static Cookie ToCookie(HttpCookie cookie, string newDomain)
        {
            return new Cookie
            {
                Name = cookie.Name,
                Value = cookie.Value,
                Domain = newDomain,
                Expires = cookie.Expires,
                Path = cookie.Path,
                Secure = cookie.Secure
            };
        }
        
        #endregion

        #endregion
    }
}