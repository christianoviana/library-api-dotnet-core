using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;

namespace Library.API.Domain.Results.Exceptions
{
    public class UnprocessableEntityObjectResult : ObjectResult
    {
        public UnprocessableEntityObjectResult(ModelStateDictionary modelState) : base(modelState)
        {
            StatusCode = (int) HttpStatusCode.UnprocessableEntity;
        }
    }
}
