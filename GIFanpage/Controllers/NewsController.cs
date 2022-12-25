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
    public class NewsController : Controller
    {
        private GIFanpageDbContext db = new GIFanpageDbContext();

       
        public ActionResult IndexView()
        {
            List<New> newInfos = db.News.ToList();

            return View(newInfos);
        }

        // GET: News/Details/5
        public ActionResult Details(int newInfo)
        {
            New newInfos = db.News.Where(n => n.NewsID == newInfo).FirstOrDefault();

            List<New> numberNews = db.News.Where(n => n.NewsID == newInfo).ToList();

            int totalNumberNews = numberNews.Count();
            ViewBag.totalNumberNews = totalNumberNews;

            return View(newInfos);
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
