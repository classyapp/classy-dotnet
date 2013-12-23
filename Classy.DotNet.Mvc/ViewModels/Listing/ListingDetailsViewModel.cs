using Classy.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Mvc.ViewModels.Listing
{
    public class ListingDetailsViewModel<TListingMetadata>
    {
        public ListingView Listing { get; set; }
        public TListingMetadata Metadata { get; set; }
    }
}
