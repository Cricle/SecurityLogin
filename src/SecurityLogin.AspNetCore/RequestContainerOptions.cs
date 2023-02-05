using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace SecurityLogin.AspNetCore
{
    public class RequestContainerOptions
    {
        public string APPKeyHeader { get; set; } = "SecurityLogin.AppKey";

        public string AccessHeader { get; set; } = "SecurityLogin.Access";

        public string AuthHeader { get; set; } = "Auth";

        public Func<HttpContext,Task<bool>> IsSkip { get; set; }

        public Func<HttpContext, Task<AuthenticateResult>> SkipResult { get; set; }

        public bool NoAppLogin { get; set; } = true;

        public bool NoUserCheck { get; set; }

        public bool AppFailNoUser { get; set; }

        public string AuthenticationScheme { get; set; } = "default";
    }
}
