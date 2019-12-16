namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Expiry : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "Expiry", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "Expiry");
        }
    }
}
