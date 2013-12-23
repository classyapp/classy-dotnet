using Classy.Models.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Classy.DotNet.Mvc.ViewModels.Reviews
{
    public class ProfileReviewViewModel<TProMetadata>
    {
        public string ProfileId { get; set; }
        public bool IsProxy { get; set; }
        [Required]
        public int Rank { get; set; }
        [Required]
        public string Comments { get; set; }
        public ExtendedContactInfoView ContactInfo { get; set; }
        public TProMetadata Metadata { get; set; }
    }
}