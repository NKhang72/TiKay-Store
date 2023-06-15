using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace TiKayStore.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        public DbSet<tb_Advertisement> Advertisements { get; set; }
        public DbSet<tb_Deal> tb_Deal { get; set; }
        public DbSet<tb_Header> tb_Headers { get; set; }
        public DbSet<tb_Menu>   tb_Menus { get; set; }
        public DbSet<tb_News> tb_News { get; set; }
        public DbSet<tb_Order> tb_Orders { get; set; }
        public DbSet<tb_OrderDetail> tb_OrderDetails { get; set; }
        public DbSet<tb_Product> tb_Products { get; set; }
        public DbSet<tb_ProductCategory> tb_ProductCategories { get; set; }
        public DbSet<tb_ProductImage> tb_ProductImages { get; set; }
        public DbSet<tb_User> tb_Users { get; set; }


        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}