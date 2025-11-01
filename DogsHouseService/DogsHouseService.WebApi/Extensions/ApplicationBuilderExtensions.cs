using DogsHouseService.WebApi.Middlewares;

namespace DogsHouseService.WebApi.Extensions
{

    /// <summary>
    /// Application builder extensions
    /// </summary>
    internal static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Use DogsHouseService middlewares.
        /// </summary>
        /// <param name="app">The web application.</param>
        /// <returns>The web application with middleware.</returns>
        public static WebApplication UseDogsHouseServiceMiddleware(this WebApplication app)
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRateLimiter();
            app.UseHttpsRedirection();

            return app;
        }

        /// <summary>
        /// Map DogsHouseService routes.
        /// </summary>
        /// <param name="app">The web applicatin.</param>
        /// <returns>The web application with routes.</returns>
        public static WebApplication MapDogsHouseServiceRoutes(this WebApplication app)
        {
            app.MapControllers().RequireRateLimiting("PerSecondLimit");

            return app;
        }
    }
}
