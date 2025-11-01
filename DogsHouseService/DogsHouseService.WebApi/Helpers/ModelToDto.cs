using DogsHouseService.Sevices.Models;
using DogsHouseService.WebApi.Models.Dtos.Read;

namespace DogsHouseService.WebApi.Helpers
{

    /// <summary>
    /// Converters from Models to DTOs
    /// </summary>
    public static class ModelToDto
    {
        /// <summary>
        /// Converts DogModel to DogDto
        /// </summary>
        /// <param name="model">The dog model to convert.</param>
        /// <returns>The dog dto.</returns>
        public static DogDto ToDogDto(DogModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            return new DogDto
            {
                Name = model.Name,
                Color = model.Color,
                TailLength = model.TailLength,
                Weight = model.Weight,
            };
        }
    }
}
