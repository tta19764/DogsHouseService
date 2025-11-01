using DogsHouseService.Sevices.Models;
using DogsHouseService.WebApi.Models.Dtos.Create;
using DogsHouseService.WebApi.Models.Dtos.Read;
using DogsHouseService.WebApi.Models.Dtos.Update;

namespace DogsHouseService.WebApi.Helpers
{

    /// <summary>
    /// Converters from DTOs to Models
    /// </summary>
    public static class DtoToModel
    {
        /// <summary>
        /// Converts CreateDogDto to DogModel
        /// </summary>
        /// <param name="dogDto">The create dog dto to convert.</param>
        /// <returns>The dog model.</returns>
        public static DogModel FromCreateDogDtoToModel(CreateDogDto dogDto)
        {
            ArgumentNullException.ThrowIfNull(dogDto);

            return new DogModel
            {
                Name = dogDto.Name,
                Color = dogDto.Color,
                TailLength = dogDto.TailLength,
                Weight = dogDto.Weight,
            };
        }

        /// <summary>
        /// Converts UpdateDogDto to DogModel
        /// </summary>
        /// <param name="dogDto">The update dog dto to convert.</param>
        /// <returns>The dog model.</returns>
        public static DogModel FromUpdateDogDtoToModel(UpdateDogDto dogDto)
        {
            ArgumentNullException.ThrowIfNull(dogDto);

            return new DogModel
            {
                Name = dogDto.Name,
                Color = dogDto.Color,
                TailLength = dogDto.TailLength,
                Weight = dogDto.Weight,
            };
        }
    }
}
