using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SecurityLogin.AspNetCore
{
    public abstract class CrossAuthenticationHandlerBase<TRequestContainer>: AuthenticationHandler<AuthenticationSchemeOptions>
    {
        protected CrossAuthenticationHandlerBase(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, 
            UrlEncoder encoder,
#if !NET8_0_OR_GREATER
            ISystemClock clock,
#endif
            IRequestContainerConverter<TRequestContainer> requestContainerConverter)
            :base(options,logger,encoder
#if !NET8_0_OR_GREATER
                 ,clock
#endif
                 )
        {
            RequestContainerConverter = requestContainerConverter ?? throw new ArgumentNullException(nameof(requestContainerConverter));
        }

        public IRequestContainerConverter<TRequestContainer> RequestContainerConverter { get; }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            Throws.ThrowHttpContextIsNull(Context);
            if (!await IsSkipAuthAsync())
            {
                var container = await RequestContainerConverter.ConvertAsync(Context);
                try
                {
                    return await CheckAsync();
                }
                finally
                {
                    if (container is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }
            return await SkipAsync();
        }
        protected abstract Task<AuthenticateResult> CheckAsync();

        protected abstract Task<AuthenticateResult> SkipAsync();

        protected abstract Task<bool> IsSkipAuthAsync();
    }
}
