using Classy.DotNet.Security;
using Classy.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Text;

namespace Classy.DotNet.Services
{
    public class LocalizationService : ServiceBase
    {
        private readonly string RESOURCE_URL = ENDPOINT_BASE_URL + "/resource/{0}";

        public LocalizationResourceView GetResourceByKey(string key)
        {
            try
            {
                var client = ClassyAuth.GetWebClient();
                var url = string.Format(RESOURCE_URL, key);
                var resourceJson = client.DownloadString(url);
                var resource = resourceJson.FromJson<LocalizationResourceView>();
                return resource;
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }

        public LocalizationResourceView SetResourceValues(string key, IDictionary<string, string> values)
        {
            try
            {
                var client = ClassyAuth.GetWebClient();
                var url = string.Format(RESOURCE_URL, key);
                var resourceJson = client.UploadString(url, string.Concat("{\"Values\":", values.ToJson()));
                var resource = resourceJson.FromJson<LocalizationResourceView>();
                return resource;
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }
        }
    }
}
