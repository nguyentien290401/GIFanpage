using GIFanpage.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
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

        public ActionResult EditUser(int? id)
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

        [HttpPost]
        public ActionResult EditUser(User user)
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
                return RedirectToAction("UserAccount", "Dashboard");
            }
            ViewBag.PlaystyleID = new SelectList(db.Playstyles, "PlaystyleID", "PlaystyleName", user.PlaystyleID);
            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "RoleName", user.RoleID);
            return View();
        }

        public ActionResult DetailUser(int? id)
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

        public ActionResult DeleteUser(int? id)
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

        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index", "Dashboard");
        }







        public ActionResult CharacterInfo()
        {
            return View(db.Characters.ToList());
        }

        public ActionResult CreateCharacter()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateCharacter([Bind(Include = "CharacterID,CharacterName,CharacterVision,CharacterDescription,CharacterRarity,CharacterRegion,CharacterBirthday,CharacterImageCard,CharacterImageOriginal")] Character character, HttpPostedFileBase fileCard, HttpPostedFileBase fileOriginal)
        {
            if (ModelState.IsValid)
            {
                if (fileCard == null || fileOriginal == null)
                {
                    db.Characters.Add(character);
                    db.SaveChanges();
                    return RedirectToAction("CharacterInfo", "Dashboard");
                }
                else
                {
                    // Define image card file
                    var myFileCard = fileCard.FileName;
                    var pathCard = "~/Content/Image/" + myFileCard;
                    fileCard.SaveAs(Server.MapPath(pathCard));
                    character.CharacterImageCard = pathCard;

                    // Define image original file
                    var myFileOriginal = fileOriginal.FileName;
                    var pathOriginal = "~/Content/Image/" + myFileOriginal;
                    fileOriginal.SaveAs(Server.MapPath(pathOriginal));
                    character.CharacterImageOriginal = pathOriginal;

                }

                db.Characters.Add(character);
                db.SaveChanges();
                return RedirectToAction("UserAccount", "Dashboard");
            }

            return View(character);
        }

        public ActionResult DetailCharacter(int character)
        {
            Character characters = db.Characters.Where(c => c.CharacterID == character).FirstOrDefault();

            return View(characters);
        }


        //public ActionResult EditCharacter(Character character, int char)
        //{
        //    Character characters = db.Characters.Where(c => c.CharacterID == char).FirstOrDefault();
        //    return View(character, characters = char);
        //}

        //[HttpPost]
        //public ActionResult EditCharacter([Bind(Include = "CharacterID,CharacterName,CharacterVision,CharacterDescription,CharacterRarity,CharacterRegion,CharacterBirthday,CharacterImageCard,CharacterImageOriginal")] Character character)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(character).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("CharacterInfo", "Dashboard");
        //    }
        //    return View(character);
        //}

        public ActionResult DeleteCharacter(int character)
        {
            Character characters = db.Characters.Where(c => c.CharacterID == character).FirstOrDefault();

            return View(character);
        }

        public ActionResult DeleteCharacterConfirmed(int? id)
        {
            Character character = db.Characters.Find(id);
            db.Characters.Remove(character);
            db.SaveChanges();
            return RedirectToAction("CharacterInfo");
        }












        public ActionResult NewInfo()
        {
            return View(db.News.ToList());
        }

        public ActionResult Question()
        {
            return View(db.Asks.ToList());
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