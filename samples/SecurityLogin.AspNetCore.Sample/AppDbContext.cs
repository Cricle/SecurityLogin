using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SecurityLogin.Test.AspNetCore.Models;

namespace SecurityLogin.AspNetCore
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ValueStore> ValueStores => Set<ValueStore>();
    }
}
