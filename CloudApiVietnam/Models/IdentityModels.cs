using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace CloudApiVietnam.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class User : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Formulieren> Formulieren { get; set; }
        public DbSet<FormContent> FormContent { get; set; }
        public DbSet<Image> Image { get; set; }


        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        } 
        //Overide de standaard db structuur


        protected override void OnModelCreating(DbModelBuilder modelBuilder) 
        { 
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<User>().ToTable("Users").Ignore(p => p.PhoneNumber);
            modelBuilder.Entity<User>().ToTable("Users").Ignore(p => p.PhoneNumberConfirmed);
            modelBuilder.Entity<User>().ToTable("Users").Ignore(p => p.TwoFactorEnabled);
            modelBuilder.Entity<User>().ToTable("Users").Ignore(p => p.EmailConfirmed);
            modelBuilder.Entity<User>().ToTable("Users").Ignore(p => p.LockoutEnabled);
            modelBuilder.Entity<User>().ToTable("Users").Ignore(p => p.LockoutEndDateUtc);
            modelBuilder.Entity<User>().ToTable("Users").Ignore(p => p.AccessFailedCount);  
        }

    }
}