using Classy.Models.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Mvc.ViewModels.Security
{
    public class RegistrationViewModel<TMetadata>
    {
        public bool IsProfessional { get; set; }
        public TMetadata Metadata { get; set; }

        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
