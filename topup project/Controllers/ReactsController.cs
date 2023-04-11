using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using topup_project.Migrations;
using topup_project.Models;

namespace topup_project.Controllers
{
    public class ReactsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Reacts
        public ActionResult Index(int? ideaId = -1)
        {
            if (ideaId == -1)
                return View(db.Reacts.ToList());
            else
            {
                var idea = db.Ideas.FirstOrDefault(t => t.Id == ideaId);
                ViewData["idea"] = idea;
                return View(db.Reacts.Where(i => i.IdeaId == ideaId).ToList());
            }
        }

        // GET: Reacts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            React react = db.Reacts.Find(id);
            if (react == null)
            {
                return HttpNotFound();
            }
            return View(react);
        }

        // GET: Reacts/Create
        public ActionResult Create(int? ideaId = -1)
        {
            ViewData["ideaId"] = ideaId;
            return View();
        }

        // POST: Reacts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Reaction,IdeaId")] React react)
        {
            if (ModelState.IsValid)
            {
                react.UserId = User.Identity.GetUserId();
                db.Reacts.Add(react);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(react);
        }

        // GET: Reacts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            React react = db.Reacts.Find(id);
            if (react == null)
            {
                return HttpNotFound();
            }
            return View(react);
        }

        // POST: Reacts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Reaction,UserId,IdeaId")] React react)
        {
            if (ModelState.IsValid)
            {
                db.Entry(react).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(react);
        }

        // GET: Reacts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            React react = db.Reacts.Find(id);
            if (react == null)
            {
                return HttpNotFound();
            }
            return View(react);
        }

        // POST: Reacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            React react = db.Reacts.Find(id);
            db.Reacts.Remove(react);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Like (int? ideaId = -1)
        {
            

            var userId = User.Identity.GetUserId();

            var react = db.Reacts.Where(r => r.UserId == userId && r.IdeaId == ideaId).FirstOrDefault();

            if (null == react) {
                react = new React
                {
                    Reaction = 1,
                    UserId = userId,
                    IdeaId = (int) ideaId
                };

                db.Reacts.Add(react);
            } else {
                react.Reaction = react.Reaction == 1 ? 0 : 1;
                db.Reacts.AddOrUpdate(react);
            }

            db.SaveChanges();

            return RedirectToAction("Details", "Ideas", new { id = react.IdeaId });
        }
        public ActionResult DisLike(int? ideaId = -1)
        {


            var userId = User.Identity.GetUserId();

            var react = db.Reacts.Where(r => r.UserId == userId && r.IdeaId == ideaId).FirstOrDefault();

            if (null == react)
            {
                react = new React
                {
                    Reaction = -1,
                    UserId = userId,
                    IdeaId = (int)ideaId
                };

                db.Reacts.Add(react);
            }
            else
            {
                react.Reaction = react.Reaction == -1 ? 0 : -1;
                db.Reacts.AddOrUpdate(react);
            }

            db.SaveChanges();

            return RedirectToAction("Details", "Ideas", new { id = react.IdeaId });
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
