using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Classy.DotNet.Mvc.Controllers
{
    public interface IReviewSubCriteria<TReviewSubCriteria>
    {
        decimal CalculateScore();
        IDictionary<string, decimal> ToDictionary();
        TReviewSubCriteria FromDictionary(IDictionary<string, decimal> subCriteria);
    }
}
