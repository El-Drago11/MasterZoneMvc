namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_72 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Licenses", "LicenseCertificateHTMLContent", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Licenses", "LicenseCertificateHTMLContent");
        }
    }
}
