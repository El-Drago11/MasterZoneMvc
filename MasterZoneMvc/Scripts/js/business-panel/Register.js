$(document).ready(function () {
    StartLoading();
    getAllActiveParentBusinessCategories();
});

function getAllActiveParentBusinessCategories() {
    let _url = "/api/BusinessCategory/Parent/GetAllActive";

    $.ajax({
        type: "GET",
        url: _url,
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

            // --------------------- append parent categories in dropdown
            var res_Categories = '<option value="0">Select Category</option>';

            for (var i = 0; i < response.data.length; i++) {
                    res_Categories += '<option value="' + response.data[i].Id + '">' + response.data[i].Name + '</option>';
            }
            $("#ddlBusinessCategory").html('').append(res_Categories);
            // --------------------- append parent categories in dropdown

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


function getAllActiveSubCategoriesByParentCategory(id) {
    let _url = "/api/BusinessCategory/Parent/" + id + "/GetAllActiveSubCategories";
    StartLoading();
    $.ajax({
        type: "GET",
        url: _url,
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

            if (response.data) {

                var res_Categories = '';
                // --------------------- append parent categories in dropdown
                var res_Categories = '<option value="0">Select Sub Category</option>';

                for (var i = 0; i < response.data.SubCategories.length; i++) {
                    var item = response.data.SubCategories[i];
                    res_Categories += '<option value="' + item.Id + '">' + item.Name + '</option>';
                }

                $("#ddlBusinessSubCategory").html('').append(res_Categories);
                // --------------------- append parent categories in dropdown
            }
            else {
                $.iaoAlert({
                    msg: 'Could not get sub category data!',
                    type: "error",
                    mode: "dark",
                });
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

function btnRegisterFormClick() {
    let is_valid = true;
    $(".error-class").html('');

    let _businessCategoryId = $("#ddlBusinessCategory").val();
    let _businessSubCategoryId = $("#ddlBusinessSubCategory").val();
    let _firstName = $("#txtFirstName").val().trim();
    let _lastName = $("#txtLastName").val().trim();
    let _email = $("#txtEmail").val().trim();
    let _phoneNumber = $("#txtPhoneNumber").val().trim();
    let _password = $("#txtPassword").val().trim();
    let _confirmPassword = $("#txtConfirmPassword").val().trim();

    let _error_businessCategoryId = $("#error_ddlBusinessCategory");
    let _error_firstName = $("#error_txtFirstName");
    let _error_lastName = $("#error_txtLastName");
    let _error_email = $("#error_txtEmail");
    let _error_phoneNumber = $("#error_txtPhoneNumber");
    let _error_password = $("#error_txtPassowrd");
    let _error_confirmPassword = $("#error_txtConfirmPassword");

    var TestEmail = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    var phone_test = /^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$/im;

    if (validate_IsEmptySelectInputFieldValue(_businessCategoryId) || parseInt(_businessCategoryId) < 0) {
        is_valid = false;
        _error_businessCategoryId.html('Please select a category!');
    }
    if (validate_IsEmptyStringInputFieldValue(_firstName)) {
        is_valid = false;
        _error_firstName.html('Enter First Name!');
    }
    if (validate_IsEmptyStringInputFieldValue(_lastName)) {
        is_valid = false;
        _error_lastName.html('Enter Last Name!');
    }

    if (validate_IsEmptyStringInputFieldValue(_phoneNumber)) {
        is_valid = false;
        _error_phoneNumber.html('Enter Phone Number!');
    }
    else if (_phoneNumber.indexOf(' ') >= 0) {
        is_valid = false;
        _error_phoneNumber.html('Phone number will not contain any empty/white space!');
    }
    else if (!phone_test.test(_phoneNumber)) {
        is_valid = false;
        _error_phoneNumber.html('Please enter valid phone number!');
    }

    if (validate_IsEmptyStringInputFieldValue(_password)) {
        is_valid = false;
        _error_password.html('Enter Password!');
    }
    else if (validate_IsEmptyStringInputFieldValue(_confirmPassword)) {
        is_valid = false;
        _error_confirmPassword.html('Enter Confirm Password!');
    }
    else if (_confirmPassword != _password) {
        is_valid = false;
        _error_confirmPassword.html("Password doesn't match!");
    }

    if (validate_IsEmptyStringInputFieldValue(_email)) {
        is_valid = false;
        _error_email.html('Enter Email!');
    }
    else if (!TestEmail.test(_email)) {
        is_valid = false;
        _error_email.html('Please enter a valid Email!');
    }

    if (is_valid) {
        StartLoading();
        var _dataBusinessCategory = _businessCategoryId;
        if (_businessSubCategoryId > 0) {
            _dataBusinessCategory = _businessSubCategoryId
        }

        var data = new FormData();
        var countryPhoneCode = '+64';
        data.append("BusinessCategoryId", _dataBusinessCategory);
        data.append("Email", _email);
        data.append("Password", _password);
        data.append("FirstName", _firstName);
        data.append("LastName", _lastName);
        data.append("PhoneNumber", _phoneNumber);

        $.ajax({
            url: '/Business/Register',
            data: data,
            processData: false,
            mimeType: 'multipart/form-data',
            contentType: false,
            //contentType: 'application/json',
            type: 'POST',
            success: function (dataResponse) {

                //--Parse into Json of response-json-string
                dataResponse = JSON.parse(dataResponse);

                //--If successfully added
                if (dataResponse.status === 1) {
                    //swal("Success!", dataResponse.message, "success");
                    window.location.href = '/Business/Dashboard/';
                }
                else {
                    $.iaoAlert({
                        msg: dataResponse.message,
                        type: "error",
                        mode: "dark",
                    });

                    StopLoading();
                    //removeBtnLoading(btnSelector);
                    //enableContinueBtn();
                }

            },
            error: function (result) {
                StopLoading();
                //removeBtnLoading(btnSelector);
                //enableContinueBtn();

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

function onParentCategoryChange() {
    var parentId = $('#ddlBusinessCategory').val();
    getAllActiveSubCategoriesByParentCategory(parentId);
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