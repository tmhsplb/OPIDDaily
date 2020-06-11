namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DbCheckNotes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AncientChecks", "Notes", c => c.String());
            AddColumn("dbo.RChecks", "Notes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RChecks", "Notes");
            DropColumn("dbo.AncientChecks", "Notes");
        }
    }
}
