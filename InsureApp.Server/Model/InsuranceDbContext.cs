using Microsoft.EntityFrameworkCore;

namespace InsureApp.Server.Model
{
    public class InsuranceDbContext : DbContext
    {
        public InsuranceDbContext(DbContextOptions options): base(options) { }

        public DbSet<EndUser> EndUsers { get; set; }
        public DbSet<InsuranceAgent> InsuranceAgents { get; set; }
        public DbSet<InsuranceType> InsuranceTypes { get; set; }
        public DbSet<InsuranceReport> InsuranceReports { get; set; }
        public DbSet<InsuranceDocument> InsuranceDocuments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<InsuranceType>()
                .HasIndex(u => u.Name)
                .IsUnique();
            modelBuilder.Entity<EndUser>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<EndUser>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<InsuranceAgent>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<InsuranceAgent>()
                .HasIndex(u => u.Email)
                .IsUnique();
            modelBuilder.Entity<InsuranceReport>()
                .HasOne(x => x.EndUser)
                .WithMany(u => u.InsuranceReports)
                .HasForeignKey(r => r.EndUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InsuranceReport>()
                .HasOne(d => d.InsuranceAgent)
                .WithMany(a => a.HandledReports)
                .HasForeignKey(b => b.InsuranceAgentId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<InsuranceReport>()
                .HasOne(e => e.InsuranceType)
                .WithMany(t => t.InsuranceReports)
                .HasForeignKey(f => f.InsuranceTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InsuranceDocument>()
                .HasOne(g => g.InsuranceReport)
                .WithMany(h => h.Documents)
                .HasForeignKey(i => i.InsuranceReportId);
        }


    }
}
