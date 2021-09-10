using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDS.Models
{
    public class SchemaRepoEF : SchemaRepoBase, ISchemaRepo
    {
        private readonly SchemaDbContext context;       

        public SchemaRepoEF(SchemaDbContext ctx) =>
            context = ctx;
        
        public override IQueryable<SchemaItem> SchemaItems =>
            context.SchemaItems;

    }
}
