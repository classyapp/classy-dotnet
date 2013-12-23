using Classy.Models.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Mvc.ViewModels.Listing
{
    public class CreateListingViewModel<TListingMetadata>
    {
        // basic
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        public LocationView Location { get; set; }
        public bool AutoPublish { get; set; }
        // meta
        public TListingMetadata Metadata { get; set; }

        // TODO: products and bookable items
    }
}
