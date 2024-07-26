namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_20 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Trainings",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        InstructorUserLoginId = c.Long(nullable: false),
                        InstructorEmail = c.String(),
                        InstructorMobileNumber = c.String(),
                        InstructorAlternateNumber = c.String(),
                        TrainingName = c.String(),
                        ShortDescription = c.String(),
                        Description = c.String(),
                        IsPaid = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AdditionalPriceInformation = c.String(),
                        StartDate = c.String(),
                        StartDate_DateTimeFormat = c.DateTime(nullable: false),
                        EndDate = c.String(),
                        EndDate_DateTimeFormat = c.DateTime(nullable: false),
                        StartTime_24HF = c.String(),
                        EndTime_24HF = c.String(),
                        CenterName = c.String(),
                        Location = c.String(),
                        Address = c.String(),
                        City = c.String(),
                        State = c.String(),
                        Country = c.String(),
                        PinCode = c.String(),
                        LocationUrl = c.String(),
                        MusicType = c.String(),
                        EnergyLevel = c.String(),
                        DanceStyle = c.String(),
                        Status = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Trainings");
        }
    }
}
