using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Text;
using System.Net;

namespace Classy.DotNet
{
    public class ValidationError
    {
        public string FieldName { get; set; }
        public string ErrorCode { get; set; }
    }

    public class ClassyValidationException : Exception
    {
        public IList<ValidationError> Errors { get; private set; }

        public ClassyValidationException(IList<ValidationError> errors) : base() 
        {
            Errors = errors;
        }
    }

    public static class ValidationErrorExtensions
    {
        public class ResponseStatus
        {
            public IList<ValidationError> Errors { get; set; }
        }

        public class BadRequest
        {
            public ResponseStatus ResponseStatus { get; set; }
        }

        public static ClassyValidationException ToValidationException(this WebException wex)
        {
            var badRequest = wex.GetResponseBody().FromJson<BadRequest>();
            if (badRequest == null)
            {
                badRequest = new BadRequest
                {
                    ResponseStatus = new ResponseStatus
                    {
                        Errors = new List<ValidationError> {
                            new ValidationError {
                                ErrorCode = (wex.Response as HttpWebResponse).StatusDescription
                            }
                        }
                    }
                };
            }
            return new ClassyValidationException(badRequest.ResponseStatus.Errors);
        }
    }
}
