using Microsoft.EntityFrameworkCore;

namespace MDS.Models
{

    public class SchemaDbContext : DbContext
    {

        public SchemaDbContext(DbContextOptions<SchemaDbContext> options)
            : base(options) { }

        public DbSet<SchemaItem> SchemaItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {        
            modelBuilder.Entity<SchemaItem>()
                .ToTable("Integration Message Schema");
            modelBuilder.Entity<SchemaItem>()
                .HasKey(k => new { k.IntMessageID, k.IntMessageLineID });
            modelBuilder.Entity<SchemaItem>()
                .Property(c => c.IntMessageID)
                .HasColumnName("Int_ Message ID");
            modelBuilder.Entity<SchemaItem>()
                .Property(c => c.IntMessageLineID)
                .HasColumnName("Int_ Message Line ID");
            modelBuilder.Entity<SchemaItem>()
                .Property(c => c.ElementName)
                .HasColumnName("Element Name");
            modelBuilder.Entity<SchemaItem>()
                .Property(c => c.LineType)
                .HasColumnName("Line Type");
            modelBuilder.Entity<SchemaItem>()
                .Property(c => c.ParentElementID)
                .HasColumnName("Parent Element ID");
            modelBuilder.Entity<SchemaItem>()
                .Ignore(c => c.XPath);
        }
    }
}
