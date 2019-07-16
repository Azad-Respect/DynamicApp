using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicsAppSolutionWebApi.Controllers
{
    public class WorklistController : Controller
    {
        // GET: Worklist
        public ActionResult Index()
        {
            return View();
        }
    }
}