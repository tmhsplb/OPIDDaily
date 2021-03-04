namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InHouseTexts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TextMsgs", "InHouse", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TextMsgs", "InHouse");
        }
    }
}
