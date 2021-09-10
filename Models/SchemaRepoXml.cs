using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace MDS.Models
{
    public class SchemaRepoXml : SchemaRepoBase, ISchemaRepo
    {
        private readonly List<SchemaItem> list;

        public SchemaRepoXml()
        {
            list = new List<SchemaItem>();
            var doc = new XmlDocument();
            doc.Load("C:\\Users\\zozo\\Downloads\\ims.xml");
            XmlNodeList rows = doc.SelectNodes("/a/IMS");
            foreach (XmlNode row in rows)
            {
                var item = new SchemaItem();
                var attributes = row.Attributes;
                foreach (XmlAttribute attr in attributes)
                {
                    switch (attr.Name)
                    {
                        case "ElementName": item.ElementName = attr.Value; break;
                        case "IntMessageLineID": item.IntMessageLineID = int.Parse(attr.Value); break;
                        case "IntMessageID": item.IntMessageID = int.Parse(attr.Value); break;
                        case "ParentElementID": item.ParentElementID = int.Parse(attr.Value); break;
                        case "LineType": item.LineType = int.Parse(attr.Value); break;
                        case "Indentation": item.Indentation = int.Parse(attr.Value); break;
                        case "Value": item.Value = attr.Value; break;
                        case "Multiplicity": item.Multiplicity = int.Parse(attr.Value); break;
                        default: break;
                    }
                }
                list.Add(item);
            }
        }

        public override IQueryable<SchemaItem> SchemaItems =>               
                list.AsQueryable();                  
    }
}
