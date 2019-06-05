using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OPIDDaily.Controllers
{
    public class HomeController : UsersController
    {
        public ActionResult Home()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (IsInRole("SuperAdmin"))
                {
                    // This is the launch method on ~/App_Start/RouteConfig.cs
                    // Present the SuperAdmin menu if the application is
                    // launched with sa in the browser's RememberMe cookie
                    // or if the user logs in as sa. 
                    return RedirectToAction("Home", "Superadmin");
                }
            }

            return View();
        }
    }
}