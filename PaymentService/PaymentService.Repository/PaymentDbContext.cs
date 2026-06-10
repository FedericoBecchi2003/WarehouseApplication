using Microsoft.EntityFrameworkCore;

namespace PaymentService.Repository;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
    {
    }

    public DbSet<PaymentEntity> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<PaymentEntity>().HasKey(p => p.Id);
    }
}
