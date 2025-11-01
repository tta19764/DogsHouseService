using DogsHouseService.Services.Database.Entities;
using DogsHouseService.Sevices.Enums;
using DogsHouseService.Sevices.Interfaces.Services;
using DogsHouseService.Sevices.Models;
using DogsHouseService.WebApi.Helpers;
using DogsHouseService.WebApi.Logging;
using DogsHouseService.WebApi.Models.Dtos.Create;
using DogsHouseService.WebApi.Models.Dtos.Read;
using DogsHouseService.WebApi.Models.Dtos.Update;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DogsHouseService.WebApi.Controllers
{

    /// <summary>
    /// Controller for managing dogs in the Dogs House Service.
    /// </summary>
    /// <param name="dogService">The service for managing dogs.</param>
    /// <param name="logger">The logger for the controller.</param>
    [Route("")]
    [ApiController]
    public class DogsController(IDogService dogService, ILogger<DogsController> logger) : ControllerBase
    {
        private readonly IDogService dogService = dogService ?? throw new ArgumentNullException(nameof(dogService));
        private readonly ILogger<DogsController> logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <summary>
        /// Retrieves a list of dogs with optional sorting and pagination.
        /// </summary>
        /// <param name="attribute">The attribute for sorting.</param>
        /// <param name="order">The sorting order.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>A list of dog DTOs.</returns>
        [HttpGet("dogs")]
        public async Task<ActionResult<IEnumerable<DogDto>>> GetDogs(
        [FromQuery] SortBy attribute = SortBy.Name,
        [FromQuery] string order = "asc",
        [FromQuery] int? pageNumber = null,
        [FromQuery] int? pageSize = null)
        {
            logger.RetrievingDogs();
            var dogs = await dogService.GetAllSortedAsync(attribute, order, pageNumber, pageSize);
            return Ok(dogs.Select(ModelToDto.ToDogDto));
        }

        /// <summary>
        /// Retrieves a specific dog by name.
        /// </summary>
        /// <param name="name">The dogs name.</param>
        /// <returns>The dog DTO.</returns>
        [HttpGet("dogs/{name}")]
        public async Task<ActionResult<DogDto>> GetDog(string name)
        {
            var dog = await dogService.GetByIdAsync(name);
            if (dog == null)
            {
                logger.DogRetrievalFailed(name, "not found");
                return NotFound($"Dog with name '{name}' not found.");
            }

            logger.DogRetrieved(name);
            return Ok(ModelToDto.ToDogDto(dog));
        }

        /// <summary>
        /// Creates a new dog.
        /// </summary>
        /// <param name="dogDto">The new dog.</param>
        /// <returns>The created dog DTO.</returns>
        [HttpPost("dog")]
        public async Task<ActionResult<DogDto>> CreateDog([FromBody] CreateDogDto dogDto)
        {
            if (dogDto == null)
            {
                return BadRequest("Dog data is required.");
            }

            try
            {
                var created = await dogService.AddAsync(DtoToModel.FromCreateDogDtoToModel(dogDto));
                logger.DogCreated(created.Name);
                return Ok(ModelToDto.ToDogDto(created));
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
            {
                logger.DogCreationFailed(dogDto.Name, ex.Message);
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing dog.
        /// </summary>
        /// <param name="dogDto">The updated dog.</param>
        /// <returns>The updated dog DTO.</returns>
        [HttpPost("dog/update")]
        public async Task<ActionResult<DogDto>> UpdateDog([FromBody] UpdateDogDto dogDto)
        {
            if (dogDto == null)
            {
                return BadRequest("Dog data is required.");
            }

            try
            {
                var updated = await dogService.UpdateAsync(DtoToModel.FromUpdateDogDtoToModel(dogDto));

                logger.DogUpdated(updated.Name);

                return Ok(ModelToDto.ToDogDto(updated));
            }
            catch (Exception ex) when (ex is ArgumentException or KeyNotFoundException)
            {
                logger.DogUpdateFailed(dogDto.Name, ex.Message);
                return NotFound(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a dog by name.
        /// </summary>
        /// <param name="name">The name of the dog to delete.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpDelete("dog/{name}")]
        public async Task<IActionResult> DeleteDog(string name)
        {
            try
            {
                await dogService.DeleteAsync(name);
                logger.DogDeleted(name);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                logger.DogDeletionFailed(name, ex.Message);
                return NotFound(new { error = ex.Message });
            }
        }
    }
}
