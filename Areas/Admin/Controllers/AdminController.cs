using Food_Sharing_Food.Areas.Admin.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web;
using Microsoft.AspNet.Identity;

namespace Food_Sharing_Food.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {

        private readonly ApplicationDbContext context;
        public AdminController()
        {
            context = new ApplicationDbContext();
        }
        // GET: Admin/Admin
        public ActionResult Index()
        {
            return View();
        }


        [Authorize]
        public ActionResult AccountUser()
        {
            var usernonRole = context.Users
                .Where(user => !context.Roles.Any(role => role.Id == user.Id || user.Email == "admin@gmail.com"))
                .ToList();

            return View(usernonRole);
        }
        [Authorize]
        public ActionResult AccountRole()
        {
            var usernonRole = context.Users
                .Where(user => !context.Roles.Any(role => role.Id == user.Id || user.Email != "admin@gmail.com"))
                .ToList();
            return View(usernonRole);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAccountUserConfirm(string id)
        {
            var user = context.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            context.Users.Remove(user);
            context.SaveChanges();
            return RedirectToAction("AccountUser", "Admin");
        }
    }
}