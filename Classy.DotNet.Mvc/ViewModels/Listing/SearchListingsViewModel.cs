using Classy.Models.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Mvc.ViewModels.Listing
{
    public class SearchListingsViewModel<TListingMetadata>
    {
        public string Tag { get; set; }
        public string ListingType { get; set; }
        public TListingMetadata Metadata { get; set; }
        public LocationView Location { get; set; }
        public double? PriceMin { get; set; }
        public double? PriceMax { get; set; }

        //
        public IList<ListingView> Results { get; set; }
    }
}
