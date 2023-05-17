using Microsoft.AspNetCore.Authentication;
using SecurityLogin.AccessSession;
using System;
using System.Security.Claims;
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
    [Flags]
    public enum UserStatusFailTypes
    {
        NoAppLogin,
        NoUserSnapshot
    }
    public class CrossAuthenticationHandler<TUserSnapshot> : CrossAuthenticationHandlerBase<UserStatusContainer<TUserSnapshot>>
    {
        private static readonly Task<bool> falseTask = Task.FromResult(false);

        public RequestContainerOptions<TUserSnapshot> RequestContainerOptions { get; }

        public CrossAuthenticationHandler(IRequestContainerConverter<UserStatusContainer<TUserSnapshot>> requestContainerConverter,
            RequestContainerOptions<TUserSnapshot> requestContainerOptions)
            : base(requestContainerConverter)
        {
            RequestContainerOptions = requestContainerOptions;
        }

        protected override async Task<AuthenticateResult> CheckAsync()
        {
            Throws.ThrowHttpContextIsNull(HttpContext);
            var res = await RequestContainerConverter.ConvertAsync(HttpContext!);
            var val = RequestContainerOptions;
            if (!val.NoAppLogin)
            {
                if (!res.HasAppSnapshot)
                {
                    await RunFailOptionsAsync(res, UserStatusFailTypes.NoAppLogin);
                    return await FailAsync(UserStatusFailTypes.NoAppLogin);
                }
            }
            if (!val.NoUserCheck)
            {
                if (!res.HasUserSnapshot)
                {
                    await RunFailOptionsAsync(res,UserStatusFailTypes.NoUserSnapshot);
                    return await FailAsync(UserStatusFailTypes.NoUserSnapshot);
                }
            }
            var succeedTicket = await SucceedAsync(res, val);
            var succeed = RequestContainerOptions.Succeed;
            if (succeed != null)
            {
                await RequestContainerOptions.Succeed!.Invoke(HttpContext!, res, succeedTicket);
            }
            HttpContext!.Features.Set(res);
            if (res.UserSnapshot != null)
            {
                HttpContext.Features.Set(res.UserSnapshot);
            }
            if (res.AppSnapshot != null)
            {
                HttpContext.Features.Set(res.AppSnapshot);
            }
            return AuthenticateResult.Success(succeedTicket);            
        }
        protected async Task RunFailOptionsAsync(UserStatusContainer<TUserSnapshot> container,UserStatusFailTypes type)
        {
            var succeed = RequestContainerOptions.Failed;
            if (succeed != null)
            {
                await RequestContainerOptions.Failed!.Invoke(HttpContext, container, type);
            }
        }
        protected virtual Task<AuthenticationTicket> SucceedAsync(UserStatusContainer<TUserSnapshot> container,RequestContainerOptions<TUserSnapshot> options)
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
                return falseTask;
            }
            return val.IsSkip(HttpContext);
        }
    }
}
