namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LockClient : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "LCK", c => c.Boolean(nullable: false));
            DropColumn("dbo.Clients", "EXP");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Clients", "EXP", c => c.Boolean(nullable: false));
            DropColumn("dbo.Clients", "LCK");
        }
    }
}
