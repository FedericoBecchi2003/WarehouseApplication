using Microsoft.EntityFrameworkCore;
using WarehouseService.Business;
using WarehouseService.Repository;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1. CONFIGURAZIONE DATABASE (PostgreSQL)
// ============================================================
// Legge la stringa di connessione dal file appsettings.json o dalle variabili d'ambiente di Docker
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<WarehouseDbContext>(options =>
    options.UseNpgsql(connectionString));

// ============================================================
// 2. DEPENDENCY INJECTION (Registrazione Servizi)
// ============================================================
// Diciamo a .NET quale classe usare quando qualcuno chiede un IWarehouseService
builder.Services.AddScoped<IWarehouseService, WarehouseService.Business.WarehouseService>();

// (Spazio per il futuro: qui aggiungeremo il client HTTP per altri servizi)
// es. builder.Services.AddWarehouseClient(builder.Configuration["Urls:WarehouseApi"]);

// ============================================================
// 3. CONFIGURAZIONE API E SWAGGER
// ============================================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ============================================================
// 4. AUTOMATISMO MIGRATIONS (La Magia)
// ============================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<WarehouseDbContext>();
        Console.WriteLine("⏳ Verificando il database PostgreSQL...");
        
        // Questo comando applica tutte le migrations pendenti. Se il DB non c'è, lo crea.
        context.Database.Migrate(); 
        
        Console.WriteLine("✅ ROOOAAR! 🦁 Database connesso, tabelle create e aggiornate con successo!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Errore fatale di connessione al database: {ex.Message}");
    }
}

// ============================================================
// 5. PIPELINE HTTP
// ============================================================
// Espone Swagger sempre, comodissimo per testare l'esame anche su Docker
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

app.Run();