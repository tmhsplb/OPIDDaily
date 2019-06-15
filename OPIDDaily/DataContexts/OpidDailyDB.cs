
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using OpidDailyEntities;
 
namespace OPIDDaily.DataContexts
{
    public class OpidDailyDB : DbContext
    {
        public OpidDailyDB()
          // :base("OPIDEntities")  PLB: Commented out on 4/7/19
          : base(Config.ConnectionString) //  PLB: Added on 4/7/19
        {
        }

        public DbSet<Invitation> Invitations { get; set; }

        public DbSet<Client> Clients { get; set; }
    }
    
}