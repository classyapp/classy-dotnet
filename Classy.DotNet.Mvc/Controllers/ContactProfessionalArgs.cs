using Classy.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Classy.DotNet.Mvc.Controllers
{
    public class ContactProfessionalArgs<TProMetadata>
    {
        public string ReplyToEmail { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public ProfileView ProfessionalProfile { get; set; }
        public TProMetadata TypedMetadata { get; set; }
    }
}
