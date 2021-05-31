using Microsoft.AspNetCore.Http;

namespace Serilog.RequestResponse.Extensions.Models
{
    public interface IRuleForRemoveLogHttp
    {
        bool Match(HttpContext httpContext);
        TypeRemoveHttpLog Type { get; }
    }

    public class RuleRemoveLogHttpByMethod : IRuleForRemoveLogHttp
    {
        private HttpMethod _httpMethod;

        public RuleRemoveLogHttpByMethod(HttpMethod httpMethod)
        {
            _httpMethod = httpMethod;
        }

        public TypeRemoveHttpLog Type { get; } = TypeRemoveHttpLog.Method;

        public bool Match(HttpContext httpContext)
        {
            return httpContext.Request.Method == _httpMethod.ToString();
        }
    }

    public class RuleRemoveLogHttByStatusCode : IRuleForRemoveLogHttp
    {
        private int _statusCodeInicio;
        private int _statusCodeFim;
        public TypeRemoveHttpLog Type { get; } = TypeRemoveHttpLog.StatusCode;

        public RuleRemoveLogHttByStatusCode(int statusCodeInicio, int statusCodeFim)
        {
            _statusCodeInicio = statusCodeInicio;
            _statusCodeFim = statusCodeFim;
        }

        public bool Match(HttpContext httpContext)
        {
            var status = httpContext.Response.StatusCode.GetHashCode();

            return (status >= _statusCodeInicio && status <= _statusCodeFim);
        }
    }

    public enum TypeRemoveHttpLog
    {
        StatusCode = 1,
        Method = 2
    }

}
