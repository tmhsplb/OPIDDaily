namespace OPIDDaily.DataContexts.IdentityMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgencyId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "AgencyId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "AgencyId");
        }
    }
}
