using MDS.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

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
            var baseDoc = new MdsSchema(repository);
            //return Ok($"{baseDoc.DocumentVar(120000, ActionType.Add)}\n" +
            //          $"{baseDoc.DocumentVar(90000, ActionType.Remove)}");

            //return Ok($"{baseDoc.All()}");
            return Ok($"{XsdInferer.Infer(baseDoc.All())}\n{XsdInferer.Infer(baseDoc.ToString())}\n{new MdsSchema(repository).XsdDocument}");
            //return Ok($"{XsdInferer.Infer(baseDoc.ToString())}");

            //return Ok(XsdInferer.Infer(document.OuterXml));
        }
    }
}
