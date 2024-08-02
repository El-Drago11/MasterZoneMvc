using MasterZoneMvc.DAL;
using MasterZoneMvc.Repository;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using MasterZoneMvc.ViewModels;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using MasterZoneMvc.Common;
using System.Globalization;
using System.Drawing.Drawing2D;
using MasterZoneMvc.Models.Enum;
using MasterZoneMvc.Models;
using MasterZoneMvc.PageTemplateViewModels;
using MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel;
using static QRCoder.PayloadGenerator.SwissQrCode.Iban;

namespace MasterZoneMvc.Services
{
    public class BookingService
    {
        private MasterZoneDbContext db;
        private readonly StoredProcedureRepository storedProcedureRepository;
        private CouponService couponService;
        private NotificationService notificationService;
        private PlanService planService;
        private BusinessStudentService businessStudentService;
        private EventService eventService;
        private ClassService classService;
        private GroupService groupService;
        private LicenseService licenseService;
        private TrainingService trainingService;
        private CourseService courseService;

        public BookingService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedProcedureRepository = new StoredProcedureRepository(db);
            couponService = new CouponService(db);
            notificationService = new NotificationService(db);
            planService = new PlanService(db);
            businessStudentService = new BusinessStudentService(db);
            eventService = new EventService(db);
            classService = new ClassService(db);
            licenseService = new LicenseService(db);
            trainingService = new TrainingService(db);
            courseService = new CourseService(db);
        }

        public ServiceResponse_VM ValidatePlanBooking()
        {
            ServiceResponse_VM serviceResponse_VM = new ServiceResponse_VM();
            return serviceResponse_VM;
        }

        /// <summary>
        /// Book Plan for User/Student
        /// </summary>
        public ServiceResponse_VM CreatePlanBooking(CreatePlanBooking_VM createPlanBooking_VM)
        {
            // Book plan for User/Student

            ServiceResponse_VM serviceResponse_VM = new ServiceResponse_VM();
            decimal _TotalAmount = 0;
            decimal _CouponDiscount = 0;
            decimal _CouponDiscountValue = 0;
            int _IsApproved = (createPlanBooking_VM.OnlinePayment == 1) ? 1 : 0;

            // Get Plan detail
            var _PlanDetail = storedProcedureRepository.SP_ManageBusinessPlans_Get<BusinessPlan_VM>(new SP_ManageBusinessPlans_Params_VM() { Id = createPlanBooking_VM.PlanId, Mode = 2, PlanType = createPlanBooking_VM.PlanType });

            var planData = planService.GetPlanDataById(createPlanBooking_VM.PlanId,createPlanBooking_VM.PlanType);

            

            if (_PlanDetail == null)
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.ErrorMessage.PlanNotFound;
                return serviceResponse_VM;
            }
            
            // if coupon applied
            if(createPlanBooking_VM.CouponId > 0)
            {
                // Get Coupon Details
                var _CouponDetail = couponService.GetCouponDetailById(createPlanBooking_VM.CouponId);
                _CouponDiscountValue = _CouponDetail.DiscountValue;

                if (_CouponDetail.IsFixedAmount == 1)
                {
                    _CouponDiscount = _CouponDetail.DiscountValue;
                }
                else
                {
                    // Calculate Discount value from dicount percentage on Plan/Package Amount
                    _CouponDiscount = (_PlanDetail.Price * ((decimal)_CouponDetail.DiscountValue / 100));
                }
            }

            // Calculate Total Amount after all discounts if any 
            _TotalAmount = _PlanDetail.Price - _CouponDiscount;

            // Check if Total Amount is equal to the Paid Amount
            if(createPlanBooking_VM.TotalAmountPaid != _TotalAmount)
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.ErrorMessage.PaidAmountNotEqualsToTotalAmount;
                return serviceResponse_VM;
            }

            // Plan Validity Date Calculations
            DateTime _PlanStartDate = DateTime.UtcNow.Date;
            DateTime _PlanEndDate = _PlanStartDate;

            #region Set End Date for the Plan/Package based on the package duraiton type
            if(_PlanDetail.BusinessPlanDurationTypeId == 1)
            {
                // weekly
                _PlanEndDate = _PlanStartDate.AddDays(7);
            }
            else if(_PlanDetail.BusinessPlanDurationTypeId == 2  || _PlanDetail.PlanDurationTypeKey == "per_monthly")
            {
                //monthly
                _PlanEndDate = _PlanStartDate.AddMonths(1);
            }
            else if(_PlanDetail.BusinessPlanDurationTypeId == 3 || _PlanDetail.PlanDurationTypeKey == "per_Yearly")
            {
                // yearly
                _PlanEndDate = _PlanStartDate.AddYears(1);
            }
            else if(_PlanDetail.BusinessPlanDurationTypeId == 4)
            {
                // Per Class
                _PlanEndDate = _PlanStartDate; //Assuming per class package is for only of that day. 
            }
            #endregion
          

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Steps:
                    // Order Creation
                    // Payment Response
                    // PlanBooking creation
                    // Consume Coupon if applied
                    // Send Notifications
                    // AddOrUpdate Student link with Business if not linked. (BusinessStudents table)

                    #region Make Plan Booking/Purchase
                    // Create Order
                    SP_InsertUpdateOrder_Params_VM sP_InsertUpdateOrder_Params_VM = new SP_InsertUpdateOrder_Params_VM()
                    {
                        UserLoginId = createPlanBooking_VM.UserLoginId,
                        ItemId = createPlanBooking_VM.PlanId,
                        ItemType = "Plan",
                        OnlinePayment = createPlanBooking_VM.OnlinePayment,
                        PaymentMethod = createPlanBooking_VM.PaymentMethod,
                        CouponId = createPlanBooking_VM.CouponId,
                        CouponDiscountValue = _CouponDiscountValue,
                        TotalDiscount = _CouponDiscount,
                        IsTaxable = 0,
                        Gst = 0,
                        TotalAmount = createPlanBooking_VM.TotalAmountPaid,
                        Status = 1,
                        Mode = 1,
                        OwnerUserLoginId = planData.BusinessOwnerLoginId
                    };
                    var order = storedProcedureRepository.SP_InsertUpdateOrder_Get<SPResponseViewModel>(sP_InsertUpdateOrder_Params_VM);


                   
                    // Create Payment Response
                    SP_InsertUpdatePaymentResponse_Params_VM sP_InsertUpdatePaymentResponse_Params_VM = new SP_InsertUpdatePaymentResponse_Params_VM()
                    {
                        OrderId = order.Id,
                        Method = createPlanBooking_VM.PaymentMethod,
                        Amount = createPlanBooking_VM.TotalAmountPaid,
                        TransactionID = createPlanBooking_VM.TransactionID,
                        Description = createPlanBooking_VM.PaymentDescription,
                        IsApproved = _IsApproved,
                        Provider = createPlanBooking_VM.PaymentProvider,
                        ResponseStatus = createPlanBooking_VM.PaymentResponseStatus,
                        SubmittedByLoginId = createPlanBooking_VM.UserLoginId,
                        
                        Mode = 1
                    };
                    var paymentResponse = storedProcedureRepository.SP_InsertUpdatePaymentResponse_Get<SPResponseViewModel>(sP_InsertUpdatePaymentResponse_Params_VM);


                    var planBookingTyepId = 0;

                    if (createPlanBooking_VM.PlanType == 1)
                    {
                        planBookingTyepId = 1;
                    }
                    else
                    {
                        planBookingTyepId = 2;
                    }
                    // Create Plan-Booking
                    SP_InsertUpdatePlanBooking_Params_VM sP_InsertUpdatePlanBooking_Params_VM = new SP_InsertUpdatePlanBooking_Params_VM()
                    {
                        OrderId = order.Id,
                        PlanId = createPlanBooking_VM.PlanId,
                        PlanStartDate = _PlanStartDate.ToString("yyyy-MM-dd"),
                        PlanEndDate = _PlanEndDate.ToString("yyyy-MM-dd"),
                        StudentUserLoginId = createPlanBooking_VM.UserLoginId,
                        PlanBookingType = planBookingTyepId,
                        

                        Mode = 1
                    };

                    var planBooking = storedProcedureRepository.SP_InsertUpdatePlanBooking_Get<SPResponseViewModel>(sP_InsertUpdatePlanBooking_Params_VM,createPlanBooking_VM.PlanType);

                    if(createPlanBooking_VM.CouponId > 0)
                    {
                        var consumeCoupon = couponService.AddCouponConsumption(createPlanBooking_VM.CouponId, createPlanBooking_VM.UserLoginId);
                    }

                    // Send Notification to Business Owner
                    BasicProfileDetail_VM studentBasicProfileDetail_VM = new BasicProfileDetail_VM();
                    SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessOwnerLoginId", "0"),
                            new SqlParameter("userLoginId", createPlanBooking_VM.UserLoginId),
                            new SqlParameter("mode", "1")
                            };

                    var respStudentBasicDetail = db.Database.SqlQuery<BasicProfileDetail_VM>("exec sp_ManageStudentProfile @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();
                    var studentName = respStudentBasicDetail.FirstName + " " + respStudentBasicDetail.LastName;

                    notificationService.InsertUpdateNotification(new SPInsertUpdateNotification_Params_VM()
                    {
                        NotificationText = String.Format(Resources.NotificationMessage.BusinessNotification_NewPlanBookingMessage, planData.Name, studentName, _PlanStartDate.ToString("yyyy-MM-dd")),
                        NotificationTitle = String.Format(Resources.NotificationMessage.BusinessNotification_NewPlanBookingTitle, planData.Name),
                        NotificationType = NotificationTypes.NewBooking.ToString(),
                        NotificationUsersList = planData.BusinessOwnerLoginId.ToString(),
                        SubmittedByLoginId = 0,
                        Mode = 1,
                        IsNotificationLinkable = 1,
                        ItemId = planBooking.Id,
                        ItemTable = "PlanBookings"
                    });

                    // send notification to Student/User
                    notificationService.InsertUpdateNotification(new SPInsertUpdateNotification_Params_VM()
                    {
                        NotificationText = String.Format(Resources.NotificationMessage.UserNotification_NewPlanBookingMessage, planData.Name, DateTime.UtcNow.AddMinutes(330).ToString("yyyy-MM-dd")),
                        NotificationTitle = String.Format(Resources.NotificationMessage.UserNotification_NewPlanBookingTitle, planData.Name),
                        NotificationType = NotificationTypes.NewBooking.ToString(),
                        NotificationUsersList = createPlanBooking_VM.UserLoginId.ToString(),
                        SubmittedByLoginId = 0,
                        Mode = 1,
                        IsNotificationLinkable = 1,
                        ItemId = planBooking.Id,
                        ItemTable = "PlanBookings"
                    });

                    if(createPlanBooking_VM.PlanType == 0)
                    {
                    // Link Student with Business

                    var respBusinessStudentLinking = businessStudentService.AddStudentLinkWithBusinessOwner(planData.BusinessOwnerLoginId, createPlanBooking_VM.UserLoginId);
                    }
                    #endregion

                    db.SaveChanges(); // Save changes to the database

                    transaction.Commit(); // Commit the transaction if everything is successful
                    serviceResponse_VM.Status = 1;
                    serviceResponse_VM.Message = "Plan Purchased!";
                }
                catch (Exception ex)
                {
                    // Handle exceptions and perform error handling or logging
                    transaction.Rollback(); // Roll back the transaction
                    serviceResponse_VM.Status = -100;


                    serviceResponse_VM.Message = Resources.ErrorMessage.InternalServerErrorMessage;
                }
            } // transaction scope ends

            return serviceResponse_VM;
        }

        /// <summary>
        /// Book Event OR Event Purchase 
        /// </summary>
        /// <param name="createEventBooking_VM"></param>
        /// <returns></returns>
        public ServiceResponse_VM CreateEventBooking(CreateEventBooking_VM createEventBooking_VM)
        {
            // Book Event for User/Student

            ServiceResponse_VM serviceResponse_VM = new ServiceResponse_VM();
            decimal _TotalAmount = 0;
            decimal _CouponDiscount = 0;
            decimal _CouponDiscountValue = 0;
            int _IsApproved = (createEventBooking_VM.OnlinePayment == 1) ? 1 : 0;

            // Get Event detail
            var _EventData = eventService.GetEventDataById(createEventBooking_VM.EventId);

            if (_EventData == null)
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.ErrorMessage.EventNotFound;
                return serviceResponse_VM;
            }

            // If Paid Event 
            if (_EventData.IsPaid == 1)
            {
                // if coupon applied
                if (createEventBooking_VM.CouponId > 0)
                {
                    // Get Coupon Details
                    var _CouponDetail = couponService.GetCouponDetailById(createEventBooking_VM.CouponId);
                    _CouponDiscountValue = _CouponDetail.DiscountValue;

                    if (_CouponDetail.IsFixedAmount == 1)
                    {
                        _CouponDiscount = _CouponDetail.DiscountValue;
                    }
                    else
                    {
                        // Calculate Discount value from dicount percentage on Event Amount
                        _CouponDiscount = (_EventData.Price * ((decimal)_CouponDetail.DiscountValue / 100));
                    }
                }

                // Calculate Total Amount after all discounts if any 
                _TotalAmount = _EventData.Price - _CouponDiscount;

                // Check if Total Amount is equal to the Paid Amount
                if (createEventBooking_VM.TotalAmountPaid != _TotalAmount)
                {
                    serviceResponse_VM.Status = -1;
                    serviceResponse_VM.Message = Resources.ErrorMessage.PaidAmountNotEqualsToTotalAmount;
                    return serviceResponse_VM;
                }
            }
            else
            {
                // Free Event
                _IsApproved = 1;
                _TotalAmount = 0;
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Steps:
                    // Order Creation
                    // Payment Response
                    // EventBooking creation
                    // Consume Coupon if applied
                    // Send Notifications
                    // AddOrUpdate Student link with Business if not linked. (BusinessStudents table)

                    #region Make Event Booking/Purchase
                    // Create Order
                    SP_InsertUpdateOrder_Params_VM sP_InsertUpdateOrder_Params_VM = new SP_InsertUpdateOrder_Params_VM()
                    {
                        UserLoginId = createEventBooking_VM.UserLoginId,
                        ItemId = createEventBooking_VM.EventId,
                        ItemType = "Event",
                        OnlinePayment = createEventBooking_VM.OnlinePayment,
                        PaymentMethod = createEventBooking_VM.PaymentMethod,
                        CouponId = createEventBooking_VM.CouponId,
                        CouponDiscountValue = _CouponDiscountValue,
                        TotalDiscount = _CouponDiscount,
                        IsTaxable = 0,
                        Gst = 0,
                        TotalAmount = _TotalAmount,
                        Status = 1,
                        Mode = 1,
                        OwnerUserLoginId = _EventData.UserLoginId,
                    };
                    var order = storedProcedureRepository.SP_InsertUpdateOrder_Get<SPResponseViewModel>(sP_InsertUpdateOrder_Params_VM);

                    // Create Payment Response
                    SP_InsertUpdatePaymentResponse_Params_VM sP_InsertUpdatePaymentResponse_Params_VM = new SP_InsertUpdatePaymentResponse_Params_VM()
                    {
                        OrderId = order.Id,
                        Method = createEventBooking_VM.PaymentMethod,
                        Amount = createEventBooking_VM.TotalAmountPaid,
                        TransactionID = createEventBooking_VM.TransactionID,
                        Description = createEventBooking_VM.PaymentDescription,
                        IsApproved = _IsApproved,
                        Provider = createEventBooking_VM.PaymentProvider,
                        ResponseStatus = createEventBooking_VM.PaymentResponseStatus,
                        SubmittedByLoginId = createEventBooking_VM.UserLoginId,
                        Mode = 1
                    };

                    var paymentResponse = storedProcedureRepository.SP_InsertUpdatePaymentResponse_Get<SPResponseViewModel>(sP_InsertUpdatePaymentResponse_Params_VM);

                    // Create Ticket-QR code and save
                    string _QRCodeTicketImage = eventService.CreateAndSaveQRCodeTicket(_EventData.Id, order.Id);

                    // Create Event-Booking
                    SP_InsertUpdateEventBooking_Params_VM sp_InsertUpdateEventBooking_Params_VM = new SP_InsertUpdateEventBooking_Params_VM()
                    {
                        OrderId = order.Id,
                        EventId = createEventBooking_VM.EventId,
                        UserLoginId = createEventBooking_VM.UserLoginId,
                        EventQRCodeTicket = _QRCodeTicketImage,
                        Mode = 1
                    };

                    var eventBooking = storedProcedureRepository.SP_InsertUpdateEventBooking<SPResponseViewModel>(sp_InsertUpdateEventBooking_Params_VM);

                    // Increase Total Joined
                    var responseEventTotalJoinedUpdate = eventService.UpdateEventTotalJoinedById(_EventData.Id);

                    if (createEventBooking_VM.CouponId > 0)
                    {
                        var consumeCoupon = couponService.AddCouponConsumption(createEventBooking_VM.CouponId, createEventBooking_VM.UserLoginId);
                    }

                    // Send Notification to Business Owner

                    // Get Student Basic-Profile Detail
                    BasicProfileDetail_VM studentBasicProfileDetail_VM = new BasicProfileDetail_VM();
                    SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessOwnerLoginId", "0"),
                            new SqlParameter("userLoginId", createEventBooking_VM.UserLoginId),
                            new SqlParameter("mode", "1")
                            };

                    var respStudentBasicDetail = db.Database.SqlQuery<BasicProfileDetail_VM>("exec sp_ManageStudentProfile @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();
                    var studentName = respStudentBasicDetail.FirstName + " " + respStudentBasicDetail.LastName;

                    // Send notification to Business/Event Owner
                    notificationService.InsertUpdateNotification(new SPInsertUpdateNotification_Params_VM()
                    {
                        NotificationText = String.Format(Resources.NotificationMessage.BusinessNotification_NewEventBookingMessage, _EventData.Title, studentName, DateTime.UtcNow.AddMinutes(330).ToString("yyyy-MM-dd")),
                        NotificationTitle = String.Format(Resources.NotificationMessage.BusinessNotification_NewEventBookingTitle, _EventData.Title),
                        NotificationType = NotificationTypes.NewBooking.ToString(),
                        NotificationUsersList = _EventData.UserLoginId.ToString(),
                        SubmittedByLoginId = 0,
                        Mode = 1,
                        IsNotificationLinkable = 1,
                        ItemId = eventBooking.Id,
                        ItemTable = "EventBookings",
                        OrderId = order.Id
                    });

                    // send notification to Student/User
                    notificationService.InsertUpdateNotification(new SPInsertUpdateNotification_Params_VM()
                    {
                        NotificationText = String.Format(Resources.NotificationMessage.UserNotification_NewEventBookingMessage, _EventData.Title, DateTime.UtcNow.AddMinutes(330).ToString("yyyy-MM-dd")),
                        NotificationTitle = String.Format(Resources.NotificationMessage.UserNotification_NewEventBookingTitle,_EventData.Title),
                        NotificationType = NotificationTypes.NewBooking.ToString(),
                        NotificationUsersList = createEventBooking_VM.UserLoginId.ToString(),
                        SubmittedByLoginId = 0,
                        Mode = 1,
                        IsNotificationLinkable = 1,
                        ItemId = eventBooking.Id,
                        ItemTable = "EventBookings",
                        OrderId = order.Id
                    });

                    // Link Student with Business
                    var respBusinessStudentLinking = businessStudentService.AddStudentLinkWithBusinessOwner(_EventData.UserLoginId, createEventBooking_VM.UserLoginId);
                    #endregion

                    db.SaveChanges(); // Save changes to the database

                    transaction.Commit(); // Commit the transaction if everything is successful
                    serviceResponse_VM.Status = 1;
                    serviceResponse_VM.Message = "Event Purchased!";

                    //send mails 
                    EmailSender emailSender = new EmailSender();

                    string msg = emailSender.EventBookedMailMessage(_EventData, _QRCodeTicketImage);

                    emailSender.Send(studentName, _EventData.Title + " event booked!", respStudentBasicDetail.Email, msg, "");
                }
                catch (Exception ex)
                {
                    // Handle exceptions and perform error handling or logging
                    transaction.Rollback(); // Roll back the transaction
                    serviceResponse_VM.Status = -100;
                    serviceResponse_VM.Message = Resources.ErrorMessage.InternalServerErrorMessage;
                }
            } // transaction scope ends

            return serviceResponse_VM;
        }

        /// <summary>
        /// Verify if Event Can be booked by User.
        /// </summary>
        /// <param name="createEventBooking_VM"></param>
        /// <returns>
        /// Returns Status = 1 if it can be purchased, and negative value if some error/issue 
        /// </returns>
        public ServiceResponse_VM VerifyEventCanBeBooked(CreateEventBooking_VM createEventBooking_VM)
        {
            // Book Event for User/Student

            ServiceResponse_VM serviceResponse_VM = new ServiceResponse_VM();
            serviceResponse_VM.Status = 1;

            decimal _TotalAmount = 0;
            decimal _CouponDiscount = 0;

            // Get Event detail
            var _EventData = eventService.GetEventDataById(createEventBooking_VM.EventId);

            if (_EventData == null)
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.ErrorMessage.EventNotFound;
                return serviceResponse_VM;
            }
            else if (_EventData.EndDateTime <= DateTime.UtcNow)
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.ErrorMessage.ItemNotAvailableForBooking;
                return serviceResponse_VM;
            }
            else if(eventService.IsAlreadyEventPurchased(createEventBooking_VM.EventId, createEventBooking_VM.UserLoginId))
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.ErrorMessage.EventAlreadyPurchased;
                return serviceResponse_VM;
            }

            // If Paid Event 
            if(_EventData.IsPaid == 1)
            {
                // if coupon applied
                if (createEventBooking_VM.CouponId > 0)
                {
                    // Verify Coupon can be applied

                    // Get Coupon Code Detail By Coupon Id
                    var couponDetail = couponService.GetCouponDetailById(createEventBooking_VM.CouponId);
                    DateTime _couponEndDate = (couponDetail == null) ? DateTime.UtcNow.Date.AddDays(-1) : DateTime.ParseExact(couponDetail.EndDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                    if (couponDetail == null)
                    {
                        //coupon not found
                        serviceResponse_VM.Status = -1;
                        serviceResponse_VM.Message = Resources.VisitorPanel.InvalidCoupon;
                        return serviceResponse_VM;
                    }
                    else if (_couponEndDate < DateTime.UtcNow.Date)
                    {
                        //coupon has been expired
                        serviceResponse_VM.Status = -1;
                        serviceResponse_VM.Message = Resources.VisitorPanel.CouponExpired_ErrorMessage;
                        return serviceResponse_VM;
                    }

                    // Get User Consumption Detail for same Coupon if it has been used
                    var couponConsumption = couponService.GetUserCouponConsumption(createEventBooking_VM.UserLoginId, couponDetail.Id);
                    if (couponConsumption != null)
                    {
                        // coupon code already used
                        serviceResponse_VM.Status = -1;
                        serviceResponse_VM.Message = Resources.VisitorPanel.CouponAlreadyUsed_ErrorMessage;
                        return serviceResponse_VM;
                    }
                }

                // Calculate Total Amount after all discounts if any 
                _TotalAmount = _EventData.Price - _CouponDiscount;

                // Check if Total Amount is equal to 0 or negative
                if (_TotalAmount <= 0)
                {
                    serviceResponse_VM.Status = -1;
                    serviceResponse_VM.Message = Resources.ErrorMessage.AmountCannotBeZero;
                    return serviceResponse_VM;
                }
            }
            else
            {
                // Free Event
            }

            return serviceResponse_VM;
        }

        /// <summary>
        /// Verify if Plan Can be booked by User.
        /// </summary>
        /// <param name="createPlanBooking_VM"></param>
        /// <returns>
        /// Returns Status = 1 if it can be purchased, and negative value if some error/issue 
        /// </returns>
        public ServiceResponse_VM VerifyPlanCanBeBooked(CreatePlanBooking_VM createPlanBooking_VM)
        {
            // Book Event for User/Student

            ServiceResponse_VM serviceResponse_VM = new ServiceResponse_VM();
            serviceResponse_VM.Status = 1;
            decimal _TotalAmount = 0;
            decimal _CouponDiscount = 0;

            // Get Class detail
            var _PlanData = planService.GetPlanDataById(createPlanBooking_VM.PlanId,createPlanBooking_VM.PlanType);

            if (_PlanData == null)
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.ErrorMessage.PlanNotFound;
                return serviceResponse_VM;
            }
            else if (planService.IsAlreadyPlanPurchased(createPlanBooking_VM.PlanId, createPlanBooking_VM.UserLoginId))
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.ErrorMessage.PlanAlreadyPurchased;
                return serviceResponse_VM;
            }

            // if coupon applied
            if (createPlanBooking_VM.CouponId > 0)
            {
                // Verify Coupon can be applied

                // Get Coupon Code Detail By Coupon Id
                var couponDetail = couponService.GetCouponDetailById(createPlanBooking_VM.CouponId);
                DateTime _couponEndDate = (couponDetail == null) ? DateTime.UtcNow.Date.AddDays(-1) : DateTime.ParseExact(couponDetail.EndDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                if (couponDetail == null)
                {
                    //coupon not found
                    serviceResponse_VM.Status = -1;
                    serviceResponse_VM.Message = Resources.VisitorPanel.InvalidCoupon;
                    return serviceResponse_VM;
                }
                else if (_couponEndDate < DateTime.UtcNow.Date)
                {
                    //coupon has been expired
                    serviceResponse_VM.Status = -1;
                    serviceResponse_VM.Message = Resources.VisitorPanel.CouponExpired_ErrorMessage;
                    return serviceResponse_VM;
                }

                // Get User Consumption Detail for same Coupon if it has been used
                var couponConsumption = couponService.GetUserCouponConsumption(createPlanBooking_VM.UserLoginId, couponDetail.Id);
                if (couponConsumption != null)
                {
                    // coupon code already used
                    serviceResponse_VM.Status = -1;
                    serviceResponse_VM.Message = Resources.VisitorPanel.CouponAlreadyUsed_ErrorMessage;
                    return serviceResponse_VM;
                }
            }

            // Calculate Total Amount after all discounts if any 
            _TotalAmount = _PlanData.Price - _CouponDiscount;

            // Check if Total Amount is equal to 0 or negative
            if (_TotalAmount <= 0)
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.ErrorMessage.AmountCannotBeZero;
                return serviceResponse_VM;
            }

            return serviceResponse_VM;
        }

        /// <summary>
        /// Book Class for User/Student
        /// </summary>
        /// <param name="createPlanBooking_VM"></param>
        /// <returns></returns>
        public ServiceResponse_VM CreateClassBooking(CreateClassBooking_VM createClassBooking_VM)
        {
            // Book plan for User/Student

            ServiceResponse_VM serviceResponse_VM = new ServiceResponse_VM();
            decimal _TotalAmount = 0;
            decimal _CouponDiscount = 0;
            decimal _CouponDiscountValue = 0;
            int _IsApproved = (createClassBooking_VM.OnlinePayment == 1) ? 1 : 0;

            // Get Class detail
            var _ClassDetail = classService.GetClassDataByID(createClassBooking_VM.ClassId);

            if (_ClassDetail == null)
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = "Class not found!";
                return serviceResponse_VM;
            }

            var classBatches = storedProcedureRepository.SP_ManageClassBatches_GetAll<BatchViewModel>(new SP_ManageClassBatches_Params_VM()
            {
                ClassId = createClassBooking_VM.ClassId,
                Mode = 1
            });

            var batchData = classBatches.Where(x => x.Id == createClassBooking_VM.BatchId).FirstOrDefault();

            if (classBatches == null || classBatches.Count() <= 0 || batchData == null)
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.ErrorMessage.ClassBatchNotFound_ErrorMessage;
                return serviceResponse_VM;
            }

            // check the limit of students bookings for the batch. [booking available or not]
            var batchBookingCount = classService.GetCurrentClassBatchBookingCount(createClassBooking_VM.ClassId, createClassBooking_VM.BatchId, createClassBooking_VM.JoinClassDate);
            if(batchData.StudentMaxStrength <= batchBookingCount)
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.ErrorMessage.ClassBatchFull_ErrorMessage;
                return serviceResponse_VM;
            }

            // if coupon applied
            if (createClassBooking_VM.CouponId > 0)
            {
                // Get Coupon Details
                var _CouponDetail = couponService.GetCouponDetailById(createClassBooking_VM.CouponId);
                _CouponDiscountValue = _CouponDetail.DiscountValue;

                if (_CouponDetail.IsFixedAmount == 1)
                {
                    _CouponDiscount = _CouponDetail.DiscountValue;
                }
                else
                {
                    // Calculate Discount value from dicount percentage on Plan/Package Amount
                    _CouponDiscount = (_ClassDetail.Price * ((decimal)_CouponDetail.DiscountValue / 100));
                }
            }

            // Calculate Total Amount after all discounts if any 
            _TotalAmount = _ClassDetail.Price - _CouponDiscount;

            // Check if Total Amount is equal to the Paid Amount
            if (createClassBooking_VM.TotalAmountPaid != _TotalAmount)
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.ErrorMessage.PaidAmountNotEqualsToTotalAmount;
                return serviceResponse_VM;
            }

            // Class Validity Date Calculations
            DateTime _ClassStartDate = DateTime.ParseExact(createClassBooking_VM.JoinClassDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            //DateTime _ClassStartDate = createClassBooking_VM.JoinClassDate;
            DateTime _ClassEndDate = _ClassStartDate;
            #region Set End Date for the Class/Package based on the package duraiton type
            if (_ClassDetail.ClassPriceType == "per_class")
            {
                // Per Class
                _ClassEndDate = _ClassStartDate;
            }
            else if (_ClassDetail.ClassPriceType == "per_month")
            {
                //monthly
                _ClassEndDate = _ClassStartDate.AddMonths(1);
            }
            else if (_ClassDetail.ClassPriceType == "demo")
            {
                // demo
                _ClassEndDate = _ClassStartDate;
            }

            #endregion

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Steps:
                    // Order Creation
                    // Payment Response
                    // PlanBooking creation
                    // Consume Coupon if applied
                    // Send Notifications
                    // AddOrUpdate Student link with Business if not linked. (BusinessStudents table)
                    // Add Student to ClassGroup.

                    #region Make Class Booking/Purchase
                    // Create Order
                    SP_InsertUpdateOrder_Params_VM sP_InsertUpdateOrder_Params_VM = new SP_InsertUpdateOrder_Params_VM()
                    {
                        UserLoginId = createClassBooking_VM.UserLoginId,
                        ItemId = createClassBooking_VM.ClassId,
                        ItemType = "Class",
                        OnlinePayment = createClassBooking_VM.OnlinePayment,
                        PaymentMethod = createClassBooking_VM.PaymentMethod,
                        CouponId = createClassBooking_VM.CouponId,
                        CouponDiscountValue = _CouponDiscountValue,
                        TotalDiscount = _CouponDiscount,
                        IsTaxable = 0,
                        Gst = 0,
                        TotalAmount = createClassBooking_VM.TotalAmountPaid,
                        Status = 1,
                        Mode = 1,
                        OwnerUserLoginId = _ClassDetail.BusinessOwnerLoginId
                    };
                    var order = storedProcedureRepository.SP_InsertUpdateOrder_Get<SPResponseViewModel>(sP_InsertUpdateOrder_Params_VM);

                    // Create Payment Response
                    SP_InsertUpdatePaymentResponse_Params_VM sP_InsertUpdatePaymentResponse_Params_VM = new SP_InsertUpdatePaymentResponse_Params_VM()
                    {
                        OrderId = order.Id,
                        Method = createClassBooking_VM.PaymentMethod,
                        Amount = createClassBooking_VM.TotalAmountPaid,
                        TransactionID = createClassBooking_VM.TransactionID,
                        Description = createClassBooking_VM.PaymentDescription,
                        IsApproved = _IsApproved,
                        Provider = createClassBooking_VM.PaymentProvider,
                        ResponseStatus = createClassBooking_VM.PaymentResponseStatus,
                        SubmittedByLoginId = createClassBooking_VM.UserLoginId,
                        Mode = 1
                    };
                    var paymentResponse = storedProcedureRepository.SP_InsertUpdatePaymentResponse_Get<SPResponseViewModel>(sP_InsertUpdatePaymentResponse_Params_VM);

                    // Create Class-QR code and save
                    string _QRCodeTicket = "";

                    if (_ClassDetail.ClassMode == "Offline")
                    {
                        _QRCodeTicket = classService.CreateAndSaveQRCodeTicket(_ClassDetail.Id, order.Id);
                    }
                    else
                    {
                        _QRCodeTicket = "";
                    }

                    // Create Class-Booking
                    SP_InsertUpdateClassBooking_Params_VM sP_InsertUpdateClassBooking_Params_VM = new SP_InsertUpdateClassBooking_Params_VM()
                    {
                        OrderId = order.Id,
                        ClassId = createClassBooking_VM.ClassId,
                        UserLoginId = createClassBooking_VM.UserLoginId,
                        ClassQRCode = _QRCodeTicket,
                        ClassStartDate = _ClassStartDate.ToString("yyyy-MM-dd"),
                        ClassEndDate = _ClassEndDate.ToString("yyyy-MM-dd"),
                        BatchId = createClassBooking_VM.BatchId,
                        Mode = 1
                    };

                    var classBooking = storedProcedureRepository.SP_InsertUpdateClassBooking_Get<SPResponseViewModel>(sP_InsertUpdateClassBooking_Params_VM);

                    if (createClassBooking_VM.CouponId > 0)
                    {
                        var consumeCoupon = couponService.AddCouponConsumption(createClassBooking_VM.CouponId, createClassBooking_VM.UserLoginId);
                    }

                    // Send Notification to Business Owner
                    var classData = classService.GetClassDataByID(createClassBooking_VM.ClassId);
                    BasicProfileDetail_VM studentBasicProfileDetail_VM = new BasicProfileDetail_VM();
                    SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessOwnerLoginId", "0"),
                            new SqlParameter("userLoginId", createClassBooking_VM.UserLoginId),
                            new SqlParameter("mode", "1")
                            };

                    var respStudentBasicDetail = db.Database.SqlQuery<BasicProfileDetail_VM>("exec sp_ManageStudentProfile @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();
                    var studentName = respStudentBasicDetail.FirstName + " " + respStudentBasicDetail.LastName;

                    // send notification to business
                    notificationService.InsertUpdateNotification(new SPInsertUpdateNotification_Params_VM()
                    {
                        NotificationText = String.Format(Resources.NotificationMessage.BusinessNotification_NewClassBookingMessage, classData.Name,studentName,_ClassStartDate.ToString("yyyy-MM-dd")),
                        NotificationTitle = String.Format(Resources.NotificationMessage.BusinessNotification_NewClassBookingTitle, classData.Name),
                        NotificationType = NotificationTypes.NewBooking.ToString(),
                        NotificationUsersList = classData.BusinessOwnerLoginId.ToString(),
                        SubmittedByLoginId = 0,
                        Mode = 1,
                        IsNotificationLinkable = 1,
                        ItemId = classBooking.Id,
                        ItemTable = "ClassBookings",
                        OrderId = order.Id,
                    });

                    // send notification to Student/User
                    notificationService.InsertUpdateNotification(new SPInsertUpdateNotification_Params_VM()
                    {
                        NotificationText = String.Format(Resources.NotificationMessage.UserNotification_NewClassBookingMessage, classData.Name, DateTime.UtcNow.AddMinutes(330).ToString("yyyy-MM-dd")),
                        NotificationTitle = String.Format(Resources.NotificationMessage.UserNotification_NewClassBookingTitle, classData.Name),
                        NotificationType = NotificationTypes.NewBooking.ToString(),
                        NotificationUsersList = createClassBooking_VM.UserLoginId.ToString(),
                        SubmittedByLoginId = 0,
                        Mode = 1,
                        IsNotificationLinkable = 1,
                        ItemId = classBooking.Id,
                        ItemTable = "ClassBookings",
                        OrderId = order.Id,
                    });

                    // Link Student with Business
                    var respBusinessStudentLinking = businessStudentService.AddStudentLinkWithBusinessOwner(classData.BusinessOwnerLoginId, createClassBooking_VM.UserLoginId);

                    // Add User to Class-Group 
                    groupService = new GroupService(db);
                    var addMemberInGroupResponse = groupService.AddMemberInGroup(new AddGroupMember_VM
                    {
                        GroupId = batchData.GroupId,
                        GroupUserLoginId = classData.BusinessOwnerLoginId,
                        MemberLoginId = createClassBooking_VM.UserLoginId
                    });

                    #endregion

                    db.SaveChanges(); // Save changes to the database

                    transaction.Commit(); // Commit the transaction if everything is successful
                    serviceResponse_VM.Status = 1;
                    serviceResponse_VM.Message = "Class Purchased!";
                    //send mails 
                    EmailSender classBookingMail = new EmailSender();

                    string msg = classBookingMail.ClassBookedMailMessage(_ClassDetail, _QRCodeTicket);

                    classBookingMail.Send(studentName, "Your Class booking successfully done " + classData.Name, respStudentBasicDetail.Email, msg, "");
                }
                catch (Exception ex)
                {
                    // Handle exceptions and perform error handling or logging
                    transaction.Rollback(); // Roll back the transaction
                    serviceResponse_VM.Status = -100;
                    serviceResponse_VM.Message = Resources.ErrorMessage.InternalServerErrorMessage;
                }
            } // transaction scope ends

            return serviceResponse_VM;
        }

        /// <summary>
        /// Verify if Class Can be booked by User.
        /// </summary>
        /// <param name="createClassBooking_VM"></param>
        /// <returns>
        /// Returns Status = 1 if it can be purchased, and negative value if some error/issue 
        /// </returns>
        public ServiceResponse_VM VerifyClassCanBeBooked(CreateClassBooking_VM createClassBooking_VM)
        {
            // Book Event for User/Student

            ServiceResponse_VM serviceResponse_VM = new ServiceResponse_VM();
            serviceResponse_VM.Status = 1;
            decimal _TotalAmount = 0;
            decimal _CouponDiscount = 0;

            // Get Class detail
            var _ClassDetail = classService.GetClassDataByID(createClassBooking_VM.ClassId);
            DateTime _ClassJoinDate = DateTime.ParseExact(createClassBooking_VM.JoinClassDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            string dayName = _ClassJoinDate.ToString("dddd");

            if (_ClassDetail == null)
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.ErrorMessage.ClassNotFound;
                return serviceResponse_VM;
            }
            else if (_ClassJoinDate < DateTime.UtcNow.Date)
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.ErrorMessage.JoinDateNotValid;
                return serviceResponse_VM;
            }
            else if (!_ClassDetail.ClassDays.Contains(dayName))
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = String.Format(Resources.ErrorMessage.ClassAvailabilityDays, _ClassDetail.ClassDays);
                return serviceResponse_VM;
            }
            else if (classService.IsAlreadyClassPurchased(createClassBooking_VM.ClassId, createClassBooking_VM.UserLoginId, createClassBooking_VM.JoinClassDate))
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.ErrorMessage.ClassAlreadyPurchased;
                return serviceResponse_VM;
            } 

            // check the limit of students bookings for the batch. [booking available or not]
            var classBatches = storedProcedureRepository.SP_ManageClassBatches_GetAll<BatchViewModel>(new SP_ManageClassBatches_Params_VM()
            {
                ClassId = createClassBooking_VM.ClassId,
                Mode = 1
            });

            var batchData = classBatches.Where(x => x.Id == createClassBooking_VM.BatchId).FirstOrDefault();

            if (classBatches == null || classBatches.Count() <= 0 || batchData == null)
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.ErrorMessage.ClassBatchFull_ErrorMessage;
                return serviceResponse_VM;
            }

            var batchBookingCount = classService.GetCurrentClassBatchBookingCount(createClassBooking_VM.ClassId, createClassBooking_VM.BatchId, createClassBooking_VM.JoinClassDate);
            if (batchData.StudentMaxStrength <= batchBookingCount)
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.ErrorMessage.ClassBatchNotFound_ErrorMessage;
                return serviceResponse_VM;
            }

            // If Paid Class 
            if (_ClassDetail.IsPaid == 1)
            {
                // if coupon applied
                if (createClassBooking_VM.CouponId > 0)
                {
                    // Verify Coupon can be applied

                    // Get Coupon Code Detail By Coupon Id
                    var couponDetail = couponService.GetCouponDetailById(createClassBooking_VM.CouponId);
                    DateTime _couponEndDate = (couponDetail == null) ? DateTime.UtcNow.Date.AddDays(-1) : DateTime.ParseExact(couponDetail.EndDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                    if (couponDetail == null)
                    {
                        //coupon not found
                        serviceResponse_VM.Status = -1;
                        serviceResponse_VM.Message = Resources.VisitorPanel.InvalidCoupon;
                        return serviceResponse_VM;
                    }
                    else if (_couponEndDate < DateTime.UtcNow.Date)
                    {
                        //coupon has been expired
                        serviceResponse_VM.Status = -1;
                        serviceResponse_VM.Message = Resources.VisitorPanel.CouponExpired_ErrorMessage;
                        return serviceResponse_VM;
                    }

                    // Get User Consumption Detail for same Coupon if it has been used
                    var couponConsumption = couponService.GetUserCouponConsumption(createClassBooking_VM.UserLoginId, couponDetail.Id);
                    if (couponConsumption != null)
                    {
                        // coupon code already used
                        serviceResponse_VM.Status = -1;
                        serviceResponse_VM.Message = Resources.VisitorPanel.CouponAlreadyUsed_ErrorMessage;
                        return serviceResponse_VM;
                    }
                }

                // Calculate Total Amount after all discounts if any 
                _TotalAmount = _ClassDetail.Price - _CouponDiscount;

                // Check if Total Amount is equal to 0 or negative
                if (_TotalAmount <= 0)
                {
                    serviceResponse_VM.Status = -1;
                    serviceResponse_VM.Message = Resources.ErrorMessage.AmountCannotBeZero;
                    return serviceResponse_VM;
                }
            }
            else
            {
                // Free Class
            }

            return serviceResponse_VM;
        }

        /// <summary>
        /// Get all Bookings by Student/User.
        /// </summary>
        /// <param name="userLoginId">User-Login-Id</param>
        /// <param name="lastRecordId">Last-Fetched-Recod-Id</param>
        /// <param name="recordLimit">No. of records to return</param>
        /// <returns>All Bookings done by user</returns>
        public List<UserBookingList_VM> GetAllBookingList(long userLoginId, long lastRecordId, int recordLimit)
        {
            return storedProcedureRepository.SP_GetAllStudentCourseDetail_GetAll<UserBookingList_VM>(new SP_GetAllStudentCourseDetail_Param_VM
            {
                UserLoginId = userLoginId,
                LastRecordId = lastRecordId,
                RecordLimit = recordLimit,
                Mode = 3
            });
        }

        /// <summary>
        /// Book License for Business-Owner (Request Certificate-License bookings)
        /// </summary>
        public ServiceResponse_VM CreateLicenseBooking(CreateLicenseBooking_VM createLicenseBooking_VM)
        {
            // cross check license can be booked before creating order.
            var resp_VerifyLicenseBooking = VerifyLicenseCanBeBooked(createLicenseBooking_VM);
            if(resp_VerifyLicenseBooking.Status <= 0)
            {
                return resp_VerifyLicenseBooking;
            }

            // Book License for Business-Owner.
            ServiceResponse_VM serviceResponse_VM = new ServiceResponse_VM();
            decimal _TotalAmount = 0;
            decimal _SubTotalAmount = 0;
            decimal _GSTAmount = 0;
            decimal _CouponDiscount = 0;
            decimal _CouponDiscountValue = 0;
            int _IsApproved = (createLicenseBooking_VM.OnlinePayment == 1) ? 0 : 0; // On approval licenses are assigned to business Owner
            long OwnerUserLoginId = 1; // SuperAdmin login id

            //var _LicenseDetail = licenseService.GetLicenseRecordDataById(createLicenseBooking_VM.LicenseId);

            // Get License Record data
            var licenseData = licenseService.GetLicenseRecordDataById(createLicenseBooking_VM.LicenseId);

            if (licenseData == null)
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.ErrorMessage.LicenseNotFound;
                return serviceResponse_VM;
            }

            // Calculate Total Amount after all discounts if any 
            _SubTotalAmount = (licenseData.Price * createLicenseBooking_VM.Quantity);
            _GSTAmount = _SubTotalAmount * (licenseData.GSTPercent / (decimal)100.00);
            _TotalAmount = (_SubTotalAmount + _GSTAmount) - _CouponDiscount;

            // Check if Total Amount is equal to the Paid Amount
            if (createLicenseBooking_VM.TotalAmountPaid != _TotalAmount)
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.ErrorMessage.PaidAmountNotEqualsToTotalAmount;
                return serviceResponse_VM;
            }


            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Steps:
                    // Order Creation
                    // Payment Response
                    // License Booking creation
                    // Consume Coupon if applied
                    // Send Notifications

                    #region Make License Booking/Purchase
                    // Create Order
                    SP_InsertUpdateOrder_Params_VM sP_InsertUpdateOrder_Params_VM = new SP_InsertUpdateOrder_Params_VM()
                    {
                        UserLoginId = createLicenseBooking_VM.UserLoginId,
                        ItemId = createLicenseBooking_VM.LicenseId,
                        ItemType = "License",
                        OnlinePayment = createLicenseBooking_VM.OnlinePayment,
                        PaymentMethod = createLicenseBooking_VM.PaymentMethod,
                        CouponId = createLicenseBooking_VM.CouponId,
                        CouponDiscountValue = _CouponDiscountValue,
                        TotalDiscount = _CouponDiscount,
                        IsTaxable = (_GSTAmount > 0) ? 1 : 0,
                        Gst = _GSTAmount,
                        TotalAmount = createLicenseBooking_VM.TotalAmountPaid,
                        Status = 1,
                        Mode = 1,
                        OwnerUserLoginId = OwnerUserLoginId
                    };
                    var order = storedProcedureRepository.SP_InsertUpdateOrder_Get<SPResponseViewModel>(sP_InsertUpdateOrder_Params_VM);

                    // Create Payment Response
                    SP_InsertUpdatePaymentResponse_Params_VM sP_InsertUpdatePaymentResponse_Params_VM = new SP_InsertUpdatePaymentResponse_Params_VM()
                    {
                        OrderId = order.Id,
                        Method = createLicenseBooking_VM.PaymentMethod,
                        Amount = createLicenseBooking_VM.TotalAmountPaid,
                        TransactionID = createLicenseBooking_VM.TransactionID,
                        Description = createLicenseBooking_VM.PaymentDescription,
                        IsApproved = _IsApproved,
                        Provider = createLicenseBooking_VM.PaymentProvider,
                        ResponseStatus = createLicenseBooking_VM.PaymentResponseStatus,
                        SubmittedByLoginId = createLicenseBooking_VM.UserLoginId,
                        Mode = 1
                    };
                    var paymentResponse = storedProcedureRepository.SP_InsertUpdatePaymentResponse_Get<SPResponseViewModel>(sP_InsertUpdatePaymentResponse_Params_VM);

                    // Create License-Booking
                    SP_InsertUpdateLicenseBooking_Params_VM sP_InsertUpdateLicenseBooking_Params_VM = new SP_InsertUpdateLicenseBooking_Params_VM()
                    {
                        OrderId = order.Id,
                        LicenseId = createLicenseBooking_VM.LicenseId,
                        BusinessOwnerLoginId = createLicenseBooking_VM.UserLoginId,
                        Quantity = createLicenseBooking_VM.Quantity,
                        Status = 1,
                        SubmittedByLoginId = createLicenseBooking_VM.UserLoginId,
                        Mode = 1
                    };

                    var licenseBooking = storedProcedureRepository.SP_InsertUpdateLicenseBooking_Get<SPResponseViewModel>(sP_InsertUpdateLicenseBooking_Params_VM);

                    #endregion ------------------------------

                    db.SaveChanges(); // Save changes to the database

                    transaction.Commit(); // Commit the transaction if everything is successful
                    serviceResponse_VM.Status = 1;
                    serviceResponse_VM.Message = "License Purchased!";
                }
                catch (Exception ex)
                {
                    // Handle exceptions and perform error handling or logging
                    transaction.Rollback(); // Roll back the transaction
                    serviceResponse_VM.Status = -100;
                    serviceResponse_VM.Message = Resources.ErrorMessage.InternalServerErrorMessage;
                }
            } // transaction scope ends

            return serviceResponse_VM;
        }

        /// <summary>
        /// Verify if License Can be booked by Business-User.
        /// </summary>
        /// <param name="createLicenseBooking_VM"></param>
        /// <returns>
        /// Returns Status = 1 if it can be purchased, and negative value if some error/issue 
        /// </returns>
        public ServiceResponse_VM VerifyLicenseCanBeBooked(CreateLicenseBooking_VM createLicenseBooking_VM)
        {
            // Book License for Business
            ServiceResponse_VM serviceResponse_VM = new ServiceResponse_VM();
            serviceResponse_VM.Status = 1;
            decimal _TotalAmount = 0;
            decimal _CouponDiscount = 0;

            // Get License detail
            var _LicenseData = licenseService.GetLicenseRecordDataById(createLicenseBooking_VM.LicenseId);

            if (_LicenseData == null)
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.ErrorMessage.LicenseNotFound;
                return serviceResponse_VM;
            }
            // Check if this business already have License quantity available for use.
            var licenseBookingsQuantity = licenseService.GetApprovedLicenseBookingListHavingRemainingQuantity(createLicenseBooking_VM.UserLoginId, createLicenseBooking_VM.LicenseId);
            if (licenseBookingsQuantity.Count() > 0)
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.BusinessPanel.LicenseBooking_LicenseAlreadyAvailableInBooking_ErrorMessage;
                return serviceResponse_VM;
            }
            // Check if already have a pending request for same license
            var licenseBookingList = db.LicenseBookings.Where(lb => lb.BusinessOwnerLoginId == createLicenseBooking_VM.UserLoginId && lb.LicenseId == createLicenseBooking_VM.LicenseId && lb.Status == (int)LicenseBookingStatus.Pending).ToList();
            if (licenseBookingList.Count() > 0)
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.BusinessPanel.LicenseBooking_AlreadyHavePendingLicenseRequest_ErrorMessage;
                return serviceResponse_VM;
            }

            // Calculate Total Amount after all discounts if any 
            _TotalAmount = (_LicenseData.Price * createLicenseBooking_VM.Quantity) - _CouponDiscount;

            // Check if Total Amount is equal to 0 or negative
            if (_LicenseData.IsPaid == 1 && _TotalAmount <= 0)
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.ErrorMessage.AmountCannotBeZero;
                return serviceResponse_VM;
            }

            return serviceResponse_VM;
        }

        /// <summary>
        /// Book Training for User/Student
        /// </summary>
        /// <param name="createTrainingBooking_VM">Required data for training boking</param>
        /// <returns>Returns +ve value(1) if booked otherwise -ve status with message. </returns>
        public ServiceResponse_VM CreateTrainingBooking(CreateTrainingBooking_VM createTrainingBooking_VM)
        {
            // Book Training for User/Student

            ServiceResponse_VM serviceResponse_VM = new ServiceResponse_VM();
            decimal _TotalAmount = 0;
            decimal _CouponDiscount = 0;
            decimal _CouponDiscountValue = 0;
            int _IsApproved = (createTrainingBooking_VM.OnlinePayment == 1) ? 1 : 0;

            // Get Training detail
            var trainingData = trainingService.GetTrainingDataById(createTrainingBooking_VM.TrainingId);

            if (trainingData == null)
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.ErrorMessage.PlanNotFound;
                return serviceResponse_VM;
            }

            // if coupon applied
            if (createTrainingBooking_VM.CouponId > 0)
            {
                // Get Coupon Details
                var _CouponDetail = couponService.GetCouponDetailById(createTrainingBooking_VM.CouponId);
                _CouponDiscountValue = _CouponDetail.DiscountValue;

                if (_CouponDetail.IsFixedAmount == 1)
                {
                    _CouponDiscount = _CouponDetail.DiscountValue;
                }
                else
                {
                    // Calculate Discount value from dicount percentage on Plan/Package Amount
                    _CouponDiscount = (trainingData.Price * ((decimal)_CouponDetail.DiscountValue / 100));
                }
            }

            // Calculate Total Amount after all discounts if any 
            _TotalAmount = trainingData.Price - _CouponDiscount;

            // Check if Total Amount is equal to the Paid Amount
            if (createTrainingBooking_VM.TotalAmountPaid != _TotalAmount)
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.ErrorMessage.PaidAmountNotEqualsToTotalAmount;
                return serviceResponse_VM;
            }

            // Training Validity Date Calculations
            DateTime _TrainingStartDate = trainingData.StartDate_DateTimeFormat;
            DateTime _TrainingEndDate = trainingData.EndDate_DateTimeFormat;

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Steps:
                    // Order Creation
                    // Payment Response
                    // PlanBooking creation
                    // Consume Coupon if applied
                    // Send Notifications
                    // AddOrUpdate Student link with Business if not linked. (BusinessStudents table)

                    #region Make Plan Booking/Purchase
                    // Create Order
                    SP_InsertUpdateOrder_Params_VM sP_InsertUpdateOrder_Params_VM = new SP_InsertUpdateOrder_Params_VM()
                    {
                        UserLoginId = createTrainingBooking_VM.UserLoginId,
                        ItemId = createTrainingBooking_VM.TrainingId,
                        ItemType = "Plan",
                        OnlinePayment = createTrainingBooking_VM.OnlinePayment,
                        PaymentMethod = createTrainingBooking_VM.PaymentMethod,
                        CouponId = createTrainingBooking_VM.CouponId,
                        CouponDiscountValue = _CouponDiscountValue,
                        TotalDiscount = _CouponDiscount,
                        IsTaxable = 0,
                        Gst = 0,
                        TotalAmount = createTrainingBooking_VM.TotalAmountPaid,
                        Status = 1,
                        Mode = 1,
                        OwnerUserLoginId = trainingData.UserLoginId
                    };
                    var order = storedProcedureRepository.SP_InsertUpdateOrder_Get<SPResponseViewModel>(sP_InsertUpdateOrder_Params_VM);

                    // Create Payment Response
                    SP_InsertUpdatePaymentResponse_Params_VM sP_InsertUpdatePaymentResponse_Params_VM = new SP_InsertUpdatePaymentResponse_Params_VM()
                    {
                        OrderId = order.Id,
                        Method = createTrainingBooking_VM.PaymentMethod,
                        Amount = createTrainingBooking_VM.TotalAmountPaid,
                        TransactionID = createTrainingBooking_VM.TransactionID,
                        Description = createTrainingBooking_VM.PaymentDescription,
                        IsApproved = _IsApproved,
                        Provider = createTrainingBooking_VM.PaymentProvider,
                        ResponseStatus = createTrainingBooking_VM.PaymentResponseStatus,
                        SubmittedByLoginId = createTrainingBooking_VM.UserLoginId,
                        Mode = 1
                    };
                    var paymentResponse = storedProcedureRepository.SP_InsertUpdatePaymentResponse_Get<SPResponseViewModel>(sP_InsertUpdatePaymentResponse_Params_VM);

                    // Create Training-Booking
                    SP_InsertUpdateTrainingBooking_Params_VM sp_InsertUpdateTrainingBooking_Params_VM = new SP_InsertUpdateTrainingBooking_Params_VM()
                    {
                        OrderId = order.Id,
                        TrainingId = createTrainingBooking_VM.TrainingId,
                        TrainingStartDate = _TrainingStartDate.ToString("yyyy-MM-dd"),
                        TrainingEndDate = _TrainingEndDate.ToString("yyyy-MM-dd"),
                        StudentUserLoginId = createTrainingBooking_VM.UserLoginId,
                        Mode = 1
                    };

                    var trainingBooking = storedProcedureRepository.SP_InsertUpdateTrainingBooking_Get<SPResponseViewModel>(sp_InsertUpdateTrainingBooking_Params_VM);

                    if (createTrainingBooking_VM.CouponId > 0)
                    {
                        var consumeCoupon = couponService.AddCouponConsumption(createTrainingBooking_VM.CouponId, createTrainingBooking_VM.UserLoginId);
                    }

                    // Send Notification to Business Owner
                    BasicProfileDetail_VM studentBasicProfileDetail_VM = new BasicProfileDetail_VM();
                    SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessOwnerLoginId", "0"),
                            new SqlParameter("userLoginId", createTrainingBooking_VM.UserLoginId),
                            new SqlParameter("mode", "1")
                            };

                    var respStudentBasicDetail = db.Database.SqlQuery<BasicProfileDetail_VM>("exec sp_ManageStudentProfile @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();
                    var studentName = respStudentBasicDetail.FirstName + " " + respStudentBasicDetail.LastName;

                    notificationService.InsertUpdateNotification(new SPInsertUpdateNotification_Params_VM()
                    {
                        NotificationText = String.Format(Resources.NotificationMessage.BusinessNotification_NewTrainingBookingMessage, trainingData.TrainingName, studentName, _TrainingStartDate.ToString("yyyy-MM-dd")),
                        NotificationTitle = String.Format(Resources.NotificationMessage.BusinessNotification_NewTrainingBookingTitle, trainingData.TrainingName),
                        NotificationType = NotificationTypes.NewBooking.ToString(),
                        NotificationUsersList = trainingData.UserLoginId.ToString(),
                        SubmittedByLoginId = 0,
                        Mode = 1,
                        IsNotificationLinkable = 1,
                        ItemId = trainingBooking.Id,
                        ItemTable = "TrainingBookings"
                    });

                    // send notification to Student/User
                    notificationService.InsertUpdateNotification(new SPInsertUpdateNotification_Params_VM()
                    {
                        NotificationText = String.Format(Resources.NotificationMessage.UserNotification_NewTrainingBookingMessage, trainingData.TrainingName, DateTime.UtcNow.AddMinutes(330).ToString("yyyy-MM-dd")),
                        NotificationTitle = String.Format(Resources.NotificationMessage.UserNotification_NewTrainingBookingTitle, trainingData.TrainingName),
                        NotificationType = NotificationTypes.NewBooking.ToString(),
                        NotificationUsersList = createTrainingBooking_VM.UserLoginId.ToString(),
                        SubmittedByLoginId = 0,
                        Mode = 1,
                        IsNotificationLinkable = 1,
                        ItemId = trainingBooking.Id,
                        ItemTable = "TrainingBookings"
                    });

                    // Link Student with Business
                    var respBusinessStudentLinking = businessStudentService.AddStudentLinkWithBusinessOwner(trainingData.UserLoginId, createTrainingBooking_VM.UserLoginId);
                    #endregion

                    db.SaveChanges(); // Save changes to the database

                    transaction.Commit(); // Commit the transaction if everything is successful
                    serviceResponse_VM.Status = 1;
                    serviceResponse_VM.Message = "Training Purchased!";
                }
                catch (Exception ex)
                {
                    // Handle exceptions and perform error handling or logging
                    transaction.Rollback(); // Roll back the transaction
                    serviceResponse_VM.Status = -100;
                    serviceResponse_VM.Message = Resources.ErrorMessage.InternalServerErrorMessage;
                }
            } // transaction scope ends

            return serviceResponse_VM;
        }

        /// <summary>
        /// Verify if Training Can be booked by User.
        /// </summary>
        /// <param name="createTrainingBooking_VM"></param>
        /// <returns>
        /// Returns Status = 1 if it can be purchased, and negative value if some error/issue 
        /// </returns>
        public ServiceResponse_VM VerifyTrainingCanBeBooked(CreateTrainingBooking_VM createTrainingBooking_VM)
        {
            // Book Training for User/Student

            ServiceResponse_VM serviceResponse_VM = new ServiceResponse_VM();
            serviceResponse_VM.Status = 1;
            decimal _TotalAmount = 0;
            decimal _CouponDiscount = 0;

            // Get Training detail
            var _TrainingData = trainingService.GetTrainingDataById(createTrainingBooking_VM.TrainingId);

            if (_TrainingData == null)
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.ErrorMessage.TrainingNotFound;
                return serviceResponse_VM;
            }
            else if(_TrainingData.EndDate_DateTimeFormat <= DateTime.UtcNow)
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.ErrorMessage.ItemNotAvailableForBooking;
                return serviceResponse_VM;
            }
            else if (trainingService.IsAlreadyTrainingPurchased(createTrainingBooking_VM.TrainingId, createTrainingBooking_VM.UserLoginId))
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.ErrorMessage.TrainingAlreadyPurchased;
                return serviceResponse_VM;
            }

            // if coupon applied
            if (createTrainingBooking_VM.CouponId > 0)
            {
                // Verify Coupon can be applied

                // Get Coupon Code Detail By Coupon Id
                var couponDetail = couponService.GetCouponDetailById(createTrainingBooking_VM.CouponId);
                DateTime _couponEndDate = (couponDetail == null) ? DateTime.UtcNow.Date.AddDays(-1) : DateTime.ParseExact(couponDetail.EndDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                if (couponDetail == null)
                {
                    //coupon not found
                    serviceResponse_VM.Status = -1;
                    serviceResponse_VM.Message = Resources.VisitorPanel.InvalidCoupon;
                    return serviceResponse_VM;
                }
                else if (_couponEndDate < DateTime.UtcNow.Date)
                {
                    //coupon has been expired
                    serviceResponse_VM.Status = -1;
                    serviceResponse_VM.Message = Resources.VisitorPanel.CouponExpired_ErrorMessage;
                    return serviceResponse_VM;
                }

                // Get User Consumption Detail for same Coupon if it has been used
                var couponConsumption = couponService.GetUserCouponConsumption(createTrainingBooking_VM.UserLoginId, couponDetail.Id);
                if (couponConsumption != null)
                {
                    // coupon code already used
                    serviceResponse_VM.Status = -1;
                    serviceResponse_VM.Message = Resources.VisitorPanel.CouponAlreadyUsed_ErrorMessage;
                    return serviceResponse_VM;
                }
            }

            // Calculate Total Amount after all discounts if any 
            _TotalAmount = _TrainingData.Price - _CouponDiscount;

            // Check if Total Amount is equal to 0 or negative
            if (_TotalAmount <= 0)
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.ErrorMessage.AmountCannotBeZero;
                return serviceResponse_VM;
            }

            return serviceResponse_VM;
        }



        /// <summary>
        /// Verify if Course Can be booked by User.
        /// </summary>
        /// <param name="createCourseBooking_VM"></param>
        /// <returns></returns>
        public ServiceResponse_VM VerifyCourseCanBeBooked(CreateCourseBooking_VM createCourseBooking_VM)
        {

            ServiceResponse_VM serviceResponse_VM = new ServiceResponse_VM();
            serviceResponse_VM.Status = 1;
            decimal _TotalAmount = 0;
            decimal _CouponDiscount = 0;

            // Get Course detail
            var _CourseDetail = courseService.GetCourseDataByID(createCourseBooking_VM.CourseId);


            if (_CourseDetail == null)
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.ErrorMessage.CourseNotFound;
                return serviceResponse_VM;
            }

            else if (courseService.IsAlreadyCoursePurchased(createCourseBooking_VM.CourseId, createCourseBooking_VM.UserLoginId, createCourseBooking_VM.JoinCourseDate))
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.ErrorMessage.CourseAlreadyPurchased;
                return serviceResponse_VM;
            }

            // If Paid Class 
            if (_CourseDetail.IsPaid == 1)
            {
                // if coupon applied
                if (createCourseBooking_VM.CouponId > 0)
                {
                    // Verify Coupon can be applied

                    // Get Coupon Code Detail By Coupon Id
                    var couponDetail = couponService.GetCouponDetailById(createCourseBooking_VM.CouponId);
                    DateTime _couponEndDate = (couponDetail == null) ? DateTime.UtcNow.Date.AddDays(-1) : DateTime.ParseExact(couponDetail.EndDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                    if (couponDetail == null)
                    {
                        //coupon not found
                        serviceResponse_VM.Status = -1;
                        serviceResponse_VM.Message = Resources.VisitorPanel.InvalidCoupon;
                        return serviceResponse_VM;
                    }
                    else if (_couponEndDate < DateTime.UtcNow.Date)
                    {
                        //coupon has been expired
                        serviceResponse_VM.Status = -1;
                        serviceResponse_VM.Message = Resources.VisitorPanel.CouponExpired_ErrorMessage;
                        return serviceResponse_VM;
                    }

                    // Get User Consumption Detail for same Coupon if it has been used
                    var couponConsumption = couponService.GetUserCouponConsumption(createCourseBooking_VM.UserLoginId, couponDetail.Id);
                    if (couponConsumption != null)
                    {
                        // coupon code already used
                        serviceResponse_VM.Status = -1;
                        serviceResponse_VM.Message = Resources.VisitorPanel.CouponAlreadyUsed_ErrorMessage;
                        return serviceResponse_VM;
                    }
                }

                // Calculate Total Amount after all discounts if any 
                _TotalAmount = _CourseDetail.Price - _CouponDiscount;


                // Check if Total Amount is equal to 0 or negative
                if (_TotalAmount <= 0)
                {
                    serviceResponse_VM.Status = -1;
                    serviceResponse_VM.Message = Resources.ErrorMessage.AmountCannotBeZero;
                    return serviceResponse_VM;
                }
            }
            else
            {
                // Free course
            }
            return serviceResponse_VM;
        }

        /// <summary>
        /// To Create Course Booking Detail 
        /// </summary>
        /// <param name="createCourseBooking_VM"></param>
        /// <returns></returns>
        public ServiceResponse_VM CreateCourseBooking(CreateCourseBooking_VM createCourseBooking_VM)
        {
            // Book plan for User/Student

            ServiceResponse_VM serviceResponse_VM = new ServiceResponse_VM();
            decimal _TotalAmount = 0;
            decimal _CouponDiscount = 0;
            decimal _CouponDiscountValue = 0;
            int _IsApproved = (createCourseBooking_VM.OnlinePayment == 1) ? 1 : 0;

            // Get Course detail
            var _CourseDetail = courseService.GetCourseDataByID(createCourseBooking_VM.CourseId);

            if (_CourseDetail == null)
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = "Course not found!";
                return serviceResponse_VM;
            }

            // if coupon applied
            if (createCourseBooking_VM.CouponId > 0)
            {
                // Get Coupon Details
                var _CouponDetail = couponService.GetCouponDetailById(createCourseBooking_VM.CouponId);
                _CouponDiscountValue = _CouponDetail.DiscountValue;

                if (_CouponDetail.IsFixedAmount == 1)
                {
                    _CouponDiscount = _CouponDetail.DiscountValue;
                }
                else
                {
                    // Calculate Discount value from dicount percentage on Plan/Package Amount
                    _CouponDiscount = (_CourseDetail.Price * ((decimal)_CouponDetail.DiscountValue / 100));
                }
            }

            // Calculate Total Amount after all discounts if any 
            _TotalAmount = _CourseDetail.Price - _CouponDiscount;

            // Check if Total Amount is equal to the Paid Amount
            if (createCourseBooking_VM.TotalAmountPaid != _TotalAmount)
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.ErrorMessage.PaidAmountNotEqualsToTotalAmount;
                return serviceResponse_VM;
            }


            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    #region Make Course Booking/Purchase
                    // Create Order
                    SP_InsertUpdateOrder_Params_VM sP_InsertUpdateOrder_Params_VM = new SP_InsertUpdateOrder_Params_VM()
                    {
                        UserLoginId = createCourseBooking_VM.UserLoginId,
                        ItemId = createCourseBooking_VM.CourseId,
                        ItemType = "Course",
                        OnlinePayment = createCourseBooking_VM.OnlinePayment,
                        PaymentMethod = createCourseBooking_VM.PaymentMethod,
                        CouponId = createCourseBooking_VM.CouponId,
                        CouponDiscountValue = _CouponDiscountValue,
                        TotalDiscount = _CouponDiscount,
                        IsTaxable = 0,
                        Gst = 0,
                        TotalAmount = createCourseBooking_VM.TotalAmountPaid,
                        Status = 1,
                        Mode = 1,
                        OwnerUserLoginId = _CourseDetail.BusinessOwnerLoginId
                    };
                    var order = storedProcedureRepository.SP_InsertUpdateOrder_Get<SPResponseViewModel>(sP_InsertUpdateOrder_Params_VM);

                    // Create Payment Response
                    SP_InsertUpdatePaymentResponse_Params_VM sP_InsertUpdatePaymentResponse_Params_VM = new SP_InsertUpdatePaymentResponse_Params_VM()
                    {
                        OrderId = order.Id,
                        Method = createCourseBooking_VM.PaymentMethod,
                        Amount = createCourseBooking_VM.TotalAmountPaid,
                        TransactionID = createCourseBooking_VM.TransactionID,
                        Description = createCourseBooking_VM.PaymentDescription,
                        IsApproved = _IsApproved,
                        Provider = createCourseBooking_VM.PaymentProvider,
                        ResponseStatus = createCourseBooking_VM.PaymentResponseStatus,
                        SubmittedByLoginId = createCourseBooking_VM.UserLoginId,
                        Mode = 1
                    };
                    var paymentResponse = storedProcedureRepository.SP_InsertUpdatePaymentResponse_Get<SPResponseViewModel>(sP_InsertUpdatePaymentResponse_Params_VM);



                    //  var classBooking = storedProcedureRepository.SP_InsertUpdateClassBooking_Get<SPResponseViewModel>(sP_InsertUpdateClassBooking_Params_VM);

                    if (createCourseBooking_VM.CouponId > 0)
                    {
                        var consumeCoupon = couponService.AddCouponConsumption(createCourseBooking_VM.CouponId, createCourseBooking_VM.UserLoginId);
                    }

                    var courseData = courseService.GetCourseDataByID(createCourseBooking_VM.CourseId);
                    BasicProfileDetail_VM studentBasicProfileDetail_VM = new BasicProfileDetail_VM();
                    SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessOwnerLoginId", "0"),
                            new SqlParameter("userLoginId", createCourseBooking_VM.UserLoginId),
                            new SqlParameter("mode", "1")
                            };

                    var respStudentBasicDetail = db.Database.SqlQuery<BasicProfileDetail_VM>("exec sp_ManageStudentProfile @id,@businessOwnerLoginId,@userLoginId,@mode", queryParams).FirstOrDefault();
                    var studentName = respStudentBasicDetail.FirstName + " " + respStudentBasicDetail.LastName;

                    // Link Student with Business
                    var respBusinessStudentLinking = businessStudentService.AddStudentLinkWithBusinessOwner(courseData.BusinessOwnerLoginId, createCourseBooking_VM.UserLoginId);

                    // Create Course-Booking
                    SP_InsertUpdateCourseBooking_Params_VM sp_InsertUpdateCourseBooking_Params_VM = new SP_InsertUpdateCourseBooking_Params_VM()
                    {
                        OrderId = order.Id,
                        CourseId = createCourseBooking_VM.CourseId,
                        UserLoginId = createCourseBooking_VM.UserLoginId,
                        Mode = 1
                    };

                    var courseBooking = storedProcedureRepository.SP_InsertUpdateCourseBooking_Get<SPResponseViewModel>(sp_InsertUpdateCourseBooking_Params_VM);

                    // Send Notification to Business Owner
                  

                    // send notification to business
                    notificationService.InsertUpdateNotification(new SPInsertUpdateNotification_Params_VM()
                    {
                        NotificationText = String.Format(Resources.NotificationMessage.BusinessNotification_NewCourseBookingMessage, courseData.Name, studentName),
                        NotificationTitle = String.Format(Resources.NotificationMessage.BusinessNotification_NewCourseBookingTitle, courseData.Name),
                        NotificationType = "CourseBooking",
                        NotificationUsersList = courseData.BusinessOwnerLoginId.ToString(),
                        SubmittedByLoginId = 0,
                        ItemId = courseBooking.Id,
                        ItemTable = "CourseBookings",
                        OrderId = order.Id,
                        Mode = 1
                    });

                    // send notification to Student/User
                    notificationService.InsertUpdateNotification(new SPInsertUpdateNotification_Params_VM()
                    {
                        NotificationText = String.Format(Resources.NotificationMessage.UserNotification_NewCourseBookingMessage, courseData.Name, DateTime.UtcNow.AddMinutes(330).ToString("yyyy-MM-dd")),
                        NotificationTitle = String.Format(Resources.NotificationMessage.UserNotification_NewCourseBookingTitle, courseData.Name),
                        NotificationType = "CourseBooking",
                        NotificationUsersList = createCourseBooking_VM.UserLoginId.ToString(),
                        SubmittedByLoginId = 0,
                        ItemId = courseBooking.Id,
                        ItemTable = "CourseBookings",
                        OrderId = order.Id,
                        Mode = 1
                    });

                   
                    //// Add User to Class-Group 
                    //groupService = new GroupService(db);
                    //var addMemberInGroupResponse = groupService.AddMemberInGroup(new AddGroupMember_VM
                    //{

                    //    GroupUserLoginId = courseData.BusinessOwnerLoginId,
                    //    MemberLoginId = createCourseBooking_VM.UserLoginId
                    //});

                    #endregion

                    db.SaveChanges(); // Save changes to the database

                    transaction.Commit(); // Commit the transaction if everything is successful
                    serviceResponse_VM.Status = 1;
                    serviceResponse_VM.Message = "Course Purchased!";
                    //send mails 
                    EmailSender courseBookingMail = new EmailSender();

                    string msg = courseBookingMail.CourseBookedMailMessage(_CourseDetail);

                    courseBookingMail.Send(studentName, "Your Course booking successfully done " + courseData.Name, respStudentBasicDetail.Email, msg, "");
                }
                catch (Exception ex)
                {
                    // Handle exceptions and perform error handling or logging
                    transaction.Rollback(); // Roll back the transaction
                    serviceResponse_VM.Status = -100;
                    serviceResponse_VM.Message = Resources.ErrorMessage.InternalServerErrorMessage;
                }
            } // transaction scope ends

            return serviceResponse_VM;
        }


        /// <summary>
        /// To Get Sports Booking Cheaque Detail By BusinessOwnerLoginId 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        public SportsBookingCheaqueDetail_VM GetSportsBooingCheaqueDetail_Get(long businessOwnerLoginId)
        {
            return storedProcedureRepository.SP_ManageBusinessSportsBookingCheaqueDetail<SportsBookingCheaqueDetail_VM>(new SP_ManageBusinessSportsBookingCheaqueDetail_Param_VM()
            {
                BusinessOwnerLoginId = businessOwnerLoginId,
                Mode = 1
            });
        }


         public ServiceResponse_VM VerifyBusinessPlanCanBeBooked(CreatePlanBooking_VM createPlanBooking_VM)
        {
            // Book Package for Business/User

            ServiceResponse_VM serviceResponse_VM = new ServiceResponse_VM();
            serviceResponse_VM.Status = 1;
            decimal _TotalAmount = 0;
            

            // Get Class detail
            var _PlanData = planService.GetMainPlanDataById(createPlanBooking_VM.PlanId);

            if (_PlanData == null)
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.ErrorMessage.PlanNotFound;
                return serviceResponse_VM;
            }
            else if (planService.IsAlreadyMainPlanPurchased(createPlanBooking_VM.PlanId, createPlanBooking_VM.UserLoginId))
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.ErrorMessage.PlanAlreadyPurchased;
                return serviceResponse_VM;
            }

           

            // Calculate Total Amount after all discounts if any 
            _TotalAmount = _PlanData.Price - _PlanData.Discount;

            // Check if Total Amount is equal to 0 or negative
            if (_TotalAmount <= 0)
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.ErrorMessage.AmountCannotBeZero;
                return serviceResponse_VM;
            }

            return serviceResponse_VM;
        }





        /// <summary>
        /// Main Plan Purchase Booking 
        /// </summary>
        /// <param name="createPlanBooking_VM"></param>
        /// <returns></returns>
        public ServiceResponse_VM CreateBusinessMainPlanBooking(CreatePlanBooking_VM createPlanBooking_VM)
        {
            // Book plan for User/Student

            ServiceResponse_VM serviceResponse_VM = new ServiceResponse_VM();
            decimal _TotalAmount = 0;
           
            int _IsApproved = (createPlanBooking_VM.OnlinePayment == 1) ? 1 : 0;

            // Get Plan detail
            var _PlanDetail = storedProcedureRepository.SP_ManageMainPackage_Get<MainPlan_VM>(new SP_ManageBusinessPlans_Params_VM() { Id = createPlanBooking_VM.PlanId, Mode = 4 });

            var planData = planService.GetMainPlanDataById(createPlanBooking_VM.PlanId);

            if (_PlanDetail == null)
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.ErrorMessage.PlanNotFound;
                return serviceResponse_VM;
            }

            
            // Calculate Total Amount after all discounts if any 
            _TotalAmount = _PlanDetail.Price;

            // Check if Total Amount is equal to the Paid Amount
            if (createPlanBooking_VM.TotalAmountPaid != _TotalAmount)
            {
                serviceResponse_VM.Status = -1;
                serviceResponse_VM.Message = Resources.ErrorMessage.PaidAmountNotEqualsToTotalAmount;
                return serviceResponse_VM;
            }

            // Plan Validity Date Calculations
            DateTime _PlanStartDate = DateTime.UtcNow.Date;
            DateTime _PlanEndDate = _PlanStartDate;

            #region Set End Date for the Plan/Package based on the package duraiton type
          if (_PlanDetail.PlanDurationTypeKey == "per_monthly")
            {
                // yearly
                _PlanEndDate = _PlanStartDate.AddYears(1);
            }
            else if (_PlanDetail.PlanDurationTypeKey == "per_Yearly")
            {
                // Per Class
                _PlanEndDate = _PlanStartDate; //Assuming per class package is for only of that day. 
            }
            #endregion

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Steps:
                    // Order Creation
                    // Payment Response
                    // PlanBooking creation
                    // Consume Coupon if applied
                    // Send Notifications
                    // AddOrUpdate Student link with Business if not linked. (BusinessStudents table)

                    #region Make Plan Booking/Purchase
                    // Create Order
                    SP_InsertUpdateOrder_Params_VM sP_InsertUpdateOrder_Params_VM = new SP_InsertUpdateOrder_Params_VM()
                    {
                        UserLoginId = createPlanBooking_VM.UserLoginId,
                        ItemId = createPlanBooking_VM.PlanId,
                        ItemType = "Plan",
                        OnlinePayment = createPlanBooking_VM.OnlinePayment,
                        PaymentMethod = createPlanBooking_VM.PaymentMethod,
                        CouponId = createPlanBooking_VM.CouponId,
                        CouponDiscountValue = 0,
                        TotalDiscount = 0,
                        IsTaxable = 0,
                        Gst = 0,
                        TotalAmount = createPlanBooking_VM.TotalAmountPaid,
                        Status = 1,
                        Mode = 1,
                        OwnerUserLoginId = createPlanBooking_VM.UserLoginId
                    };
                    var order = storedProcedureRepository.SP_InsertUpdateOrder_Get<SPResponseViewModel>(sP_InsertUpdateOrder_Params_VM);

                    // Create Payment Response
                    SP_InsertUpdatePaymentResponse_Params_VM sP_InsertUpdatePaymentResponse_Params_VM = new SP_InsertUpdatePaymentResponse_Params_VM()
                    {
                        OrderId = order.Id,
                        Method = createPlanBooking_VM.PaymentMethod,
                        Amount = createPlanBooking_VM.TotalAmountPaid,
                        TransactionID = createPlanBooking_VM.TransactionID,
                        Description = createPlanBooking_VM.PaymentDescription,
                        IsApproved = _IsApproved,
                        Provider = createPlanBooking_VM.PaymentProvider,
                        ResponseStatus = createPlanBooking_VM.PaymentResponseStatus,
                        SubmittedByLoginId = createPlanBooking_VM.UserLoginId,
                        Mode = 1
                    };
                    var paymentResponse = storedProcedureRepository.SP_InsertUpdatePaymentResponse_Get<SPResponseViewModel>(sP_InsertUpdatePaymentResponse_Params_VM);

                    // Create Plan-Booking
                    SP_InsertUpdateMainPackageBooking_Param_VM sP_InsertUpdatePlanBooking_Params_VM = new SP_InsertUpdateMainPackageBooking_Param_VM()
                    {
                        OrderId = order.Id,
                        PlanId = createPlanBooking_VM.PlanId,
                        PlanStartDate = _PlanStartDate.ToString("yyyy-MM-dd"),
                        PlanEndDate = _PlanEndDate.ToString("yyyy-MM-dd"),
                        BusinessOwnerLoginId = createPlanBooking_VM.UserLoginId,
                        Mode = 1,
                        Status = 1
                    };

                    var planBooking = storedProcedureRepository.SP_InsertUpdateMainPlanBooking_Get<SPResponseViewModel>(sP_InsertUpdatePlanBooking_Params_VM);

                  

                    // Send Notification to Business Owner
                    BasicProfileDetail_VM studentBasicProfileDetail_VM = new BasicProfileDetail_VM();
                    SqlParameter[] queryParams = new SqlParameter[] {
                            new SqlParameter("id", "0"),
                            new SqlParameter("businessOwnerLoginId",  createPlanBooking_VM.UserLoginId),
                            new SqlParameter("mode", "1")
                            };

                    var respStudentBasicDetail = db.Database.SqlQuery<BasicProfileDetail_VM>("exec sp_ManageBusinessProfile @id,@businessOwnerLoginId,@mode", queryParams).FirstOrDefault();
                    var studentName = respStudentBasicDetail.FirstName + " " + respStudentBasicDetail.LastName;

                    notificationService.InsertUpdateNotification(new SPInsertUpdateNotification_Params_VM()
                    {
                        NotificationText = String.Format(Resources.NotificationMessage.BusinessNotification_NewPlanBookingMessage, planData.Name, studentName, _PlanStartDate.ToString("yyyy-MM-dd")),
                        NotificationTitle = String.Format(Resources.NotificationMessage.BusinessNotification_NewPlanBookingTitle, planData.Name),
                        NotificationType = NotificationTypes.NewBooking.ToString(),
                        NotificationUsersList = "",
                        SubmittedByLoginId = 0,
                        Mode = 1,
                        IsNotificationLinkable = 1,
                        ItemId = planBooking.Id,
                        ItemTable = "PlanBookings"
                    });

                   
                    #endregion

                    db.SaveChanges(); // Save changes to the database

                    transaction.Commit(); // Commit the transaction if everything is successful
                    serviceResponse_VM.Status = 1;
                    serviceResponse_VM.Message = "Plan Purchased!";
                }
                catch (Exception ex)
                {
                    // Handle exceptions and perform error handling or logging
                    transaction.Rollback(); // Roll back the transaction
                    serviceResponse_VM.Status = -100;
                    serviceResponse_VM.Message = Resources.ErrorMessage.InternalServerErrorMessage;
                }
            } // transaction scope ends

            return serviceResponse_VM;
        }



        /// <summary>
        /// To Get Current Main Package Detail By UserLoginId
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <returns></returns>
        public MainPlanBooking_VM GetCurrentMainPackageDetail(long businessOwnerLoginId)
        {
            return storedProcedureRepository.SP_ManageMainPackageBookingGet<MainPlanBooking_VM>(new SP_ManageMainPackageBooking_Param_VM()
            {
                UserLoginId = businessOwnerLoginId,
                Mode = 3
            });
        }


    }
}