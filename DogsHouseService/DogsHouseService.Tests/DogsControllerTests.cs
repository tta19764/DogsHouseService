using DogsHouseService.Services.Database.Entities;
using DogsHouseService.Sevices.Enums;
using DogsHouseService.Sevices.Interfaces.Services;
using DogsHouseService.Sevices.Models;
using DogsHouseService.WebApi.Controllers;
using DogsHouseService.WebApi.Models.Dtos.Create;
using DogsHouseService.WebApi.Models.Dtos.Read;
using DogsHouseService.WebApi.Models.Dtos.Update;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DogsHouseService.Tests
{
    public class DogsControllerTests
    {
        private readonly Mock<IDogService> dogServiceMock;
        private readonly Mock<ILogger<DogsController>> loggerMock;
        private readonly DogsController controller;

        public DogsControllerTests()
        {
            dogServiceMock = new Mock<IDogService>();
            loggerMock = new Mock<ILogger<DogsController>>();
            controller = new DogsController(dogServiceMock.Object, loggerMock.Object);
        }

        [Fact]
        public async Task GetDogs_ShouldReturnOkWithDogs_WhenServiceReturnsDogs()
        {
            // Arrange
            var dogs = new[]
            {
                new DogModel { Name = "Neo", Color = "red&amber", TailLength = 22, Weight = 32 },
                new DogModel { Name = "Jessy", Color = "black&white", TailLength = 7, Weight = 14 }
            };

            dogServiceMock.Setup(s => s.GetAllSortedAsync(
                It.IsAny<SortBy>(),
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<int?>()
            )).ReturnsAsync(dogs);

            // Act
            var result = await controller.GetDogs();

            // Assert
            var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
            var data = okResult.Value.ShouldBeAssignableTo<IEnumerable<DogDto>>()!.ToList();
            data.Count.ShouldBe(2);
            data[0].Name.ShouldBe("Neo");
            data[1].Name.ShouldBe("Jessy");
        }

        [Fact]
        public async Task GetDogs_ShouldReturnEmptyList_WhenNoDogsExist()
        {
            // Arrange
            dogServiceMock.Setup(s => s.GetAllSortedAsync(
                It.IsAny<SortBy>(),
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<int?>()
            )).ReturnsAsync([]);

            // Act
            var result = await controller.GetDogs();

            // Assert
            var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
            var data = okResult.Value.ShouldBeAssignableTo<IEnumerable<DogDto>>();
            data.ShouldBeEmpty("If no dogs exist, API should return empty array, not null or error.");
        }

        [Fact]
        public async Task GetDog_ShouldReturnOk_WhenDogFound()
        {
            // Arrange
            var dog = new DogModel() { Name = "Neo", Color = "red & amber", TailLength = 22, Weight = 32 };
            dogServiceMock.Setup(s => s.GetByIdAsync("Neo"))
                .ReturnsAsync(dog);

            // Act
            var result = await controller.GetDog("Neo");

            // Assert
            var ok = result.Result.ShouldBeOfType<OkObjectResult>();
            var dto = ok.Value.ShouldBeOfType<DogDto>();
            dto.Name.ShouldBe("Neo");
        }

        [Fact]
        public async Task GetDog_ShouldReturnNotFound_WhenDogDoesNotExist()
        {
            // Arrange
            dogServiceMock.Setup(s => s.GetByIdAsync("Ghost")).ReturnsAsync((DogModel?)null);

            // Act
            var result = await controller.GetDog("Ghost");

            // Assert
            result.Result.ShouldBeOfType<NotFoundObjectResult>("API should return 404 if dog not found.");
        }

        [Fact]
        public async Task CreateDog_ShouldReturnCreatedDog_WhenValid()
        {
            // Arrange
            var createDto = new CreateDogDto { Name = "Doggy", Color = "red", TailLength = 173, Weight = 33 };
            var created = new DogDto { Name = "Doggy", Color = "red", TailLength = 173, Weight = 33 };

            dogServiceMock.Setup(s => s.AddAsync(It.IsAny<DogModel>()))
                .ReturnsAsync(new DogModel { Name = created.Name, Color = created.Color, TailLength = created.TailLength, Weight = created.Weight });

            // Act
            var result = await controller.CreateDog(createDto);

            // Assert
            var ok = result.Result.ShouldBeOfType<OkObjectResult>();
            var dto = ok.Value.ShouldBeOfType<DogDto>();
            dto.Name.ShouldBe("Doggy");
        }

        [Fact]
        public async Task CreateDog_ShouldReturnBadRequest_WhenDogAlreadyExists()
        {
            // Arrange
            var createDto = new CreateDogDto { Name = "Doggy", Color = "red", TailLength = 10, Weight = 20 };

            dogServiceMock.Setup(s => s.AddAsync(It.IsAny<DogModel>()))
                .ThrowsAsync(new InvalidOperationException("The dog with this name already in the database"));

            // Act
            var result = await controller.CreateDog(createDto);

            // Assert
            result.Result.ShouldBeOfType<BadRequestObjectResult>("Should return 400 when duplicate name error occurs.");
        }

        [Fact]
        public async Task CreateDog_ShouldReturnBadRequest_WhenInvalidJson()
        {
            // Simulated by passing null DTO
            var result = await controller.CreateDog(null!);
            result.Result.ShouldBeOfType<BadRequestObjectResult>("Null body (invalid JSON) should produce BadRequest.");
        }

        [Fact]
        public async Task UpdateDog_ShouldReturnUpdatedDog_WhenValid()
        {
            // Arrange
            var updateDto = new UpdateDogDto { Name = "Neo", Color = "red", TailLength = 25, Weight = 35 };
            var updated = new DogDto { Name = "Neo", Color = "red", TailLength = 25, Weight = 35 };

            dogServiceMock.Setup(s => s.UpdateAsync(It.IsAny<DogModel>()))
                .ReturnsAsync(new DogModel { Name = updated.Name, Color = updated.Color, TailLength = updated.TailLength, Weight = updated.Weight });

            // Act
            var result = await controller.UpdateDog(updateDto);

            // Assert
            var ok = result.Result.ShouldBeOfType<OkObjectResult>();
            var dto = ok.Value.ShouldBeOfType<DogDto>();
            dto.Weight.ShouldBe(35);
        }

        [Fact]
        public async Task UpdateDog_ShouldReturnNotFound_WhenDogDoesNotExist()
        {
            // Arrange
            var updateDto = new UpdateDogDto { Name = "Ghost", Color = "white", TailLength = 10, Weight = 20 };

            dogServiceMock.Setup(s => s.UpdateAsync(It.IsAny<DogModel>()))
                .ThrowsAsync(new KeyNotFoundException("Not found"));

            // Act
            var result = await controller.UpdateDog(updateDto);

            // Assert
            result.Result.ShouldBeOfType<NotFoundObjectResult>("Updating a non-existing dog must return 404.");
        }

        [Fact]
        public async Task DeleteDog_ShouldReturnOk_WhenDeleted()
        {
            // Arrange
            dogServiceMock.Setup(s => s.DeleteAsync("Neo")).Returns(Task.CompletedTask);

            // Act
            var result = await controller.DeleteDog("Neo");

            // Assert
            result.ShouldBeOfType<OkResult>("Successful delete should return HTTP 200 OK.");
        }

        [Fact]
        public async Task DeleteDog_ShouldReturnNotFound_WhenDogMissing()
        {
            // Arrange
            dogServiceMock.Setup(s => s.DeleteAsync("Ghost"))
                .ThrowsAsync(new KeyNotFoundException("Not found"));

            // Act
            var result = await controller.DeleteDog("Ghost");

            // Assert
            result.ShouldBeOfType<NotFoundObjectResult>("Deleting missing dog must return 404.");
        }
    }
}
