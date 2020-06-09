namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PocketCheckNotes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PocketChecks", "Notes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PocketChecks", "Notes");
        }
    }
}
