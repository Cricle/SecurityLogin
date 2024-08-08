using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SecurityLogin.AppLogin.Models;
using SecurityLogin.Test.AspNetCore.Models;

namespace SecurityLogin.AspNetCore
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ValueStore> ValueStores => Set<ValueStore>();

        public DbSet<AppInfo> AppInfos => Set<AppInfo>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<AppInfo>(x =>
            {
                x.HasKey(x => x.AppKey);
                x.HasData(new AppInfo
                {
                    AppKey="123",
                    AppSecret="123"
                });
            });
            base.OnModelCreating(builder);
        }
    }
}
