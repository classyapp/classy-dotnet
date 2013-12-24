using Classy.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Classy.DotNet.Mvc.ViewModels.Profiles
{
    public class ClaimProfileViewModel<TProMetadata>
    {
        public string ProfileId { get; set; }
        public ProfessionalInfoView ProfessionalInfo { get; set; }
        public TProMetadata Metadata { get; set; }
    }
}