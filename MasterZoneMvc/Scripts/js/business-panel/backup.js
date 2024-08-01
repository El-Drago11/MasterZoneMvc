var global_AdminUserId='';
var UserToken_Global = '';
$(document).ready(function () {

    $.get("/Business/GetBusinessAdminToken", null, function (dataToken) {
        if (dataToken != "" && dataToken != null) {
            UserToken_Global = dataToken;
            GetAllMasterIdDetails();
        }
        else {
            $.get("/Business/GetStaffToken", null, function (dataToken) {
                if (dataToken != "" && dataToken != null) {
                    UserToken_Global = dataToken;
                }
                else {
                    StopLoading();
                }
            });
        }
    });
    StopLoading();
});

function btnSubmitFormClick() {
    let is_valid = true;
    $(".error-class").html('');

    let _name = $("#txtBackup").val().trim();


    let _error_name = $("#error_txtBackup");


    // var TestEmail = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    //var phone_test = /^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$/im;



    if (validate_IsEmptyStringInputFieldValue(_name)) {
        is_valid = false;
        _error_name.html('Enter Backup Name!');
    }


    if (is_valid) {
        var data = new FormData();
        data.append("Backup", _name);



        $.ajax({
            url: '/Business/Backup',
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
    $('#btnAddStudent').click(function () {
        $('#sectionAddStaff').show();
        $('#btnAddStudent').hide();
        $('#sectionViewStaff').hide();
        document.getElementById("pageTextchange").innerHTML = "Add Backup";
        document.getElementById("pageStageChange").innerHTML = "Add Message";

    });
});*/


function EditBackup() {
    $('#sectionAddStaff').show();
    $('#sectionViewStaff').hide();
    document.getElementById("myText").innerHTML = "Edit Backup";
    document.getElementById("ChangeUpdateText").innerHTML = "Update";
    document.getElementById("pageTextchange").innerHTML = "Edit Backup";
    document.getElementById("pageStageChange").innerHTML = "Edit Backup";
    $('#ChangeUpdateText').show();
    $('#btnAddStudent').hide();
    $(".error-class").html('');
    $('#btnAddStudent').click(function () {
        document.getElementById("myText").innerHTML = "Add Backup";
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

    $("#txtLeaveMessage").val('');
    $('#sectionViewStaff').show();
    $('#sectionAddStaff').hide();
    $('#btnAddStudent').show();
    document.getElementById("pageTextchange").innerHTML = "Message";
    document.getElementById("pageStageChange").innerHTML = "Message";
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


function getBusinessBackup() {
    //$('#dnwBackup').prop('disabled', true);
    if (global_AdminUserId !== undefined && global_AdminUserId !== null && !isNaN(global_AdminUserId)) {
        window.location.href = "/Business/BackupDatabase?userLoginId=" + encodeURIComponent(global_AdminUserId);
    } else {
        $.iaoAlert({
            msg: '@(Resources.ErrorMessage.TechincalErrorMessage)',
            type: "error",
            mode: "dark",
        });
    }
    //$('#dnwBackup').prop('disabled', false);
}
function GetAllMasterIdDetails() {

    var _url = "/api/Business/GetAllMasterIdDetail";

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
            global_AdminUserId = response.data.BusinessOwnerLoginId;
            StopLoading();
        },
        error: function (result) {
            StopLoading();

            if (result["status"] == 401) {
                $.iaoAlert({
                    msg: '@(Resources.ErrorMessage.UnAuthorizedErrorMessage)',
                    type: "error",
                    mode: "dark",
                });
            }
            else {
                $.iaoAlert({
                    msg: '@(Resources.ErrorMessage.TechincalErrorMessage)',
                    type: "error",
                    mode: "dark",
                });
            }
        }
    });
}