namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TextMsgVid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TextMsgs", "Vid", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TextMsgs", "Vid");
        }
    }
}
