﻿using System;
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
    public class AsksController : Controller
    {
        private GIFanpageDbContext db = new GIFanpageDbContext();

        // GET: List of all questions
        public ActionResult Index()
        {
            var asks = db.Asks.Include(a => a.Category).Include(a => a.User).ToList();   /*.Include(a => a.Submission)*/
            return View(asks);
        }

        // Get: All questions of each accounts
        public ActionResult IndexQuestions(int id)
        {
            var asks = db.Asks.Where(t => t.UserID == id).ToList();
            return View(asks);
        }

        
        // GET: Asks/Details/5
        public ActionResult Details(int? ask)
        {
            if (ask == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Ask asks = db.Asks.Find(ask);
            asks.ViewCount++;
            db.SaveChanges();
            if (asks == null)
            {
                return HttpNotFound();
            }
            return View(asks);
        }

        [HttpPost]
        public ActionResult AddComment(Comment comment, int cmtID)
        {
            Ask ask = db.Asks.Where(i => i.AskID == comment.AskID).FirstOrDefault();
            if (ask != null)
            {
                ask.CommentCount++;
                db.Comments.Add(comment);
                db.SaveChanges();
            }

            return RedirectToAction("Details", "Asks", new { ask = cmtID });
        }

        // GET: Asks/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            ViewBag.UserID = Session["CurrentUserID"];
            //ViewBag.SubmissionID = new SelectList(db.Submissions, "SubmissionID", "SubmissionName");
           // ViewBag.UserID = new SelectList(db.Users, "UserID", "Name");
            return View();
        }

        // POST: Asks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AskID,Title,Description,Content,CreateDate,ViewCount,VotesCount,FileName,FilePath,CurrentUserVoteType,UserID,CategoryID,SubmissionID")] Ask ask)
        {
            if (ModelState.IsValid)
            {
                db.Asks.Add(ask);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", ask.CategoryID);
            //ViewBag.SubmissionID = new SelectList(db.Submissions, "SubmissionID", "SubmissionName", ask.SubmissionID);*/
            ViewBag.UserID = new SelectList(db.Users, "UserID", "Name", ask.UserID);
            return View(ask);
        }

        // GET: Asks/Edit/5
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
            //ViewBag.SubmissionID = new SelectList(db.Submissions, "SubmissionID", "SubmissionName", ask.SubmissionID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "Name", ask.UserID);
            return View(ask);
        }

        // POST: Asks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AskID,Title,Description,Content,CreateDate,ViewCount,CommentCount,VotesCount,FileName,FilePath,CurrentUserVoteType,UserID,CategoryID,SubmissionID")] Ask ask)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ask).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", ask.CategoryID);
            //ViewBag.SubmissionID = new SelectList(db.Submissions, "SubmissionID", "SubmissionName", ask.SubmissionID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "Name", ask.UserID);
            return View(ask);
        }

        // GET: Asks/Delete/5
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

        // POST: Asks/Delete/5
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
