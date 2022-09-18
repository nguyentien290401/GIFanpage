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

        // GET: News
        public ActionResult Index()
        {
            return View(db.News.ToList());
        }

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

        // GET: News/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: News/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NewsID,NewsTitle,NewsDescription,NewsImage,NewsContent,CreateDate")] New newInfo, HttpPostedFileBase fileNew)
        {
            if (ModelState.IsValid)
            {
                if (fileNew != null)
                {
                    // Define image card file
                    var fileName = fileNew.FileName;
                    var pathFile = "~/Content/Image/" + fileName;
                    fileNew.SaveAs(Server.MapPath(pathFile));
                    newInfo.NewsImage = pathFile;
                }

                db.News.Add(newInfo);
                db.SaveChanges();
                return RedirectToAction("IndexView");
            }

            return View(newInfo);
        }

        // GET: News/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            New newInfo = db.News.Find(id);
            if (newInfo == null)
            {
                return HttpNotFound();
            }
            return View(newInfo);
        }

        // POST: News/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NewsID,NewsTitle,NewsDescription,NewsImage,NewsContent,CreateDate")] New newInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(newInfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(newInfo);
        }

        // GET: News/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            New newInfo = db.News.Find(id);
            if (newInfo == null)
            {
                return HttpNotFound();
            }
            return View(newInfo);
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            New newInfo = db.News.Find(id);
            db.News.Remove(newInfo);
            db.SaveChanges();
            return RedirectToAction("Index");
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
