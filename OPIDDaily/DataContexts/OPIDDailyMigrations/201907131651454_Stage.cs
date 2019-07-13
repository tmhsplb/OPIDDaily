namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Stage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "Stage", c => c.String());
            AddColumn("dbo.Clients", "Screened", c => c.DateTime(nullable: false));
            AddColumn("dbo.Clients", "CheckedIn", c => c.DateTime(nullable: false));
            AddColumn("dbo.Clients", "Interviewing", c => c.DateTime(nullable: false));
            AddColumn("dbo.Clients", "Interviewed", c => c.DateTime(nullable: false));
            AddColumn("dbo.Clients", "BackOffice", c => c.DateTime(nullable: false));
            AddColumn("dbo.Clients", "Done", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "Done");
            DropColumn("dbo.Clients", "BackOffice");
            DropColumn("dbo.Clients", "Interviewed");
            DropColumn("dbo.Clients", "Interviewing");
            DropColumn("dbo.Clients", "CheckedIn");
            DropColumn("dbo.Clients", "Screened");
            DropColumn("dbo.Clients", "Stage");
        }
    }
}
