using MDS.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            return this.Ok(repository.SchemaItems);
        }

    }
}
