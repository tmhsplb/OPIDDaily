namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Nullable_RCheck_DOB : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RChecks", "DOB", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RChecks", "DOB", c => c.DateTime(nullable: false));
        }
    }
}
