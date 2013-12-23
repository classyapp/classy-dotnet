using Classy.Models;
using Classy.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace Classy.DotNet.Security
{
    public class ClassyIdentity : IIdentity
    {
        public string AuthenticationType { get { return "classy API"; } }

        public bool IsAuthenticated { get; set; }

        public string Name { get; set; }

        public ProfileView Profile { get; set; }
    }
}