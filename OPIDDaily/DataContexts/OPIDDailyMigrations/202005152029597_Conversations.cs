namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Conversations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TextMsgs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        From = c.String(),
                        Msg = c.String(),
                        Client_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.Client_Id)
                .Index(t => t.Client_Id);
            
            AddColumn("dbo.Clients", "Conversation", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TextMsgs", "Client_Id", "dbo.Clients");
            DropIndex("dbo.TextMsgs", new[] { "Client_Id" });
            DropColumn("dbo.Clients", "Conversation");
            DropTable("dbo.TextMsgs");
        }
    }
}
