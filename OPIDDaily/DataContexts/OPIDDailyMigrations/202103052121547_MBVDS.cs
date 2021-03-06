namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MBVDS : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MBVDs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MBVDId = c.Int(nullable: false),
                        MBVDName = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MBVDs");
        }
    }
}
