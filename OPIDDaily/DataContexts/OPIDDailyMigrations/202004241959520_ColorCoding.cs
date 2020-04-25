namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ColorCoding : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "Msgs", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "Msgs");
        }
    }
}
