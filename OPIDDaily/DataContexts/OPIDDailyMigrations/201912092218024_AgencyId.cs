namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgencyId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invitations", "AgencyId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Invitations", "AgencyId");
        }
    }
}
