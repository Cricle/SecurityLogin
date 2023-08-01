using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SecurityLogin.AccessSession;
using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
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

        public CrossAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock, 
            IRequestContainerConverter<UserStatusContainer<TUserSnapshot>> requestContainerConverter,
            RequestContainerOptions<TUserSnapshot> requestContainerOptions)
            : base(options, logger, encoder, clock,requestContainerConverter)
        {
            RequestContainerOptions = requestContainerOptions;
        }

        protected override async Task<AuthenticateResult> CheckAsync()
        {
            Throws.ThrowHttpContextIsNull(Context);
            var res = await RequestContainerConverter.ConvertAsync(Context!);
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
                succeedTicket=await RequestContainerOptions.Succeed!.Invoke(Context!, res, succeedTicket);
            }
            Context!.Features.Set(res);
            if (res.UserSnapshot != null)
            {
                Context.Features.Set(res.UserSnapshot);
            }
            if (res.AppSnapshot != null)
            {
                Context.Features.Set(res.AppSnapshot);
            }
            return AuthenticateResult.Success(succeedTicket);            
        }
        protected async Task RunFailOptionsAsync(UserStatusContainer<TUserSnapshot> container,UserStatusFailTypes type)
        {
            var succeed = RequestContainerOptions.Failed;
            if (succeed != null)
            {
                await RequestContainerOptions.Failed!.Invoke(Context, container, type);
            }
        }
        protected virtual Task<AuthenticationTicket> SucceedAsync(UserStatusContainer<TUserSnapshot> container,RequestContainerOptions<TUserSnapshot> options)
        {
            if (options.SucceedDoNothing)
            {
                return Task.FromResult(new AuthenticationTicket(new ClaimsPrincipal(), options.AuthenticationScheme));
            }
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
            return val.SkipResult(Context);
        }

        protected override Task<bool> IsSkipAuthAsync()
        {
            var val = RequestContainerOptions;
            if (val.IsSkip == null)
            {
                return falseTask;
            }
            return val.IsSkip(Context);
        }
    }
}
