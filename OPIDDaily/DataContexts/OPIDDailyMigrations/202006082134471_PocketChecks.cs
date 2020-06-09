namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PocketChecks : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PocketChecks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Name = c.String(),
                        DOB = c.DateTime(nullable: false),
                        Item = c.String(),
                        Num = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PocketChecks");
        }
    }
}
