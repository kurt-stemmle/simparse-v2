using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Simparse.Controllers
{

    [Route("/")]
    public class HomeController : Controller
    {
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("dashboard")]
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
