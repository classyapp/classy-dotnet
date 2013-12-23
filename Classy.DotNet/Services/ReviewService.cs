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

        private readonly string SUBMIT_REVIEW_DATA = @"{{""Content"":""{0}"",""Score"":{1},""ContactInfo"":{2},""Metadata"":{3}}}";

        public PostReviewResponse SubmitProfileReview(
            string profileId,
            int rank,
            string comments,
            ExtendedContactInfoView contactInfo,
            IList<CustomAttributeView> metadata)
        {
            try
            {
                var client = ClassyAuth.GetAuthenticatedWebClient();
                var url = string.Format(SUBMIT_REVIEW_URL, profileId);
                var reviewJson = client.UploadString(url, 
                    string.Format(SUBMIT_REVIEW_DATA, 
                        comments, 
                        rank, 
                        contactInfo != null ? contactInfo.ToJson() : null,
                        metadata != null ? metadata.ToJson() : null));
                var reviewResponse = reviewJson.FromJson<PostReviewResponse>();
                return reviewResponse;
            }
            catch(WebException wex)
            {
                if (wex.IsBadRequest())
                {
                    throw wex.ToValidationException();
                }
                throw wex;
            }
        }
    }
}
