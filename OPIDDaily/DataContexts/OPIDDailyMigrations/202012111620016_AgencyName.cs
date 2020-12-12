namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgencyName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "AgencyName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "AgencyName");
        }
    }
}
