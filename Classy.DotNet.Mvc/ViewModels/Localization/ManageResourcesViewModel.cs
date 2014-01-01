using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Mvc.ViewModels.Localization
{
    public class ManageResourcesViewModel
    {
        public string ResourceKey { get; set; }
        public IDictionary<string, string> Values { get; set; }
    }
}
