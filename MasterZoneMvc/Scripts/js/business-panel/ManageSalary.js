var StaffId_Global = 0;
var UserToken_Global = "";

$(document).ready(function () {
    $.get("/Business/GetBusinessAdminToken", null, function (dataToken) {
        if (dataToken != "" && dataToken != null) {
            UserToken_Global = dataToken;
            GetAllStaff();
            //StopLoading();
        }
        else {

        }
    });
});

function GetAllStaff() {
    var _url = "/api/Staff/GetAll";

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

            var _table = $('#tblStaff_ManageSalary').DataTable();
            _table.destroy();

            var sno = 0;
            var _status = '';
            var _action = '';

            var data = [];
            for (var i = 0; i < response.data.length; i++) {
                sno++;
                _action = '<div class="edbt">';
                _action += '<a class="btn btn-info btn-sm" href="javascript:EditSalary(' + response.data[i].Id + ');"><i class="fas fa-edit"></i> Update Salary</a>';
                _action += '</div>';

                data.push([
                    sno,
                    response.data[i].StaffCategoryName,
                    response.data[i].FirstName + " " + response.data[i].LastName,
                    response.data[i].MonthlySalary,
                    _action
                ]);
            }

            $('#tblStaff_ManageSalary').DataTable({
                "data": data,
                "paging": true,
                "lengthChange": true,
                "searching": true,
                "ordering": true,
                "info": true,
                "autoWidth": false,
                "responsive": true,
            });
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

function GetStaffById(id) {
    var _url = "/api/Staff/GetById/" + id;
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

            $("#txtStaff_SalaryModal").val(response.data.FirstName + ' ' + response.data.LastName);
            $("#txtSalary_SalaryModal").val(response.data.MonthlySalary);
            StaffId_Global = response.data.Id;

            openEditSalaryModal();
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

function btnUpdate_SalaryModal() {
    let is_valid = true;
    $(".error-SalaryModal").html('');

    let _monthlyincome = $("#txtSalary_SalaryModal").val().trim();

    let _error_monthlyincome = $("#error_txtSalary_SalaryModal");

    if (validate_IsEmptyStringInputFieldValue(_monthlyincome)) {
        is_valid = false;
        _error_monthlyincome.html('Enter Salary!');
    }
    else if (isNaN(_monthlyincome) || parseInt(_monthlyincome) < 0) {
        is_valid = false;
        _error_monthlyincome.html('Salary must be greater than 0');
    }

    if (is_valid) {
        var data = new FormData();
        data.append("Id", StaffId_Global);
        data.append("Salary", _monthlyincome);

        $.ajax({
            url: '/api/Staff/UpdateSalary',
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

                closeEditSalaryModal();
                GetAllStaff();
                //StopLoading();
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

function EditSalary(id) {
    ResetSalaryModal();
    GetStaffById(id);
}

function ResetSalaryModal() {
    $(".error-SalaryModal").html('');

    $("#txtStaff_SalaryModal").val('');
    $("#txtSalary_SalaryModal").val('');
    StaffId_Global = 0;
}

function openEditSalaryModal() {
    $('#btnOpenSalaryModal').click();
}

function closeEditSalaryModal() {
    $('#btnCancel_SalaryModal').click();
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