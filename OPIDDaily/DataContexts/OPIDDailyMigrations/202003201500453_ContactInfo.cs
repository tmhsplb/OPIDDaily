namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ContactInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "AKA", c => c.String());
            AddColumn("dbo.Clients", "Email", c => c.String());
            AddColumn("dbo.Clients", "BirthCity", c => c.String());
            AddColumn("dbo.Clients", "BirthState", c => c.String());
            AddColumn("dbo.Clients", "Phone", c => c.String());
            AddColumn("dbo.Clients", "CurrentAddress", c => c.String());
            AddColumn("dbo.Clients", "City", c => c.String());
            AddColumn("dbo.Clients", "Staat", c => c.String());
            AddColumn("dbo.Clients", "Zip", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "Zip");
            DropColumn("dbo.Clients", "Staat");
            DropColumn("dbo.Clients", "City");
            DropColumn("dbo.Clients", "CurrentAddress");
            DropColumn("dbo.Clients", "Phone");
            DropColumn("dbo.Clients", "BirthState");
            DropColumn("dbo.Clients", "BirthCity");
            DropColumn("dbo.Clients", "Email");
            DropColumn("dbo.Clients", "AKA");
        }
    }
}
