using Microsoft.AspNet.Identity.EntityFramework;
using OPIDDaily.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OPIDDaily.Models;
using OPIDDaily.DAL;

namespace OPIDDaily.Controllers
{
    public class RoleController : Controller
    {
        IdentityDB context;

        public RoleController()
        {
            context = new IdentityDB();
        }

        /// <summary>
        /// Get All Roles
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //    var roles = context.Roles.ToList();
            //    return View(roles);
            return View("Roles");
        }

        public JsonResult GetRoles(int page, int rows)
        {
            List<RoleViewModel> roles = DataManager.GetRoles();

            var jsonData = new
            {
                total = 1,
                page,
                records = roles.Count,
                rows = roles
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public string AddRole(string name)
        {
            IdentityRole role = new IdentityRole();
            role.Name = name;
            context.Roles.Add(role);
            context.SaveChanges();
            return "Success";
        }

        public string EditRole(string id, string name)
        {
            IdentityRole role = context.Roles.Find(id);

            if (role != null)
            {
                role.Name = name;
                context.SaveChanges();
                return "Success";
            }

            return "Failure";
        }

        public string DeleteRole(string id)
        {
            IdentityRole role = context.Roles.Find(id);

            if (role != null)
            {
                context.Roles.Remove(role);
                context.SaveChanges();
                return "Success";
            }

            return "Failure";
        }
    }
}