namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TDCJ : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "SDTDCJ", c => c.Boolean(nullable: false));
            AddColumn("dbo.Clients", "SDVREG", c => c.Boolean(nullable: false));
            DropColumn("dbo.Clients", "SDOSID");
            DropColumn("dbo.Clients", "SDOSDL");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Clients", "SDOSDL", c => c.Boolean(nullable: false));
            AddColumn("dbo.Clients", "SDOSID", c => c.Boolean(nullable: false));
            DropColumn("dbo.Clients", "SDVREG");
            DropColumn("dbo.Clients", "SDTDCJ");
        }
    }
}
