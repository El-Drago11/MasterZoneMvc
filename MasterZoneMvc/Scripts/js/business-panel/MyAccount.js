$(document).ready(function () {
    console.log(API_Base_URL);
});

function btnSubmitClick() {
    let is_valid = true;
    $(".error-class").html('');

    let _currentpassword = $("#txtCurrentPassword").val().trim();
    let _newpassword = $("#txtNewPassword").val().trim();
    let _confirmpassword = $("#txtConfirmPassword").val().trim();

    
   
    let _error_currentpassword = $("#error_txtCurrentPassword");
    let _error_newpassword = $("#error_txtNewPassowrd");
    let _error_confirmpassword = $("#error_txtConfirmPassword");

   // var TestEmail = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
   // var phone_test = /^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$/im;


    
    if (validate_IsEmptyStringInputFieldValue(_currentpassword)) {
        is_valid = false;
        _error_currentpassword.html('Enter Current password!');
    }

    if (validate_IsEmptyStringInputFieldValue(_newpassword)) {
        is_valid = false;
        _error_newpassword.html('Enter  New Password!');
    }
    if (validate_IsEmptyStringInputFieldValue(_confirmpassword)) {
        is_valid = false;
        _error_confirmpassword.html('Enter Confirm Password!');
    }
    else if (validate_IsEmptyStringInputFieldValue(_confirmpassword)) {
        is_valid = false;
        _error_confirmpassword.html('Enter Confirm Password!');
    }
    else if (_confirmpassword != _password) {
        is_valid = false;
        _error_confirmpassword.html("Password doesn't match!");
    }


    
    if (is_valid) {
        var data = new FormData();
        data.append("CurrentPassword", _currentpassword);
        data.append("NewPassword", _newpassword);
        data.append("ConfirmPassword", _confirmpassword);

        $.ajax({
            url: '/Business/ManageStaff',
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
                    //swal("Success!", dataResponse.message, "success");
                    window.location.href = '/Business/Dashboard/';
                }
                else {
                    swal({
                        title: '',
                        imageUrl: '/Content/svg/error.svg',
                        text: dataResponse.message
                    });

                    //StopLoading();
                    removeBtnLoading(btnSelector);
                    enableContinueBtn();
                }

            },
            error: function (result) {
                //StopLoading();
                removeBtnLoading(btnSelector);
                enableContinueBtn();

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




/*$(document).ready(function () {
    $('#btnAddStaff').click(function () {
        $('#sectionAddStaff').show();
        $('#sectionViewStaff').hide();
      
        //document.getElementById("pageTextchange").innerHTML = "Add Staff";
       // document.getElementById("pageStageChange").innerHTML = "Add Staff";

    });
});
*/

function EditStaff() {
    $('#sectionAddStaff').show();
    $('#sectionViewStaff').hide();
    document.getElementById("myText").innerHTML = "Edit Staff";
    document.getElementById("ChangeUpdateText").innerHTML = "Update";
    document.getElementById("pageTextchange").innerHTML = "Edit Staff";
    document.getElementById("pageStageChange").innerHTML = "Edit Staff";
    $('#ChangeUpdateText').show();
    $('#btnAddStaff').hide();
    $(".error-class").html('');
    $('#btnAddStaff').click(function () {
        document.getElementById("myText").innerHTML = "Add Staff";
        document.getElementById("ChangeUpdateText").innerHTML = "Save";
        $(".error-class").html('');
    });
};

function ShowingViewStaffList() {

    $('#sectionViewStaff').show();

}


/* function ResetAddView() {
     $("#ddlManageStaff").val('');
     $("#txtEmail").val('');
     $("#txtStaffName").val('');
     $("#txtPassword").val('');
     $("#txtConfirmPassword").val('');
     $('#ShowingViewStaffList').show();
}*/
function ResetAddView() {
    $("#ddlManageStaff").val('');
    $("#txtEmail").val('');
    $("#txtStaffName").val('');
    $("#txtPassword").val('');
    $("#txtConfirmPassword").val('');
    $('#sectionViewStaff').show();
    $('#sectionAddStaff').hide();
    $('#btnAddStaff').show();
    document.getElementById("pageTextchange").innerHTML = "View Staff";
    document.getElementById("pageStageChange").innerHTML = "View Staff";
    $(".error-class").html('');
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
function ConfirmDeleteStaff(sid) {
    swal({
        title: "Delete Staff",
        text: "Are you sure to delete this Staff-Member?",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: '#DD6B55',
        confirmButtonText: 'Yes',
        cancelButtonText: "No"
    }, function (isConfirm) {
        if (!isConfirm) return;
        DeleteStaff(sid);
    });
}

function DeleteStaff(sid) {
    StartLoading();
    $.ajax({
        type: "GET",
        url: "/DeleteStaffById?staffID=" + sid,
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
                    //--Get Staff List
                    //GetStaffList();
                    FilterStaffByBranch();
                }, 100);
            }
            else {
                $.iaoAlert({
                    msg: 'There is some technical error, please try again!',
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

function ConfirmChangeStatusStaff(sid) {
    swal({
        title: "Change Status",
        text: "Are you sure to change status of this Staff-Member?",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: '#DD6B55',
        confirmButtonText: 'Yes',
        cancelButtonText: "No"
    }, function (isConfirm) {
        if (!isConfirm) return;
        ChangeStatusStaff(sid);
    });
}

function ChangeStatusStaff(sid) {
    StartLoading();
    $.ajax({
        type: "GET",
        url: "/ChangeStaffStatusById?staffID=" + sid,
        headers: {
            "Authorization": "Bearer " + UserToken_Global,
            "Content-Type": "application/json"
        },
        contentType: 'application/json',
        success: function (dataResponse) {
            StopLoading();

            //--Check if staff-status successfully updated
            if (dataResponse.status == 1) {
                setTimeout(function () {
                    swal("Success!", dataResponse.message, "success");
                    //--Get Staff List
                    //GetStaffList();
                    FilterStaffByBranch();
                }, 100);
            }
            else {
                $.iaoAlert({
                    msg: 'There is some technical error, please try again!',
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
///
function swl01() {
    swal({
        title: "Are you sure?",
        text: "Once deleted, you will not be able to recover this imaginary file!",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                swal("Oof! Your imaginary file has been deleted!", {
                    icon: "success",
                });
            } else {
                swal("Your imaginary file is safe!");
            }
        });

}
///////////////////////////////validation for basic ///////////////////////
function btnSubmitFormClick() {
    let is_valid = true;
    $(".error-class").html('');

    let _name = $("#txtName").val().trim();
    let _password = $("#txtPassword").val().trim();
   



    let _error_name = $("#error_txtName");
    let _error_password = $("#error_txtPassowrd");
    

    // var TestEmail = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    // var phone_test = /^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$/im;



    if (validate_IsEmptyStringInputFieldValue(_name)) {
        is_valid = false;
        _error_name.html('Enter Name!');
    }

    if (validate_IsEmptyStringInputFieldValue(_password)) {
        is_valid = false;
        _error_password.html('Enter  Password!');
    }
    


    if (is_valid) {
        var data = new FormData();
        data.append("Name", _name);
        data.append("Password", _password);
       

        $.ajax({
            url: '/Business/MyAccount',
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
                    //swal("Success!", dataResponse.message, "success");
                    window.location.href = '/Business/Dashboard/';
                }
                else {
                    swal({
                        title: '',
                        imageUrl: '/Content/svg/error.svg',
                        text: dataResponse.message
                    });

                    //StopLoading();
                    removeBtnLoading(btnSelector);
                    enableContinueBtn();
                }

            },
            error: function (result) {
                //StopLoading();
                removeBtnLoading(btnSelector);
                enableContinueBtn();

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




$(document).ready(function () {
    $('#btnAddStaff').click(function () {
        $('#sectionAddStaff').show();
        $('#sectionViewStaff').hide();
        $(".error-class").html();
        //document.getElementById("pageTextchange").innerHTML = "Add Staff";
        // document.getElementById("pageStageChange").innerHTML = "Add Staff";

    });
});


