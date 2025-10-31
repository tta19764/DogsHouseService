namespace DogsHouseService.WebApi.Extensions
{
    internal static class ApplicationBuilderExtensions
    {
        public static WebApplication UseDogsHouseServiceMiddleware(this WebApplication app)
        {
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            return app;
        }

        public static WebApplication MapDogsHouseServiceRoutes(this WebApplication app)
        {
            app.MapControllers();

            return app;
        }
    }
}
