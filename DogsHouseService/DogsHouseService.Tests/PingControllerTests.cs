using DogsHouseService.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogsHouseService.Tests
{
    public class PingControllerTests
    {
        [Fact]
        public void Ping_ShouldReturnConfiguredVersion()
        {
            // Arrange
            var expectedResult = "Dogshouseservice.Version1.0.1";
            var inMemorySettings = new Dictionary<string, string?>
            {
                { "AppSettings:Version", "1.0.1" },
                { "AppSettings:ApplicationName", "Dogshouseservice" }
            };

            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var controller = new PingController(config);

            // Act
            var result = controller.Ping() as OkObjectResult;

            // Assert
            result.ShouldNotBeNull("The controller must return a result.");
            result!.Value.ShouldBe(expectedResult, $"The returned value mast match with {expectedResult}");
        }
    }
}
