using DogsHouseService.Services.Database.Entities;
using DogsHouseService.Sevices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogsHouseService.Services.Database.Helpers
{
    public static class Converters
    {
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
