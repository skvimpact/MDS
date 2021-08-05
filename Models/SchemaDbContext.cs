using Microsoft.EntityFrameworkCore;

namespace MDS.Models
{

    public class SchemaDbContext : DbContext
    {

        public SchemaDbContext(DbContextOptions<SchemaDbContext> options)
            : base(options) { }

        public DbSet<SchemaItem> SchemaItems { get; set; }
    }
}
