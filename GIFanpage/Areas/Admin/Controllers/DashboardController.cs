using GIFanpage.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GIFanpage.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        private GIFanpageDbContext db = new GIFanpageDbContext();

        // GET: Admin/Dashboard
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserAccount()
        {
            var users = db.Users.Where(t => t.RoleID != 1).ToList();
            return View(users.ToList());
        }

        public ActionResult CreateUser()
        {
            User user = new User();
            ViewBag.PlaystyleID = new SelectList(db.Playstyles, "PlaystyleID", "PlaystyleName");
            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "RoleName");

            return View(user);

        }

        public ActionResult EditUser()
        {

        }

        [HttpPost]
        public ActionResult CreateUser(User user)
        {
            if (ModelState.IsValid)
            {
                if (user.ImageUpload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(user.ImageUpload.FileName);
                    string extension = Path.GetExtension(user.ImageUpload.FileName);
                    fileName = fileName + extension;
                    user.UserImg = "~/Content/Image/" + fileName;
                    user.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/Content/Image/"), fileName));
                }

                db.Users.Add(user);
                db.SaveChanges();

                Session["CurrentUserID"] = user.UserID;
                Session["CurrentUserName"] = user.Name;
                Session["CurrentUserEmail"] = user.Email;
                Session["CurrentUserPassword"] = user.PasswordHash;
                Session["CurrentUserRoleID"] = user.RoleID;
                Session["CurrentUserLike"] = 0;
                return RedirectToAction("UserAccount", "Dashboard");
            }

            ViewBag.PlaystyleID = new SelectList(db.Playstyles, "PlaystyleID", "PlaystyleName", user.PlaystyleID);
            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "RoleName", user.RoleID);
            return View(user);
        }


        public ActionResult CharacterInfo()
        {
            return View(db.Characters.ToList());
        }

        public ActionResult NewInfo()
        {
            return View(db.News.ToList());
        }

        public ActionResult Question()
        {
            return View(db.Asks.ToList());
        }
    }
}