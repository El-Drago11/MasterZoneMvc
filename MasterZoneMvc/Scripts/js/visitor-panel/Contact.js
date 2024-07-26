$(document).ready(function () {
    StartLoading();
    GetContactDetail();
    StopLoading();
});

function GetContactDetail() {
    let _url = "/api/ContactDetail/GetContactDetail";

    $.ajax({
        type: "GET",
        url: _url,
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

            if (response.data != null) {
                var _htmlDataContacDetails = `
                    <div class="d-flex">
                        <div><i class="fa fa-home" aria-hidden="true"></i></div>
                        <div>
                            <p>Location</p>
                            <p>
                                ${response.data.Address.replace(/\n/g, "<br/>")}
                            </p>
                        </div>
                    </div>

                    <div class="d-flex">
                        <div><i class="fa fa-envelope-o" aria-hidden="true"></i></div>
                        <div>
                            <p>Email</p>
                            <p>${response.data.Email}</p>
                        </div>
                    </div>

                    <div class="d-flex">
                        <div><i class="fa fa-phone" aria-hidden="true"></i></div>
                        <div>
                            <p>Phone</p>
                            <p>${response.data.ContactNumber1}</p>
                            <p>${response.data.ContactNumber2}</p>
                        </div>
                    </div>
                `;
                //$('#AddressContact_Details_Info').append('<div><i class="fa fa-home" aria-hidden="true"></i></div><div><p>Location</p><div><p>' + response.data.Address.replace(/\n/g, "<br/>") + '</p></div></div><div class="d-flex"><div><i class="fa fa-envelope-o" aria-hidden="true"></i></div><div><p>Email</p><p>' + response.data.Email + '</p></div></div><div class="d-flex"><div><i class="fa fa-phone" aria-hidden="true"></i></div><div> <p>Phone</p><p>' + response.data.ContactNumber1 + '</p><p>' + response.data.ContactNumber2 + '</p></div></div>');

                $('#AddressContact_Details_Info').append(_htmlDataContacDetails);
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


function btnSubmitUser() {
    let is_valid = true;

    // var _Mode = (StaffId_Global > 0) ? 2 : 1;
    //var _Mode = 1;
    $('.error-class').html('');
    let _name = $("#text_Contact_Name").val().trim();
    let _contact_email_address = $("#text_Email_Address").val().trim();
    let _contact_phone_detail = $("#text_phone").val();
    let _contact_messages = $("#text_Messages").val();

    let _error_name = $("#error_text_Contact_Name");
    let _error_contact_email_address = $("#error_text_Email_Address");
    let _error_contact_phone_detail = $("#error_text_phone");
    let _error_contact_messages = $("#error_text_Messages");

    var TestEmail = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    var phone_test = /^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$/im;

    if (validate_IsEmptyStringInputFieldValue(_name)) {
        is_valid = false;
        _error_name.html('Enter Name!');
    }

    if (validate_IsEmptyStringInputFieldValue(_contact_email_address)) {
        is_valid = false;
        _error_contact_email_address.html('Enter Email Address!');
    }
    else if (!TestEmail.test(_contact_email_address)) {
        is_valid = false;
        _error_contact_email_address.html('Please enter a valid Email Address!');
    }

    if (validate_IsEmptyStringInputFieldValue(_contact_phone_detail)) {
        is_valid = false;
        _error_contact_phone_detail.html('Enter Phone Number!');
    }
    else if (_contact_phone_detail.indexOf(' ') >= 0) {
        is_valid = false;
        _error_contact_phone_detail.html('Phone number will not contain any empty/white space!');
    }
    else if (!phone_test.test(_contact_phone_detail)) {
        is_valid = false;
        _error_contact_phone_detail.html('Please enter valid phone number!');
    }
    if (validate_IsEmptyStringInputFieldValue(_contact_messages)) {
        is_valid = false;
        _error_contact_messages.html('Enter Message!');
    }

    if (is_valid) {
        StartLoading();

        var _Params = {
            "Name": _name,
            "EmailAddress": _contact_email_address,
            "PhoneNumber": _contact_phone_detail,
            "Message": _contact_messages
        };
        $.ajax({
            url: '/api/ContactDetail/SendContactUsMessage',
            data: JSON.stringify(_Params),
            processData: false,
            // mimeType: 'multipart/form-data',
            // contentType: false,
            contentType: 'application/json',
            type: 'POST',
            success: function (dataResponse) {

                //--If successfully sent 
                if (dataResponse.status == 1) {
                    $.iaoAlert({
                        msg: 'Message sent successfully!',
                        type: "success",
                        mode: "dark",
                    });
                }
                else {
                    swal({
                        title: dataResponse.message,
                        icon: 'error',
                        text: dataResponse.message
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