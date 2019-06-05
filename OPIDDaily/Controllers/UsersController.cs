using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OPIDDaily.DataContexts;
using OPIDDaily.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OPIDDaily.Controllers
{
    public class UsersController : Controller
    {

        public bool IsInRole(string role)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                IdentityDB context = new IdentityDB();
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var s = UserManager.GetRoles(user.GetUserId());

                if (s[0].ToString() == role)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        public bool InUse(string userName)
        {
            using (IdentityDB identitycontext = new IdentityDB())
            {
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(identitycontext));
                ApplicationUser user = UserManager.FindByName(userName);

                if (user != null)
                {
                    return true;
                }
            }

            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                var user = opiddailycontext.Invitations.Where(i => i.UserName == userName).SingleOrDefault();

                if (user != null)
                {
                    return true;
                }
            }
            
            return false;
        }

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;

                if (IsInRole("SuperAdmin"))
                {
                    return RedirectToAction("Home", "Superadmin");
                }

                else if (IsInRole("FrontDesk"))
                {
                    return RedirectToAction("Home", "FrontDesk");
                }

                ViewBag.Warning = "User in unrecognized role.";
            }
            else
            {
                ViewBag.Warning = "Not Logged In";
            }

            return View();
        }
    }
}