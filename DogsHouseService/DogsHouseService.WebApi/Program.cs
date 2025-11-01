using DogsHouseService.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDogsHouseServiceServices(builder.Configuration);

var app = builder.Build();

app.UseDogsHouseServiceMiddleware();
app.MapDogsHouseServiceRoutes();

await app.RunAsync();
