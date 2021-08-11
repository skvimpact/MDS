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

            var a = repository.SchemaItems.ToList();
            var elements = new Dictionary<int,XmlElement>();
            foreach(var item in repository.SchemaItems)
            {
                elements[item.IntMessageLineID] =
                    document.CreateElement(string.Empty, item.ElementName, string.Empty);
                if (item.LineType != 0)
                    elements[item.IntMessageLineID].InnerText = item.ElementName;
            }

            foreach (var item in repository.SchemaItems)
            {
                if (item.ParentElementID == 0)
                    document.AppendChild(elements[item.IntMessageLineID]);
                else               
                    elements[item.ParentElementID]
                        .AppendChild(elements[item.IntMessageLineID]);               
            }

            //    foreach (var item in elements.Keys)
            //{
            //    if (item.ParentID == 0)                
            //        document.AppendChild(elements[item]);                
            //    else
            //        elements[item].AppendChild(null);
            //}


           // var s = document.OuterXml;

           // return this.Ok(repository.SchemaItems);
           return this.Ok(document.OuterXml);
        }

    }
}
