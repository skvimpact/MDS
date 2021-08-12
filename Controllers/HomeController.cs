using MDS.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace MDS.Controllers
{
    public class HomeController : Controller
    {
        private ISchemaRepo repository;

        public HomeController(ISchemaRepo repo)
        {
            repository = repo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            XmlDocument document = new XmlDocument();
            XmlDeclaration xmlDeclaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);
            document.InsertBefore(xmlDeclaration, document.DocumentElement);

            var items = repository.SchemaItems.ToList();

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

                        item.Indentation = repository.Indentation(item.IntMessageLineID);
                        break;
                    case 2:
                        xmlElements[item.ParentElementID].Attributes.Append(xmlAttributes[item.IntMessageLineID]);
                        break;
                    default:
                        throw new InvalidOperationException("unknown item type");
                }
            }

            xmlElements[110000].AppendChild(xmlElements[120000].CloneNode(true));
            xmlElements[110000].AppendChild(xmlElements[120000].CloneNode(true));
            xmlElements[110000].AppendChild(xmlElements[120000].CloneNode(true));

            var s = document.OuterXml;

            // return this.Ok(repository.SchemaItems);
            return Ok(document.OuterXml);
        }

        [HttpGet]
        public IActionResult Index2()
        {
            XmlDocument document = new XmlDocument();
            XmlDeclaration xmlDeclaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);
            document.InsertBefore(xmlDeclaration, document.DocumentElement);

            XmlElement root = document.CreateElement("xs", "schema", "http://www.w3.org/2001/XMLSchema");

            //XmlAttribute xs = document.CreateAttribute(string.Empty, "xs", string.Empty);
            //xs.Value = "http://www.w3.org/2001/XMLSchema";

            XmlAttribute attributeFormDefault = document.CreateAttribute(string.Empty, "attributeFormDefault", string.Empty);
            attributeFormDefault.Value = "unqualified";

            XmlAttribute elementFormDefault = document.CreateAttribute(string.Empty, "elementFormDefault", string.Empty);
            elementFormDefault.Value = "qualified";

            //root.Attributes.Append(xs);
            root.Attributes.Append(attributeFormDefault);
            root.Attributes.Append(elementFormDefault);

            document.AppendChild(root);


            XmlElement rootElement = document.CreateElement("xs", "element", "");

            XmlAttribute name = document.CreateAttribute("name");
            name.Value = "myRootName";
            rootElement.Attributes.Append(name);

            XmlAttribute type = document.CreateAttribute("type");
            type.Value = "ct_myRootName";
            rootElement.Attributes.Append(type);

            root.AppendChild(rootElement);

            var s = document.OuterXml;
            return Ok(document.OuterXml);
        }
    }
}
