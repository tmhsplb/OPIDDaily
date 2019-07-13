namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceTicket : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "ServiceDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Clients", "ServiceTicket", c => c.String());
            AddColumn("dbo.Clients", "WaitTime", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "WaitTime");
            DropColumn("dbo.Clients", "ServiceTicket");
            DropColumn("dbo.Clients", "ServiceDate");
        }
    }
}
