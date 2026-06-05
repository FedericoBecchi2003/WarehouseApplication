using Microsoft.EntityFrameworkCore;
using PaymentService.Business;
using PaymentService.Repository;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IPaymentService, PaymentService.Business.PaymentService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
    context.Database.Migrate();
}

app.MapOpenApi();
app.MapScalarApiReference();

app.UseAuthorization();
app.MapControllers();

app.Run();
