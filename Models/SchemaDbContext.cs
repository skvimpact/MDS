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
         
            modelBuilder.Entity<SchemaItem>().HasKey(u => new {u.IntMessageID, u.IntMessageLineID });
            modelBuilder.Entity<SchemaItem>().ToTable("Integration Message Schema");
        }
    }
}
