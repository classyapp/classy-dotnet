using Classy.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Mvc.ViewModels.Profiles
{
    public class PublicProfileViewModel<TProMetadata, TReviewSubCriteria>
    {
        public ProfileView Profile { get; set; }
        public TReviewSubCriteria ReviewSubCriteria { get; set; }
        public TProMetadata TypedMetadata { get; set; }
    }
}
