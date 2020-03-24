namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HeadOfHousehold : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "HeadOfHousehold", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "HeadOfHousehold");
        }
    }
}
