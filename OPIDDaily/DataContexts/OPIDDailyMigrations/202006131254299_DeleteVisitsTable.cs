namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteVisitsTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Visits", "Client_Id", "dbo.Clients");
            DropIndex("dbo.Visits", new[] { "Client_Id" });
            DropTable("dbo.Visits");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Visits",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Item = c.String(),
                        Check = c.String(),
                        Status = c.String(),
                        Notes = c.String(),
                        Client_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.Visits", "Client_Id");
            AddForeignKey("dbo.Visits", "Client_Id", "dbo.Clients", "Id");
        }
    }
}
