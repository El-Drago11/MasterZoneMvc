using MasterZoneMvc.Common;
using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.Common.ValidationHelpers;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using MasterZoneMvc.PageTemplateViewModels;
using MasterZoneMvc.Repository;
using MasterZoneMvc.Services;
using MasterZoneMvc.ViewModels;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace MasterZoneMvc.WebAPIs
{
    public class ContactDetailAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private EmailSender emailSender;
        private BusinessOwnerService businessOwnerService;
        private ContactService contactservice;
        private StoredProcedureRepository storedProcedureRepository;
        private FileHelper fileHelper;

        public ContactDetailAPIController()
        {
            db = new MasterZoneDbContext();
            emailSender = new EmailSender();
            businessOwnerService = new BusinessOwnerService(db);
            contactservice = new ContactService(db);
            fileHelper = new FileHelper();
            storedProcedureRepository = new StoredProcedureRepository(db);
        }

        private ValidateUserLogin_VM ValidateLoggedInUser()
        {
            //--Get User Identity
            var identity = User.Identity as ClaimsIdentity;

            WebApiValidationHelper webApiValidationHelper = new WebApiValidationHelper(db);
            return webApiValidationHelper.ValidateLoggedInLogin(identity);
        }

        /// <summary>
        /// Send Contact-Us Form Message as Email to Masterzone Admin 
        /// </summary>
        /// <param name="contactUs_VM"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ContactDetail/SendContactUsMessage")]
        public HttpResponseMessage SendContactUsMessage(ContactUsMessage_VM contactUs_VM)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                Error_VM error_VM = contactUs_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
                BusinessContactDetail_VM businesstimingDetail = new BusinessContactDetail_VM();
                businesstimingDetail = businessOwnerService.GetBusinessTiming(contactUs_VM.businessOwnerLoginId);

                var msg = "You have received a message from Contact page. <br/> <b>" + contactUs_VM.Name + "</b><br><b>" + contactUs_VM.EmailAddress + "</b><br><b>" + contactUs_VM.PhoneNumber + "</b><br><br/><b>Message:</b><br/><p>" + contactUs_VM.Message + "</p>";

                EmailSender emailSender = new EmailSender();
                emailSender.Send("Admin", "Contact us request from " + contactUs_VM.Name, businesstimingDetail.Email, msg, "");

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new { };
                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);

            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        /// <summary>
        /// Add or Update the Contact details by SuperAdmin 
        /// Displays on Masterzone Contact-Us page
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [Route("api/ContactDetail/AddUpdate")]
        public HttpResponseMessage AddUpdateContactUs()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                long _LoginID_Exact = validateResponse.UserLoginId;

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                ContactDetial_VM contactUsAddress_VM = new ContactDetial_VM();
                contactUsAddress_VM.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                contactUsAddress_VM.Email = HttpRequest.Params["Email"].Trim();
                contactUsAddress_VM.ContactNumber1 = HttpRequest.Params["ContactNumber1"].Trim();
                contactUsAddress_VM.ContactNumber2 = HttpRequest.Params["ContactNumber2"].Trim();
                contactUsAddress_VM.Title = HttpRequest.Params["Title"].Trim();
                contactUsAddress_VM.Description = HttpRequest.Params["Description"].Trim();
                contactUsAddress_VM.ContactTitle = HttpRequest.Params["ContactTitle"].Trim();
                contactUsAddress_VM.ContactDescription = HttpRequest.Params["ContactDescription"].Trim();
                contactUsAddress_VM.Address = HttpRequest.Params["Address"].Trim();
                contactUsAddress_VM.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                // Validate infromation passed
                Error_VM error_VM = contactUsAddress_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
                // Get Attached Files
                HttpFileCollection files = HttpRequest.Files;
                HttpPostedFile _BusinesssImageFile = files["Image"];
                contactUsAddress_VM.Image = _BusinesssImageFile; // for validation
                string _BusinessImageFileNameGenerated = ""; // will contain the generated file name
                string _PreviousBusinessImageFileName = ""; // will be used to delete the file while updating.

                // Check if a new banner image is uploaded
                if (files.Count > 0)
                {
                    if (_BusinesssImageFile != null)
                    {
                        // Generate a new image filename
                        _BusinessImageFileNameGenerated = fileHelper.GenerateFileNameTimeStamp(_BusinesssImageFile);
                    }
                }

                if (contactUsAddress_VM.Mode == 1)
                {
                    var respGetBusinessImageDetail = contactservice.GetContactDetail_Get();

                    if (respGetBusinessImageDetail != null && _BusinesssImageFile == null) // Check if respGetBusinessAboutDetail is not null
                    {
                        // If no image update then update the name of the previous image else need to delete the previous image
                        if (respGetBusinessImageDetail.Image == null)
                        {
                            _PreviousBusinessImageFileName = ""; // Set it to an empty string or handle it as needed
                        }
                        else
                        {
                            _BusinessImageFileNameGenerated = respGetBusinessImageDetail.Image;
                        }
                    }
                    else
                    {
                        // Handle the case where respGetBusinessAboutDetail is null
                        // You can set _PreviousBusinessAboutImageFileName to an empty string or handle it as needed.
                        _PreviousBusinessImageFileName = ""; // Set it to an empty string or handle it as needed
                    }
                }


                SqlParameter[] queryParams = new SqlParameter[] {
                    new SqlParameter("id", contactUsAddress_VM.Id),
                    new SqlParameter("email", contactUsAddress_VM.Email),
                    new SqlParameter("contactNumber", contactUsAddress_VM.ContactNumber1),
                    new SqlParameter("phoneNumber", contactUsAddress_VM.ContactNumber2),
                    new SqlParameter("address", contactUsAddress_VM.Address),
                    new SqlParameter("image" ,_BusinessImageFileNameGenerated),
                    new SqlParameter("title",contactUsAddress_VM.Title),
                    new SqlParameter("description",contactUsAddress_VM.Description),
                    new SqlParameter("contactTitle" ,contactUsAddress_VM.ContactTitle),
                    new SqlParameter("contactDescription",contactUsAddress_VM.ContactDescription),
                    new SqlParameter("submittedByLoginId", _LoginID_Exact),
                    new SqlParameter("mode", "1")
                };

                var resp = db.Database.SqlQuery<SPResponseViewModel>("exec sp_InsertUpdateContactDetails @id,@email,@contactNumber,@phoneNumber,@address,@image,@title,@description,@contactTitle,@contactDescription,@submittedByLoginId,@mode", queryParams).FirstOrDefault();

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                if (resp.ret == 1)
                {
                    // Update Find  Image.
                    #region Insert-Update  Image on Server
                    if (files.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(_PreviousBusinessImageFileName))
                        {
                            // remove the previous image file
                            string RemoveImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_SuperAdminContactImage), _PreviousBusinessImageFileName);
                            fileHelper.DeleteAttachedFileFromServer(RemoveImageFileWithPath);
                        }

                        // save the new image file
                        string NewImageFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(StaticResources.FileUploadPath_SuperAdminContactImage), _BusinessImageFileNameGenerated);
                        fileHelper.SaveUploadedFile(_BusinesssImageFile, NewImageFileWithPath);
                    }
                    #endregion
                }
                apiResponse.status = resp.ret;
                apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                apiResponse.data = new { };
                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        /// <summary>
        /// Get Masterzone-Admin Contact-Details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        [Route("api/ContactDetail/GetContactDetail")]
        public HttpResponseMessage GetContactDetail()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();

                long _LoginID_Exact = validateResponse.UserLoginId;

                ContactDetailViewModel resp = new ContactDetailViewModel();
                resp = contactservice.GetContactDetail_Get();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = resp;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }


        [HttpGet]
        // [Authorize(Roles = "SuperAdmin")]
        [Route("api/ContactDetail/GetContactDetailForVisitorPanel")]
        public HttpResponseMessage GetContactDetailForVisitorPanel()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();

                long _LoginID_Exact = validateResponse.UserLoginId;

                ContactDetailViewModel resp = new ContactDetailViewModel();
                resp = contactservice.GetContactDetail_Get();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = resp;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }



        /// <summary>
        /// To  Send Contact Detail  
        /// </summary>
        /// <param name="contactUs_VM"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ContactDetail/SendContactDetailMessage")]
        public HttpResponseMessage SendContactDetailMessage(ContactUsMessage_VM contactUs_VM)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                Error_VM error_VM = contactUs_VM.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                var msg = "You have received a message from Contact page. <br/> <b>" + contactUs_VM.Name + "</b><br><b>" + contactUs_VM.EmailAddress + "</b><br><b>" + contactUs_VM.PhoneNumber + "</b><br><br/><b>Message:</b><br/><p>" + contactUs_VM.Message + "</p>";

                EmailSender emailSender = new EmailSender();
                emailSender.Send("Admin", "Contact us request from " + contactUs_VM.Name, contactUs_VM.EmailAddress, msg, "");

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = new { };
                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);

            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }


        /// <summary>
        /// To Add/Update Contact Detail 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/ContactDetail/AddUpdateContactNumber")]
        public HttpResponseMessage AddUpdateContactNumber()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
                }

                long _LoginID_Exact = validateResponse.UserLoginId;

                //--Create object of HttpRequest
                var HttpRequest = HttpContext.Current.Request;
                ContactNumberViewModel ContactNumberViewModel = new ContactNumberViewModel();
                ContactNumberViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                ContactNumberViewModel.Location1 = HttpRequest.Params["Location1"].Trim();
                ContactNumberViewModel.ContactNumber1 = HttpRequest.Params["ContactNumber1"].Trim();
                ContactNumberViewModel.Location2 = HttpRequest.Params["Location2"].Trim();
                ContactNumberViewModel.ContactNumber2 = HttpRequest.Params["ContactNumber2"].Trim();
                ContactNumberViewModel.Location3 = HttpRequest.Params["Location3"].Trim();
                ContactNumberViewModel.ContactNumber3 = HttpRequest.Params["ContactNumber3"].Trim();
                ContactNumberViewModel.Location4 = HttpRequest.Params["Location4"].Trim();
                ContactNumberViewModel.ContactNumber4 = HttpRequest.Params["ContactNumber4"].Trim();
                ContactNumberViewModel.Location5 = HttpRequest.Params["Location5"].Trim();
                ContactNumberViewModel.ContactNumber5 = HttpRequest.Params["ContactNumber5"].Trim();
                ContactNumberViewModel.Location6 = HttpRequest.Params["Location6"].Trim();
                ContactNumberViewModel.ContactNumber6 = HttpRequest.Params["ContactNumber6"].Trim();
                ContactNumberViewModel.Location7 = HttpRequest.Params["Location7"].Trim();
                ContactNumberViewModel.ContactNumber7 = HttpRequest.Params["ContactNumber7"].Trim();
                ContactNumberViewModel.Location8 = HttpRequest.Params["Location8"].Trim();
                ContactNumberViewModel.ContactNumber8 = HttpRequest.Params["ContactNumber8"].Trim();
                ContactNumberViewModel.Mode = Convert.ToInt32(HttpRequest.Params["Mode"]);

                // Validate infromation passed
                Error_VM error_VM = ContactNumberViewModel.ValidInformation();

                if (!error_VM.Valid)
                {
                    apiResponse.status = -1;
                    apiResponse.message = error_VM.Message;
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                var resp = storedProcedureRepository.SP_InsertUpdateContactNumberDetail_Get<SPResponseViewModel>(new SP_InsertUpdateContactNumber_Param_VM
                {
                    Id = ContactNumberViewModel.Id,
                    UserLoginId = _LoginID_Exact,
                    Location1 = ContactNumberViewModel.Location1,
                    ContactNumber1 = ContactNumberViewModel.ContactNumber1,
                    Location2 = ContactNumberViewModel.Location2,
                    ContactNumber2 = ContactNumberViewModel.ContactNumber2,
                    Location3 = ContactNumberViewModel.Location3,
                    ContactNumber3 = ContactNumberViewModel.ContactNumber3,
                    Location4 = ContactNumberViewModel.Location4,
                    ContactNumber4 = ContactNumberViewModel.ContactNumber4,
                    Location5 = ContactNumberViewModel.Location5,
                    ContactNumber5 = ContactNumberViewModel.ContactNumber5,
                    Location6 = ContactNumberViewModel.Location6,
                    ContactNumber6 = ContactNumberViewModel.ContactNumber6,
                    Location7 = ContactNumberViewModel.Location7,
                    ContactNumber7 = ContactNumberViewModel.ContactNumber7,
                    Location8 = ContactNumberViewModel.Location8,
                    ContactNumber8 = ContactNumberViewModel.ContactNumber8,
                    SubmittedByLoginId = _LoginID_Exact,
                    Mode = ContactNumberViewModel.Mode
                });

                if (resp.ret <= 0)
                {
                    apiResponse.status = resp.ret;
                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }


                apiResponse.status = resp.ret;
                apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                apiResponse.data = new { };
                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        /// <summary>
        /// To Get Contact Number Detail 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/ContactDetail/GetContactNumberDetail")]
        public HttpResponseMessage GetContactNumberDetail()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();

                long _LoginID_Exact = validateResponse.UserLoginId;

                ContactNumberDetail_VM resp = new ContactNumberDetail_VM();
                resp = contactservice.GetContactNumberDetail_Get();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = resp;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        /// <summary>
        /// To Get Contact Number Detail For Visitor Panel (Home Page )
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ContactDetail/GetContactNumberDetailForVisitorPanel")]
        public HttpResponseMessage GetContactNumberDetailForVisitorPanel()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();

                long _LoginID_Exact = validateResponse.UserLoginId;

                ContactNumberDetail_VM resp = new ContactNumberDetail_VM();
                resp = contactservice.GetContactNumberDetail_Get();

                apiResponse.status = 1;
                apiResponse.message = "success";
                apiResponse.data = resp;

                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }

    }
}