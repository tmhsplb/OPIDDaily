namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveTwoDates : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Clients", "ReferralDate");
            DropColumn("dbo.Clients", "AppearanceDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Clients", "AppearanceDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Clients", "ReferralDate", c => c.DateTime(nullable: false));
        }
    }
}
