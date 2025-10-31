using DogsHouseService.Services.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace DogsHouseService.Services.Database.Data
{
    public class DogsHouseServiceDbContext : DbContext
    {
        public DogsHouseServiceDbContext(DbContextOptions<DogsHouseServiceDbContext> options)
            : base(options)
        {
        }

        public DbSet<Dog> Dogs { get; set; }
    }
}
