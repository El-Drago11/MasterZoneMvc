
var UserToken_Global = "";
//var superadminId_Global = 0;

$(document).ready(function () {
    StartLoading();
    $.get("/SuperAdmin/GetSuperAdminToken", null, function (dataAdminToken) {
        if (dataAdminToken != "" && dataAdminToken != null) {

            UserToken_Global = dataAdminToken;
            EditContactUs();

        }
        else {

            //$.get("/SubAdmin/GetSubAdminCookieDetail", null, function (dataSubAdminToken) {
            //    if (dataSubAdminToken != "" && dataSubAdminToken != null) {

            //        UserToken_Global = dataSubAdminToken;

            //    }
            //    else {

            //    }
            //});
        }

        StopLoading();
    });
});

function AddUpdate_ContactUs() {
    let is_valid = true;
    $(".error-class").html('');

    // var _Mode = (StaffId_Global > 0) ? 2 : 1;


    let _email = $("#textEmailContactUser").val().trim();
    let _contactNumber = $("#textContactNumber").val().trim();
    let _phoneNumber = $("#textPhoneNumber").val().trim();
    let _address = $("#textAddress").val().trim();

    let _error_email = $("#error_textEmailContactUser");
    let _error_contactNumber = $("#error_textContactNumber");
    let _error_address = $("#error_textAddress");

    var TestEmail = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    var phone_test = /^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$/im;

    if (validate_IsEmptyStringInputFieldValue(_email)) {
        is_valid = false;
        _error_email.html('Enter Email!');
    }
    else if (!TestEmail.test(_email)) {
        is_valid = false;
        _error_email.html('Please enter a valid Email!');
    }
    if (validate_IsEmptyStringInputFieldValue(_contactNumber)) {
        is_valid = true;
        _error_contactNumber.html('Enter Contact Number!');
    }
    else if (_contactNumber.indexOf(' ') >= 0) {
        is_valid = true;
        _error_contactNumber.html('Contact number will not contain any empty/white space!');
    }

    else if (!phone_test.test(_contactNumber)) {
        is_valid = true;
        _error_contactNumber.html('Please enter valid contact number!');
    }

    if (validate_IsEmptyStringInputFieldValue(_address)) {
        is_valid = false;
        _error_address.html('Enter Address!');
    }


    if (is_valid) {
        StartLoading();

        var data = new FormData();
        //data.append("Id", superadminId_Global);
        data.append("Email", _email);
        data.append("ContactNumber1", _contactNumber);
        data.append("ContactNumber2", _phoneNumber);
        data.append("Address", _address);

        $.ajax({
            url: '/api/ContactDetail/AddUpdate',
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


                //ResetAddView();
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
function EditContactUs() {
    var _url = "/api/ContactDetail/GetContactDetail";

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

            if (response.data != null) {
                $("#textEmailContactUser").val(response.data.Email);
                $("#textContactNumber").val(response.data.ContactNumber1);
                $("#textPhoneNumber").val(response.data.ContactNumber2);
                $("#textAddress").val(response.data.Address);
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



////// -----------    FIELD VALIDATION HANDLER FUNCTIONS  --------------------------

const validate_IsEmptyStringInputFieldValue = function (inputFieldValue) {
    if (inputFieldValue == '' || inputFieldValue.replace(/\s/g, "") == "")
        return true;
    return false;
}

const validate_IsEmptySelectInputFieldValue = function (inputFieldValue) {
    if (inputFieldValue == undefined || inputFieldValue == null || inputFieldValue == '' || inputFieldValue == 0)
        return true;
    return false;
}
////// -----------    FIELD VALIDATION HANDLER FUNCTIONS  --------------------------
