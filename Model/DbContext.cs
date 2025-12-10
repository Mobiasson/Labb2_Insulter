using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace Insulter.Model;

public partial class InsultContext : DbContext {
    public InsultContext() {
    }

    public InsultContext(DbContextOptions<InsultContext> options) : base(options) {
    }

    public virtual DbSet<Insult> Insults { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=localhost;Database=insulter;Integrated Security=True;TrustServerCertificate=True;");
}
