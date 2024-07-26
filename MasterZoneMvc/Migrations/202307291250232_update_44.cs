namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_44 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserCertificates",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CertificateId = c.Long(nullable: false),
                        UserLoginId = c.Long(nullable: false),
                        TrainingId = c.Long(nullable: false),
                        CertificateBookingId = c.Long(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
        }
        
        public override void Down()
        {
            DropTable("dbo.UserCertificates");
        }
    }
}
