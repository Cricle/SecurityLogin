using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using SecurityLogin.AccessSession;
using System;
using System.Threading.Tasks;

namespace SecurityLogin.AspNetCore
{
    public class RequestContainerOptions<TUserSnapshot>
    {
        public string APPKeyHeader { get; set; } = SecurityLoginConsts.DefaultAPPKeyHeader;

        public string AccessHeader { get; set; } = SecurityLoginConsts.DefaultAPPAccessHeader;

        public string AuthHeader { get; set; } = SecurityLoginConsts.DefaultAuthHeader;

        public Func<HttpContext?,Task<bool>>? IsSkip { get; set; }

        public Func<HttpContext?, Task<AuthenticateResult>>? SkipResult { get; set; }

        public Func<HttpContext, UserStatusContainer<TUserSnapshot>, AuthenticationTicket, Task>? Succeed { get; set; }

        public Func<HttpContext?, UserStatusContainer<TUserSnapshot>, UserStatusFailTypes, Task>? Failed { get; set; }

        public bool NoAppLogin { get; set; } = true;

        public bool NoUserCheck { get; set; }

        public bool AppFailNoUser { get; set; }

        public string AuthenticationScheme { get; set; } = SecurityLoginConsts.AuthenticationScheme;
    }
}
