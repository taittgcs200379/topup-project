using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using topup_project.Migrations;
using topup_project.Models;
using PagedList;
using System.Web.UI.WebControls;

namespace topup_project.Controllers
{
    public class IdeasController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Ideasy
        public ActionResult Index(int? page,int? pageSize,int? topicId = -1 )
        {
            
            if (topicId == -1)
                return View(db.Ideas.ToList());

            else
            {
                if(page==null)
                {
                    page = 1;
                }
                if(pageSize==null)
                {
                    pageSize = 5;
                }
                ViewBag.PageSize = pageSize;
                var topic = db.Topics.FirstOrDefault(t => t.Id == topicId);
                ViewData["topic"] = topic;
                return View(db.Ideas.Where(i => i.TopicId == topicId).ToList().ToPagedList((int)page, (int)pageSize));
            }
        }

        // GET: Ideas/Details/5
        public ActionResult Details(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Idea idea = db.Ideas.Find(id);
            
            if (idea == null)
            {
                return HttpNotFound();
            }

            var likes = db.Reacts.Where(r => r.IdeaId == id && r.Reaction == 1).ToList();
            var dislikes = db.Reacts.Where(r => r.IdeaId == id && r.Reaction == -1).ToList();
            var topic = db.Topics.FirstOrDefault(t => t.Id == idea.TopicId);
            //if (topic.LastDeadLine >= DateTime.Now) 
            
                ViewData["NumOfLike"] = likes.Count();
                ViewData["NumOfDislike"] = dislikes.Count();
                ViewData["CommentList"] = db.Comments.Where(c => c.IdeaId == id).ToList();
                ViewData["topic"] = topic;
            

            if (topic.LastDeadLine < DateTime.Now)
            {
                ViewData["OverDeadlineError"] = "Section is over.";
            }


            return View(idea);
        }

        // GET: Ideas/Create
        public ActionResult Create(int? topicId = -1)
        {
            ViewData["topicId"] = topicId;
            List<Category> categoryList = db.Categories.ToList();
            ViewBag.Departments = new SelectList(categoryList, "Id", "Name");
            return View();
        }

        // POST: Ideas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Text,DateTime,CategoryId,TopicId,IsAccepted")] Idea idea, HttpPostedFileBase file)
        {
            var topic = db.Topics.FirstOrDefault(t => t.Id == idea.TopicId);
           
            if (topic.FirstDeadLine >= DateTime.Now && ModelState.IsValid)
            {
                
                
                idea.UserId = User.Identity.GetUserId();
                idea.DateTime = DateTime.Now;
                idea.FilePath = "";

                db.Ideas.Add(idea);
                db.SaveChanges();

                var coordinatorId = db.Roles.Where(r => r.Name == "QA Coordinator").Select(r => r.Id).FirstOrDefault();
                var coordinators = db.Users.Where(u => u.Roles.Where(r => r.RoleId == coordinatorId).Any()).ToList();

                foreach(var coordinator in coordinators) {
                    SendEmail(coordinator.Email);
                }

                if (file != null && file.ContentLength > 0) {
                    string fileName = Path.GetFileName(file.FileName);
                    string filePath = Path.Combine(Server.MapPath("~/Content/Document/Topic"), idea.TopicId.ToString(), idea.Id.ToString());
                    if (!Directory.Exists(filePath)) {
                        Directory.CreateDirectory(filePath);
                    }
                    filePath = Path.Combine(filePath, fileName);
                    file.SaveAs(filePath);
                    idea.FilePath = filePath;
                    
                    db.Ideas.AddOrUpdate(idea);
                    db.SaveChanges();
                }
                
                return RedirectToAction("Index", new { topicId = idea.TopicId });
            }

            if (topic.FirstDeadLine < DateTime.Now)
            {   
                ViewData["OverDeadlineError"] = "Deadline is over.";
            }
            

            ViewData["topicId"] = idea.TopicId;
            List<Category> categoryList = db.Categories.ToList();
            ViewBag.Departments = new SelectList(categoryList, "Id", "Name");

            return View(idea);
        }

        // GET: Ideas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Idea idea = db.Ideas.Find(id);
            if (idea == null)
            {
                return HttpNotFound();
            }
            return View(idea);
        }

        // POST: Ideas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Text,FilePath,DateTime,UserId,CategoryId,TopicId,IsAccepted")] Idea idea)
        {
            if (ModelState.IsValid)
            {
                db.Entry(idea).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(idea);
        }

        // GET: Ideas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Idea idea = db.Ideas.Find(id);
            if (idea == null)
            {
                return HttpNotFound();
            }
            return View(idea);
        }

        // POST: Ideas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Idea idea = db.Ideas.Find(id);
            db.Ideas.Remove(idea);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public void ExportToExcel(int topicId)
        {
            var ideas = db.Ideas.Where(i => i.TopicId == topicId).ToList();

            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Idea");
            ws.Cells["A1"].Value = "Id";
            ws.Cells["B1"].Value = "DateTime";
            ws.Cells["C1"].Value = "UserId";
            ws.Cells["D1"].Value = "CategoryId";
            ws.Cells["E1"].Value = "TopicId";
            int rowStart = 2;
            foreach(var item in ideas)
            {
               

                ws.Cells[string.Format("A{0}", rowStart)].Value = item.Id.ToString();
                ws.Cells[string.Format("B{0}", rowStart)].Value = item.DateTime.ToString();
                ws.Cells[string.Format("C{0}", rowStart)].Value = item.UserId.ToString();
                ws.Cells[string.Format("D{0}", rowStart)].Value = item.CategoryId.ToString();
                ws.Cells[string.Format("E{0}", rowStart)].Value = item.TopicId.ToString();
                rowStart++;
            }
            ws.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment:filename" + "IdeaReport.xlsx");
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();

        }
        public ActionResult ExportZIP(int topicId)
        {
            var path = Path.Combine(Server.MapPath("~/Content/Document/Topic"), topicId.ToString());

            if (Directory.Exists(path))
            {
                var zipPath = Path.Combine(Server.MapPath("~/Content/Document/Topic"), $"topic_{topicId}.zip");

                ZipFile.CreateFromDirectory(path, zipPath);

                byte[] fileBytes = System.IO.File.ReadAllBytes(zipPath);

                System.IO.File.Delete(zipPath);

                return File(fileBytes, MediaTypeNames.Application.Zip, Path.GetFileName(zipPath));
            }

            return Redirect("/topics");
        }
        public static bool SendEmail(string _to)
        {
            string _from = "tantait83@gmail.com";
            string _password = "bogheenucicupgyi";

            string _subject = $"New idea has been created ";
            string _body = $"new status coming";


            MailMessage message = new MailMessage(
                from: _from,
                to: _to,
                subject: _subject,
                body: _body
            );

            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;
            message.ReplyToList.Add(new MailAddress(_from));
            message.Sender = new MailAddress(_from);

            try
            {
                SmtpClient client = new SmtpClient("smtp.gmail.com");
                client.Port = 587;
                client.Credentials = new NetworkCredential(_from, _password);
                client.EnableSsl = true;
                client.Send(message);
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
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
