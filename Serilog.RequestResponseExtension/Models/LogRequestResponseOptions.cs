using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Serilog.RequestResponse.Extensions.Models
{
    public class LogRequestResponseOptions
    {
        public bool UseFilterOrMiddlewareException { get; set; } = true;
        public IList<IRuleForRemoveLogHttp> RulesRemoveLogHttp { get; private set; } = new List<IRuleForRemoveLogHttp>();


        public LogRequestResponseOptions RemoveLogHttp(IRuleForRemoveLogHttp removeLogHttp)
        {
            RulesRemoveLogHttp.Add(removeLogHttp);

            return this;
        }

        public bool NotShouldLog(HttpContext context, TypeRemoveHttpLog type)
        {
            var roles = RulesRemoveLogHttp.Where(x => x.Type == type).ToList();

            foreach (var role in roles)
            {
                if (role.Match(context))
                    return true;
            }

            return false;
        }

        public bool ExistsRuleForNotLog(HttpContext context)
        {
            foreach (var role in RulesRemoveLogHttp.OrderBy(x => x.Type))
                if (role.Match(context)) return true;

            return false;
        }
    }

    public enum HttpMethod
    {
        POST,
        GET,
        PUT,
        PATCH
    }
}
