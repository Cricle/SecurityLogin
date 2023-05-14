using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using SecurityLogin.AccessSession;
using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SecurityLogin.AspNetCore
{
    public static class SecurityLoginConsts
    {
        public const string AuthenticationScheme = "se-defaute";
        public const string DefaultAuthHeader = "Authorization";
        public const string DefaultAPPKeyHeader = "SecurityLogin.AppKey";
        public const string DefaultAPPAccessHeader = "SecurityLogin.Access";
    }
    internal static class TaskCache
    {
        public static readonly Task<bool> falseTask = Task.FromResult(false);
    }
    [Flags]
    public enum UserStatusFailTypes
    {
        NoAppLogin,
        NoUserSnapshot
    }
    public class CrossAuthenticationHandler<TUserSnapshot> : CrossAuthenticationHandlerBase<UserStatusContainer<TUserSnapshot>>
        where TUserSnapshot : UserSnapshot
    {
        public RequestContainerOptions RequestContainerOptions { get; }

        public CrossAuthenticationHandler(IRequestContainerConverter<UserStatusContainer<TUserSnapshot>> requestContainerConverter,
            RequestContainerOptions requestContainerOptions)
            : base(requestContainerConverter)
        {
            RequestContainerOptions = requestContainerOptions;
        }

        protected override async Task<AuthenticateResult> CheckAsync()
        {
            var res = await RequestContainerConverter.ConvertAsync(HttpContext);
            var val = RequestContainerOptions;
            if (!val.NoAppLogin)
            {
                if (!res.HasAppSnapshot)
                {
                    return await FailAsync(UserStatusFailTypes.NoAppLogin);
                }
            }
            if (!val.NoUserCheck)
            {
                if (!res.HasUserSnapshot)
                {
                    return await FailAsync(UserStatusFailTypes.NoUserSnapshot);
                }
            }
            var succeedTicket = await SucceedAsync(res, val);
            return AuthenticateResult.Success(succeedTicket);
        }
        protected virtual Task<AuthenticationTicket> SucceedAsync(UserStatusContainer<TUserSnapshot> container,RequestContainerOptions options)
        {
            return Task.FromResult(new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity[]
            {
                new ClaimsIdentity(Array.Empty<Claim>(),options.AuthenticationScheme)
            }), options.AuthenticationScheme));
        }
        protected virtual Task<AuthenticateResult> FailAsync(UserStatusFailTypes type)
        {
            return Task.FromResult(AuthenticateResult.Fail(type.ToString()));
        }

        protected override Task<AuthenticateResult> SkipAsync()
        {
            var val = RequestContainerOptions;
            if (val.SkipResult == null)
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }
            return val.SkipResult(HttpContext);
        }

        protected override Task<bool> IsSkipAuthAsync()
        {
            var val = RequestContainerOptions;
            if (val.IsSkip == null)
            {
                return TaskCache.falseTask;
            }
            return val.IsSkip(HttpContext);
        }
    }
}
