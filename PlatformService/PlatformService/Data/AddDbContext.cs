using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public class AddDbContext : DbContext
    {
        public AddDbContext(DbContextOptions<AddDbContext> option) : base(option)
        {
            
        }
        public DbSet<Platform> Platforms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Platform>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd(); // Это настраивает столбец ID для автогенерации
        }
    }
}
