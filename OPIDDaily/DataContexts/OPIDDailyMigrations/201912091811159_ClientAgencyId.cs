namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClientAgencyId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "AgencyId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "AgencyId");
        }
    }
}
