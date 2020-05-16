namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TextMsgTo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TextMsgs", "To", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TextMsgs", "To");
        }
    }
}
