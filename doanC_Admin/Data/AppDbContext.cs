using Microsoft.EntityFrameworkCore;
using doanC_Admin.Models;

namespace doanC_Admin.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<LocationPoint> LocationPoints { get; set; }
    }
}