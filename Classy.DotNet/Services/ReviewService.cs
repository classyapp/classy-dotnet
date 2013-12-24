using Classy.Models;
using Classy.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ServiceStack.Text;
using Classy.DotNet.Security;
using System.Net;

namespace Classy.DotNet.Services
{
    public class ReviewService : ServiceBase
    {
        private readonly string SUBMIT_REVIEW_URL = ENDPOINT_BASE_URL + "/profile/{0}/reviews/new?returnrevieweeprofile=true&returnreviewerprofile=true";

        public PostReviewResponse SubmitProfileReview(
            string profileId,
            decimal rank,
            string comments,
            IDictionary<string, decimal> subCriteria,
            ExtendedContactInfoView contactInfo,
            IDictionary<string, string> metadata)
        {
            try
            {
                var client = ClassyAuth.GetAuthenticatedWebClient();
                var url = string.Format(SUBMIT_REVIEW_URL, profileId);
                var reviewJson = client.UploadString(url, 
                    new {
                        Content = comments,
                        Score = rank,
                        SubCriteria = subCriteria,
                        ContactInfo = contactInfo,
                        Metadata = metadata
                    }.ToJson());
                var reviewResponse = reviewJson.FromJson<PostReviewResponse>();
                return reviewResponse;
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
    }
}
