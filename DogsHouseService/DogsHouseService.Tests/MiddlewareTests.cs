using DogsHouseService.WebApi.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DogsHouseService.Tests
{
    public class MiddlewareTests
    {
        private readonly Mock<ILogger<ExceptionHandlingMiddleware>> loggerMock;
        private readonly DefaultHttpContext context;

        public MiddlewareTests()
        {
            loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
            context = new DefaultHttpContext();
        }

        private ExceptionHandlingMiddleware CreateMiddlewareThatThrows(Exception exception)
        {
            Task next(HttpContext _) => throw exception;
            return new ExceptionHandlingMiddleware(next, loggerMock.Object);
        }

        [Theory]
        [InlineData(typeof(ArgumentException), HttpStatusCode.BadRequest)]
        [InlineData(typeof(KeyNotFoundException), HttpStatusCode.NotFound)]
        [InlineData(typeof(InvalidOperationException), HttpStatusCode.Conflict)]
        [InlineData(typeof(Exception), HttpStatusCode.InternalServerError)]
        public async Task Middleware_ShouldReturnExpectedStatusCode_WhenExceptionThrown(Type exceptionType, HttpStatusCode expectedStatus)
        {
            // Arrange
            var exception = (Exception)Activator.CreateInstance(exceptionType, "Test exception")!;
            var middleware = CreateMiddlewareThatThrows(exception);

            context.Response.Body = new MemoryStream();

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)expectedStatus, context.Response.StatusCode);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();

            var response = JsonSerializer.Deserialize<JsonElement>(responseBody);

            Assert.Equal("application/json", context.Response.ContentType);
            Assert.Equal((int)expectedStatus, response.GetProperty("statusCode").GetInt32());
            Assert.NotNull(response.GetProperty("error").GetString());
        }
    }
}
