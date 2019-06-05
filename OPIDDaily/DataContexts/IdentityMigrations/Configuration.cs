namespace OPIDDaily.DataContexts.IdentityMigrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<OPIDDaily.DataContexts.IdentityDB>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            MigrationsDirectory = @"DataContexts\IdentityMigrations";
            ContextKey = "OPIDDaily.DataContexts.IdentityDB";
        }

        protected override void Seed(OPIDDaily.DataContexts.IdentityDB context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}
