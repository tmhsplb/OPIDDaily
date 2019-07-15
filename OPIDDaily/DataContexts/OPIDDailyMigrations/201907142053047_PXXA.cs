namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PXXA : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "PND", c => c.Boolean(nullable: false));
            AddColumn("dbo.Clients", "XID", c => c.Boolean(nullable: false));
            AddColumn("dbo.Clients", "XBC", c => c.Boolean(nullable: false));
            AddColumn("dbo.Clients", "Active", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "Active");
            DropColumn("dbo.Clients", "XBC");
            DropColumn("dbo.Clients", "XID");
            DropColumn("dbo.Clients", "PND");
        }
    }
}
