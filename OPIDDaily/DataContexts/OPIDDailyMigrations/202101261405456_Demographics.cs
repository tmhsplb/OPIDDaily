namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Demographics : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "Incarceration", c => c.String());
            AddColumn("dbo.Clients", "HousingStatus", c => c.String());
            AddColumn("dbo.Clients", "USCitizen", c => c.String());
            AddColumn("dbo.Clients", "Gender", c => c.String());
            AddColumn("dbo.Clients", "Ethnicity", c => c.String());
            AddColumn("dbo.Clients", "Race", c => c.String());
            AddColumn("dbo.Clients", "MilitaryVeteran", c => c.String());
            AddColumn("dbo.Clients", "DischargeStatus", c => c.String());
            AddColumn("dbo.Clients", "Disabled", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "Disabled");
            DropColumn("dbo.Clients", "DischargeStatus");
            DropColumn("dbo.Clients", "MilitaryVeteran");
            DropColumn("dbo.Clients", "Race");
            DropColumn("dbo.Clients", "Ethnicity");
            DropColumn("dbo.Clients", "Gender");
            DropColumn("dbo.Clients", "USCitizen");
            DropColumn("dbo.Clients", "HousingStatus");
            DropColumn("dbo.Clients", "Incarceration");
        }
    }
}
