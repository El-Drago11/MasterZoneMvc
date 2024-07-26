namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_9 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BusinessStudents",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BusinessOwnerId = c.Long(nullable: false),
                        StudentId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.BusinessStudents");
        }
    }
}
