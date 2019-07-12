namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DOBandAge : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "DOB", c => c.DateTime(nullable: false));
            AddColumn("dbo.Clients", "Age", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "Age");
            DropColumn("dbo.Clients", "DOB");
        }
    }
}
