using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GIFanpage.Models;

namespace GIFanpage.Areas.Admin.Controllers
{
    public class NewInfoController : Controller
    {
        private GIFanpageDbContext db = new GIFanpageDbContext();

        // GET: Admin/NewInfo
        public ActionResult Index(string Search = "", int PageNo = 1)
        {
            //Search
            List<New> newInfos = db.News.Where(s => s.NewsTitle.Contains(Search)).ToList();

            if (newInfos.Count() == 0)
            {
                ViewBag.Msg = "Data Not Found";
                return View();
            }

            // Total Users Account
            int TotalNew = newInfos.Count();
            ViewBag.TotalNew = TotalNew;

            //Pagination

            int NoOfRecordsPerPage = 4;
            int NoOfPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(newInfos.Count) / Convert.ToDouble(NoOfRecordsPerPage)));
            int NoOfRecordsToSkip = (PageNo - 1) * NoOfRecordsPerPage;
            ViewBag.PageNo = PageNo;
            ViewBag.NoOfPages = NoOfPages;
            newInfos = newInfos.Skip(NoOfRecordsToSkip).Take(NoOfRecordsPerPage).ToList();

            return View(newInfos);
        }

        // GET: Admin/NewInfo/Details/5
        public ActionResult Details(int newInfo)
        {
            New newInfos = db.News.Where(c => c.NewsID == newInfo).FirstOrDefault();
            List<New> numberNews = db.News.Where(n => n.NewsID == newInfo).ToList();

            int totalNumberNews = numberNews.Count();
            ViewBag.totalNumberNews = totalNumberNews;

            return View(newInfos);
        }

        // GET: Admin/NewInfo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/NewInfo/Create
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
                return RedirectToAction("Index");
            }

            return View(newInfo);
        }

        // GET: Admin/NewInfo/Edit/5
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

        // POST: Admin/NewInfo/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NewsID,NewsTitle,NewsDescription,NewsImage,NewsContent,CreateDate")] New newInfo, HttpPostedFileBase fileNew)
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

                db.Entry(newInfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(newInfo);
        }

        // GET: Admin/NewInfo/Delete/5
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

        // POST: Admin/NewInfo/Delete/5
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
