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
    }
}
