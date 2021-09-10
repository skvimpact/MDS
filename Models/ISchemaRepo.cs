using System.Linq;

namespace MDS.Models
{
    public interface ISchemaRepo
    {
        IQueryable<SchemaItem> SchemaItems { get; }
        int Indentation(int id);
        string FullPath(int id);
        string Xml { get; }
        void Mutiply();
    }
}
