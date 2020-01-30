namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DOB : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RChecks", "DOB", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RChecks", "DOB");
        }
    }
}
