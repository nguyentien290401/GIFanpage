using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GIFanpage.Models;
using System.IO;

namespace GIFanpage.Controllers
{
    public class UsersController : Controller
    {
        private GIFanpageDbContext db = new GIFanpageDbContext();

        

        // GET: Users/Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login([Bind(Include = "Email,PasswordHash")] User user)
        {
            // Add Validation
            var usr = db.Users.Where(u => u.Email == user.Email && u.PasswordHash == user.PasswordHash).FirstOrDefault();
            if (usr != null)
            {
                Session["CurrentUserID"] = usr.UserID;
                Session["CurrentUserName"] = usr.Name;
                Session["CurrentUserImg"] = usr.UserImg;
                Session["CurrentUserRoleID"] = usr.RoleID;
                Session["CurrentUserLike"] = 0;
                if (usr.RoleID == 1)
                {
                    Session["IsAdmin"] = true;
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                else if (usr.RoleID == 2)
                {
                    return RedirectToAction("Index", "Home");
                }

            }
            else
            {
                ModelState.AddModelError("", "Name or Password is wrong.");

            }
            return View();
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }



        // GET: Users
        public ActionResult Index()
        {
            var users = db.Users.Include(u => u.Playstyle).Include(u => u.Role);
            return View(users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Register()
        {
            User user = new User();
            ViewBag.PlaystyleID = new SelectList(db.Playstyles, "PlaystyleID", "PlaystyleName");
            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "RoleName");
            return View(user);
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(/*[Bind(Include = "UserID,Name,Email,PasswordHash,UserImg,PlaystyleID,RoleID")]*/ User user)
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
                return RedirectToAction("Index", "Home");
            }

            ViewBag.PlaystyleID = new SelectList(db.Playstyles, "PlaystyleID", "PlaystyleName", user.PlaystyleID);
            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "RoleName", user.RoleID);
            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.PlaystyleID = new SelectList(db.Playstyles, "PlaystyleID", "PlaystyleName", user.PlaystyleID);
            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "RoleName", user.RoleID);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(/*[Bind(Include = "UserID,Name,Email,UserImg,PlaystyleID,RoleID")]*/ User user)
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

                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PlaystyleID = new SelectList(db.Playstyles, "PlaystyleID", "PlaystyleName", user.PlaystyleID);
            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "RoleName", user.RoleID);
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
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
