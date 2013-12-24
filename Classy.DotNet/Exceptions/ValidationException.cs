﻿using System;
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

    public class ClassyException : Exception
    {
        public IList<ValidationError> Errors { get; private set; }

        public ClassyException(string errorCode) : base()
        {
            Errors = new List<ValidationError> {
                new ValidationError {
                    ErrorCode = errorCode
                }
            };
        }

        public ClassyException(IList<ValidationError> errors) : base() 
        {
            Errors = errors;
        }
    }

    public static class ValidationErrorExtensions
    {
        public class ResponseStatus
        {
            public string ErrorCode { get; set; }
            public string Message { get; set; }
            public IList<ValidationError> Errors { get; set; }
        }

        public class BadRequest
        {
            public ResponseStatus ResponseStatus { get; set; }
        }

        public static ClassyException ToClassyException(this WebException wex)
        {
            var badRequest = wex.GetResponseBody().FromJson<BadRequest>();
            if (badRequest.ResponseStatus.Errors.Count == 0)
            {
                return new ClassyException(badRequest.ResponseStatus.Message);
            }
            return new ClassyException(badRequest.ResponseStatus.Errors);
        }
    }
}
