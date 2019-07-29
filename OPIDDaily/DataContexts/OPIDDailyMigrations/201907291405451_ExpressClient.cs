namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExpressClient : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "EXP", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "EXP");
        }
    }
}
