var UserToken_Global = "";

$(document).ready(function () {

    $.get("/Business/GetBusinessAdminToken", null, function (dataToken) {
        if (dataToken != "" && dataToken != null) {
            UserToken_Global = dataToken;
            EditBusinessProfile();
        }
        else {
            StopLoading();
        }
    });
});

function ResetProfileform() {
    // Reset data
    $("#txtProfileFirstName").val('');
    $("#txtProfileLastName").val('');
    $("#txtEmail").val('');
    $("#txtProfileAbout").val('');
    $("#fileProfileImage_ManageProfile").val('');
    $("#ProfileImage").attr('src', '');
    $("#fileBusinesslogo_ManageProfile").val('');
    $("#BusinessLogo").attr('src', '');
    //  $("#ProfileImage").addClass('d-none');

    $(".error-class").html('');
}

function btnAddUpdateProfile() {
    let is_valid = true;
    $(".error-class").html('');

    // var _Mode = (StaffId_Global > 0) ? 2 : 1;
    var _Mode = 1;

    let _staffFirstName = $("#txtProfileFirstName").val().trim();
    let _staffLastName = $("#txtProfileLastName").val().trim();
    let _email = $("#txtEmail").val().trim();
    let _about = $("#txtProfileAbout").val().trim();

    let _error_staffFirstName = $("#error_txtProfileFirstName");
    let _error_staffLastName = $("#error_txtProfileLastName");
    let _error_email = $("#error_txtEmail");
    let _error__about = $("#error_txtProfileAbout");

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

    if (validate_IsEmptyStringInputFieldValue(_about)) {
        is_valid = false;
        _error__about.html('please enter a valid About!');
    }

    if (is_valid) {
        StartLoading();

        var data = new FormData();
        //data.append("Id", StaffId_Global);
        data.append("Email", _email);
        data.append("FirstName", _staffFirstName);
        data.append("LastName", _staffLastName);
        data.append("About", _about);
        data.append("Mode", _Mode);

        var _staffImageFile_MS = $("#fileProfileImage_ManageProfile").get(0);
        var _staffImageFiles = _staffImageFile_MS.files;
        data.append('ProfileImage', _staffImageFiles[0]);
        var _staffLogoFile_MS = $("#fileBusinesslogo_ManageProfile").get(0);
        var _staffLogoFiles = _staffLogoFile_MS.files;
        data.append('BusinessLogo', _staffLogoFiles[0]);

        $.ajax({
            url: '/api/Business/Profile/AddUpdate',
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
                EditBusinessProfile();
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
// Image File Preview -------------------------------------------------------------
document.getElementById('fileBusinesslogo_ManageProfile').addEventListener('change', handleBusinessLogoUpload);

function handleBusinessLogoUpload(event) {
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
        $('#BusinessLogo').addClass('d-none'); // hide the preview image
        return;
    }
    if (fileSize > maxSize) {
        $.iaoAlert({
            msg: 'Image size too large. Please select a smaller image(< 10 MB).',
            type: "error",
            mode: "dark",
        });
        event.target.value = null; // clear the file input element
        $('#BusinessLogo').addClass('d-none'); // hide the preview image

        return;
    }

    // image size is within the limit, display the preview image
    const reader = new FileReader();
    reader.onload = function (event) {
        document.getElementById('BusinessLogo').src = event.target.result;
        $('#BusinessLogo').removeClass('d-none');
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

function EditBusinessProfile() {
    
    var _url = "/api/Business/Profile/Get/";

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

            $("#txtProfileFirstName").val(response.data.FirstName);
            $("#txtProfileLastName").val(response.data.LastName);
            $("#txtEmail").val(response.data.Email);
            $("#txtProfileAbout").val(response.data.About);

            $("#fileProfileImage_ManageProfile").val('');
            if (response.data.ProfileImageWithPath != "") {
                $("#ProfileImage").attr('src', response.data.ProfileImageWithPath);
                $("#ProfileImage").removeClass('d-none');
            }
            $("#fileBusinesslogo_ManageProfile").val('');
            if (response.data.BusinessLogoWithPath != "") {
                $("#BusinessLogo").attr('src', response.data.BusinessLogoWithPath);
                $("#BusinessLogo").removeClass('d-none');
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