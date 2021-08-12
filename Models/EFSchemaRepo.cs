using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDS.Models
{
    public class EFSchemaRepo : ISchemaRepo
    {
        private SchemaDbContext context;
        private Dictionary<int, int> idToParentId;

        public EFSchemaRepo(SchemaDbContext ctx)
        {
            context = ctx;

            idToParentId = new Dictionary<int, int>();
            foreach (var item in SchemaItems)
                idToParentId[item.IntMessageLineID] = item.ParentElementID;
        }

        public IQueryable<SchemaItem> SchemaItems =>
            context.SchemaItems;

        public int Indentation(int id) => indentation(id);

        private int indentation(int id) =>
            idToParentId[id] == 0 ? 0 : 1 + indentation(idToParentId[id]);

    }
}
