using MasterZoneMvc.DAL;
using MasterZoneMvc.Repository;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Services
{
    public class OrderService
    {
        private MasterZoneDbContext db;
        private StoredProcedureRepository storedProcedureRepository;
        
        public OrderService(MasterZoneDbContext dbContext)
        {
            db = dbContext;
            storedProcedureRepository = new StoredProcedureRepository(db);
        }

        /// <summary>
        /// Get Order Table-Data By Id
        /// </summary>
        /// <param name="id">Order Id</param>
        /// <returns>Order Table Record</returns>
        public OrderViewModel GetOrderDataById(long id)
        {
            return storedProcedureRepository.SP_ManageOrder_Get<OrderViewModel>(new ViewModels.StoredProcedureParams.SP_ManageOrder_Params_VM()
            {
                Id = id,
                Mode = 1
            });
        }

        /// <summary>
        /// Get Payment-Response Table-Data By Order-Id
        /// </summary>
        /// <param name="orderId">Order Id</param>
        /// <returns>Payment-Response Table Record</returns>
        public PaymentResponse_VM GetPaymentResponseData(long orderId)
        {
            return storedProcedureRepository.SP_ManagePaymentResponse_Get<PaymentResponse_VM>(new ViewModels.StoredProcedureParams.SP_ManagePaymentResponse_Params_VM()
            {
                OrderId = orderId,
                Mode = 1
            });
        }

    }
}