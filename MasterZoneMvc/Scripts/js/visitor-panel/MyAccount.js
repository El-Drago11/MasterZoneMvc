var UserToken_Global = "";
//var UpdateCardPaymentDetailId = 0;
var objPaymentDetailsData = {
    'UPIList': [],
    'PaytmList' : []
};

$(document).ready(function () {

    $.get("/Home/GetStudentToken", null, function (dataToken) {
        if (dataToken != "" && dataToken != null) {
            UserToken_Global = dataToken;
            //EditPaymentDetail();
            EditStudentProfile();
            // getallenquirylist();
        }
        else {
            StopLoading();
        }
    });
});

function ResetProfileform() {
    // Reset data
    $("#txtStudentFirstName").val('');
    $("#txtStudentLastName").val('');
    $("#txtStudentEmail").val('');
    $("#txtStudentContactNumber").val('');
    $("#fileProfileImage_ManageProfile").val('');
    $("#ProfileImage").attr('src', '');

    //  $("#ProfileImage").addClass('d-none');

    $(".error-class").html('');
}

function btnStudentAddUpdateProfile() {
    let is_valid = true;
    $(".error-class").html('');

    var _Mode = 1;

    let _staffFirstName = $("#txtStudentFirstName").val().trim();
    let _staffLastName = $("#txtStudentLastName").val().trim();
    let _email = $("#txtStudentEmail").val().trim();
    let _phoneNumber = $("#txtStudentContactNumber").val().trim();

    let _error_staffFirstName = $("#error_txtStudentFirstName");
    let _error_staffLastName = $("#error_txtStudentLastName");
    let _error_email = $("#error_txtStudentEmail");
    let _error_phoneNumber = $("#error_StudentContactNumber");

    var TestEmail = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    //var phone_test = /^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$/im;

    if (validate_IsEmptyStringInputFieldValue(_staffFirstName)) {
        is_valid = false;
        _error_staffFirstName.html('Enter First Name!');
    }

    if (validate_IsEmptyStringInputFieldValue(_staffLastName)) {
        is_valid = false;
        _error_staffLastName.html('Enter Last Name!');
    }

    if (validate_IsEmptyStringInputFieldValue(_email)) {
        is_valid = false;
        _error_email.html('Enter Email!');
    }
    else if (!TestEmail.test(_email)) {
        is_valid = false;
        _error_email.html('Please enter a valid Email!');
    }

    //if (validate_IsEmptyStringInputFieldValue(_phoneNumber)) {
    //    is_valid = false;
    //    _error_phoneNumber.html('please enter a valid PhoneNumber!');
    //}

    if (is_valid) {
        StartLoading();

        var data = new FormData();
        data.append("FirstName", _staffFirstName);
        data.append("LastName", _staffLastName);
        data.append("Email", _email);
        data.append("PhoneNumber", _phoneNumber);
        data.append("Mode", _Mode);

        var _studentImageFile_MS = $("#fileProfileImage_ManageProfile").get(0);
        var _studentImageFiles = _studentImageFile_MS.files;
        data.append('ProfileImage', _studentImageFiles[0]);


        $.ajax({
            url: '/api/Home/Profile/AddUpdateProfile',
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

                ResetProfileform();
                StopLoading();
                EditStudentProfile();
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

// Image File Preview -------------------------------------------------------------
document.getElementById('fileProfileImage_ManageProfile').addEventListener('change', handleProfileImageUpload);

function handleProfileImageUpload(event) {
    const file = event.target.files[0];
    const fileSize = file.size / 1024; // size in kilobytes
    const maxSize = 10 * 1024 * 1024; // maximum size in kilobytes
    const fileType = file.type;
    const validImageTypes = ['image/jpeg', 'image/png'];

    if (!validImageTypes.includes(fileType)) {
        $.iaoAlert({
            msg: 'Invalid image type. Please select a JPEG, PNG image.',
            type: "error",
            mode: "dark",
        });
        event.target.value = null; // clear the file input element
        $('#ProfileImage').addClass('d-none'); // hide the preview image
        return;
    }
    if (fileSize > maxSize) {
        $.iaoAlert({
            msg: 'Image size too large. Please select a smaller image(< 10 MB).',
            type: "error",
            mode: "dark",
        });
        event.target.value = null; // clear the file input element
        $('#ProfileImage').addClass('d-none'); // hide the preview image

        return;
    }

    // image size is within the limit, display the preview image
    const reader = new FileReader();
    reader.onload = function (event) {
        document.getElementById('ProfileImage').src = event.target.result;
        $('#ProfileImage').removeClass('d-none');
    }

    reader.readAsDataURL(file);
}


////// -----------    FIELD VALIDATION HANDLER FUNCTIONS  --------------------------

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
////// -----------    FIELD VALIDATION HANDLER FUNCTIONS  --------------------------

function EditStudentProfile() {

    var _url = "/api/Student/GetProfileDetail";

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

            $("#txtStudentFirstName").val(response.data.FirstName);
            $("#txtStudentLastName").val(response.data.LastName);
            $("#txtStudentContactNumber").val(response.data.PhoneNumber);
            $("#txtStudentEmail").val(response.data.Email);


            $("#fileProfileImage_ManageProfile").val('');
            if (response.data.ProfileImageWithPath != "") {
                $("#ProfileImage").attr('src', response.data.ProfileImageWithPath);
                $("#ProfileImage").removeClass('d-none');
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

function ResetProfileform() {
    // Reset data
    $("#txtStudentFirstName").val('');
    $("#txtStudentLastName").val('');
    $("#txtStudentEmail").val('');
    $("#txtStudentContactNumber").val('');
    $("#fileProfileImage_ManageProfile").val('');
    $("#ProfileImage").attr('src', '');

    //  $("#ProfileImage").addClass('d-none');

    $(".error-class").html('');
}



function ResetUPIForm() {
    $('#PaymentUpiId .error-class').html('');
    $('#txtUPIName').val('');
    $('#txthiddenUPIId').val('0');
}

function ResetPaytmForm() {
    $('#PaymentpytmId .error-class').html('');
    $('#txtPaytm').val('');
    $('#txthiddenPaytmId').val('0');
}

$('#btnAddUPI').click(function () {
    ResetUPIForm();
    $('#PaymentUpiId').show();
    $('#EditDelete').addClass('d-none');
    $('#btnAddUPI').hide();
    $('#btnUpdateUPIs').hide();
    $('#btnSubmitUPI').show();
});

function CancelUPI() {
    $('#PaymentUpiId').hide();
    if (objPaymentDetailsData.UPIList.length == 0) {
        $('#btnAddUPI').show();
    }
}

$('#btnAddPaytm').click(function () {
    ResetPaytmForm();
    $('#PaymentpytmId').show();
    $('#btnAddPaytm').hide();

    $('#btnUpdatePaytmD').hide();
    $('#SubmitAddUpdatePaytm').show();
});

function CancelPaytm() {
    $('#PaymentpytmId').hide();
    if (objPaymentDetailsData.PaytmList.length == 0) {
        $('#btnAddPaytm').show();
    }
}

function openEditSalaryModal(id) {
    $('#btnOpenSalaryModal').click();
    EditModelDetail(id);
    $('#btnCancel_SalaryModal').hide();
}

function closeEditSalaryModal() {
    $('#btnCancel_SalaryModal').click();
}

function ResetCardform() {
    // Reset data
    $("#txtCardNumber").val('');
    $("#txtUserName").val('');
    $("#txtExpMonth").val('');
    $("#txtExpYear").val('');

    $(".error-class").html('');
    $('#SubmitCardPaymentDetail').show();
    $('#UpdateCardPaymentDetail').hide();
}

function btnAddUpdatePayment() {
    let is_valid = true;
    $(".error-class").html('');

    // var _Mode = (StaffId_Global > 0) ? 2 : 1;
    var _Mode = 1;

    let _cardnumber = $("#txtCardNumber").val().trim();
    var maskedCardNumber = _cardnumber.replace(/\d(?=\d{4})/g, "*");
    let _cardname = $("#txtUserName").val().trim();
    let _expmonth = $("#txtExpMonth").val().trim();
    let _expyear = $("#txtExpYear").val().trim();

    let _error_cardnumber = $("#error_txtCardNumber");
    let _error_cardname = $("#error_txtUserName");
    let _error_expmonth = $("#error_txtExpMonth");
    let _error_expyear = $("#error_txtExpYear");

    var TestEmail = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    //var phone_test = /^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$/im;

    if (validate_IsEmptyStringInputFieldValue(_cardnumber)) {
        is_valid = false;
        _error_cardnumber.html('Enter Card Number!');
    }

    if (validate_IsEmptyStringInputFieldValue(_cardname)) {
        is_valid = false;
        _error_cardname.html('Enter Card Name!');
    }

    if (validate_IsEmptyStringInputFieldValue(_expmonth)) {
        is_valid = false;
        _error_expmonth.html('Enter Exp Month!');
    }

    if (validate_IsEmptyStringInputFieldValue(_expyear)) {
        is_valid = false;
        _error_expyear.html('please enter a valid Exp Year!');
    }

    if (is_valid) {
        StartLoading();

        var data = new FormData();
        //data.append("Id", StaffId_Global);
        data.append("CardNumber", _cardnumber);
        data.append("CardName", _cardname);
        data.append("ExpMonth", _expmonth);
        data.append("ExpYear", _expyear);
        data.append("Mode", _Mode);

        $.ajax({
            url: '/api/PaymentDetail/AddUpdateCardDetail',
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
                    $('#btnCancel_SalaryModal').click();
                    swal("Success!", dataResponse.message, "success");
                    ResetCardform();
                    getAllPaymentLists();
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

                //ResetCardform();
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

function EditPaymentDetail() {

    var _url = "/api/PaymentDetail/GetById/";

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


            $("#txtCardNumber").val(response.data.CardNumber);
            $("#txtUserName").val(response.data.CardName);
            $("#txtExpMonth").val(response.data.ExpMonth);
            $("#txtExpYear").val(response.data.ExpYear);


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

/*var changed = document.getElementById('account_changed');

changed.value = new Array(account.value.length - 3).join('x') + account.value.substr(account.value.length - 4, 4);*/

function getAllPaymentLists() {
    $('#GETUPI').html('');
    $('#ShowPaymentDetails').html('');
    $("#GETPAYTM").html('');
    let _url = "/api/PaymentDetail/GetAllPaymentDetails";
    $('#PaymentUpiId').hide();
    $('#PaymentpytmId').hide();

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

            // var resp = '';
            for (var i = 0; i < response.data.CardsList.length; i++) {
                if (response.data.CardsList.length > 1) {
                    $('#btnOpenSalaryModal').hide();
                }
                else {
                    $('#btnOpenSalaryModal').show();
                }

                $('#ShowPaymentDetails').append('<div class="card" style="margin-right: 20px; margin-bottom: 20px; min-width: 45%; height: auto;"><div class="card__front card__part" style="padding:10px;"><p class= "card_numer"  style = "text-align:center;" > ' + response.data.CardsList[i].CardNumber + '</p ><div style="display:flex;justify-content:space-between;"><div class="card__space-75"><span class="card__label">Card holder</span><p class="card__info" id="EditCardName">' + response.data.CardsList[i].CardName + '</p></div><div class="card__space-25"><span class="card__label">Expires</span><p class="card__info">' + response.data.CardsList[i].ExpMonth + '/' + response.data.CardsList[i].ExpYear + ' </p></div></div></div><div style="display: flex;justify-content: center;padding:10px;"><button type="button" class="btn btn-sm btn-info"  style="margin-right: 10px;"onclick="EditModelDetail(' + response.data.CardsList[i].PaymentDetailId + ')"><i class="fas fa-pencil"></i></button><button type="button" class="btn btn-sm btn-danger" onclick="confrimDelete_ManageStaff(' + response.data.CardsList[i].PaymentDetailId + ')"><i class="fas fa-trash"></i></button></div></div>');
                //openEditModal(response.data.CardsList[i].id);
            }

            for (var i = 0; i < response.data.UPIList.length; i++) {
                $("#txtUPIName").val(response.data.UPIList[i].UPIId);

                $("#GETUPI").append('<label>' + response.data.UPIList[i].UPIId + '</label>');
                $("#txthiddenUPIId").val(response.data.UPIList[i].PaymentDetailId);
                $("#DeleteUPIId").attr('onclick', 'confrimDelete_ManageStaff(' + response.data.UPIList[i].PaymentDetailId + ');');
            }

            if (response.data.UPIList.length > 0) {
                objPaymentDetailsData.UPIList = response.data.UPIList;

                $('#btnAddUPI').hide();
                $("#EditDelete").removeClass('d-none');
            }
            else {
                objPaymentDetailsData.UPIList = []; 
                $('#btnAddUPI').show();
                $("#EditDelete").addClass('d-none');
            }

            for (var i = 0; i < response.data.PaytmList.length; i++) {

                $("#txtPaytm").val(response.data.PaytmList[i].PaytmId);
                $("#GETPAYTM").append('<label>' + response.data.PaytmList[i].PaytmId + '</label>');
                $("#txthiddenPaytmId").val(response.data.PaytmList[i].PaymentDetailId);
                $("#DeletePaytmId").attr('onclick', 'confrimDelete_ManageStaff(' + response.data.PaytmList[i].PaymentDetailId + ');');
            }

            if (response.data.PaytmList.length > 0) {
                objPaymentDetailsData.PaytmList = response.data.PaytmList;
                $('#btnAddPaytm').hide();
                $("#EditDeletePaytm").removeClass("d-none");
            }
            else {
                objPaymentDetailsData.PaytmList = [];
                $('#btnAddPaytm').show();
                $("#EditDeletePaytm").addClass("d-none");
            }

            // $('#ShowPaymentDetails').append(resp);
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

$("#btnUpdateUPIs").click()
{
    $("#GETUPI").val('');
}
function ResetUPI() {
    // Reset data
    //$("#GETUPI").val('');
    //$("#txtUPIName").val('');
}

function ShowAddUpdateUPI() {
    $('#error_txtUPIName').html('');
    $('#PaymentUpiId').show();
    $('#btnUpdateUPIs').show();
    $('#btnSubmitUPI').hide();
    $('#txtUPIName').val(objPaymentDetailsData.UPIList[0].UPIId);
}

//function hideAddUpdateUPI() {
//    $('#error_txtUPIName').html('');
//    $('#PaymentUpiId').hide();
//    $('#btnUpdateUPIs').hide();
//    $('#btnSubmitUPI').hide();
//}

function ShowAddUpdatePaytm() {
    $('#error_txtPaytm').html('');
    $('#PaymentpytmId').show();
    $('#btnUpdatePaytmD').show();
    $('#SubmitAddUpdatePaytm').hide();
    $('#txtPaytm').val(objPaymentDetailsData.PaytmList[0].PaytmId);
}

//function hideAddUpdatePaytm() {
//    $('#PaymentpytmId').hide();
//    $('#btnUpdatePaytmD').hide();
//    $('#SubmitAddUpdatePaytm').hide();
//}

function btnAddUpdateUPI() {
    let is_valid = true;
    $(".error-class").html('');

    // var _Mode = (StaffId_Global > 0) ? 2 : 1;
    var _Mode = 1;

    let _upiId = $("#txtUPIName").val().trim();

    let _error_upiId = $("#error_txtUPIName");

    if (validate_IsEmptyStringInputFieldValue(_upiId)) {
        is_valid = false;
        _error_upiId.html('Enter UPIId !');
    }

    if (is_valid) {
        StartLoading();

        var data = new FormData();
        //data.append("Id", StaffId_Global);
        data.append("UPIId", _upiId);
        data.append("Mode", _Mode);

        $.ajax({
            url: '/api/PaymentDetails/AddUpdateUPIDetail/',
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
                    //ResetUPI();
                    //$('#PaymentUpiId').hide();
                    getAllPaymentLists();

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
function btnUpdateUPI() {
    let is_valid = true;
    $(".error-class").html('');

    // var _Mode = (StaffId_Global > 0) ? 2 : 1;


    let _upiId = $("#txtUPIName").val().trim();
    let _paymentUpiId = $("#txthiddenUPIId").val();

    let _error_upiId = $("#error_txtUPIName");


    if (validate_IsEmptyStringInputFieldValue(_upiId)) {
        is_valid = false;
        _error_upiId.html('Enter UPIId !');
    }



    if (is_valid) {
        StartLoading();

        var data = new FormData();
        data.append("Id", _paymentUpiId);
        data.append("UPIId", _upiId);




        $.ajax({
            url: '/api/PaymentDetail/UpdateUPIById/',
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
                    getAllPaymentLists();
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
function btnAddUpdatePaytm() {
    let is_valid = true;
    $(".error-class").html('');

    // var _Mode = (StaffId_Global > 0) ? 2 : 1;
    var _Mode = 1;

    let _paytmId = $("#txtPaytm").val().trim();

    let _error_paytmId = $("#error_txtPaytm");



    if (validate_IsEmptyStringInputFieldValue(_paytmId)) {
        is_valid = false;
        _error_paytmId.html('Enter PaytmId !');
    }



    if (is_valid) {
        StartLoading();

        var data = new FormData();
        //data.append("Id", StaffId_Global);
        data.append("PaytmId", _paytmId);
        data.append("Mode", _Mode);



        $.ajax({
            url: '/api/PaymentDetail/AddUpdatePaytmDetail/',
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
                    //hideAddUpdatePaytm();
                    getAllPaymentLists();
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

function EditModelDetail(id) {
    var _url = "/api/PaymentDetail/CCAvenueGetById/" + id;

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
            $('#UpdateCardPaymentDetailId').val(response.data.CardsList.PaymentDetailId);
            $("#txtCardNumber").val(response.data.CardsList.CardNumber);
            $("#txtUserName").val(response.data.CardsList.CardName);
            $("#txtExpMonth").val(response.data.CardsList.ExpMonth);
            $("#txtExpYear").val(response.data.CardsList.ExpYear);

            $('#btnOpenSalaryModal').click();
            $('#UpdateCardPaymentDetail').show();
            $('#SubmitCardPaymentDetail').hide();
            
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

function btnUpdateModel() {
    let is_valid = true;
    $(".error-class").html('');

    // var _Mode = (StaffId_Global > 0) ? 2 : 1;
    // var _Mode = 1;
    var UpdateCardPaymentDetailId = $("#UpdateCardPaymentDetailId").val();
    let _cardnumber = $("#txtCardNumber").val().trim();
    var maskedCardNumber = _cardnumber.replace(/\d(?=\d{4})/g, "*");
    let _cardname = $("#txtUserName").val().trim();
    let _expmonth = $("#txtExpMonth").val().trim();
    let _expyear = $("#txtExpYear").val().trim();

    let _error_cardnumber = $("#error_txtCardNumber");
    let _error_cardname = $("#error_txtUserName");
    let _error_expmonth = $("#error_txtExpMonth");
    let _error_expyear = $("#error_txtExpYear");

    if (validate_IsEmptyStringInputFieldValue(_cardnumber)) {
        is_valid = false;
        _error_cardnumber.html('Enter Card Number!');
    }

    if (validate_IsEmptyStringInputFieldValue(_cardname)) {
        is_valid = false;
        _error_cardname.html('Enter Card Name!');
    }

    if (validate_IsEmptyStringInputFieldValue(_expmonth)) {
        is_valid = false;
        _error_expmonth.html('Enter Exp Month!');
    }

    if (validate_IsEmptyStringInputFieldValue(_expyear)) {
        is_valid = false;
        _error_expyear.html('please enter a valid Exp Year!');
    }

    if (is_valid) {
        StartLoading();

        var data = new FormData();
        data.append("Id", UpdateCardPaymentDetailId);
        data.append("CardNumber", _cardnumber);
        data.append("CardName", _cardname);
        data.append("ExpMonth", _expmonth);
        data.append("ExpYear", _expyear);

        $.ajax({
            url: '/api/PAymentDetail/UpdateCardPaymentDetail/',
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
                    $('#btnCancel_SalaryModal').click();
                    swal("Success!", dataResponse.message, "success");
                    ResetCardform();
                    //---get Payment List
                    getAllPaymentLists();
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

function PaytmUpdation() {
    let is_valid = true;
    $(".error-class").html('');

    // var _Mode = (StaffId_Global > 0) ? 2 : 1;
    //var _Mode = 1;
    var PaymentDetailPaymentId = $("#txthiddenPaytmId").val();
    let _paytmId = $("#txtPaytm").val().trim();

    let _error_paytmId = $("#error_txtPaytm");



    if (validate_IsEmptyStringInputFieldValue(_paytmId)) {
        is_valid = false;
        _error_paytmId.html('Enter PaytmId !');
    }



    if (is_valid) {
        StartLoading();

        var data = new FormData();
        data.append("Id", PaymentDetailPaymentId);
        data.append("PaytmId", _paytmId);
        //  data.append("Mode", _Mode);



        $.ajax({
            url: '/api/PaymentDetail/UpdatePaytm/',
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
                getAllPaymentLists();
                //--If successfully added/updated
                if (dataResponse.status === 1) {
                    swal("Success!", dataResponse.message, "success");

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
function confrimDelete_ManageStaff(id) {
    swal({
        title: "Delete Package",
        text: "Are you sure to delete this Package",
        type: "warning",
        buttons: {
            cancel: true,
            confirm: "Yes",
        }
    })
        .then((willDelete) => {
            if (willDelete) {
                DeleteStaff(id);
            } else {
                //swal("Your imaginary file is safe!");
            }
        });
}

function DeleteStaff(id) {
    StartLoading();
    $.ajax({
        type: "POST",
        url: "/api/PaymentDetail/DeleteById/" + id,
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
                getAllPaymentLists();
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

//function btnAddCardModalClick() {
//    $('#SubmitCardPaymentDetail').show();
//    $('#UpdateCardPaymentDetail').hide();
//    $('#btnOpenSalaryModal').click();
//}