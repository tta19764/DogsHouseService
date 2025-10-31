using DogsHouseService.Services.Database.Data;
using DogsHouseService.Services.Database.Helpers;
using DogsHouseService.Services.Database.Interfaces;
using DogsHouseService.Services.Database.Repositories;
using DogsHouseService.Sevices.Enums;
using DogsHouseService.Sevices.Interfaces.Services;
using DogsHouseService.Sevices.Models;

namespace DogsHouseService.Services.Database.Servicies
{
    /// <summary>
    /// Service for managing dog entities.
    /// </summary>
    /// <params name="repository">The dog repository.</params>
    public class DogService(IDogRepository repository) : IDogService
    {
        private readonly IDogRepository repository = repository ?? throw new ArgumentNullException(nameof(repository));

        /// <summary>
        /// Adds a new dog entity.
        /// </summary>
        /// <param name="model">The dog model to add.</param>
        /// <returns>The added dog model.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the model is null.</exception>
        public Task<DogModel> AddAsync(DogModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            if (model.TailLength < 0)
            {
                throw new ArgumentException("Tail length cannot be negative.", nameof(model));
            }

            if (model.Weight <= 0)
            {
                throw new ArgumentException("Weight must be greater than zero.", nameof(model));
            }

            return AddInternalAsync(model);
        }

        /// <summary>
        /// Deletes a dog entity by its identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the dog to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task DeleteAsync(object id)
        {
            return this.repository.DeleteAsync(id);
        }

        /// <summary>
        /// Retrieves all dog entities with optional pagination.
        /// </summary>
        /// <param name="pageNumber">The number of the page.</param>
        /// <param name="pageSize">The number of models on the retrieved page.</param>
        /// <returns>A collection of dog models.</returns>
        public Task<IEnumerable<DogModel>> GetAllAsync(int? pageNumber = null, int? pageSize = null)
        {
            if (pageNumber != null && pageSize != null)
            {
                int page = pageNumber > 0 ? pageNumber.Value : 1;
                int row = pageSize > 0 ? pageSize.Value : 1;

                return this.repository
                    .GetAllAsync(page, row)
                    .ContinueWith(task => task.Result.Select(Converters.DogEntityToModel));
            }

            return this.repository
                .GetAllAsync()
                .ContinueWith(task => task.Result.Select(Converters.DogEntityToModel));
        }

        /// <summary>
        /// Retrieves a dog entity by its identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the dog.</param>
        /// <returns>The dog model if found; otherwise, null.</returns>
        public Task<DogModel?> GetByIdAsync(object id)
        {
            return this.repository
                .GetByIdAsync(id)
                .ContinueWith(task =>
                    task.Result != null
                        ? Converters.DogEntityToModel(task.Result)
                        : null);
        }

        /// <summary>
        /// Updates an existing dog entity.
        /// </summary>
        /// <param name="model">The dog model to update.</param>
        /// <returns>The updated dog model.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the model is null.</exception>
        public Task<DogModel> UpdateAsync(DogModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            if (model.TailLength < 0)
            {
                throw new ArgumentException("Tail length cannot be negative.", nameof(model));
            }

            if (model.Weight <= 0)
            {
                throw new ArgumentException("Weight must be greater than zero.", nameof(model));
            }

            return UpdateInternalAsync(model);
        }

        public async Task<IEnumerable<DogModel>> GetAllSortedAsync(SortBy attribute = SortBy.Name, string order = "asc", int? pageNumber = null, int? pageSize = null)
        {
            var dogs = await this.GetAllAsync();

            dogs = attribute switch
            {
                SortBy.Name => order.Equals("desc"
                                        , StringComparison.CurrentCultureIgnoreCase)
                                        ? dogs.OrderByDescending(d => d.Name)
                                        : dogs.OrderBy(d => d.Name),
                SortBy.Color => order.Equals("desc"
                                        , StringComparison.CurrentCultureIgnoreCase)
                                        ? dogs.OrderByDescending(d => d.Color)
                                        : dogs.OrderBy(d => d.Color),
                SortBy.TailLength => order.Equals("desc"
                                        , StringComparison.CurrentCultureIgnoreCase)
                                        ? dogs.OrderByDescending(d => d.TailLength)
                                        : dogs.OrderBy(d => d.TailLength),
                SortBy.Weight => order.Equals("desc"
                                        , StringComparison.CurrentCultureIgnoreCase)
                                        ? dogs.OrderByDescending(d => d.Weight)
                                        : dogs.OrderBy(d => d.Weight),
                _ => throw new ArgumentOutOfRangeException(nameof(attribute), "Invalid sort attribute."),
            };
            if (pageNumber != null && pageSize != null)
            {
                int page = pageNumber > 0 ? pageNumber.Value : 1;
                int row = pageSize > 0 ? pageSize.Value : 1;
                dogs = dogs.Skip((page - 1) * row).Take(row);
            }

            return dogs;
        }

        private async Task<DogModel> AddInternalAsync(DogModel model)
        {
            var addedEntity = await repository.AddAsync(Converters.DogModelToEntity(model));
            return Converters.DogEntityToModel(addedEntity);
        }

        private async Task<DogModel> UpdateInternalAsync(DogModel model)
        {
            var updatedEntity = await repository.UpdateAsync(Converters.DogModelToEntity(model));
            return Converters.DogEntityToModel(updatedEntity);
        }
    }
}
