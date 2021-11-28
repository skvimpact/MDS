using System.Linq;

namespace MDS.Models
{
    public interface ISchemaRepo
    {
        IQueryable<SchemaItem> SchemaItems { get; }
        int Indentation(int id);
        string XPath(int id);
        string Xml { get; }
        string XmlBase { get; }
        public string XmlVar();
        void Mutiply();
    }
}
