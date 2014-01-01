using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Classy.DotNet.Mvc.ViewModels.Localization
{
    public class EnvironmentSettingsViewModel
    {
        [Display(Name = "EnvSettings_Culture")]
        public string CultureCode { get; set; }
        public string CultureName { get; set; }
        public IEnumerable<SelectListItem> SupportedCulturesList { get; set; }

        public string CountryName { get; set; }
        [Display(Name = "EnvSettings_Country")]
        public string CountryCode { get; set; }
        public IEnumerable<SelectListItem> CountryList { get; set; }
        
        public string CurrencyName { get; set; }
        [Display(Name = "EnvSettings_Currency")]
        public string CurrencyCode { get; set; }
        public IEnumerable<SelectListItem> SupportedCurrenciesList { get; set; }
    }
}
