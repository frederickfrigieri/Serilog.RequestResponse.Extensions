using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Serilog.RequestResponse.Extensions.Filters
{
    public class FilterException : ExceptionFilterAttribute
    {
        public FilterException() { }

        public override void OnException(ExceptionContext context)
        {
            var customException = context.Exception.ConstroiCustomExceptionDetails();
            context.HttpContext.Response.StatusCode = customException.StatusCode;

            if (customException.Dados != null)
                context.Result = new JsonResult(customException.Dados);

            context.HttpContext.Items.Add("Exception", context.Exception.ToString());

            base.OnException(context);
        }
    }
}
