using Microsoft.EntityFrameworkCore;

namespace Insulter.Model;

public partial class InsultContext : DbContext {
    public InsultContext() {
    }

    public InsultContext(DbContextOptions<InsultContext> options) : base(options) {
    }
    public virtual DbSet<Insult> Insults { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=localhost;Database=Insulter;Integrated Security=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Insult>(entity => {
            entity.Property(e => e.Text).IsRequired(false).HasMaxLength(200);
        });

        modelBuilder.Entity<Category>(entity => {
            entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity => {
            entity.Property(e => e.Name).HasMaxLength(100);
        });
    }
}
