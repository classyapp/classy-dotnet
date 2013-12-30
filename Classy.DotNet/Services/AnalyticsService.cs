using Classy.DotNet.Security;
using Classy.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Text;
using System.Net;

namespace Classy.DotNet.Services
{
    public class AnalyticsService : ServiceBase
    {
        private readonly string LOG_ACTIVITY_URL = ENDPOINT_BASE_URL + "/stats/push";

        public TripleView LogActivity(string subjectId, string predicate, string objectId)
        {
            try
            {
                var client = ClassyAuth.GetWebClient();
                var tripleJson = client.UploadString(LOG_ACTIVITY_URL, new
                {
                    SubjectId = subjectId,
                    Predicate = predicate,
                    ObjectId = objectId
                }.ToJson());
                var triple = tripleJson.FromJson<TripleView>();
                return triple;
            }
            catch(WebException wex)
            {
                throw wex.ToClassyException();
            }
        }
    }
}
