using Microsoft.EntityFrameworkCore;
using PaymentService.Business;
using PaymentService.Repository;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1. CONFIGURAZIONE DATABASE (PostgreSQL)
// ============================================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseNpgsql(connectionString));

// ============================================================
// 2. DEPENDENCY INJECTION (Registrazione Servizi)
// ============================================================
builder.Services.AddScoped<IPaymentService, PaymentService.Business.PaymentService>();

// ============================================================
// 3. CONFIGURAZIONE API E SWAGGER
// ============================================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ============================================================
// 4. AUTOMATISMO MIGRATIONS
// ============================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<PaymentDbContext>();
        Console.WriteLine("⏳ Verificando il database PostgreSQL...");
        context.Database.Migrate();
        Console.WriteLine("✅ Database connesso, tabelle create con successo!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Errore di connessione al database: {ex.Message}");
    }
}

// ============================================================
// 5. PIPELINE HTTP
// ============================================================
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

app.Run();
