namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RequestedDocument : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "RequestedDocument", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "RequestedDocument");
        }
    }
}
