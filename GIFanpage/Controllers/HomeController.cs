using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GIFanpage.Models;
//using GIFanpage.Filters;

namespace GIFanpage.Controllers
{
    public class HomeController : Controller
    {
        GIFanpageDbContext db = new GIFanpageDbContext();
        public ActionResult Index()
        {
            return View();
        }

       
    }
}