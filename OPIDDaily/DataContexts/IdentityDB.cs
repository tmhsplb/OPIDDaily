using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OPIDDaily.Models;

namespace OPIDDaily.DataContexts
{
    public class IdentityDB : IdentityDbContext<ApplicationUser>
    {
        public IdentityDB()
             : base(Config.ConnectionString, throwIfV1Schema: false)
        {
        }

        public static IdentityDB Create()
        {
            return new IdentityDB();
        }

        public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
        {
            public ApplicationDbContext()
                 // : base("OPIDConnectionString", throwIfV1Schema: false)
                 : base(Config.ConnectionString, throwIfV1Schema: false)
            //: base(Config.DefaultConnection, throwIfV1Schema: false)
            {
            }

            public static ApplicationDbContext Create()
            {
                return new ApplicationDbContext();
            }
        }
    }
}