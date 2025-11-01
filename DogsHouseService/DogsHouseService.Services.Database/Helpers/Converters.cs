using DogsHouseService.Services.Database.Entities;
using DogsHouseService.Sevices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogsHouseService.Services.Database.Helpers
{
    /// <summary>
    /// Converters between models and entities
    /// </summary>
    public static class Converters
    {
        /// <summary>
        /// Converts DogModel to Dog entity
        /// </summary>
        /// <param name="model">The dog model.</param>
        /// <returns>The dog entity.</returns>
        public static Dog DogModelToEntity(DogModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            return new Dog
            {
                Name = model.Name,
                Color = model.Color,
                TailLength = model.TailLength,
                Weight = model.Weight,
            };
        }

        /// <summary>
        /// Converts Dog entity to DogModel
        /// </summary>
        /// <param name="entity">The dog entity.</param>"
        /// <returns>The dog model.</returns>
        public static DogModel DogEntityToModel(Dog entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            return new DogModel
            {
                Name = entity.Name,
                Color = entity.Color,
                TailLength = entity.TailLength,
                Weight = entity.Weight,
            };
        }
    }
}
