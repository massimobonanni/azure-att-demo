using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ApplicationInsight.Presentation.Controllers
{
    public class ProbeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}