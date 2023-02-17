using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using topup_project.Models;

namespace topup_project.Controllers
{
    public class UserController : Controller
    {
        private ApplicationUserManager _userManager;


        public UserController()
        {
        }

        public UserController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().Get<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: Role
        public ActionResult Index()
        {
            List<User> list = new List<User>();
            foreach (var user in UserManager.Users)
                list.Add(new User(user));

            return View(list);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Create(User model)
        {
            var user = new ApplicationUser() { UserName = model.Name };
            await UserManager.CreateAsync(user);
            return RedirectToAction("Index");
        }
        
        public async Task<ActionResult> Details(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            return View(new User(user));
        }
        public async Task<ActionResult> Delete(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            return View(new User(user));
        }
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            await UserManager.DeleteAsync(user);

            return RedirectToAction("Index");
        }
    }
}
