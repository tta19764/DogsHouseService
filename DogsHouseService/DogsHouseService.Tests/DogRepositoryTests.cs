using DogsHouseService.Services.Database.Data;
using DogsHouseService.Services.Database.Entities;
using DogsHouseService.Services.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System.Text;
using Xunit;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DogsHouseService.Tests
{
    public class DogRepositoryTests
    {
        private static DogsHouseServiceDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<DogsHouseServiceDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new DogsHouseServiceDbContext(options);
        }

        private static Dog CreateDog(string name = "Buddy", int tail = 10, int weight = 20)
            => new Dog { Name = name, Color = "Brown", TailLength = tail, Weight = weight };

        [Fact]
        public async Task AddAsync_ShouldAddDog_WhenValid()
        {
            // Arrange
            using var context = CreateContext();
            var repo = new DogRepository(context);
            var dog = CreateDog();

            // Act
            var addedDog = await repo.AddAsync(dog);

            // Assert
            context.Dogs.ShouldHaveSingleItem("The context must contain a single item.");
            addedDog.Name.ShouldBe($"{dog.Name}", $"The dog's name must be {dog.Name}.");
        }

        [Fact]
        public async Task AddAsync_ShouldThrow_WhenDogWithSameNameExists()
        {
            // Arrange
            using var context = CreateContext();
            var repo = new DogRepository(context);
            var dog = CreateDog("Rex");
            await repo.AddAsync(dog);
            var errorMessage = "The dog with this name already in the database";

            var duplicate = CreateDog("Rex");

            // Act & Assert
            var ex = await Should.ThrowAsync<InvalidOperationException>(() => repo.AddAsync(duplicate), "Must throw InvalidOperationException.");
            ex.Message.ShouldContain(errorMessage, customMessage: $"The error message must contain \"{errorMessage}\"");
        }

        [Fact]
        public async Task AddAsync_ShouldThrow_WhenTailLengthIsNegative()
        {
            // Arrange
            using var context = CreateContext();
            var repo = new DogRepository(context);
            var dog = CreateDog("Spike", -5);
            var errorMessage = "Tail length cannot be negative";

            // Act & Assert
            var ex = await Should.ThrowAsync<ArgumentException>(() => repo.AddAsync(dog), "Must throw ArgumentException.");
            ex.Message.ShouldContain(errorMessage, customMessage: $"The error message must contain \"{errorMessage}\"");
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllDogs()
        {
            // Arrange
            using var context = CreateContext();
            var dogs = new[] { CreateDog("A"), CreateDog("B") };
            var expectedCount = dogs.Length;
            context.Dogs.AddRange(dogs);
            await context.SaveChangesAsync();

            // Act
            var repo = new DogRepository(context);
            var result = await repo.GetAllAsync();

            // Assert
            result.Count().ShouldBe(expectedCount, $"The repository must return exectly {expectedCount} entities.");
        }

        [Fact]
        public async Task GetAllAsync_Paged_ShouldReturnCorrectPage()
        {
            // Arrange
            using var context = CreateContext();
            var dogs = new[] { CreateDog("A"), CreateDog("B"), CreateDog("C"), CreateDog("D") };
            var expectedDogsNames = new[] { dogs[2].Name, dogs[3].Name };
            int pageNumber = 2, pageSize = 2;
            context.Dogs.AddRange(dogs);
            await context.SaveChangesAsync();

            StringBuilder customMessage = new StringBuilder("The repository must return two dogs with names:");
            foreach (var name in expectedDogsNames)
            {
                customMessage.Append($" \"{name}\"");
            }

            // Act
            var repo = new DogRepository(context);
            var result = (await repo.GetAllAsync(pageNumber: pageNumber, pageSize: pageSize)).ToList();

            // Assert
            result.Select(d => d.Name).ShouldBe(expectedDogsNames, customMessage.ToString());
        }

        [Fact]
        public async Task GetAllAsync_Paged_ShouldThrowArgumentOutOfRangeException_WhenPageInvalid()
        {
            // Arrange
            using var context = CreateContext();
            int pageNumber = -2, pageSize = 2;
            context.Dogs.Add(CreateDog("A"));
            await context.SaveChangesAsync();
            var repo = new DogRepository(context);

            // Act & Assert
            await Should.ThrowAsync<ArgumentOutOfRangeException>(() => repo.GetAllAsync(pageNumber, pageSize), "The repository must throw ArgumentOutOfRangeException");
        }

        [Fact]
        public async Task GetAllAsync_Paged_ShouldThrowArgumentOutOfRangeException_WhenSizeInvalid()
        {
            // Arrange
            using var context = CreateContext();
            int pageNumber = 2, pageSize = -2;
            context.Dogs.Add(CreateDog("A"));
            await context.SaveChangesAsync();
            var repo = new DogRepository(context);

            // Act & Assert
            await Should.ThrowAsync<ArgumentOutOfRangeException>(() => repo.GetAllAsync(pageNumber, pageSize), "The repository must throw ArgumentOutOfRangeException");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnDog_WhenExists()
        {
            // Arrange
            using var context = CreateContext();
            var dog = CreateDog("Rex");
            context.Dogs.Add(dog);
            await context.SaveChangesAsync();

            // Act
            var repo = new DogRepository(context);
            var result = await repo.GetByIdAsync("Rex");

            // Assert
            result.ShouldNotBeNull("Repository must return a valid value.");
            result!.Name.ShouldBe(dog.Name, $"The returned dog's name must be \"{dog.Name}\".");
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyDog()
        {
            // Arrange
            using var context = CreateContext();
            var dog = CreateDog("Rocky");
            context.Dogs.Add(dog);
            await context.SaveChangesAsync();

            var repo = new DogRepository(context);
            var updated = CreateDog("Rocky", tail: 20, weight: 30);

            // Act
            var result = await repo.UpdateAsync(updated);

            // Assert
            await context.Dogs.SingleAsync(d => d.Name == "Rocky");
            result.TailLength.ShouldBe(dog.TailLength, $"The dog's tail length must be {dog.TailLength}");
            result.Weight.ShouldBe(30, $"The dog's tail length must be {dog.Weight}");
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenDogDoesNotExist()
        {
            // Arrange
            using var context = CreateContext();
            var repo = new DogRepository(context);
            var updated = CreateDog("Ghost");

            // Act & Assert
            await Should.ThrowAsync<KeyNotFoundException>(() => repo.UpdateAsync(updated), "The repository must throw KeyNotFoundException");
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveDog_WhenExists()
        {
            // Arrange
            using var context = CreateContext();
            var dog = CreateDog("Milo");
            context.Dogs.Add(dog);
            await context.SaveChangesAsync();

            // Act
            var repo = new DogRepository(context);
            await repo.DeleteAsync("Milo");

            // Assert
            context.Dogs.ShouldBeEmpty("The context after the operation must be empty.");
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrow_WhenDogDoesNotExist()
        {
            // Arrange
            using var context = CreateContext();
            var repo = new DogRepository(context);

            // Act & Assert
            await Should.ThrowAsync<KeyNotFoundException>(() => repo.DeleteAsync("NoDog"), "The repositry must throw KeyNotFoundException.");
        }
    }
}
