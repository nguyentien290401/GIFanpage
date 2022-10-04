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
    public class CharacterInfoController : Controller
    {
        private GIFanpageDbContext db = new GIFanpageDbContext();

        // GET: Admin/CharacterInfo
        public ActionResult Index(string Search = "", int PageNo = 1)
        {
            //Search
            List<Character> characters = db.Characters.Where(s => s.CharacterName.Contains(Search) || s.CharacterRarity.Contains(Search) || s.CharacterVision.Contains(Search) || s.CharacterRegion.Contains(Search)).ToList();

            if (characters.Count() == 0)
            {
                ViewBag.Msg = "Data Not Found";
                return View();
            }

            // Total Users Account
            int TotalCharacter = characters.Count();
            ViewBag.TotalCharacter = TotalCharacter;

            //Pagination

            int NoOfRecordsPerPage = 4;
            int NoOfPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(characters.Count) / Convert.ToDouble(NoOfRecordsPerPage)));
            int NoOfRecordsToSkip = (PageNo - 1) * NoOfRecordsPerPage;
            ViewBag.PageNo = PageNo;
            ViewBag.NoOfPages = NoOfPages;
            characters = characters.Skip(NoOfRecordsToSkip).Take(NoOfRecordsPerPage).ToList();

            return View(characters);
        }

        // GET: Admin/CharacterInfo/Details/5
        public ActionResult Details(int character)
        {
            Character characters = db.Characters.Where(c => c.CharacterID == character).FirstOrDefault();

            return View(characters);
        }

        // GET: Admin/CharacterInfo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/CharacterInfo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CharacterID,CharacterName,CharacterVision,CharacterDescription,CharacterRarity,CharacterRegion,CharacterBirthday,CharacterImageCard,CharacterImageOriginal")] Character character, HttpPostedFileBase fileCard, HttpPostedFileBase fileOriginal)
        {
            if (ModelState.IsValid)
            {
                if (fileCard == null || fileOriginal == null)
                {
                    db.Characters.Add(character);
                    db.SaveChanges();
                    return RedirectToAction("Index");
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
                return RedirectToAction("Index");
            }

            return View(character);
        }

        // GET: Admin/CharacterInfo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Character character = db.Characters.Find(id);
            if (character == null)
            {
                return HttpNotFound();
            }
            return View(character);
        }

        // POST: Admin/CharacterInfo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CharacterID,CharacterName,CharacterVision,CharacterDescription,CharacterRarity,CharacterRegion,CharacterBirthday,CharacterImageCard,CharacterImageOriginal")] Character character)
        {
            if (ModelState.IsValid)
            {
                db.Entry(character).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(character);
        }

        // GET: Admin/CharacterInfo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Character character = db.Characters.Find(id);
            if (character == null)
            {
                return HttpNotFound();
            }
            return View(character);
        }

        // POST: Admin/CharacterInfo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Character character = db.Characters.Find(id);
            db.Characters.Remove(character);
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
