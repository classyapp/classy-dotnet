using Classy.Models.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Mvc.ViewModels.Listing
{
    public class ProductPricingViewModel
    {
        // TODO: How do we share models with Classy.Models.Response but still have front-end validation mechanism?
        [Required]
        public string SKU { get; set; }
        [Range(0, Int16.MaxValue)]
        public double? Price { get; set; }
        [Range(0, Int16.MaxValue)]
        public double? CompareAtPrice { get; set; }

        [Required]
        [Range(1, Int16.MaxValue)]
        public int? Quantity { get; set; }

        [Range(0, Int16.MaxValue)]
        public int? DomesticRadius { get; set; }
        [Range(0, Int16.MaxValue)]
        public decimal? DomesticShippingPrice { get; set; }
        [Range(0, Int16.MaxValue)]
        public decimal? InternationalShippingPrice { get; set; }
    }

    public class CreateListingViewModel<TListingMetadata>
    {
        // basic
        [Display(Name="CreateListing_Title")]
        [Required(ErrorMessage="CreateListing_Title_Required")]
        public string Title { get; set; }
        [Display(Name = "CreateListing_Content")]
        [Required(ErrorMessage="CreateListing_Content_Required")]
        public string Content { get; set; }
        public ProductPricingViewModel PricingInfo { get; set; }
        public LocationView Location { get; set; }
        public bool AutoPublish { get; set; }
        // meta
        public TListingMetadata Metadata { get; set; }

        // TODO: products and bookable items
    }
}
