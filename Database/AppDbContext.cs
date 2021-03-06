using ABM_CMS.Models.Token;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ABM_CMS.Database
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        //Roles for app
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>().HasData(
                new {Id = "1", Name = "Admin", NormalizedName = "ADMIN"},
                new {Id = "2", Name = "User", NormalizedName = "USER"},
                new {Id = "3", Name = "Moderator", NormalizedName = "MODERATOR"}
            );

           // builder.Entity<TokenModel>().HasOne(o => o.User).WithMany(m => m.Tokens).HasForeignKey(k => k.UserId);
        }
        
        public DbSet<RefreshTokenModel> RefreshTokens { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }
    }
}