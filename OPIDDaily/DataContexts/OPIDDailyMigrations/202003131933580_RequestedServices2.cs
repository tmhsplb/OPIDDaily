namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RequestedServices2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "State", c => c.String());
            AddColumn("dbo.Clients", "SDSSC", c => c.Boolean(nullable: false));
            DropColumn("dbo.Clients", "SDSCC");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Clients", "SDSCC", c => c.Boolean(nullable: false));
            DropColumn("dbo.Clients", "SDSSC");
            DropColumn("dbo.Clients", "State");
        }
    }
}
