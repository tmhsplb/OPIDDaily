
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

        public DbSet<Agency> Agencies { get; set; }

        public DbSet<MBVD> MBVDS { get; set; }

        public DbSet<Invitation> Invitations { get; set; }

        public DbSet<Client> Clients { get; set; }

        public DbSet<TextMsg> TextMsgs { get; set; }

        public DbSet<RCheck> RChecks { get; set; }

        public DbSet<AncientCheck> AncientChecks { get; set; }

        public DbSet<PocketCheck> PocketChecks { get; set; }
    }
    
}