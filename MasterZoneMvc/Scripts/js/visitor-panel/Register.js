
function btnRegisterClick() {

    let is_valid = true;
    $(".error-class").html('');


    let _firstName = $("#txtFirstName").val().trim();
    let _lastName = $("#txtLastName").val().trim();
    let _email = $("#txtEmail").val().trim();
    let _password = $("#txtPassword").val().trim();
    let _confirmPassword = $("#txtConfirmPassword").val().trim();

    let _error_firstName = $("#error_txtFirstName");
    let _error_lastName = $("#error_txtLastName");
    let _error_email = $("#error_txtEmail");
    let _error_password = $("#error_txtPassword");
    let _error_confirmPassword = $("#error_txtConfirmPassword");

    var TestEmail = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    var phone_test = /^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$/im;



    if (validate_IsEmptyStringInputFieldValue(_firstName)) {
        is_valid = false;
        _error_firstName.html('Enter First Name!');
    }
    if (validate_IsEmptyStringInputFieldValue(_lastName)) {
        is_valid = false;
        _error_lastName.html('Enter Last Name!');
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
        var data = new FormData();
        //var countryPhoneCode = '+64';

        data.append("Email", _email);
        data.append("Password", _password);
        data.append("FirstName", _firstName);
        data.append("LastName", _lastName);

        $.ajax({
            url: '/Home/Register',
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
                    window.location.href = '/Home/MyClass/';
                }
                else {
                    $.iaoAlert({
                        msg: dataResponse.message,
                        type: "error",
                        mode: "dark",
                    });

                    StopLoading();
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