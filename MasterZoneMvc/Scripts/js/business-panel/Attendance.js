$(document).ready(function () {
    console.log(API_Base_URL);
});

function btnSubmitFormClick() {
    let is_valid = true;
    $(".error-class").html('');

    let _name = $("#txtName").val().trim();
    let _position = $("#txtPosition").val().trim();
    let _office = $("#txtOffice").val().trim();
    let _age = $("#txtAge").val().trim();



    let _error_name = $("#error_txtName");
    let _error_position = $("#error_txtPosition");
    let _error_office = $("#error_txtOffice");
    let _error_age = $("#error_txtAge");


    // var TestEmail = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    //var phone_test = /^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$/im;



    if (validate_IsEmptyStringInputFieldValue(_name)) {
        is_valid = false;
        _error_name.html('Enter  Name!');
    }
    if (validate_IsEmptyStringInputFieldValue(_office)) {
        is_valid = false;
        _error_office.html('Enter Office!');
    }
    if (validate_IsEmptyStringInputFieldValue(_position)) {
        is_valid = false;
        _error_position.html('Enter Position!');
    }


    if (validate_IsEmptyStringInputFieldValue(_age)) {
        is_valid = false;
        _error_age.html('Enter Age!');
    }


    if (is_valid) {
        var data = new FormData();
        data.append("Name", _name);
        data.append("Office", _office);
        data.append("Position", _position);
        data.append("Age", _age);


        $.ajax({
            url: '/Business/Advertisement',
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


/*
$(document).ready(function () {
    $('#btnAddStaff').click(function () {
        $('#sectionAddStaff').show();
        $('#btnAddStaff').hide();
        $('#sectionViewStaff').hide();
        document.getElementById("pageTextchange").innerHTML = "Add Advertisements";
        document.getElementById("pageStageChange").innerHTML = "Add Advertisements";

    });
});*/


function EditAttendance() {
    $('#sectionAddStaff').show();
    $('#sectionViewStaff').hide();
    document.getElementById("myText").innerHTML = "Edit Attendance";
    document.getElementById("ChangeUpdateText").innerHTML = "Update";
    document.getElementById("pageTextchange").innerHTML = "Edit Attendance";
    document.getElementById("pageStageChange").innerHTML = "Edit Attendance";
    $('#ChangeUpdateText').show();
    $('#btnAddStaff').hide();
    $(".error-class").html('');
    $('#btnAddStaff').click(function () {
        document.getElementById("myText").innerHTML = "Add Attendance";
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

    $("#txtName").val('');
    $("#txtOffice").val('');
    $("#txtPosition").val('');
    $("#txtAge").val('');
    $('#sectionViewStaff').show();
    $('#sectionAddStaff').hide();
    $('#btnAddStaff').show();
    document.getElementById("pageTextchange").innerHTML = "Advertisements";
    document.getElementById("pageStageChange").innerHTML = "Advertisements";
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