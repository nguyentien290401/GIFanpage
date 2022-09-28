using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using GIFanpage.Models;

namespace GIFanpage.Controllers
{
    public class QuizsController : Controller
    {
        private GIFanpageDbContext db = new GIFanpageDbContext();

        // GET: Quizs
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddQuizCategory()
        {
            if (Session["CurrentUserID"] != null)
            {
                int userID = Convert.ToInt32(Session["CurrentUserID"].ToString());

                List<QuizCategory> quizCategoryList = db.QuizCategories.Where(q => q.UserID == userID).OrderBy(q => q.QuizCategoryID).ToList();
                ViewData["quizCategoryList"] = quizCategoryList;

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Users");
            }
        }

        [HttpPost]
        public ActionResult AddQuizCategory(QuizCategory quizCategory)
        {
            List<QuizCategory> quizCategoryList = db.QuizCategories.OrderByDescending(q => q.QuizCategoryID).ToList();
            ViewData["quizCategoryList"] = quizCategoryList;
            QuizCategory db_Q_Category = new QuizCategory();

            Random r = new Random();

            db_Q_Category.QuizCategoryName = quizCategory.QuizCategoryName;

            db_Q_Category.UserID = Convert.ToInt32(Session["CurrentUserID"].ToString());
            db_Q_Category.QuizCategoryPassword = r.Next().ToString();

            db.QuizCategories.Add(db_Q_Category);
            db.SaveChanges();

            return RedirectToAction("AddQuizCategory", "Quizs");
        }

        public ActionResult AddQuizQuestion()
        {
            if (Session["CurrentUserID"] != null)
            {
                int userID = Convert.ToInt32(Session["CurrentUserID"].ToString());

                List<QuizCategory> quizCategoryList = db.QuizCategories.Where(q => q.UserID == userID).OrderBy(q => q.QuizCategoryID).ToList();
                ViewBag.quizList = new SelectList(quizCategoryList, "QuizCategoryID", "QuizCategoryName");

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Users");
            }

        }

        [HttpPost]
        public ActionResult AddQuizQuestion(QuizQuestion quizQuestion)
        {
            if (Session["CurrentUserID"] != null)
            {
                int userID = Convert.ToInt32(Session["CurrentUserID"].ToString());

                List<QuizCategory> quizCategoryList = db.QuizCategories.Where(q => q.UserID == userID).ToList();
                ViewBag.quizList = new SelectList(quizCategoryList, "QuizCategoryID", "QuizCategoryName");

                QuizQuestion db_Q_Question = new QuizQuestion();

                db_Q_Question.QuizQuestionContent = quizQuestion.QuizQuestionContent;

                db_Q_Question.QA = quizQuestion.QA;
                db_Q_Question.QB = quizQuestion.QB;
                db_Q_Question.QC = quizQuestion.QC;
                db_Q_Question.QD = quizQuestion.QD;

                db_Q_Question.CorrectAnswer = quizQuestion.CorrectAnswer;
                db_Q_Question.QuizCategoryID = quizQuestion.QuizCategoryID;

                db.QuizQuestions.Add(db_Q_Question);
                db.SaveChanges();

                TempData["msg"] = "Question Successfully Added";
                TempData.Keep();

                return RedirectToAction("AddQuizQuestion", "Quizs");
            }
            else
            {
                return RedirectToAction("Login", "Users");
            }
        }

        public ActionResult QuizExam()
        {
            return View();
        }

        [HttpPost]
        public ActionResult QuizExam(String roomPassword)
        {
            List<QuizCategory> quizCategoryList = db.QuizCategories.ToList();
            foreach (var item in quizCategoryList)
            {
                if (item.QuizCategoryPassword == roomPassword)
                {
                    List<QuizQuestion> quizQuestionList = db.QuizQuestions.Where(q => q.QuizCategoryID == item.QuizCategoryID).ToList();
                    Queue<QuizQuestion> queueQuizQuestion = new Queue<QuizQuestion>();
                    foreach (QuizQuestion quizQuestion in quizQuestionList)
                    {
                        queueQuizQuestion.Enqueue(quizQuestion);
                    }

                    TempData["ExamQuizID"] = item.QuizCategoryID;
                    TempData["ExamQuizQuestion"] = queueQuizQuestion;

                    TempData.Keep();
                    return RedirectToAction("StartQuiz");
                }
                else
                {
                    ViewBag.error = "No room found...";
                }
            }

            return View();
        }

    }
}