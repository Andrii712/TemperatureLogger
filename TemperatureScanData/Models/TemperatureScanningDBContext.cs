using System.Data.Entity;

namespace TemperatureScanData.Models
{
    class TemperatureScanningDBContext : DbContext
    {
        public TemperatureScanningDBContext() : base("TemperatureScanningConnection") { }

        public DbSet<ScanResult> ScanResults { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var instance = System.Data.Entity.SqlServer.SqlProviderServices.Instance;

            //modelBuilder.Entity<ScanResult>()
            //    .ToTable("ScanResult")
            //    .HasIndex(sr => new { sr.AccountID, sr.DirectionID, sr.DoorModuleID })
            //    .IsUnique();
        }
    }
}
