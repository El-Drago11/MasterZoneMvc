using MasterZoneMvc.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MasterZoneMvc.Services;
using MasterZoneMvc.ViewModels;
using MasterZoneMvc.Common;
using MasterZoneMvc.Models;
using MasterZoneMvc.Common.ValidationHelpers;

namespace MasterZoneMvc.Controllers
{
    public class TestController : Controller
    {
        private MasterZoneDbContext db;
        public TestController()
        {
            db = new MasterZoneDbContext();
        }

        // GET: Test
        public ActionResult Index()
        {
            BookingService bookingService = new BookingService(db);
            CreatePlanBooking_VM createPlanBooking_VM = new CreatePlanBooking_VM() { 
                PlanId = 2,
                UserLoginId = 5,
                OnlinePayment = 0,
                TransactionID = "",
                PaymentResponseStatus = "",
                PaymentProvider = "ManualPayment",
                PaymentMethod = "Cash",
                PaymentDescription = "Cash Payment",
                TotalAmountPaid = (decimal)200.00,
                CouponId = 0
            };

            bookingService.CreatePlanBooking(createPlanBooking_VM);
            return View();
        }

        public ActionResult SendTestEmail(string phone = "")
        {
            var valid = PhoneNumberValidationHelper.IsValidPhoneNumber(phone);
            return Content(valid.ToString());

            EmailSender emailSender = new EmailSender();
            emailSender.Send("Hanish Gupta", "Test Email Subject", "angulardeveloper.protolabz@gmail.com", "Test Email Message Body", "");

            return Content("Email Sent!");
        }
    }
}