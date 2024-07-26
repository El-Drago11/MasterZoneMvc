using MasterZoneMvc.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Common.Seed
{
    public class LastRecordIdDetailSeed
    {
        private MasterZoneMvc.DAL.MasterZoneDbContext context;
        public LastRecordIdDetailSeed(MasterZoneMvc.DAL.MasterZoneDbContext _context)
        {
            context = _context;
        }

        public void SeedData()
        {
            context.LastRecordIdDetails.AddOrUpdate(x => x.Id,
                //new LastRecordIdDetail() { Id = 1, Key = "master_business", Value = 1000, Prefix = "MB_" },
                //new LastRecordIdDetail() { Id = 2, Key = "master_user", Value = 1000, Prefix = "MU_" },
                //new LastRecordIdDetail() { Id = 3, Key = "master_staff", Value = 1000, Prefix = "MBS_" },
                //new LastRecordIdDetail() { Id = 4, Key = "master_business_individual", Value = 1000, Prefix = "MBI_" },
                new LastRecordIdDetail() { Id = 4, Key = "training_certificate", Value = 1000, Prefix = "TR_" }
                );
        }
    }
}