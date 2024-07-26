namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_16 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CouponConsumptions",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CouponId = c.Long(nullable: false),
                        ConsumerUserLoginId = c.Long(nullable: false),
                        ConsumptionDate = c.DateTime(nullable: false),
                        CouponCode = c.String(),
                        IsFixedAmountCoupon = c.Int(nullable: false),
                        CouponDiscountValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Coupons",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        Code = c.String(),
                        StartDate = c.String(),
                        StartDate_DateTimeFormat = c.DateTime(nullable: false),
                        EndDate = c.String(),
                        EndDate_DateTimeFormat = c.DateTime(nullable: false),
                        IsFixedAmount = c.Int(nullable: false),
                        DiscountValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalUsed = c.Int(nullable: false),
                        DiscountFor = c.Int(nullable: false),
                        SelectedStudent = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        ItemId = c.Long(nullable: false),
                        ItemType = c.String(),
                        OnlinePayment = c.Int(nullable: false),
                        PaymentMethod = c.String(),
                        CouponId = c.Long(nullable: false),
                        CouponDiscountValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalDiscount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsTaxable = c.Int(nullable: false),
                        GST = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Status = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PaymentResponses",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        OrderId = c.Long(nullable: false),
                        Provider = c.String(),
                        DateStamp = c.DateTime(nullable: false),
                        ResponseStatus = c.String(),
                        TransactionID = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Approved = c.Int(nullable: false),
                        Description = c.String(),
                        Method = c.String(),
                        CreatedByLoginId = c.Long(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PlanBookings",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        OrderId = c.Long(nullable: false),
                        PlanId = c.Long(nullable: false),
                        StudentUserLoginId = c.Long(nullable: false),
                        PlanName = c.String(),
                        PlanDescription = c.String(),
                        PlanPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlanCompareAtPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlanDurationTypeId = c.Int(nullable: false),
                        PlanDurationTypeName = c.String(),
                        StartDate = c.String(),
                        StartDate_DateTimeFormat = c.DateTime(nullable: false),
                        EndDate = c.String(),
                        EndDate_DateTimeFormat = c.DateTime(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PlanBookings");
            DropTable("dbo.PaymentResponses");
            DropTable("dbo.Orders");
            DropTable("dbo.Coupons");
            DropTable("dbo.CouponConsumptions");
        }
    }
}
