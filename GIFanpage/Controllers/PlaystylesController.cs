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
    public class PlaystylesController : Controller
    {
        private GIFanpageDbContext db = new GIFanpageDbContext();

        // GET: Playstyles
        public ActionResult Index()
        {
            return View(db.Playstyles.ToList());
        }

        // GET: Playstyles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Playstyle playstyle = db.Playstyles.Find(id);
            if (playstyle == null)
            {
                return HttpNotFound();
            }
            return View(playstyle);
        }

        // GET: Playstyles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Playstyles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PlaystyleID,PlaystyleName")] Playstyle playstyle)
        {
            if (ModelState.IsValid)
            {
                db.Playstyles.Add(playstyle);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(playstyle);
        }

        // GET: Playstyles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Playstyle playstyle = db.Playstyles.Find(id);
            if (playstyle == null)
            {
                return HttpNotFound();
            }
            return View(playstyle);
        }

        // POST: Playstyles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PlaystyleID,PlaystyleName")] Playstyle playstyle)
        {
            if (ModelState.IsValid)
            {
                db.Entry(playstyle).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(playstyle);
        }

        // GET: Playstyles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Playstyle playstyle = db.Playstyles.Find(id);
            if (playstyle == null)
            {
                return HttpNotFound();
            }
            return View(playstyle);
        }

        // POST: Playstyles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Playstyle playstyle = db.Playstyles.Find(id);
            db.Playstyles.Remove(playstyle);
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
