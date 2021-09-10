using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace MDS.Models
{
    public abstract class SchemaRepoBase
    {
//        private readonly Dictionary<int, int> idToParentId = new Dictionary<int, int>();
        public virtual IQueryable<SchemaItem> SchemaItems { get; }

        //public int Indentation(int id) => indentation(id);

        public int Indentation(int id) =>
            SchemaItems.Where(i => i.IntMessageLineID.Equals(id)).FirstOrDefault().ParentElementID.Equals(0) ?
            0 :
            1 + Indentation(SchemaItems.Where(i => i.IntMessageLineID.Equals(id)).FirstOrDefault().ParentElementID);
 //           idToParentId[id] == 0 ? 0 : 1 + indentation(idToParentId[id]);

        //public string FullPath(int id) => fullPath(id);

        public string FullPath(int id) =>
            (SchemaItems.Where(i => i.IntMessageLineID.Equals(id)).FirstOrDefault()?.ParentElementID.Equals(0) ?? false) ?
            SchemaItems.Where(i => i.IntMessageLineID.Equals(id)).FirstOrDefault()?.ElementName ?? "" :
            $"{FullPath(SchemaItems.Where(i => i.IntMessageLineID.Equals(id)).FirstOrDefault().ParentElementID)}->{SchemaItems.Where(i => i.IntMessageLineID.Equals(id)).FirstOrDefault().ElementName}";

        public string Xml
        {
            get
            {
                XmlDocument document = new XmlDocument();
                XmlDeclaration xmlDeclaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);
                document.InsertBefore(xmlDeclaration, document.DocumentElement);

                var items = SchemaItems.ToList();

                var xmlElements = new Dictionary<int, XmlElement>();
                var xmlAttributes = new Dictionary<int, XmlAttribute>();

                foreach (var item in items)
                {
                    switch (item.LineType)
                    {
                        case 0:
                        case 1:
                            xmlElements[item.IntMessageLineID] =
                                document.CreateElement(string.Empty, item.ElementName, string.Empty);
                            if (item.LineType == 1)
                                xmlElements[item.IntMessageLineID]
                                    .AppendChild(document.CreateTextNode(string.IsNullOrWhiteSpace(item.Value) ?
                                        item.ElementName :
                                        item.Value));
                            break;
                        case 2:
                            xmlAttributes[item.IntMessageLineID] =
                                document.CreateAttribute(string.Empty, item.ElementName, string.Empty);
                            xmlAttributes[item.IntMessageLineID].Value = item.Value;
                            break;
                        default:
                            throw new InvalidOperationException("unknown item type");
                    }
                }

                foreach (var item in items)
                {
                    switch (item.LineType)
                    {
                        case 0:
                        case 1:
                            if (item.ParentElementID == 0)
                                document
                                    .AppendChild(xmlElements[item.IntMessageLineID]);
                            else
                                xmlElements[item.ParentElementID]
                                    .AppendChild(xmlElements[item.IntMessageLineID]);
                            break;
                        case 2:
                            xmlElements[item.ParentElementID].Attributes.Append(xmlAttributes[item.IntMessageLineID]);
                            break;
                        default:
                            throw new InvalidOperationException("unknown item type");
                    }
                    item.Indentation = Indentation(item.IntMessageLineID);
                    item.FullPath = FullPath(item.IntMessageLineID);
                }

                //xmlElements[110000].AppendChild(xmlElements[120000].CloneNode(true));
                //xmlElements[110000].AppendChild(xmlElements[120000].CloneNode(true));
                //xmlElements[110000].AppendChild(xmlElements[120000].CloneNode(true));

                //var s = document.OuterXml;

                // return this.Ok(repository.SchemaItems);
                return document.OuterXml;
            }
            //return Ok(XsdInferer.Infer(document.OuterXml));
        }

        public void Mutiply()
        {
            var v = SchemaItems.Max(i => i.IntMessageLineID);
            foreach (var item in SchemaItems.OrderByDescending(i => i.Indentation).Select(i => i))
            {

            }
        }
    }
}
