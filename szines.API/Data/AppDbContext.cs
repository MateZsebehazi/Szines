using Microsoft.EntityFrameworkCore;
using szines.API.Models;

namespace szines.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<ColorEntity> Colors => Set<ColorEntity>();
    }
}
