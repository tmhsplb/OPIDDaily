using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OPIDDaily.DAL;
using OPIDDaily.DataContexts;
using OPIDDaily.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static OPIDDaily.DataContexts.IdentityDB;

namespace OPIDDaily.Controllers
{
    public class UsersController : Controller
    {
        private static log4net.ILog Log = log4net.LogManager.GetLogger(typeof(UsersController));

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

                else if (IsInRole("Interviewer"))
                {
                    return RedirectToAction("Home", "Interviewer");
                }

                else if (IsInRole("BackOffice"))
                {
                    // Log.Debug("User is in role BackOffice. Redirect to Backoffice.Home");
                    return RedirectToAction("Home", "BackOffice");
                }

                else if (IsInRole("Client"))
                {
                    string userName = User.Identity.Name;
                    if (userName.Equals("Client1"))
                    {
                        return RedirectToAction("InitNowServing", "Client");
                    }
                    else if (userName.Equals("Client2"))
                    {
                        return RedirectToAction("InitNowServingHistory", "Client");
                    }
                   
                }

                else if (IsInRole("CaseManager"))
                {
                    return RedirectToAction("Home", "CaseManager");
                }

                ViewBag.Warning = "User in unrecognized role.";
            }
            else
            {
                ViewBag.Warning = "Not Logged In";
                return View();
            }

            Log.Warn("Return a warning");
            return View();
        }

        public ActionResult ManageUsers()
        {
            return View("Users");
        }

        // https://stackoverflow.com/questions/4101116/asp-net-mvc-post-call-returning-string-need-help-with-format-for-jqgrid/4102155#4102155
        public ActionResult GetAgencies()
        {
            List<AgencyViewModel> agencies = Agencies.GetAgencies(); 
            List<AgencyViewModel> activeAgencies = agencies.Where(a => a.IsActive == "Yes").ToList();

            var ls = activeAgencies.ToDictionary(a => a.AgencyId, a => a.AgencyName);

            return PartialView("_SelectAgency", ls);
        }


        public string ExtendInvitation(InvitationViewModel invite)
        {
            if (InUse(invite.UserName))
            {
                string status = string.Format("The user name {0} is already in use. Please use a different user name.", invite.UserName);
                return status;
            }

            Identity.ExtendInvitation(invite);

            return "Success";
        }

        public string EditUser(InvitationViewModel invite)
        {
            string status = Users.EditUser(invite);
            return status;
        }

        public JsonResult GetUsers(int page, int rows)
        {
            List<InvitationViewModel> invitations = Identity.GetUsers();

            int pageIndex = page - 1;
            int pageSize = rows;
            int totalRecords = invitations.Count;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            invitations = invitations.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = invitations
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}