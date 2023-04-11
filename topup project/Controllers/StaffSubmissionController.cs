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
using topup_project.Models;

namespace topup_project.Controllers
{
    public class StaffSubmissionController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Topics
        public ActionResult Index()
        {
            return View(db.Topics.ToList());
        }

        public ActionResult Static()
        {
            var ideaByDepartment = new List<KeyValuePair<string, int>>();
            var departments = db.Departments.OrderBy(d => d.Name).ToList();

            foreach(var department in departments)
            {
                var userIds = db.Users.Where(u => u.DepartmentId == department.Id).Select(u => u.Id).ToList();
                var ideas = db.Ideas.Where(i => userIds.Contains(i.UserId)).ToList();

                var value = new KeyValuePair<string, int>(department.Name, ideas.Count());
                ideaByDepartment.Add(value);
            }

            ViewData["ideaByDepartment"] = ideaByDepartment;

            return View();
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
