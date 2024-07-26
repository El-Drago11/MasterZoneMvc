namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_80 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserCertificates", "LicenseId", c => c.Long(nullable: false));
            AddColumn("dbo.UserCertificates", "IssuedCertificateNumber", c => c.String());
            AddColumn("dbo.UserCertificates", "CertificateFile", c => c.String());
            AddColumn("dbo.UserCertificates", "ItemId", c => c.Long(nullable: false));
            AddColumn("dbo.UserCertificates", "ItemTable", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserCertificates", "ItemTable");
            DropColumn("dbo.UserCertificates", "ItemId");
            DropColumn("dbo.UserCertificates", "CertificateFile");
            DropColumn("dbo.UserCertificates", "IssuedCertificateNumber");
            DropColumn("dbo.UserCertificates", "LicenseId");
        }
    }
}
