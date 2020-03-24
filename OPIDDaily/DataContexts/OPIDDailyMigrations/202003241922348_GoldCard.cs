namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GoldCard : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "SDGC", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "SDGC");
        }
    }
}
