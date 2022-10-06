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
    public class QuestionManageController : Controller
    {
        private GIFanpageDbContext db = new GIFanpageDbContext();

        // GET: Admin/QuestionManage
        public ActionResult Index(string Search = "", int PageNo = 1)
        {
            //Search
            List<Ask> asks = db.Asks.Where(s => s.User.Name.Contains(Search) || s.Title.Contains(Search) || s.Category.CategoryName.Contains(Search) || s.User.Email.Contains(Search)).ToList();

            if (asks.Count() == 0)
            {
                ViewBag.Msg = "Data Not Found";
                return View();
            }

            // Total Users Account
            int TotalQuestion = asks.Count();
            ViewBag.TotalQuestion = TotalQuestion;

            //Pagination

            int NoOfRecordsPerPage = 4;
            int NoOfPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(asks.Count) / Convert.ToDouble(NoOfRecordsPerPage)));
            int NoOfRecordsToSkip = (PageNo - 1) * NoOfRecordsPerPage;
            ViewBag.PageNo = PageNo;
            ViewBag.NoOfPages = NoOfPages;
            asks = asks.Skip(NoOfRecordsToSkip).Take(NoOfRecordsPerPage).ToList();

            return View(asks);
        }

        // GET: Admin/QuestionManage/Details/5
        public ActionResult Details(int ask)
        {
            Ask asks = db.Asks.Where(a => a.AskID == ask).FirstOrDefault();
            
            return View(asks);
        }

        // GET: Admin/QuestionManage/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            ViewBag.UserID = new SelectList(db.Users, "UserID", "Name");
            return View();
        }

        // POST: Admin/QuestionManage/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AskID,Title,Description,Content,CreateDate,ViewCount,FileName,FilePath,UserID,CategoryID")] Ask ask, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file == null)
                {
                    db.Asks.Add(ask);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    var myFile = file.FileName;
                    var path = "~/Content/Image/" + myFile;
                    file.SaveAs(Server.MapPath(path));
                    ask.FilePath = path;

                    db.Asks.Add(ask);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", ask.CategoryID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "Name", ask.UserID);
            
            return View(ask);
        }

        // GET: Admin/QuestionManage/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ask ask = db.Asks.Find(id);
            if (ask == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", ask.CategoryID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "Name", ask.UserID);
            return View(ask);
        }

        // POST: Admin/QuestionManage/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AskID,Title,Description,Content,CreateDate,ViewCount,CommentCount,FilePath,IsTrue,UserID,CategoryID")] Ask ask, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file == null)
                {
                    db.Asks.Add(ask);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    var myFile = file.FileName;
                    var path = "~/Content/Image/" + myFile;
                    file.SaveAs(Server.MapPath(path));
                    ask.FilePath = path;

                    db.Entry(ask).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", ask.CategoryID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "Name", ask.UserID);
            return View(ask);
        }

        // GET: Admin/QuestionManage/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ask ask = db.Asks.Find(id);
            if (ask == null)
            {
                return HttpNotFound();
            }
            return View(ask);
        }

        // POST: Admin/QuestionManage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ask ask = db.Asks.Find(id);
            db.Asks.Remove(ask);
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
