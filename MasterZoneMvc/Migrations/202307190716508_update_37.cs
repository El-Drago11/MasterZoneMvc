namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_37 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Batches",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        GroupId = c.Long(nullable: false),
                        InstructorLoginId = c.Long(nullable: false),
                        StudentMaxStrength = c.Int(nullable: false),
                        ScheduledStartOnTime_24HF = c.String(),
                        ScheduledEndOnTime_24HF = c.String(),
                        ScheduledOnDateTime = c.DateTime(nullable: false),
                        ClassDurationSeconds = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ClassBatches",
                c => new
                    {
                        BatchId = c.Long(nullable: false),
                        ClassId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.BatchId, t.ClassId })
                .ForeignKey("dbo.Batches", t => t.BatchId, cascadeDelete: true)
                .ForeignKey("dbo.Classes", t => t.ClassId, cascadeDelete: true)
                .Index(t => t.BatchId)
                .Index(t => t.ClassId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ClassBatches", "ClassId", "dbo.Classes");
            DropForeignKey("dbo.ClassBatches", "BatchId", "dbo.Batches");
            DropIndex("dbo.ClassBatches", new[] { "ClassId" });
            DropIndex("dbo.ClassBatches", new[] { "BatchId" });
            DropTable("dbo.ClassBatches");
            DropTable("dbo.Batches");
        }
    }
}
