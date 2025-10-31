using DogsHouseService.Services.Database.Entities;
using DogsHouseService.Services.Database.Interfaces;
using DogsHouseService.Services.Database.Repositories;
using DogsHouseService.Services.Database.Servicies;
using DogsHouseService.Sevices.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace DogsHouseService.Tests
{
    public class DogServiceTests
    {
        private static DogModel CreateModel(string name = "Rex", int tail = 10, int weight = 20)
            => new()
            { Name = name, Color = "Brown", TailLength = tail, Weight = weight };

        private static Dog CreateEntity(string name = "Rex", int tail = 10, int weight = 20)
            => new()
            { Name = name, Color = "Brown", TailLength = tail, Weight = weight };

        [Fact]
        public async Task AddAsync_ShouldCallRepositoryAdd()
        {
            // Arrange
            var repoMock = new Mock<IDogRepository>();
            var model = CreateModel();
            var entity = CreateEntity();

            repoMock.Setup(r => r.AddAsync(It.IsAny<Dog>()))
                .ReturnsAsync(entity);

            var service = new DogService(repoMock.Object);

            // Act
            var result = await service.AddAsync(model);

            // Assert
            repoMock.Verify(r => r.AddAsync(It.Is<Dog>(d => d.Name == entity.Name)), Times.Once);
            result.ShouldNotBeNull("The service must return a valid result.");
            result.Name.ShouldBe(model.Name, $"The created entity must have the property name be ${model.Name}");
        }

        [Fact]
        public async Task AddAsync_ShouldThrow_WhenRepositoryThrows()
        {
            // Arrange
            var repoMock = new Mock<IDogRepository>();
            repoMock.Setup(r => r.AddAsync(It.IsAny<Dog>()))
                .ThrowsAsync(new InvalidOperationException("The dog with this name already in the database"));
            var errorMessage = "The dog with this name already in the database";

            var service = new DogService(repoMock.Object);
            var model = CreateModel();

            // Act & Assert
            var ex = await Should.ThrowAsync<InvalidOperationException>(() => service.AddAsync(model), "The service must throw InvalidOperationException.");
            ex.Message.ShouldContain(errorMessage, customMessage: $"The error message must contain: \"{errorMessage}\"");
        }

        [Fact]
        public async Task AddAsync_ShouldThrow_WhenTailIsNegative()
        {
            // Arrange
            var repoMock = new Mock<IDogRepository>();
            var service = new DogService(repoMock.Object);
            var model = CreateModel(tail: -5);
            var errorMessage = "Tail length cannot be negative";

            // Act & Assert
            var ex = await Should.ThrowAsync<ArgumentException>(() => service.AddAsync(model), "The service must throw ArgumentException");
            ex.Message.ShouldContain(errorMessage, customMessage: $"The error message must contain: \"{errorMessage}\"");
        }

        [Fact]
        public async Task AddAsync_ShouldThrow_WhenWeightIsLessThanOrEqualsZero()
        {
            // Arrange
            var repoMock = new Mock<IDogRepository>();
            var service = new DogService(repoMock.Object);
            var model = CreateModel(weight: -5);
            var errorMessage = "Weight must be greater than zero";

            // Act & Assert
            var ex = await Should.ThrowAsync<ArgumentException>(() => service.AddAsync(model), "The service must throw ArgumentException");
            ex.Message.ShouldContain(errorMessage, customMessage: $"The error message must contain: \"{errorMessage}\"");
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnMappedModels()
        {
            // Arrange
            var repoMock = new Mock<IDogRepository>();
            var dogEntities = new[] { CreateEntity("A"), CreateEntity("B") };
            var expectedCount = dogEntities.Length;
            repoMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(dogEntities);

            var service = new DogService(repoMock.Object);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            result.Count().ShouldBe(expectedCount, $"The service must return {expectedCount} models.");
            result.First().ShouldBeOfType<DogModel>("The returned result must be a collection of DogModel.");
            result.Select(d => d.Name).ShouldBe(dogEntities.Select(d => d.Name), customMessage: "The names of the returned models must match.");
        }

        [Fact]
        public async Task GetAllAsync_WithPagination_ShouldCallRepositoryWithParams()
        {
            // Arrange
            var repoMock = new Mock<IDogRepository>();
            var dogEntities = new[] { CreateEntity("X"), CreateEntity("Y") };
            int expectedCount = dogEntities.Length, pageNumber = 2, pageSize = 5;
            repoMock.Setup(r => r.GetAllAsync(pageNumber, pageSize))
                .ReturnsAsync(dogEntities);

            var service = new DogService(repoMock.Object);

            // Act
            var result = await service.GetAllAsync(pageNumber, pageSize);

            // Assert
            repoMock.Verify(r => r.GetAllAsync(pageNumber, pageSize), Times.Once, "The get all async method must be called once.");
            result.Count().ShouldBe(expectedCount, $"The result shold contain {expectedCount} models.");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnDogModel_WhenFound()
        {
            // Arrange
            var repoMock = new Mock<IDogRepository>();
            var dogEntity = CreateEntity("Rex");
            repoMock.Setup(r => r.GetByIdAsync(dogEntity.Name))
                .ReturnsAsync(dogEntity);

            var service = new DogService(repoMock.Object);

            // Act
            var result = await service.GetByIdAsync(dogEntity.Name);

            // Assert
            result.ShouldNotBeNull("The service must return a valid result.");
            result!.Name.ShouldBe(dogEntity.Name, $"The returned model must have name {dogEntity.Name}");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            var repoMock = new Mock<IDogRepository>();
            repoMock.Setup(r => r.GetByIdAsync("NoDog"))
                .ReturnsAsync((Dog?)null);

            var service = new DogService(repoMock.Object);

            // Act
            var result = await service.GetByIdAsync("NoDog");

            // Assert
            result.ShouldBeNull("The result must be null.");
        }

        [Fact]
        public async Task UpdateAsync_ShouldCallRepositoryUpdate()
        {
            // Arrange
            var repoMock = new Mock<IDogRepository>();
            var model = CreateModel("Bolt", 15, 25);
            var entity = CreateEntity("Bolt", 15, 25);

            repoMock.Setup(r => r.UpdateAsync(It.IsAny<Dog>()))
                .ReturnsAsync(entity);

            var service = new DogService(repoMock.Object);

            // Act
            var result = await service.UpdateAsync(model);

            // Assert
            repoMock.Verify(r => r.UpdateAsync(It.Is<Dog>(d => d.Name == entity.Name)), Times.Once, "The method must be called once.");
            result.Weight.ShouldBe(model.Weight, $"The updatd model must have the weight {model.Weight}.");
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenRepositoryThrows()
        {
            // Arrange
            var repoMock = new Mock<IDogRepository>();
            var errorMessage = "Not found";
            repoMock.Setup(r => r.UpdateAsync(It.IsAny<Dog>()))
                .ThrowsAsync(new KeyNotFoundException(errorMessage));

            var service = new DogService(repoMock.Object);
            // Act
            var model = CreateModel("Ghost");

            // Assert
            var ex = await Should.ThrowAsync<KeyNotFoundException>(() => service.UpdateAsync(model), "The service must throw KeyNotFoundException.");
            ex.Message.ShouldContain(errorMessage, customMessage: $"The error message must include {errorMessage}.");
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenTailIsNegative()
        {
            // Arrange
            var repoMock = new Mock<IDogRepository>();
            var service = new DogService(repoMock.Object);
            var model = CreateModel(tail: -5);
            var errorMessage = "Tail length cannot be negative";

            // Act & Assert
            var ex = await Should.ThrowAsync<ArgumentException>(() => service.UpdateAsync(model), "The service must throw ArgumentException");
            ex.Message.ShouldContain(errorMessage, customMessage: $"The error message must contain: \"{errorMessage}\"");
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenWeightIsLessThanOrEqualsZero()
        {
            // Arrange
            var repoMock = new Mock<IDogRepository>();
            var service = new DogService(repoMock.Object);
            var model = CreateModel(weight: -5);
            var errorMessage = "Weight must be greater than zero";

            // Act & Assert
            var ex = await Should.ThrowAsync<ArgumentException>(() => service.UpdateAsync(model), "The service must throw ArgumentException");
            ex.Message.ShouldContain(errorMessage, customMessage: $"The error message must contain: \"{errorMessage}\"");
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallRepositoryDelete()
        {
            // Arrange
            var repoMock = new Mock<IDogRepository>();
            var service = new DogService(repoMock.Object);

            // Act
            await service.DeleteAsync("Rex");

            // Assert
            repoMock.Verify(r => r.DeleteAsync("Rex"), Times.Once, "The method must be called once.");
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrow_WhenRepositoryThrows()
        {
            // Arrange
            var repoMock = new Mock<IDogRepository>();
            var errorMessage = "Not found";
            repoMock.Setup(r => r.DeleteAsync("X"))
                .ThrowsAsync(new KeyNotFoundException(errorMessage));

            var service = new DogService(repoMock.Object);

            // Act & Assert
            var ex = await Should.ThrowAsync<KeyNotFoundException>(() => service.DeleteAsync("X"), "The service must throw KeyNotFoundException.");
            ex.Message.ShouldContain(errorMessage, customMessage: $"The error message must contain: \"{errorMessage}\"");
        }
    }
}
