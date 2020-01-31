namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RChecks : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RChecks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RecordID = c.Int(nullable: false),
                        sRecordID = c.String(),
                        InterviewRecordID = c.Int(nullable: false),
                        sInterviewRecordID = c.String(),
                        Name = c.String(),
                        DOB = c.DateTime(),
                        sDOB = c.String(),
                        Num = c.Int(nullable: false),
                        sNum = c.String(),
                        Date = c.DateTime(),
                        sDate = c.String(),
                        Service = c.String(),
                        Disposition = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RChecks");
        }
    }
}
