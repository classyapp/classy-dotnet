using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Mvc.ViewModels.Security
{
    public class LoginViewModel
    {
        [Display(Name="Login_Email")]
        public string Email { get; set; }
        [Display(Name = "Login_Password")]
        public string Password { get; set; }
        [Display(Name = "Login_RememberMe")]
        public bool RememberMe { get; set; }
        public string RedirectUrl { get; set; }
    }
}
