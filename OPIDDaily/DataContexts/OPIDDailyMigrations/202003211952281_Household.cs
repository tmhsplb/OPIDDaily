namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Household : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "HH", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "HH");
        }
    }
}
