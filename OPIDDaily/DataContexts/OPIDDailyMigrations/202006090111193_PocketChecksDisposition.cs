namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PocketChecksDisposition : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PocketChecks", "Disposition", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PocketChecks", "Disposition");
        }
    }
}
