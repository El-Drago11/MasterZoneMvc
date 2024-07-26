var UserToken_Global = "";
var EnquiryId_Global = 0;
var EnquiryList_LastRecordId_Global = 0;
var IsViewMoreEnquiryEnabled_Global = 0;
var UpdateCardPaymentDetailId = 0;

$(document).ready(function () {
    StartLoading();
    $.get("/Home/GetStudentToken", null, function (dataToken) {
        if (dataToken != "" && dataToken != null) {
            UserToken_Global = dataToken;
            getAllBusinessOwnerLists();
        }
        else {
            $.iaoAlert({
                msg: 'Unauthorized! Invalid Token!',
                type: "error",
                mode: "dark",
            });
            window.location.href = '/home/login';
            StopLoading();
        }
    });
});

function ResetAddUpdateEnquiry() {
    // Reset data
    $("#textenquiries").val('');
    $("#textdescriptionenquiries").val('');
    $("#ddlParentBusiness_Manage").val('0');

    $(".error-class").html('');
    EnquiryId_Global = 0;

    // Enquiry Modal setup to New-Enquiry
    $('#AddUpdateEnquiryModal .modal-title').html('New Enquiry');
    $('#submitEnquiry').html('Send');
}

function btnAddUpdateEnquiry() {
    let is_valid = true;

    var _Mode = (EnquiryId_Global > 0) ? 2 : 1;
    //var _Mode = 1;
    $('.error-class').html('');

    let _enquiry = $("#textenquiries").val().trim();
    let _enquirydescription = $("#textdescriptionenquiries").val().trim();
    var _businessOwnerId = $("#ddlParentBusiness_Manage").val();

    let _error_enquiry = $("#error_txtenquiry");
    let _error_enquirydescription = $("#error_txtdescriptions");
    let _error_businessOnwerId = $("#error_ddlParentBusiness_Manage");

    if (validate_IsEmptyStringInputFieldValue(_enquiry)) {
        is_valid = false;
        _error_enquiry.html('Enter Title!');
    }

    if (validate_IsEmptyStringInputFieldValue(_enquirydescription)) {
        is_valid = false;
        _error_enquirydescription.html('Enter Description!');
    }

    if (validate_IsEmptySelectInputFieldValue(_businessOwnerId)) {
        is_valid = false;
        _error_businessOnwerId.html('Please select a business owner!');
    }

    if (is_valid) {
        StartLoading();

        var data = new FormData();

        data.append("Id", EnquiryId_Global);
        data.append("Title", _enquiry);
        data.append("Description", _enquirydescription);
        data.append("BusinessOwnerId", _businessOwnerId);
        data.append("Mode", _Mode);

        $.ajax({
            url: '/api/Enquiry/AddUpdateEnquiry',
            headers: {
                "Authorization": "Bearer " + UserToken_Global
            },
            data: data,
            processData: false,
            mimeType: 'multipart/form-data',
            contentType: false,
            //contentType: 'application/json',
            type: 'POST',
            success: function (dataResponse) {

                //--Parse into Json of response-json-string
                dataResponse = JSON.parse(dataResponse);

                //--If successfully added/updated
                if (dataResponse.status === 1) {
                    swal("Success!", dataResponse.message, "success");
                    ResetAddUpdateEnquiry();
                    $('#btnCloseEnquiryModal').click();
                    getAllEnquiries();
                }
                else {
                    swal({
                        title: 'Error!',
                        icon: 'error',
                        text: dataResponse.message
                    });
                    StopLoading();

                    return;
                }

                StopLoading();
            },
            error: function (result) {
                StopLoading();

                if (result["status"] == 401) {
                    $.iaoAlert({
                        msg: 'Unauthorized! Invalid Token!',
                        type: "error",
                        mode: "dark",
                    });
                }
                else {
                    $.iaoAlert({
                        msg: 'There is some technical error, please try again!',
                        type: "error",
                        mode: "dark",
                    });
                }
            }
        });
    }
}

function getAllBusinessOwnerLists() {

    $("#ddlParentBusiness_Manage").html('');

    let _url = "/api/Student/GetBusinessOnwers";

    $.ajax({
        type: "GET",
        url: _url,
        headers: {
            "Authorization": "Bearer " + UserToken_Global,
            "Content-Type": "application/json"
        },
        contentType: 'application/json',
        success: function (response) {
            if (response.status < 1) {
                $.iaoAlert({
                    msg: 'Unauthorized! Invalid Token!',
                    type: "error",
                    mode: "dark",
                });

                return;
            }

            var res_BusinessOwner = '<option value="0">Select Business Owner</option>';

            for (var i = 0; i < response.data.length; i++) {
                var businessOwnerName = response.data[i].FirstName + ' ' + response.data[i].LastName;
                var businessName = (response.data[i].BusinessName == '') ? '' : '(' + response.data[i].BusinessName + ')';
                res_BusinessOwner += '<option value="' + response.data[i].Id + '">' + businessOwnerName + ' ' + businessName + '</option>';
            }

            $("#ddlParentBusiness_Manage").append(res_BusinessOwner);
            getAllEnquiries();
            StopLoading();
        },
        error: function (result) {
            getAllEnquiries();
            StopLoading();

            if (result["status"] == 401) {
                $.iaoAlert({
                    msg: 'Unauthorized! Invalid Token!',
                    type: "error",
                    mode: "dark",
                });
            }
            else {
                $.iaoAlert({
                    msg: 'There is some technical error, please try again!',
                    type: "error",
                    mode: "dark",
                });
            }
        }
    });
}

function getAllEnquiries() {

    let _url = "/api/Enquiry/GetAllByStudent?lastRecordId=0";

    $.ajax({
        type: "GET",
        url: _url,
        headers: {
            "Authorization": "Bearer " + UserToken_Global,
            "Content-Type": "application/json"
        },
        contentType: 'application/json',
        success: function (response) {
            if (response.status < 1) {
                $.iaoAlert({
                    msg: 'Unauthorized! Invalid Token!',
                    type: "error",
                    mode: "dark",
                });

                return;
            }

            var enquiryList = '';

            // if no enquiries found and not a hit from ViewMore.
            if (response.data.enqurylst.length <= 0) {
                $('#enquiryList').html('<div class="w-100 text-black-50 text-center mt-4"><i>You don\'t have any enquiry!</i></div>');
                $('#btnViewMoreEnquiry').addClass('d-none');
                StopLoading();
                return;
            }

            if (response.data.enqurylst.length <= 0) {
                $('#btnViewMoreEnquiry').addClass('d-none');
            }
            else {
                EnquiryList_LastRecordId_Global = response.data.enqurylst[response.data.enqurylst.length - 1].Id;
                $('#btnViewMoreEnquiry').removeClass('d-none');
            }

            enquiryList = getHTMLBindedEnquiryData(response.data.enqurylst);

            $('#enquiryList').html(enquiryList);

            //getAllBusinessOwnerLists();

            StopLoading();
        },
        error: function (result) {
            StopLoading();
            //getAllBusinessOwnerLists();
            if (result["status"] == 401) {
                $.iaoAlert({
                    msg: 'Unauthorized! Invalid Token!',
                    type: "error",
                    mode: "dark",
                });
            }
            else {
                $.iaoAlert({
                    msg: 'There is some technical error, please try again!',
                    type: "error",
                    mode: "dark",
                });
            }
        }
    });
}

function ViewMoreEnquiries() {

    let _url = "/api/Enquiry/GetAllByStudent?lastRecordId=" + EnquiryList_LastRecordId_Global;

    $.ajax({
        type: "GET",
        url: _url,
        headers: {
            "Authorization": "Bearer " + UserToken_Global,
            "Content-Type": "application/json"
        },
        contentType: 'application/json',
        success: function (response) {
            if (response.status < 1) {
                $.iaoAlert({
                    msg: 'Unauthorized! Invalid Token!',
                    type: "error",
                    mode: "dark",
                });

                return;
            }

            var enquiryList = '';

            if (response.data.enqurylst.length <= 0) {
                $('#btnViewMoreEnquiry').addClass('d-none');
            }
            else {
                EnquiryList_LastRecordId_Global = response.data.enqurylst[response.data.enqurylst.length - 1].Id;
                $('#btnViewMoreEnquiry').removeClass('d-none');
            }

            enquiryList = getHTMLBindedEnquiryData(response.data.enqurylst);

            $('#enquiryList').append(enquiryList);

            //getAllBusinessOwnerLists();

            StopLoading();
        },
        error: function (result) {
            StopLoading();
            //getAllBusinessOwnerLists();
            if (result["status"] == 401) {
                $.iaoAlert({
                    msg: 'Unauthorized! Invalid Token!',
                    type: "error",
                    mode: "dark",
                });
            }
            else {
                $.iaoAlert({
                    msg: 'There is some technical error, please try again!',
                    type: "error",
                    mode: "dark",
                });
            }
        }
    });
}

function getHTMLBindedEnquiryData(enquiryList) {
    var htmlData = '';

    for (var i = 0; i < enquiryList.length; i++) {

        var deleteIconButton = '<i class="fas fa-trash" style="cursor:pointer;margin-left:14px;" onclick="confrimDelete_ManageEnquiry(' + enquiryList[i].Id + ')" title="Delete Enquiry"></i>';

        var editOrViewAction = '';
        if (enquiryList[i].IsReplied == 0) {
            editOrViewAction = '<i class="fas fa-edit" style="cursor:pointer" data-toggle="modal" data-target="#myEnquiryModal" onclick="EditEnquiry(' + enquiryList[i].Id + ')" title="Edit Enquiry"></i>';
        }
        else {
            editOrViewAction = '<i class="fas fa-eye" style="cursor:pointer" onclick="getEnquiryDetailById(' + enquiryList[i].Id + ')" title="View Enquiry"></i>';
        }
        var enquiry = enquiryList[i];
        //${ moment(enquiry.CreatedOn).format('YYYY-MM-DD') }
        var enquiryBusinessNamePhrase = (enquiry.BusinessName == "") ? "" : " - " + enquiry.BusinessName;
        htmlData += `
                    <div class="enq-cnt-liuijl enq-cnt-liuijl">
                        <div>
                            <div class="enq-cnt-inner border-bottom d-flex justify-content-between">
                                <a href="javascript:getEnquiryDetailById(${enquiry.Id});" class="enq-cnt-link-kddd flex-grow-1">
                                    <h6 class="enq-h-urie" >${enquiry.Title} ${enquiryBusinessNamePhrase}</h6>
                                    <p class="enq-txt-rjei">${enquiry.CreatedOn_FormatDate}</p>
                                </a>
                                <div>
                                    ${editOrViewAction} ${deleteIconButton}
                                </div>
                            </div>
                        </div>
                    </div>
                `;
    }

    return htmlData;
}

function getEnquiryDetailById(id) {
    StartLoading();
    let _url = "/api/Enquiry/GetById?id=" + id;

    $.ajax({
        type: "GET",
        url: _url,
        headers: {
            "Authorization": "Bearer " + UserToken_Global,
            "Content-Type": "application/json"
        },
        contentType: 'application/json',
        success: function (response) {
            if (response.status < 1) {
                $.iaoAlert({
                    msg: 'Unauthorized! Invalid Token!',
                    type: "error",
                    mode: "dark",
                });

                return;
            }

            if (response.data && response.data != null) {
                var modalBody = `<p class="modl-txt">${response.data.EnquiryDetail.Description}</p>`;
                if (response.data.EnquiryDetail.IsReplied == 1) {
                    modalBody += `<hr/><h6 class="text-black-50"><i class="fa fa-reply"></i> Reply <span class="float-right">${response.data.EnquiryDetail.RepliedOn_FormatDate}</span></h6>
                            <p class="modl-txt">${response.data.EnquiryDetail.ReplyBody}</p>
                        `;
                }
                $('#enq-modal1a .modal-title').html(response.data.Title);
                $('#enq-modal1a .modal-body').html(modalBody);
                $('#enq-modal1a').modal('show');
            }
            else {
                $.iaoAlert({
                    msg: 'Did not get data. Try Again later!',
                    type: "error",
                    mode: "dark",
                });
            }

            StopLoading();
        },
        error: function (result) {

            if (result["status"] == 401) {
                $.iaoAlert({
                    msg: 'Unauthorized! Invalid Token!',
                    type: "error",
                    mode: "dark",
                });
            }
            else {
                $.iaoAlert({
                    msg: 'There is some technical error, please try again!',
                    type: "error",
                    mode: "dark",
                });
            }
        }
    });
}

function confrimDelete_ManageEnquiry(id) {
    swal({
        title: "Delete Enquiry",
        text: "Are you sure to delete this Enquiry",
        type: "warning",
        buttons: {
            cancel: true,
            confirm: "Yes",
        }
    })
    .then((willDelete) => {
        if (willDelete) {
            DeleteEnquiry(id);
        } else {
            //swal("Your imaginary file is safe!");
        }
    });
}

function DeleteEnquiry(id) {
    StartLoading();
    $.ajax({
        type: "POST",
        url: "/api/Enquiry/Delete?id=" + id,
        headers: {
            "Authorization": "Bearer " + UserToken_Global,
            "Content-Type": "application/json"
        },
        contentType: 'application/json',
        success: function (dataResponse) {
            StopLoading();

            //--Check if staff successfully deleted
            if (dataResponse.status == 1) {
                setTimeout(function () {
                    swal("Success!", dataResponse.message, "success");
                }, 100);
                getAllEnquiries();
            }
            else {
                $.iaoAlert({
                    msg: dataResponse.message,
                    type: "error",
                    mode: "dark",
                });
            }

        },
        error: function (result) {
            StopLoading();

            if (result["status"] == 401) {
                $.iaoAlert({
                    msg: 'Unauthorized! Invalid Token!',
                    type: "error",
                    mode: "dark",
                });
            }
            else {
                $.iaoAlert({
                    msg: 'There is some technical error, please try again!',
                    type: "error",
                    mode: "dark",
                });
            }
        }
    });
}

function EditEnquiry(id) {
    var _url = "/api/Enquiry/GetById/?id=" + id;

    StartLoading();

    $.ajax({
        type: "GET",
        url: _url,
        headers: {
            "Authorization": "Bearer " + UserToken_Global,
            "Content-Type": "application/json"
        },
        contentType: 'application/json',
        success: function (response) {
            if (response.status < 1) {
                $.iaoAlert({
                    msg: response.message,
                    type: "error",
                    mode: "dark",
                });
                return;
            }
            ResetAddUpdateEnquiry();

            // Edit Modal setup
            $('#AddUpdateEnquiryModal .modal-title').html('Edit Enquiry');
            $('#submitEnquiry').html('Update');

            EnquiryId_Global = response.data.Id;

            $("#textenquiries").val(response.data.Title);
            $("#ddlParentBusiness_Manage").val(response.data.BusinessOwnerId);
            $("#textdescriptionenquiries").val(response.data.Description);

            $('#btnAddUpdateEnquiryModal').click();
            StopLoading();
        },
        error: function (result) {
            StopLoading();

            if (result["status"] == 401) {
                $.iaoAlert({
                    msg: 'Unauthorized! Invalid Token!',
                    type: "error",
                    mode: "dark",
                });
            }
            else {
                $.iaoAlert({
                    msg: 'There is some technical error, please try again!',
                    type: "error",
                    mode: "dark",
                });
            }
        }
    });
}

function AddEnquiry() {
    $('#btnAddUpdateEnquiryModal').click();
}

///// -----------    FIELD VALIDATION HANDLER FUNCTIONS  --------------------------
function validate_IsEmptyStringInputFieldValue(inputFieldValue) {
    if (inputFieldValue == '' || inputFieldValue.replace(/\s/g, "") == "")
        return true;
    return false;
}

function validate_IsEmptySelectInputFieldValue(inputFieldValue) {
    if (inputFieldValue == undefined || inputFieldValue == null || inputFieldValue == '' || inputFieldValue == 0)
        return true;
    return false;
}
///// -----------    FIELD VALIDATION HANDLER FUNCTIONS  --------------------------


