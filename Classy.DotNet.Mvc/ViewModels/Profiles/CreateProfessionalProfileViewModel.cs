using Classy.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Mvc.ViewModels.Profiles
{
    public class CreateProfessionalProfileViewModel<TProMetadata>
    {
        public ProfessionalInfoView ProfessionalInfo { get; set; }
        public TProMetadata Metadata { get; set; }
    }
}
