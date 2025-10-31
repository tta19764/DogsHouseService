using DogsHouseService.Services.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace DogsHouseService.Services.Database.Data
{
    public class DogsHouseServiceDbContext(DbContextOptions<DogsHouseServiceDbContext> options) : DbContext(options)
    {
        public DbSet<Dog> Dogs { get; set; }
    }
}
