namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PCHousehold : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PocketChecks", "HeadOfHousehold", c => c.Boolean(nullable: false));
            AddColumn("dbo.PocketChecks", "HH", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PocketChecks", "HH");
            DropColumn("dbo.PocketChecks", "HeadOfHousehold");
        }
    }
}
