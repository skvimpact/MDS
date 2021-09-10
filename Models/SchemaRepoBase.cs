using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace MDS.Models
{
    public abstract class SchemaRepoBase
    {
        //        private readonly Dictionary<int, int> idToParentId = new Dictionary<int, int>();
        readonly XDocument docBase;
        public virtual IQueryable<SchemaItem> SchemaItems { get; }

        //public int Indentation(int id) => indentation(id);

        public int Indentation(int id) =>
            SchemaItems.Where(i => i.IntMessageLineID.Equals(id)).FirstOrDefault().ParentElementID.Equals(0) ?
            0 :
            1 + Indentation(SchemaItems.Where(i => i.IntMessageLineID.Equals(id)).FirstOrDefault().ParentElementID);
 //           idToParentId[id] == 0 ? 0 : 1 + indentation(idToParentId[id]);

        //public string FullPath(int id) => fullPath(id);

        public string XPath(int id) =>
            (SchemaItems.Where(i => i.IntMessageLineID.Equals(id)).FirstOrDefault()?.ParentElementID.Equals(0) ?? false) ?
            SchemaItems.Where(i => i.IntMessageLineID.Equals(id)).FirstOrDefault()?.ElementName ?? "" :
            $"{XPath(SchemaItems.Where(i => i.IntMessageLineID.Equals(id)).FirstOrDefault().ParentElementID)}/{SchemaItems.Where(i => i.IntMessageLineID.Equals(id)).FirstOrDefault().ElementName}";

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
                    item.XPath = XPath(item.IntMessageLineID);
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


        public string XmlBase
        {
            get
            {
                var items = SchemaItems.ToArray();
                (int rootId, string rootName) = items
                    .Where(i => i.ParentElementID == 0)
                    .Select(i => (i.IntMessageLineID, i.ElementName))
                    .FirstOrDefault();

                XDocument document = new XDocument(
                    new XDeclaration("1.0", "utf-8", "no"),
                    new XElement(rootName));

                var xElements = new Dictionary<int, XElement> { { rootId, document.Root } };
                var xAttributes = new Dictionary<int, XAttribute>();

                foreach (var item in items.Where(i => i.ParentElementID != 0))
                {
                    switch (item.LineType)
                    {
                        case 0:
                        case 1:
                            xElements[item.IntMessageLineID] =
                                new XElement(item.ElementName, string.IsNullOrWhiteSpace(item.Value) && item.LineType == 1 ? item.ElementName : item.Value);
                            break;
                        case 2:
                            xAttributes[item.IntMessageLineID] =
                                new XAttribute(item.ElementName, item.Value);
                            break;
                        default:
                            throw new InvalidOperationException("unknown item type");
                    }
                }

                foreach (var item in items.Where(i => i.ParentElementID != 0))
                {
                    switch (item.LineType)
                    {
                        case 0:
                        case 1:
                            xElements[item.ParentElementID]
                                .Add(xElements[item.IntMessageLineID]);
                            break;
                        case 2:
                            xElements[item.ParentElementID]
                                .Add(xAttributes[item.IntMessageLineID]);
                            break;
                        default:
                            throw new InvalidOperationException("unknown item type");
                    }
                    item.Indentation = Indentation(item.IntMessageLineID);
                    item.XPath = XPath(item.IntMessageLineID);
                }
                /*
                xElements[110000].Add(xElements[120000]);
                xElements[110000].Add(xElements[120000]);
                xElements[110000].Add(xElements[120000]);
                xElements[110000].Add(xElements[120000]);
                */
                
                (int parentId, int id) el = items
                    .Where(i => i.ElementName.Equals("characteristic"))
                    .Select(i => (i.ParentElementID, i.IntMessageLineID))
                    .FirstOrDefault();
                for (int i  = 0; i < 2; i++)
                    xElements[el.parentId].Add(xElements[el.id]);

                el = items
                    .Where(i => i.ElementName.Equals("calcGovernmentCodesParams"))
                    .Select(i => (i.ParentElementID, i.IntMessageLineID))
                    .FirstOrDefault();
                for (int i = 0; i < 5; i++)
                    xElements[el.parentId].Add(xElements[el.id]);
                

                

                return $"{document.Declaration}\n{document}";
            }
            //return Ok(XsdInferer.Infer(document.OuterXml));
        }


        public string XmlVar()
        {
            XDocument docXml = XDocument.Load(new StringReader(XmlBase));
            return docXml.ToString();
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
