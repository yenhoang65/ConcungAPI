using HuongDV.models;
using Microsoft.EntityFrameworkCore;

namespace HuongDV.services
{
    public class ApplicationDbcontext : DbContext
    {
        public ApplicationDbcontext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<contact> contacts { get; set; }
        public DbSet<Product> products { get; set; }
        public DbSet<Subject> subjects { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Tienich> Tienichs { get; set; }

        public DbSet<Banner> Banners { get; set; }

    }
}
