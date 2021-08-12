using MDS.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace MDS.Controllers
{
    public class A
    {
        public int ID;
        public int ParentID;
    }
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
            var elements = new Dictionary<int,XmlElement>();
            foreach(var item in items)
            {
                elements[item.IntMessageLineID] =
                    document.CreateElement(string.Empty, item.ElementName, string.Empty);
                if (item.LineType != 0)
                    elements[item.IntMessageLineID]
                        .AppendChild(document.CreateTextNode(string.IsNullOrWhiteSpace(item.Value) ? 
                            item.ElementName :
                            item.Value));
            }

            foreach (var item in items)
            {
                if (item.ParentElementID == 0)
                {
                    document
                        .AppendChild(elements[item.IntMessageLineID]);
                    item.Indentation = 0;
                }
                else
                {
                    elements[item.ParentElementID]
                        .AppendChild(elements[item.IntMessageLineID]);
                    item.Indentation = 1 + items.Find(i => i.IntMessageLineID == item.ParentElementID).Indentation;
                }
            }

            XmlAttribute checkedOut = document.CreateAttribute("checkedout");
           // XmlAttribute damaged = document.CreateAttribute("damaged");
            checkedOut.Value = "no";
            elements[100000].Attributes.Append(checkedOut);


            elements[110000].AppendChild(elements[120000].CloneNode(true));
            elements[110000].AppendChild(elements[120000].CloneNode(true));
            elements[110000].AppendChild(elements[120000].CloneNode(true));

             var s = document.OuterXml;

            // return this.Ok(repository.SchemaItems);
            return this.Ok(document.OuterXml);
        }

    }
}
