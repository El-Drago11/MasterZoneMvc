﻿namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_34 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClassFeatures",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ClassId = c.Long(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        Icon = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ClassFeatures");
        }
    }
}
