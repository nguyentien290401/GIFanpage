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
    public class AsksController : Controller
    {
        private GIFanpageDbContext db = new GIFanpageDbContext();

        // GET: List of all questions
        public ActionResult Index()
        {
            var asks = db.Asks.Include(a => a.Category).Include(a => a.User).ToList();   /*.Include(a => a.Submission)*/
            return View(asks);
        }

        //Get: Index questions of user role
        public ActionResult IndexQuestionView(string Search /*int? page, string sortColumn = ""*/)
        {
           
            List <Ask> asks = db.Asks.Include(a => a.Category).Include(a => a.User).ToList();

            //Search
            if (Search != null)
            {
                var findAsk = db.Asks.Where(s => s.Title.Contains(Search)).Include(a => a.Category).Include(a => a.User).ToList();
                if (asks.Count == 0)
                {
                    ViewBag.Msg = "Data Not Found";
                    return View();
                }
                else
                {
                    var findAskDate = findAsk.OrderByDescending(f => f.ViewCount).ToList();
                    return View(findAskDate);
                }
            }

            ////Pagnitation 
            //if (page > 0)
            //{
            //    page = page;
            //}
            //else
            //{
            //    page = 1;                       //Set page default 
            //}
            //int limit = 4;                      //Display show 4 quesitons
            //int start = (int)(page - 1) * limit;
            //int totalQuestion = asks.Count();
            //ViewBag.totalQuestion = totalQuestion;
            //ViewBag.pageCurrent = page;
            //float numberPage = (float)totalQuestion / limit;
            //ViewBag.numberPage = (int)Math.Ceiling(numberPage);
            var asksQuestion = asks.OrderByDescending(a => a.ViewCount).ToList();            /*.Skip(start).Take(limit);*/

            ////Sorting 
            //ViewBag.sortColumn = sortColumn;
            //if (sortColumn == "TopView")
            //{
            //    asksQuestion = asks.OrderByDescending(i => i.ViewCount).ToList();
            //}
            //else if (sortColumn == "Newest")
            //{
            //    asksQuestion = asks.OrderByDescending(i => i.CreateDate).ToList();
            //}

            return View(asksQuestion);
        }

        // Get: All questions of each accounts
        public ActionResult IndexQuestions(int id, int? page)
        {
            var asks = db.Asks.Where(t => t.UserID == id);
            if (page > 0)
            {
                page = page;
            }
            else
            {
                page = 1;                       //Set page default 
            }
            int limit = 4;                      //Display show 4 quesitons
            int start = (int)(page - 1) * limit;
            int totalQuestion = asks.Count();
            ViewBag.totalQuestion = totalQuestion;
            ViewBag.pageCurrent = page;
            float numberPage = (float)totalQuestion / limit;
            ViewBag.numberPage = (int)Math.Ceiling(numberPage);
            var asksQuestion = asks.OrderByDescending(a => a.AskID).Skip(start).Take(limit);
            return View(asksQuestion.ToList());
        }


        // GET: Asks/Details/5
        public ActionResult Details(int ask)
        {
            
            Ask asks = db.Asks.Where(a => a.AskID == ask).FirstOrDefault();
            asks.ViewCount++;

            db.SaveChanges();

            //// Check Like
            for (int i = 0; i < asks.Comments.Count; i++)
            {
                var id = asks.Comments[i].CommentID;
                var user = Convert.ToInt32(Session["CurrentUserID"]);
                Vote vt = db.Votes.Where(v => v.CommentID == id && v.UserID == user).FirstOrDefault();
                if (vt != null)
                {
                    asks.Comments[i].CurrentUserVoteType = vt.VoteValue;
                }
                else
                {
                    asks.Comments[i].CurrentUserVoteType = 0;
                }
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

        public ActionResult EditComment(Comment cm, int cmtID)
        {
            Comment comment = db.Comments.Where(c => c.CommentID == cm.CommentID).FirstOrDefault();
            if (comment != null)
            {
                comment.Content = cm.Content;
                comment.CommentDate = cm.CommentDate;
                comment.UserID = cm.UserID;
                comment.AskID = cm.AskID;
                db.SaveChanges();
            }

            return RedirectToAction("Details", "Asks", new { ask = cmtID });
        }

        [HttpPost]
        public ActionResult AddSubComment(Comment comment, SubComment subComment, int askID, int subCmtID)
        {
            Ask ask = db.Asks.Where(i => i.AskID == comment.AskID).FirstOrDefault();
            Comment cmt = db.Comments.Where(i => i.CommentID == subComment.CommentID).FirstOrDefault();
            
            if (ask != null && cmt != null)
            {
                cmt.SubCommentCount++;
                db.SubComments.Add(subComment);
                db.SaveChanges();
            }

            return RedirectToAction("Details", "Asks", new { ask = askID, cmt = subCmtID });
        }

        public ActionResult EditSubComment(SubComment subCm, int askID, int subCmtID)
        {
            SubComment subComment = db.SubComments.Where(c => c.SubCommentID == subCm.SubCommentID).FirstOrDefault();
            if (subComment != null)
            {
                subComment.Content = subCm.Content;
                subComment.SubCommentDate = subCm.SubCommentDate;
                subComment.UserID = subCm.UserID;
                subComment.CommentID = subCm.CommentID;
                db.SaveChanges();
            }

            return RedirectToAction("Details", "Asks", new { ask = askID, cmt = subCmtID });
        }

        [HttpPost]
        public ActionResult Like(int ask, int cmt, int user, Vote vote)
        {
            Ask question = db.Asks.Where(q => q.AskID == ask).FirstOrDefault();
            Comment comment = db.Comments.Where(i => i.CommentID == cmt).FirstOrDefault();
            Vote vt = db.Votes.Where(v => v.UserID == user && v.CommentID == vote.CommentID).FirstOrDefault();
            comment.VotesCount++;
            if (vt != null)
            {
                vt.VoteValue = vote.VoteValue;
                comment.CurrentUserVoteType = vote.VoteValue;
            }
            else
            {
                db.Votes.Add(vote);
                comment.CurrentUserVoteType = vote.VoteValue;
            }

            db.SaveChanges();

            return RedirectToAction("Details", "Asks", new { ask = ask });
        }

        [HttpPost]
        public ActionResult DisLike(int ask, int Cmt, int user, Vote vote)
        {
            Ask question = db.Asks.Where(q => q.AskID == ask).FirstOrDefault();
            Comment comment = db.Comments.Where(i => i.CommentID == Cmt).FirstOrDefault();
            Vote vt = db.Votes.Where(v => v.UserID == user && v.CommentID == vote.CommentID).FirstOrDefault();
            comment.VotesCount--;
            if (vt != null)
            {
                vt.VoteValue = vote.VoteValue;
                comment.CurrentUserVoteType = vote.VoteValue;
            }
            else
            {
                db.Votes.Add(vote);
                comment.CurrentUserVoteType = vote.VoteValue;
            }

            db.SaveChanges();

            return RedirectToAction("Details", "Asks", new { ask = ask });
        }

        public ActionResult check(int ask, int Cmt)
        {
            Ask asks = db.Asks.Where(a => a.AskID == ask).FirstOrDefault();
            Comment comment = db.Comments.Where(i => i.CommentID == Cmt).FirstOrDefault();

            asks.IsTrue = true;
            comment.IsTrue = true;
            db.SaveChanges();

            return RedirectToAction("Details", "Asks", new { ask = ask });
        }

        public ActionResult unCheck(int ask, int Cmt)
        {
            Ask asks = db.Asks.Where(a => a.AskID == ask).FirstOrDefault();
            Comment comment = db.Comments.Where(i => i.CommentID == Cmt).FirstOrDefault();

            asks.IsTrue = false;
            comment.IsTrue = false;
            db.SaveChanges();

            return RedirectToAction("Details", "Asks", new { ask = ask });
        }

        // GET: Asks/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            ViewBag.UserID = Session["CurrentUserID"];
            //ViewBag.UserID = new SelectList(db.Users, "UserID", "UserImg");
            //ViewBag.SubmissionID = new SelectList(db.Submissions, "SubmissionID", "SubmissionName");
            // ViewBag.UserID = new SelectList(db.Users, "UserID", "Name");
            return View();
        }

        // POST: Asks/Create
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
                    return RedirectToAction("IndexQuestionView", "Asks");
                }   
                else
                {
                    var myFile = file.FileName;
                    var path = "~/Content/Image/" + myFile;
                    file.SaveAs(Server.MapPath(path));
                    ask.FilePath = path;

                    db.Asks.Add(ask);
                    db.SaveChanges();
                    return RedirectToAction("IndexQuestionView", "Asks");
                }    
                
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", ask.CategoryID);
            //ViewBag.SubmissionID = new SelectList(db.Submissions, "SubmissionID", "SubmissionName", ask.SubmissionID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "Name", ask.UserID);
            //ViewBag.UserID = new SelectList(db.Users, "UserID", "UserImg", ask.UserID);
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
            //ViewBag.UserID = new SelectList(db.Users, "UserID", "UserImg", ask.UserID);
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
