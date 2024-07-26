using MasterZoneMvc.Common;
using MasterZoneMvc.Common.Helpers;
using MasterZoneMvc.Common.ValidationHelpers;
using MasterZoneMvc.DAL;
using MasterZoneMvc.Models;
using MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel;
using MasterZoneMvc.PageTemplateViewModels;
using MasterZoneMvc.Repository;
using MasterZoneMvc.Services;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using MasterZoneMvc.ViewModels.StoredProcedureParams;
using Newtonsoft.Json;
using System.IO;
using Org.BouncyCastle.Ocsp;

namespace MasterZoneMvc.WebAPIs
{
    public class CustomFormAPIController : ApiController
    {
        private MasterZoneDbContext db;
        private FileHelper fileHelper;
        private StoredProcedureRepository storedProcedureRepository;
        private CustomFormService customFormService;
        public CustomFormAPIController()
        {
            db = new MasterZoneDbContext();
            fileHelper = new FileHelper();
            storedProcedureRepository = new StoredProcedureRepository(db);
            customFormService = new CustomFormService(db);
        }

        /// <summary>
        /// Validate Logged-in user. 
        /// </summary>
        /// <returns></returns>
        private ValidateUserLogin_VM ValidateLoggedInUser()
        {
            //--Get User Identity
            var identity = User.Identity as ClaimsIdentity;

            WebApiValidationHelper webApiValidationHelper = new WebApiValidationHelper(db);
            return webApiValidationHelper.ValidateLoggedInLogin(identity);
        }

        /// <summary>
        /// To Add Custom Form Detail 
        /// </summary>
        /// <param name="formData"></param>
        /// <returns></returns>

        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin")]
        [Route("api/CustomForm/AddUpdateCustomFormDetail")]
        public HttpResponseMessage AddUpdateCustomFormDetail(List<FormElement> formData)
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
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                // Create a new instance of CustomFormViewModel
                CustomFormViewModel customFormViewModel = new CustomFormViewModel();

                // Flag to track whether Form Name has been processed or not
                bool formNameProcessed = false;
                long formNameId = 0;
                SPResponseViewModel resp = null;
                // Iterate through each form element
                foreach (var formElement in formData)
                {
                    // Set customFormViewModel.CustomFormName using the value from the first iteration
                    if (formElement.Label == "Form Name" && !formNameProcessed)
                    {
                        customFormViewModel.CustomFormName = formElement.Value;
                        formNameProcessed = true;



                        //// Iterate through each form element
                        //foreach (var formElement in formData)
                        //{
                        //    // Set customFormViewModel.CustomFormName using the value from the first iteration
                        //    if (formElement.Label == "Form Name")
                        //    {
                        //        customFormViewModel.CustomFormName = formElement.Value;
                        //    }
                        //}
                        var HttpRequest = HttpContext.Current.Request;
                        // Set other properties of customFormViewModel
                        customFormViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                        customFormViewModel.Mode = 1;

                        // Validate information passed
                        Error_VM error_VM = customFormViewModel.ValidInformation();

                        if (!error_VM.Valid)
                        {
                            apiResponse.status = -1;
                            apiResponse.message = error_VM.Message;
                            return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                        }

                        // Call stored procedure to insert/update custom form detail
                        resp = storedProcedureRepository.SP_InsertUpdateCustomFormDetail_Get<SPResponseViewModel>(
                           new SP_InsertUpdateCustomForm_Param_VM
                           {
                               Id = customFormViewModel.Id,
                               BusinessOwnerLoginId = _BusinessOwnerLoginId,
                               CustomFormName = customFormViewModel.CustomFormName,
                               Mode = customFormViewModel.Mode,
                               SubmittedByLoginId = _BusinessOwnerLoginId
                           });

                        if (resp.ret <= 0)
                        {
                            apiResponse.status = resp.ret;
                            apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                            return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                        }

                        // Set the flag to true indicating Form Name has been processed
                        formNameProcessed = true;

                        // Save the Id obtained after saving the Form Name
                        formNameId = resp.Id;

                    }

                    //  long customFormElementId = 0;
                    else
                    {


                        // Iterate through each form element and save in CustomFormElementViewModel

                        CustomFormElementViewModel customFormElementViewModel = new CustomFormElementViewModel();

                        customFormElementViewModel.Id = formNameId;
                        customFormElementViewModel.Mode = 2;
                        if (formElement.Type == "select")
                        {
                            customFormElementViewModel.CustomFormElementName = formElement.Label;
                        }
                        else
                        {
                            customFormElementViewModel.CustomFormElementName = formElement.Label;
                        }

                        if (formElement.Type == "radio" || formElement.Type == "checkbox" || formElement.Type == "select")
                        {
                            customFormElementViewModel.CustomFormElementValue =
                                formElement.selectedOptions != null && formElement.selectedOptions.Count > 0 ?
                                formElement.selectedOptions[0].Value : "";
                        }
                        else
                        {
                            customFormElementViewModel.CustomFormElementValue = formElement.Value ?? "";
                        }



                        // Set properties based on formElement
                        customFormElementViewModel.CustomFormId = formNameId;
                        customFormElementViewModel.CustomFormElementType = formElement.Type ?? "";
                        customFormElementViewModel.CustomFormElementPlaceholder = formElement.Placeholder ?? "";

                        // Call stored procedure to insert/update custom form element detail
                        var resp1 = storedProcedureRepository.SP_InsertUpdateCustomFormDetail_Get<SPResponseViewModel>(
                            new SP_InsertUpdateCustomForm_Param_VM
                            {
                                Id = customFormElementViewModel.Id,
                                CustomFormId = customFormElementViewModel.CustomFormId,
                                CustomFormElementName = customFormElementViewModel.CustomFormElementName,
                                CustomFormElementType = customFormElementViewModel.CustomFormElementType,
                                CustomFormElementValue = customFormElementViewModel.CustomFormElementValue,
                                CustomFormElementPlaceholder = customFormElementViewModel.CustomFormElementPlaceholder,
                                Mode = customFormElementViewModel.Mode,
                                SubmittedByLoginId = _BusinessOwnerLoginId
                            });

                        // Check the result of the stored procedure for custom form element
                        if (resp1.ret <= 0)
                        {
                            apiResponse.status = resp1.ret;
                            apiResponse.message = ResourcesHelper.GetResourceValue(resp1.resourceFileName, resp1.resourceKey);
                            return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                        }




                        // Check if the form element has options and needs to save options (Mode = 3)
                        if (resp1.ret == 1 && (formElement.Type == "radio" || formElement.Type == "checkbox" || formElement.Type == "select") && formElement.options != null && formElement.options.Any(option => !string.IsNullOrEmpty(option.Label)))
                        {
                            CustomFormOptionsViewModel customFormOptionsViewModel = new CustomFormOptionsViewModel();
                            customFormOptionsViewModel.Id = resp1.Id;
                             customFormOptionsViewModel.Mode = 3;

                            // Extract option labels and join them into a single string
                            var optionLabels = formElement.options
                                .Where(option => !string.IsNullOrEmpty(option.Label))
                                .Select(option => option.Label);

                            customFormOptionsViewModel.CustomFormElementOptions = string.Join(",", optionLabels);


                            customFormOptionsViewModel.CustomFormElementId = resp1.Id;

                            // Call stored procedure to insert/update custom form element options
                            var resp2 = storedProcedureRepository.SP_InsertUpdateCustomFormDetail_Get<SPResponseViewModel>(
                                new SP_InsertUpdateCustomForm_Param_VM
                                {
                                    Id = customFormOptionsViewModel.Id,
                                    CustomFormElementId = customFormOptionsViewModel.CustomFormElementId,
                                    CustomFormElementOptions = JsonConvert.SerializeObject(customFormOptionsViewModel.CustomFormElementOptions),
                                    Mode = customFormOptionsViewModel.Mode,
                                    SubmittedByLoginId = _BusinessOwnerLoginId
                                });

                            // Check the result of the stored procedure for custom form element options
                            if (resp2.ret <= 0)
                            {
                                apiResponse.status = resp2.ret;
                                apiResponse.message = ResourcesHelper.GetResourceValue(resp2.resourceFileName, resp2.resourceKey);
                                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                            }
                        }

                       
                    }


                }

                apiResponse.status = 1;
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
        /// To Get Custom Form List  Detail By BusinessOwerLoginId 
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin")]
        [Route("api/CustomForm/GetCustomFormDetailPagination")]
        public HttpResponseMessage GetCustomFormDetailPagination()
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
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                List<CustomFormDetail_VM> resp = customFormService.GetAllCustomFormDetail(_BusinessOwnerLoginId);


                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
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
        /// To Delete the Custom Form Detail By Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin")]
        [Route("api/CustomForm/DeleteCustomFormDetailById")]
        public HttpResponseMessage DeleteCustomFormDetailById(long id)
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
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;



                // Delete By Id
                var resp = customFormService.DeleteCustomFormDetail(id);


                apiResponse.status = 1;
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
        /// To Get Custom Form Detail By Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin,Student")]
        [Route("api/CustomForm/GetCustomFormDetailById")]
        public HttpResponseMessage GetCustomFormDetailById(long id)
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
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                CustomFormDetail_VM customFormName = new CustomFormDetail_VM();

                customFormName = customFormService.GetAllCustomFormNameDetailById(id);

                List<CustomFormDetail_ViewModel> customFormDetailList = customFormService.GetAllCustomFormDetailById(id);

                foreach (var data in customFormDetailList)
                {


                    string _eventOptions = (data == null) ? "" : data.CustomFormElementOptions;
                    string[] stringArray = _eventOptions.Split(',');

                    List<CustomFormOptionsValueDetail> customFormOptionsValueDetail = new List<CustomFormOptionsValueDetail>();
                    foreach (string item in stringArray)
                    {
                        // Create an instance of CustomFormOptionsValueDetail and set its properties
                        CustomFormOptionsValueDetail option = new CustomFormOptionsValueDetail
                        {
                            CustomFormElementOptions = item // Assuming 'Name' is the property in CustomFormOptionsValueDetail
                        };

                        customFormOptionsValueDetail.Add(option);
                    }
                    data.CustomFormElementOptions = string.Join(",", customFormOptionsValueDetail.Select(item => item.CustomFormElementOptions));

                }


                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
                apiResponse.data = new
                {
                    CustomFormDetailList = customFormDetailList,
                    CustomFormName = customFormName
                };




                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.status = -500;
                apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
            }
        }


        //[HttpPost]
        //[Authorize(Roles = "BusinessAdmin,Staff")]
        //[Route("api/CustomForm/AddUpdateCustomFormDetail")]
        //public HttpResponseMessage AddUpdateCustomFormDetail(List<FormElement> formData, int mode)
        //{

        //    ApiResponse_VM apiResponse = new ApiResponse_VM();

        //    try
        //    {
        //        var validateResponse = ValidateLoggedInUser();

        //        if (validateResponse.ApiResponse_VM.status < 0)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse.ApiResponse_VM);
        //        }

        //        long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

        //        CustomFormViewModel customFormViewModel = new CustomFormViewModel();

        //        bool formNameProcessed = false;

        //        foreach (var formElement in formData)
        //        {
        //            if (formElement.Label == "Form Name" && !formNameProcessed)
        //            {
        //                customFormViewModel.CustomFormName = formElement.Value;
        //                formNameProcessed = true;

        //                var HttpRequest = HttpContext.Current.Request;
        //                customFormViewModel.Id = Convert.ToInt32(HttpRequest.Params["Id"]); // Use 'mode' parameter directly
        //                customFormViewModel.Mode = mode;

        //                Error_VM error_VM = customFormViewModel.ValidInformation();

        //                if (!error_VM.Valid)
        //                {
        //                    apiResponse.status = -1;
        //                    apiResponse.message = error_VM.Message;
        //                    return Request.CreateResponse(HttpStatusCode.BadRequest, apiResponse);
        //                }
        //            }

        //            CustomFormElementViewModel customFormElementViewModel = new CustomFormElementViewModel();

        //            Set common properties for all form elements

        //           customFormElementViewModel.CustomFormElementName = formElement.Label;
        //            customFormElementViewModel.CustomFormElementValue = formElement.Type == "select"
        //                ? formElement.selectedOptions?.FirstOrDefault()?.Value ?? ""
        //                : formElement.Value ?? "";
        //            customFormElementViewModel.CustomFormId = mode; // Use 'mode' parameter directly
        //            customFormElementViewModel.CustomFormElementType = formElement.Type ?? "";
        //            customFormElementViewModel.CustomFormElementPlaceholder = formElement.Placeholder ?? "";

        //            if (formElement.Type != null && (formElement.Type == "radio" || formElement.Type == "checkbox" || formElement.Type == "select"))
        //            {
        //                CustomFormOptionsViewModel customFormOptionsViewModel = new CustomFormOptionsViewModel();

        //                var optionLabels = formElement.options?.Where(option => !string.IsNullOrEmpty(option.Label)).Select(option => option.Label);

        //                customFormOptionsViewModel.CustomFormElementOptions = string.Join(",", optionLabels);

        //                var resp = storedProcedureRepository.SP_InsertUpdateCustomFormDetail_Get<SPResponseViewModel>(
        //                    new SP_InsertUpdateCustomForm_Param_VM
        //                    {
        //                        Id = customFormElementViewModel.Id,
        //                        BusinessOwnerLoginId = _BusinessOwnerLoginId,
        //                        CustomFormName = customFormViewModel.CustomFormName,
        //                        CustomFormElementName = customFormElementViewModel.CustomFormElementName,
        //                        CustomFormElementType = customFormElementViewModel.CustomFormElementType,
        //                        CustomFormElementValue = customFormElementViewModel.CustomFormElementValue,
        //                        CustomFormElementPlaceholder = customFormElementViewModel.CustomFormElementPlaceholder,
        //                        CustomFormElementOptions = JsonConvert.SerializeObject(customFormOptionsViewModel.CustomFormElementOptions),
        //                        Mode = customFormElementViewModel.Mode,
        //                        SubmittedByLoginId = _BusinessOwnerLoginId
        //                    });

        //                if (resp.ret <= 0)
        //                {
        //                    apiResponse.status = resp.ret;
        //                    apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
        //                    return Request.CreateResponse(HttpStatusCode.BadRequest, apiResponse);
        //                }
        //            }
        //        }

        //        apiResponse.status = 1;
        //        apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
        //        apiResponse.data = new { };
        //        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log the exception
        //        apiResponse.status = -500;
        //        apiResponse.message = Resources.ErrorMessage.InternalServerErrorMessage;
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, apiResponse);
        //    }
        //}

        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin")]
        [Route("api/CustomForm/UpdateCustomFormDetail")]
        public HttpResponseMessage UpdateCustomFormDetail(List<FormElement> formData, long Id)
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
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                // Create a new instance of CustomFormViewModel
                CustomFormViewModel customFormViewModel = new CustomFormViewModel();

                // Flag to track whether Form Name has been processed or not
                bool formNameProcessed = false;
                long formNameId = 0;
                SPResponseViewModel resp = null;
                if(Id > 0)
                {


                    // Inside the loop where you process form elements
                    foreach (var formElement in formData)
                    {
                        // Set customFormViewModel.CustomFormName using the value from the first iteration
                        if (formElement.Label == "Form Name" && !formNameProcessed)
                        {
                            customFormViewModel.CustomFormName = formElement.Value;
                            formNameProcessed = true;



                            //// Iterate through each form element
                            //foreach (var formElement in formData)
                            //{
                            //    // Set customFormViewModel.CustomFormName using the value from the first iteration
                            //    if (formElement.Label == "Form Name")
                            //    {
                            //        customFormViewModel.CustomFormName = formElement.Value;
                            //    }
                            //}
                            var HttpRequest = HttpContext.Current.Request;
                            // Set other properties of customFormViewModel
                            customFormViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                            customFormViewModel.Mode = 5;

                            // Validate information passed
                            Error_VM error_VM = customFormViewModel.ValidInformation();

                            if (!error_VM.Valid)
                            {
                                apiResponse.status = -1;
                                apiResponse.message = error_VM.Message;
                                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                            }

                            // Call stored procedure to insert/update custom form detail
                            resp = storedProcedureRepository.SP_InsertUpdateCustomFormDetail_Get<SPResponseViewModel>(
                               new SP_InsertUpdateCustomForm_Param_VM
                               {
                                   Id = customFormViewModel.Id,
                                   BusinessOwnerLoginId = _BusinessOwnerLoginId,
                                   CustomFormName = customFormViewModel.CustomFormName,
                                   Mode = customFormViewModel.Mode,
                                   SubmittedByLoginId = _BusinessOwnerLoginId
                               });

                            if (resp.ret <= 0)
                            {
                                apiResponse.status = resp.ret;
                                apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                            }

                            // Set the flag to true indicating Form Name has been processed
                            formNameProcessed = true;

                            // Save the Id obtained after saving the Form Name
                            formNameId = Id;

                        }

                        //  long customFormElementId = 0;
                        else
                        {


                            // Iterate through each form element and save in CustomFormElementViewModel

                            CustomFormElementViewModel customFormElementViewModel = new CustomFormElementViewModel();

                            customFormElementViewModel.Id = formNameId;
                            customFormElementViewModel.Mode = 6;
                            if (formElement.Type == "select")
                            {
                                customFormElementViewModel.CustomFormElementName = formElement.Label;
                            }
                            else
                            {
                                customFormElementViewModel.CustomFormElementName = formElement.Label;
                            }

                            if (formElement.Type == "radio" || formElement.Type == "checkbox" || formElement.Type == "select")
                            {
                                customFormElementViewModel.CustomFormElementValue =
                                    formElement.selectedOptions != null && formElement.selectedOptions.Count > 0 ?
                                    formElement.selectedOptions[0].Value : "";
                            }
                            else
                            {
                                customFormElementViewModel.CustomFormElementValue = formElement.Value ?? "";
                            }



                            // Set properties based on formElement
                            customFormElementViewModel.CustomFormId = formNameId;
                            customFormElementViewModel.CustomFormElementType = formElement.Type ?? "";
                            customFormElementViewModel.CustomFormElementPlaceholder = formElement.Placeholder ?? "";

                            // Call stored procedure to insert/update custom form element detail
                            var resp1 = storedProcedureRepository.SP_InsertUpdateCustomFormDetail_Get<SPResponseViewModel>(
                                new SP_InsertUpdateCustomForm_Param_VM
                                {
                                    Id = customFormElementViewModel.Id,
                                    CustomFormId = customFormElementViewModel.CustomFormId,
                                    CustomFormElementName = customFormElementViewModel.CustomFormElementName,
                                    CustomFormElementType = customFormElementViewModel.CustomFormElementType,
                                    CustomFormElementValue = customFormElementViewModel.CustomFormElementValue,
                                    CustomFormElementPlaceholder = customFormElementViewModel.CustomFormElementPlaceholder,
                                    Mode = customFormElementViewModel.Mode,
                                    SubmittedByLoginId = _BusinessOwnerLoginId
                                });

                            // Check the result of the stored procedure for custom form element
                            if (resp1.ret <= 0)
                            {
                                apiResponse.status = resp1.ret;
                                apiResponse.message = ResourcesHelper.GetResourceValue(resp1.resourceFileName, resp1.resourceKey);
                                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                            }



                            // Check if the form element has options and needs to save options (Mode = 7)
                            if (resp1.ret == 1 && (formElement.Type == "radio" || formElement.Type == "checkbox" || formElement.Type == "select") && formElement.options != null &&
                               formElement.options.Any(option => !string.IsNullOrEmpty(option.Label) || !string.IsNullOrEmpty(option.Value)))
                            {
                                CustomFormOptionsViewModel customFormOptionsViewModel = new CustomFormOptionsViewModel();
                                customFormOptionsViewModel.Id = resp1.Id;
                                customFormOptionsViewModel.Mode = 7;
                              
                                // Extract option labels and join them into a single string
                                if (formElement.Type == "select")
                                {
                                   var  optionLabels = formElement.options
                                    .Where(option => !string.IsNullOrEmpty(option.Label))
                                    .Select(option => option.Label);
                                    customFormOptionsViewModel.CustomFormElementOptions = string.Join(",", optionLabels);
                                }
                                else
                                {
                                   var  optionValue = formElement.options
                                    .Where(option => !string.IsNullOrEmpty(option.Value))
                                     .Select(option => option.Value);
                                    customFormOptionsViewModel.CustomFormElementOptions = string.Join(",", optionValue);
                                }

                                

                                //customFormOptionsViewModel.CustomFormElementOptions = string.Join(",", optionLabels);
                                

                               // customFormOptionsViewModel.CustomFormElementOptions = string.Join(",", optionLabels.Select((label, index) => $"{label}:{optionValues.ElementAtOrDefault(index)}"));


                                customFormOptionsViewModel.CustomFormElementId = resp1.Id;

                                // Call stored procedure to insert/update custom form element options
                                var resp2 = storedProcedureRepository.SP_InsertUpdateCustomFormDetail_Get<SPResponseViewModel>(
                                    new SP_InsertUpdateCustomForm_Param_VM
                                    {
                                        Id = customFormOptionsViewModel.Id,
                                        CustomFormElementId = customFormOptionsViewModel.CustomFormElementId,
                                        CustomFormElementOptions = customFormOptionsViewModel.CustomFormElementOptions.Trim('/'),
                                        Mode = customFormOptionsViewModel.Mode,
                                        SubmittedByLoginId = _BusinessOwnerLoginId
                                    });

                                // Check the result of the stored procedure for custom form element options
                                if (resp2.ret <= 0)
                                {
                                    apiResponse.status = resp2.ret;
                                    apiResponse.message = ResourcesHelper.GetResourceValue(resp2.resourceFileName, resp2.resourceKey);
                                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                                }
                            }


                            //if (resp1.ret == 1 && (formElement.Type == "radio" || formElement.Type == "checkbox" || formElement.Type == "select") && formElement.options != null && formElement.options.Any(option => !string.IsNullOrEmpty(option.Label)))
                            //{
                            //    CustomFormOptionsViewModel customFormOptionsViewModel = new CustomFormOptionsViewModel();
                            //    customFormOptionsViewModel.Id = resp1.Id;
                            //     customFormOptionsViewModel.Mode = 7;

                            //    // Extract option labels and join them into a single string
                            //    var optionLabels = formElement.options
                            //        .Where(option => !string.IsNullOrEmpty(option.Label))
                            //        .Select(option => option.Label);

                            //    customFormOptionsViewModel.CustomFormElementOptions = string.Join(",", optionLabels);


                            //    customFormOptionsViewModel.CustomFormElementId = resp1.Id;

                            //    // Call stored procedure to insert/update custom form element options
                            //    var resp2 = storedProcedureRepository.SP_InsertUpdateCustomFormDetail_Get<SPResponseViewModel>(
                            //        new SP_InsertUpdateCustomForm_Param_VM
                            //        {
                            //            Id = customFormOptionsViewModel.Id,
                            //            CustomFormElementId = customFormOptionsViewModel.CustomFormElementId,
                            //            CustomFormElementOptions = customFormOptionsViewModel.CustomFormElementOptions.Trim('/'),
                            //            //CustomFormElementOptions = JsonConvert.SerializeObject(customFormOptionsViewModel.CustomFormElementOptions),
                            //            Mode = customFormOptionsViewModel.Mode,
                            //            SubmittedByLoginId = _BusinessOwnerLoginId
                            //        });

                            //    // Check the result of the stored procedure for custom form element options
                            //    if (resp2.ret <= 0)
                            //    {
                            //        apiResponse.status = resp2.ret;
                            //        apiResponse.message = ResourcesHelper.GetResourceValue(resp2.resourceFileName, resp2.resourceKey);
                            //        return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                            //    }
                            //}


                        }


                    }

                }




                apiResponse.status = 1;
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

        //// To Add the custom form for Super Admin
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [Route("api/CustomForm/AddUpdateSuperAdminCustomFormDetail")]
        public HttpResponseMessage AddUpdateSuperAdminCustomFormDetail(List<FormElement> formData)
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
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                // Create a new instance of CustomFormViewModel
                CustomFormViewModel customFormViewModel = new CustomFormViewModel();

                // Flag to track whether Form Name has been processed or not
                bool formNameProcessed = false;
                long formNameId = 0;
                SPResponseViewModel resp = null;
                // Iterate through each form element
                foreach (var formElement in formData)
                {
                    // Set customFormViewModel.CustomFormName using the value from the first iteration
                    if (formElement.Label == "Form Name" && !formNameProcessed)
                    {
                        customFormViewModel.CustomFormName = formElement.Value;
                        formNameProcessed = true;



                        //// Iterate through each form element
                        //foreach (var formElement in formData)
                        //{
                        //    // Set customFormViewModel.CustomFormName using the value from the first iteration
                        //    if (formElement.Label == "Form Name")
                        //    {
                        //        customFormViewModel.CustomFormName = formElement.Value;
                        //    }
                        //}
                        var HttpRequest = HttpContext.Current.Request;
                        // Set other properties of customFormViewModel
                        customFormViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                        customFormViewModel.Mode = 1;

                        // Validate information passed
                        Error_VM error_VM = customFormViewModel.ValidInformation();

                        if (!error_VM.Valid)
                        {
                            apiResponse.status = -1;
                            apiResponse.message = error_VM.Message;
                            return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                        }

                        // Call stored procedure to insert/update custom form detail
                        resp = storedProcedureRepository.SP_InsertUpdateCustomFormDetail_Get<SPResponseViewModel>(
                           new SP_InsertUpdateCustomForm_Param_VM
                           {
                               Id = customFormViewModel.Id,
                               BusinessOwnerLoginId = _LoginID_Exact,
                               CustomFormName = customFormViewModel.CustomFormName,
                               Mode = customFormViewModel.Mode,
                               SubmittedByLoginId = _LoginID_Exact
                           });

                        if (resp.ret <= 0)
                        {
                            apiResponse.status = resp.ret;
                            apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                            return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                        }

                        // Set the flag to true indicating Form Name has been processed
                        formNameProcessed = true;

                        // Save the Id obtained after saving the Form Name
                        formNameId = resp.Id;

                    }

                    //  long customFormElementId = 0;
                    else
                    {


                        // Iterate through each form element and save in CustomFormElementViewModel

                        CustomFormElementViewModel customFormElementViewModel = new CustomFormElementViewModel();

                        customFormElementViewModel.Id = formNameId;
                        customFormElementViewModel.Mode = 2;
                        if (formElement.Type == "select")
                        {
                            customFormElementViewModel.CustomFormElementName = formElement.Label;
                        }
                        else
                        {
                            customFormElementViewModel.CustomFormElementName = formElement.Label;
                        }

                        if (formElement.Type == "radio" || formElement.Type == "checkbox" || formElement.Type == "select")
                        {
                            customFormElementViewModel.CustomFormElementValue =
                                formElement.selectedOptions != null && formElement.selectedOptions.Count > 0 ?
                                formElement.selectedOptions[0].Value : "";
                        }
                        else
                        {
                            customFormElementViewModel.CustomFormElementValue = formElement.Value ?? "";
                        }



                        // Set properties based on formElement
                        customFormElementViewModel.CustomFormId = formNameId;
                        customFormElementViewModel.CustomFormElementType = formElement.Type ?? "";
                        customFormElementViewModel.CustomFormElementPlaceholder = formElement.Placeholder ?? "";

                        // Call stored procedure to insert/update custom form element detail
                        var resp1 = storedProcedureRepository.SP_InsertUpdateCustomFormDetail_Get<SPResponseViewModel>(
                            new SP_InsertUpdateCustomForm_Param_VM
                            {
                                Id = customFormElementViewModel.Id,
                                CustomFormId = customFormElementViewModel.CustomFormId,
                                CustomFormElementName = customFormElementViewModel.CustomFormElementName,
                                CustomFormElementType = customFormElementViewModel.CustomFormElementType,
                                CustomFormElementValue = customFormElementViewModel.CustomFormElementValue,
                                CustomFormElementPlaceholder = customFormElementViewModel.CustomFormElementPlaceholder,
                                Mode = customFormElementViewModel.Mode,
                                SubmittedByLoginId = _BusinessOwnerLoginId
                            });

                        // Check the result of the stored procedure for custom form element
                        if (resp1.ret <= 0)
                        {
                            apiResponse.status = resp1.ret;
                            apiResponse.message = ResourcesHelper.GetResourceValue(resp1.resourceFileName, resp1.resourceKey);
                            return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                        }




                        // Check if the form element has options and needs to save options (Mode = 3)
                        if (resp1.ret == 1 && (formElement.Type == "radio" || formElement.Type == "checkbox" || formElement.Type == "select") && formElement.options != null && formElement.options.Any(option => !string.IsNullOrEmpty(option.Label)))
                        {
                            CustomFormOptionsViewModel customFormOptionsViewModel = new CustomFormOptionsViewModel();
                            customFormOptionsViewModel.Id = resp1.Id;
                            customFormOptionsViewModel.Mode = 3;

                            // Extract option labels and join them into a single string
                            var optionLabels = formElement.options
                                .Where(option => !string.IsNullOrEmpty(option.Label))
                                .Select(option => option.Label);

                            customFormOptionsViewModel.CustomFormElementOptions = string.Join(",", optionLabels);


                            customFormOptionsViewModel.CustomFormElementId = resp1.Id;

                            // Call stored procedure to insert/update custom form element options
                            var resp2 = storedProcedureRepository.SP_InsertUpdateCustomFormDetail_Get<SPResponseViewModel>(
                                new SP_InsertUpdateCustomForm_Param_VM
                                {
                                    Id = customFormOptionsViewModel.Id,
                                    CustomFormElementId = customFormOptionsViewModel.CustomFormElementId,
                                    CustomFormElementOptions = JsonConvert.SerializeObject(customFormOptionsViewModel.CustomFormElementOptions),
                                    Mode = customFormOptionsViewModel.Mode,
                                    SubmittedByLoginId = _BusinessOwnerLoginId
                                });

                            // Check the result of the stored procedure for custom form element options
                            if (resp2.ret <= 0)
                            {
                                apiResponse.status = resp2.ret;
                                apiResponse.message = ResourcesHelper.GetResourceValue(resp2.resourceFileName, resp2.resourceKey);
                                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                            }
                        }


                    }


                }

                apiResponse.status = 1;
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
        /// To Update the Custom From For Super Admin
        /// </summary>
        /// <param name="formData"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin")]
        [Route("api/CustomForm/UpdateSuperAdminCustomFormDetail")]
        public HttpResponseMessage UpdateSuperAdminCustomFormDetail(List<FormElement> formData, long Id)
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
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                // Create a new instance of CustomFormViewModel
                CustomFormViewModel customFormViewModel = new CustomFormViewModel();

                // Flag to track whether Form Name has been processed or not
                bool formNameProcessed = false;
                long formNameId = 0;
                SPResponseViewModel resp = null;
                if (Id > 0)
                {


                    // Inside the loop where you process form elements
                    foreach (var formElement in formData)
                    {
                        // Set customFormViewModel.CustomFormName using the value from the first iteration
                        if (formElement.Label == "Form Name" && !formNameProcessed)
                        {
                            customFormViewModel.CustomFormName = formElement.Value;
                            formNameProcessed = true;


                            var HttpRequest = HttpContext.Current.Request;
                            // Set other properties of customFormViewModel
                            customFormViewModel.Id = Convert.ToInt64(HttpRequest.Params["Id"]);
                            customFormViewModel.Mode = 5;

                            // Validate information passed
                            Error_VM error_VM = customFormViewModel.ValidInformation();

                            if (!error_VM.Valid)
                            {
                                apiResponse.status = -1;
                                apiResponse.message = error_VM.Message;
                                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                            }

                            // Call stored procedure to insert/update custom form detail
                            resp = storedProcedureRepository.SP_InsertUpdateCustomFormDetail_Get<SPResponseViewModel>(
                               new SP_InsertUpdateCustomForm_Param_VM
                               {
                                   Id = customFormViewModel.Id,
                                   BusinessOwnerLoginId = _LoginID_Exact,
                                   CustomFormName = customFormViewModel.CustomFormName,
                                   Mode = customFormViewModel.Mode,
                                   SubmittedByLoginId = _LoginID_Exact
                               });

                            if (resp.ret <= 0)
                            {
                                apiResponse.status = resp.ret;
                                apiResponse.message = ResourcesHelper.GetResourceValue(resp.resourceFileName, resp.resourceKey);
                                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                            }

                            // Set the flag to true indicating Form Name has been processed
                            formNameProcessed = true;

                            // Save the Id obtained after saving the Form Name
                            formNameId = Id;

                        }

                        //  long customFormElementId = 0;
                        else
                        {


                            // Iterate through each form element and save in CustomFormElementViewModel

                            CustomFormElementViewModel customFormElementViewModel = new CustomFormElementViewModel();

                            customFormElementViewModel.Id = formNameId;
                            customFormElementViewModel.Mode = 6;
                            if (formElement.Type == "select")
                            {
                                customFormElementViewModel.CustomFormElementName = formElement.Label;
                            }
                            else
                            {
                                customFormElementViewModel.CustomFormElementName = formElement.Label;
                            }

                            if (formElement.Type == "radio" || formElement.Type == "checkbox" || formElement.Type == "select")
                            {
                                customFormElementViewModel.CustomFormElementValue =
                                    formElement.selectedOptions != null && formElement.selectedOptions.Count > 0 ?
                                    formElement.selectedOptions[0].Value : "";
                            }
                            else
                            {
                                customFormElementViewModel.CustomFormElementValue = formElement.Value ?? "";
                            }



                            // Set properties based on formElement
                            customFormElementViewModel.CustomFormId = formNameId;
                            customFormElementViewModel.CustomFormElementType = formElement.Type ?? "";
                            customFormElementViewModel.CustomFormElementPlaceholder = formElement.Placeholder ?? "";

                            // Call stored procedure to insert/update custom form element detail
                            var resp1 = storedProcedureRepository.SP_InsertUpdateCustomFormDetail_Get<SPResponseViewModel>(
                                new SP_InsertUpdateCustomForm_Param_VM
                                {
                                    Id = customFormElementViewModel.Id,
                                    CustomFormId = customFormElementViewModel.CustomFormId,
                                    CustomFormElementName = customFormElementViewModel.CustomFormElementName,
                                    CustomFormElementType = customFormElementViewModel.CustomFormElementType,
                                    CustomFormElementValue = customFormElementViewModel.CustomFormElementValue,
                                    CustomFormElementPlaceholder = customFormElementViewModel.CustomFormElementPlaceholder,
                                    Mode = customFormElementViewModel.Mode,
                                    SubmittedByLoginId = _LoginID_Exact
                                });

                            // Check the result of the stored procedure for custom form element
                            if (resp1.ret <= 0)
                            {
                                apiResponse.status = resp1.ret;
                                apiResponse.message = ResourcesHelper.GetResourceValue(resp1.resourceFileName, resp1.resourceKey);
                                return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                            }



                            // Check if the form element has options and needs to save options (Mode = 7)
                            if (resp1.ret == 1 && (formElement.Type == "radio" || formElement.Type == "checkbox" || formElement.Type == "select") && formElement.options != null &&
                               formElement.options.Any(option => !string.IsNullOrEmpty(option.Label) || !string.IsNullOrEmpty(option.Value)))
                            {
                                CustomFormOptionsViewModel customFormOptionsViewModel = new CustomFormOptionsViewModel();
                                customFormOptionsViewModel.Id = resp1.Id;
                                customFormOptionsViewModel.Mode = 7;

                                // Extract option labels and join them into a single string
                                if (formElement.Type == "select")
                                {
                                    var optionLabels = formElement.options
                                     .Where(option => !string.IsNullOrEmpty(option.Label))
                                     .Select(option => option.Label);
                                    customFormOptionsViewModel.CustomFormElementOptions = string.Join(",", optionLabels);
                                }
                                else
                                {
                                    var optionValue = formElement.options
                                     .Where(option => !string.IsNullOrEmpty(option.Value))
                                      .Select(option => option.Value);
                                    customFormOptionsViewModel.CustomFormElementOptions = string.Join(",", optionValue);
                                }


                                customFormOptionsViewModel.CustomFormElementId = resp1.Id;

                                // Call stored procedure to insert/update custom form element options
                                var resp2 = storedProcedureRepository.SP_InsertUpdateCustomFormDetail_Get<SPResponseViewModel>(
                                    new SP_InsertUpdateCustomForm_Param_VM
                                    {
                                        Id = customFormOptionsViewModel.Id,
                                        CustomFormElementId = customFormOptionsViewModel.CustomFormElementId,
                                        CustomFormElementOptions = customFormOptionsViewModel.CustomFormElementOptions.Trim('/'),
                                        Mode = customFormOptionsViewModel.Mode,
                                        SubmittedByLoginId = _LoginID_Exact
                                    });

                                // Check the result of the stored procedure for custom form element options
                                if (resp2.ret <= 0)
                                {
                                    apiResponse.status = resp2.ret;
                                    apiResponse.message = ResourcesHelper.GetResourceValue(resp2.resourceFileName, resp2.resourceKey);
                                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                                }
                            }


                            


                        }


                    }

                }




                apiResponse.status = 1;
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
        /// To Get All Custom Form Detail For Super Admin 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff,SuperAdmin")]
        [Route("api/CustomForm/GetSuperCustomFormDetailPagination")]
        public HttpResponseMessage GetSuperCustomFormDetailPagination()
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
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                List<CustomFormDetail_VM> resp = customFormService.GetAllCustomFormDetail(_LoginID_Exact);


                apiResponse.status = 1;
                apiResponse.message = Resources.BusinessPanel.Success;
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
        /// To Assign /UnAssign Custom Form From Business Owner 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <param name="customFormId"></param>
        /// <param name="isAssign"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/CustomForm/AssignOrUnassignBusinessCustomForm")]
        public HttpResponseMessage AssignOrUnassignBusinessCustomForm(long businessOwnerLoginId, long customFormId, int isAssign)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }
                long _LoginID_Exact = validateResponse.UserLoginId;
                // return if invalid information
                if (businessOwnerLoginId <= 0 || customFormId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidData_ErrorMessage;
                    apiResponse.data = null;

                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                var resp = new SPResponseViewModel();
                if (isAssign == 1)
                {
                    resp = customFormService.AssignBusinessCustomForm(businessOwnerLoginId, customFormId, _LoginID_Exact);
                }
                else
                {
                    resp = customFormService.UnAssignBusinessCustomForm(businessOwnerLoginId, customFormId, _LoginID_Exact);
                }

                apiResponse.status = resp.ret;
                apiResponse.message = resp.responseMessage;
                apiResponse.data = null;

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
        /// To Get The Business Owner Detail List by Custom Id In Transfer form
        /// </summary>
        /// <param name="customformId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,SubAdmin")]
        [Route("api/CustomForm/GetAllAssignedBusinessOwnerList")]
        public HttpResponseMessage GetAllAssignedBusinessOwnerList(long customformId)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }
                else if (customformId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvaildRequestDataPassed;
                    apiResponse.data = null;

                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }
               List< BusinessSearch_ForSuperAdmin_VM> resp = new List<BusinessSearch_ForSuperAdmin_VM>();
                 resp = customFormService.GetAllAssignedBusinessOwnersToCustomFormById(customformId);

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
        /// To Get All Business Owner List Custom Form transfer Form By SuperAdmin
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/CustomForm/GetAllAssignedBusinessOwnerListDetail")]
        public HttpResponseMessage GetAllAssignedBusinessOwnerList()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }

                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                List<BusinessSearch_ForSuperAdmin_VM> resp = new List<BusinessSearch_ForSuperAdmin_VM>();
                resp = customFormService.GetAllAssignedBusinessOwnersCustomFormList(_BusinessOwnerLoginId);

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
        /// To Assign or Unassign transfer Custom Form From Business Owner Side 
        /// </summary>
        /// <param name="businessOwnerLoginId"></param>
        /// <param name="customFormId"></param>
        /// <param name="isAssign"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/CustomForm/AssignOrUnassignBusinessOwnerCustomForm")]
        public HttpResponseMessage AssignOrUnassignBusinessOwnerCustomForm(long businessOwnerLoginId, long customFormId, int isAssign)
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }
                long _LoginID_Exact = validateResponse.UserLoginId;
                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;
                // return if invalid information
                if (businessOwnerLoginId <= 0 || customFormId <= 0)
                {
                    apiResponse.status = -1;
                    apiResponse.message = Resources.ErrorMessage.InvalidData_ErrorMessage;
                    apiResponse.data = null;

                    return Request.CreateResponse(HttpStatusCode.OK, apiResponse);
                }

                var resp = new SPResponseViewModel();
                if (isAssign == 1)
                {
                    resp = customFormService.AssignBusinessCustomForm(businessOwnerLoginId, customFormId, _BusinessOwnerLoginId);
                }
                else
                {
                    resp = customFormService.UnAssignBusinessCustomForm(businessOwnerLoginId, customFormId, _BusinessOwnerLoginId);
                }

                apiResponse.status = resp.ret;
                apiResponse.message = resp.responseMessage;
                apiResponse.data = null;

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
        /// To Transfer Form Busines Owner by Another Business 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "BusinessAdmin,Staff")]
        [Route("api/CustomForm/GetAllAssignedBusinessOwnerListByBusiness")]
        public HttpResponseMessage GetAllAssignedBusinessOwnerListByBusiness()
        {
            ApiResponse_VM apiResponse = new ApiResponse_VM();
            try
            {
                var validateResponse = ValidateLoggedInUser();
                if (validateResponse.ApiResponse_VM.status < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, validateResponse);
                }

                long _BusinessOwnerLoginId = validateResponse.BusinessAdminLoginId;

                List<BusinessSearch_ForSuperAdmin_VM> resp = new List<BusinessSearch_ForSuperAdmin_VM>();
                resp = customFormService.GetAllAssignedBusinessOwnersTransferFormToAnotherBusinessList(_BusinessOwnerLoginId);

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