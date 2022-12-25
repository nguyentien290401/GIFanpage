using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GIFanpage.Models;

namespace GIFanpage.Controllers
{
    public class CharactersController : Controller
    {
        private GIFanpageDbContext db = new GIFanpageDbContext();

        public ActionResult IndexView(string Search = "")
        {
            //Search
            List<Character> characters = db.Characters.Where(s => s.CharacterName.Contains(Search)).ToList();

            if (characters.Count() == 0)
            {
                ViewBag.Msg = "Data Not Found";
                return View();
            }    
            return View(characters);
        }

        // GET: Characters/Details/5
        public ActionResult Details(int character)
        {
            Character characters = db.Characters.Where(c => c.CharacterID == character).FirstOrDefault();
            
            return View(characters);
        }

        // GET: Characters/Create
        public ActionResult Create()
        {
            return View();
        }

       
       
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
