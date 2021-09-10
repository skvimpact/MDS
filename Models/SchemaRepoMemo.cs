using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDS.Models
{
    public class MemoSchemaRepoo : SchemaRepoBase, ISchemaRepo
    {
        public override IQueryable<SchemaItem> SchemaItems =>
            (new [] { new SchemaItem { IntMessageID=3, IntMessageLineID = 9},
            new SchemaItem { IntMessageID=4, IntMessageLineID = 9},
            new SchemaItem { IntMessageID=5, IntMessageLineID = 9}}).AsQueryable();
    }
}
