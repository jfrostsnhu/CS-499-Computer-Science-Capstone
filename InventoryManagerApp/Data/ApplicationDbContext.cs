using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagerApp.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema("dbo");

            builder.Entity<ApplicationUser>(entity => entity.ToTable(name: "Users"));
            builder.Entity<IdentityUserRole<string>>(entity => entity.ToTable(name: "UserRoles"));
            builder.Entity<IdentityUserLogin<string>>(entity => entity.ToTable(name: "UserLogins"));
            builder.Entity<IdentityUserClaim<string>>(entity => entity.ToTable(name: "UserClaims"));
            builder.Entity<IdentityRole>(entity => entity.ToTable(name: "Roles"));
            builder.Entity<IdentityRoleClaim<string>>(entity => entity.ToTable(name: "RoleClaims"));
            builder.Entity<IdentityUserToken<string>>(entity => entity.ToTable(name: "UserTokens"));
        }

        public DbSet<Item> Item { get; set; } = default!;
    }
}
