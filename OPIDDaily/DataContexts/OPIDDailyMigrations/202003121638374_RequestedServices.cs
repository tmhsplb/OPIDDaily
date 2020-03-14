namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RequestedServices : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "BC", c => c.Boolean(nullable: false));
            AddColumn("dbo.Clients", "HCC", c => c.Boolean(nullable: false));
            AddColumn("dbo.Clients", "MBVD", c => c.Boolean(nullable: false));
            AddColumn("dbo.Clients", "NewTID", c => c.Boolean(nullable: false));
            AddColumn("dbo.Clients", "ReplacementTID", c => c.Boolean(nullable: false));
            AddColumn("dbo.Clients", "NewTDL", c => c.Boolean(nullable: false));
            AddColumn("dbo.Clients", "ReplacementTDL", c => c.Boolean(nullable: false));
            AddColumn("dbo.Clients", "Numident", c => c.Boolean(nullable: false));
            AddColumn("dbo.Clients", "SDBC", c => c.Boolean(nullable: false));
            AddColumn("dbo.Clients", "SDSCC", c => c.Boolean(nullable: false));
            AddColumn("dbo.Clients", "SDTID", c => c.Boolean(nullable: false));
            AddColumn("dbo.Clients", "SDTDL", c => c.Boolean(nullable: false));
            AddColumn("dbo.Clients", "SDOSID", c => c.Boolean(nullable: false));
            AddColumn("dbo.Clients", "SDOSDL", c => c.Boolean(nullable: false));
            AddColumn("dbo.Clients", "SDML", c => c.Boolean(nullable: false));
            AddColumn("dbo.Clients", "SDDD", c => c.Boolean(nullable: false));
            AddColumn("dbo.Clients", "SDSL", c => c.Boolean(nullable: false));
            AddColumn("dbo.Clients", "SDDD214", c => c.Boolean(nullable: false));
            AddColumn("dbo.Clients", "SDEBT", c => c.Boolean(nullable: false));
            AddColumn("dbo.Clients", "SDHOTID", c => c.Boolean(nullable: false));
            AddColumn("dbo.Clients", "SDSchoolRecords", c => c.Boolean(nullable: false));
            AddColumn("dbo.Clients", "SDPassport", c => c.Boolean(nullable: false));
            AddColumn("dbo.Clients", "SDJobOffer", c => c.Boolean(nullable: false));
            AddColumn("dbo.Clients", "SDOther", c => c.Boolean(nullable: false));
            AddColumn("dbo.Clients", "SDOthersd", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "SDOthersd");
            DropColumn("dbo.Clients", "SDOther");
            DropColumn("dbo.Clients", "SDJobOffer");
            DropColumn("dbo.Clients", "SDPassport");
            DropColumn("dbo.Clients", "SDSchoolRecords");
            DropColumn("dbo.Clients", "SDHOTID");
            DropColumn("dbo.Clients", "SDEBT");
            DropColumn("dbo.Clients", "SDDD214");
            DropColumn("dbo.Clients", "SDSL");
            DropColumn("dbo.Clients", "SDDD");
            DropColumn("dbo.Clients", "SDML");
            DropColumn("dbo.Clients", "SDOSDL");
            DropColumn("dbo.Clients", "SDOSID");
            DropColumn("dbo.Clients", "SDTDL");
            DropColumn("dbo.Clients", "SDTID");
            DropColumn("dbo.Clients", "SDSCC");
            DropColumn("dbo.Clients", "SDBC");
            DropColumn("dbo.Clients", "Numident");
            DropColumn("dbo.Clients", "ReplacementTDL");
            DropColumn("dbo.Clients", "NewTDL");
            DropColumn("dbo.Clients", "ReplacementTID");
            DropColumn("dbo.Clients", "NewTID");
            DropColumn("dbo.Clients", "MBVD");
            DropColumn("dbo.Clients", "HCC");
            DropColumn("dbo.Clients", "BC");
        }
    }
}
