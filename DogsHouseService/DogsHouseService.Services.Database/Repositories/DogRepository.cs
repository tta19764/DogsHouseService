using DogsHouseService.Services.Database.Data;
using DogsHouseService.Services.Database.Entities;
using DogsHouseService.Services.Database.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DogsHouseService.Services.Database.Repositories
{
    /// <summary>
    /// Repository for managing Dog entities in the database.
    /// </summary>
    public class DogRepository : AbstractRepository, IDogRepository
    {
        private readonly DbSet<Dog> dbSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="DogRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public DogRepository(DogsHouseServiceDbContext context)
            : base(context)
        {
            ArgumentNullException.ThrowIfNull(context);
            this.dbSet = context.Set<Dog>();
        }

        /// <summary>
        /// Adds a new Dog entity to the database.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The added Dog entity.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the entity is null.</exception>
        public Task<Dog> AddAsync(Dog entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            if (entity.TailLength < 0)
            {
                throw new ArgumentException("Tail length cannot be negative");
            }

            return this.AddInternalAsync(entity);
        }

        /// <summary>
        /// Deletes a Dog entity from the database by its identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the dog.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the dog with the specified id does not exist.</exception>
        public async Task DeleteAsync(object id)
        {
            var exisiting = await this.dbSet.FindAsync(id);

            if (exisiting == null)
            {
                throw new KeyNotFoundException("The dog with this id does not exist in the database");
            }

            this.dbSet.Remove(exisiting);
            await this.Context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves all Dog entities from the database.
        /// </summary>
        /// <returns>A collection of all Dog entities.</returns>
        public async Task<IEnumerable<Dog>> GetAllAsync()
        {
            return await this.dbSet
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a paginated list of Dog entities from the database.
        /// </summary>
        /// <param name="pageNumber">The number of the page to retrieve.</param>
        /// <param name="pageSize">The number of entities to retrieve.</param>
        /// <returns>A collection of Dog entities for the specified page.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when pageNumber or pageSize is less than or equal to zero.</exception>
        public Task<IEnumerable<Dog>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be greater than zero");
            }

            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero");
            }

            return GetAllPaginatedInternalAsync(pageNumber, pageSize);
        }

        /// <summary>
        /// Retrieves a Dog entity by its identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the dog.</param>
        /// <returns>The Dog entity if found; otherwise, null.</returns>
        public async Task<Dog?> GetByIdAsync(object id)
        {
            return await this.dbSet.FindAsync(id);
        }

        /// <summary>
        /// Updates an existing Dog entity in the database.
        /// </summary>
        /// <param name="entity">The new state of the entity to update.</param>
        /// <returns>The updated Dog entity.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the entity is null.</exception>
        public Task<Dog> UpdateAsync(Dog entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            if (entity.TailLength < 0)
            {
                throw new ArgumentException("Tail length cannot be negative");
            }

            return UpdateInternalAsync(entity);
        }

        private async Task<Dog> AddInternalAsync(Dog entity)
        {
            if (await dbSet.AnyAsync(d => d.Name == entity.Name))
            {
                throw new InvalidOperationException("The dog with this name already in the database");
            }

            var result = await dbSet.AddAsync(entity);
            await this.Context.SaveChangesAsync();
            return result.Entity;
        }

        private async Task<IEnumerable<Dog>> GetAllPaginatedInternalAsync(int pageNumber, int pageSize)
        {
            return await this.dbSet
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        private async Task<Dog> UpdateInternalAsync(Dog entity)
        {
            var existing = await this.dbSet.FindAsync(entity.Name);
            if (existing == null)
            {
                throw new KeyNotFoundException("The dog with this name does not exist in the database");
            }

            this.Context.Entry(existing).CurrentValues.SetValues(entity);
            await this.Context.SaveChangesAsync();
            return existing;
        }
    }
}
