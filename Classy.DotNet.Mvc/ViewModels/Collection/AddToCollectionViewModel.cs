using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Classy.DotNet.Mvc.ViewModels.Collection
{
    public class AddToCollectionViewModel
    { 
        public AddToCollectionViewModel()
        {
            CollectionList = new List<SelectListItem>
            {
                new SelectListItem {
                    Value = "d12n31123nn",
                    Text = "My Pretty Collection"
                },
                new SelectListItem {
                    Value = "",
                    Text = "Create New Collection"
                }
            };
        }

        public string CollectionId { get; set; }
        public string Title { get; set; }
        public List<SelectListItem> CollectionList { get; set; } 
    }
}
