using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using WarehouseService.Business;
using WarehouseService.Repository;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<WarehouseDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IWarehouseService, WarehouseService.Business.WarehouseService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();
    context.Database.Migrate();
}

app.MapOpenApi();
app.MapScalarApiReference();

app.UseAuthorization();
app.MapControllers();

app.Run();
