using MasterZoneMvc.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Common.Seed
{
    public class BusinessPlanDurationTypeSeed
    {
        private MasterZoneMvc.DAL.MasterZoneDbContext context;

        public BusinessPlanDurationTypeSeed(MasterZoneMvc.DAL.MasterZoneDbContext _context)
        {
            context = _context;
        }

        public void SeedData()
        {
            context.BusinessPlanDurationTypes.AddOrUpdate(x => x.Id,
                new BusinessPlanDurationType() { Id = 1, Key = "weekly", Value = "Weekly"},
                new BusinessPlanDurationType() { Id = 2, Key = "monthly", Value = "Monthly"},
                new BusinessPlanDurationType() { Id = 3, Key = "yearly", Value = "Yearly"},
                new BusinessPlanDurationType() { Id = 4, Key = "per_class", Value = "Per Class"}
                );
        }
    }
}