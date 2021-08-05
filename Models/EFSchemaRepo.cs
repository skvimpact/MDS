using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDS.Models
{
    public class EFSchemaRepo : ISchemaRepo
    {
        private SchemaDbContext context;

        public EFSchemaRepo(SchemaDbContext ctx)
        {
            context = ctx;
        }

        public IQueryable<SchemaItem> SchemaItems =>
            context.SchemaItems;
           
    }
}
