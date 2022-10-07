using ClosedXML.Excel;
using GIFanpage.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace GIFanpage.Areas.Admin.Controllers
{
    public class FeedbackUserController : Controller
    {
      
        private GIFanpageDbContext db = new GIFanpageDbContext();

        // GET: Feedbacks
        public ActionResult Index(string Search = "", int PageNo = 1)
        {
            //Search
            List<Feedback> feedbacks = db.Feedbacks.Where(s => s.User.Name.Contains(Search) || s.FeedbackTitle.Contains(Search) || s.FeedbackContent.Contains(Search)).ToList();

            if (feedbacks.Count() == 0)
            {
                ViewBag.Msg = "Data Not Found";
                return View();
            }

            // Total Users Account
            int TotalFeedback = feedbacks.Count();
            ViewBag.TotalFeedback = TotalFeedback;

            //Pagination

            int NoOfRecordsPerPage = 4;
            int NoOfPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(feedbacks.Count) / Convert.ToDouble(NoOfRecordsPerPage)));
            int NoOfRecordsToSkip = (PageNo - 1) * NoOfRecordsPerPage;
            ViewBag.PageNo = PageNo;
            ViewBag.NoOfPages = NoOfPages;
            feedbacks = feedbacks.Skip(NoOfRecordsToSkip).Take(NoOfRecordsPerPage).ToList();

            return View(feedbacks);
        }

        [HttpPost]
        public FileResult ExportToExcel()
        {
            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[4] {  
                                                     new DataColumn("FeedbackID"),
                                                     new DataColumn("Name"),
                                                     new DataColumn("FeedbackTitle"),
                                                     new DataColumn("FeedbackContent")
                                                     
                                                    });

            var feedbacks = from GIFanpagesDbContext in db.Feedbacks select GIFanpagesDbContext;

            foreach (var feedback in feedbacks)
            {
                dt.Rows.Add(feedback.FeedbackID, feedback.FeedbackTitle, feedback.FeedbackContent, feedback.User.Name);
            }

            using (XLWorkbook wb = new XLWorkbook()) //Install ClosedXml from Nuget for XLWorkbook  
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream()) //using System.IO;  
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Feedback about Fanpage.xlsx");
                }
            }
        }

        // GET: Feedbacks/Details/5
        public ActionResult Details(int feedback)
        {
            Feedback feedbacks = db.Feedbacks.Where(a => a.FeedbackID == feedback).FirstOrDefault();

            return View(feedbacks); ;
        }

        // GET: Feedbacks/Create
        public ActionResult Create()
        {

            return View();
        }

        // POST: Feedbacks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FeedbackID,FeedbackTitle,FeedbackContent,UserID")] Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                db.Feedbacks.Add(feedback);
                db.SaveChanges();
                return RedirectToAction("Index", "FeedbackUser");
            }

            return View(feedback);
        }

        

        // GET: Feedbacks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feedback feedback = db.Feedbacks.Find(id);
            if (feedback == null)
            {
                return HttpNotFound();
            }
            return View(feedback);
        }

        // POST: Feedbacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Feedback feedback = db.Feedbacks.Find(id);
            db.Feedbacks.Remove(feedback);
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