using DogsHouseService.Services.Database.Data;
using DogsHouseService.Services.Database.Interfaces;
using DogsHouseService.Services.Database.Repositories;
using DogsHouseService.Services.Database.Servicies;
using DogsHouseService.Sevices.Interfaces.Services;
using DogsHouseService.WebApi.Options;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;

namespace DogsHouseService.WebApi.Extensions
{
    /// <summary>
    /// Service collection extensions
    /// </summary>
    internal static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds DogsHouseService services to the service collection
        /// </summary>
        /// <param name="services">The application service collection.</param>
        /// <param name="configuration">The app configuration dictionary.</param>
        /// <returns>The service collection with DogsHouseService services added.</returns>
        public static IServiceCollection AddDogsHouseServiceServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add services to the container.
            services.AddDbContext<DogsHouseServiceDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("DogsHouseService.WebApi"));
            });

            var rateLimitOptions = new RateLimitingOptions();
            configuration.GetSection("RateLimiting").Bind(rateLimitOptions);

            services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("PerSecondLimit", limiterOptions =>
                {
                    limiterOptions.PermitLimit = rateLimitOptions.PermitLimit;
                    limiterOptions.Window = TimeSpan.FromSeconds(rateLimitOptions.WindowSeconds);
                    limiterOptions.QueueLimit = rateLimitOptions.QueueLimit;
                    limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });

                options.OnRejected = static (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    return new ValueTask(
                        context.HttpContext.Response.WriteAsync(
                            "Too many requests. Please try again later.", token));
                };
            });

            services.AddTransient<IDogRepository, DogRepository>()
                .AddTransient<IDogService, DogService>();

            services.AddControllers();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }
    }
}
