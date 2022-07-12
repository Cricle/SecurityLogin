using SecurityLogin.AppLogin.Models;
using System;
using System.Threading.Tasks;

namespace SecurityLogin.AppLogin
{
    public class DefaultAppService : DefaultAppService<IAppInfo, IAppInfoSnapshot, AppLoginResult>
    {
        public DefaultAppService(IAppServiceProvider provider, IAppInfoSnapshotProvider<IAppInfoSnapshot> snapshotProvider) : base(provider, snapshotProvider)
        {
        }
    }
    public class DefaultAppService<TAppInfo, TAppInfoSnapshot, TAppLoginResult> : AppService<TAppInfo, TAppInfoSnapshot, TAppLoginResult>
        where TAppInfo : IAppInfo
        where TAppInfoSnapshot : IAppInfoSnapshot
        where TAppLoginResult : AppLoginResult, new()
    {
        public DefaultAppService(IAppServiceProvider provider, IAppInfoSnapshotProvider<TAppInfoSnapshot> snapshotProvider)
            : base(provider)
        {
            SnapshotProvider = snapshotProvider ?? throw new ArgumentNullException(nameof(snapshotProvider));
        }

        public IAppInfoSnapshotProvider<TAppInfoSnapshot> SnapshotProvider { get; }

        protected override Task<TAppInfoSnapshot> GetAppInfoSnapshotAsync(string appKey)
        {
            return SnapshotProvider.GetAppInfoSnapshotAsync(appKey);
        }
    }
}
