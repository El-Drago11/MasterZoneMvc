﻿var NotificationId_Global = 0;
var UserToken_Global = "";
var _notificationsTable;

function initializeDataTable_Notification() {
    _notificationsTable = $("#tblNotifications").DataTable();
}

$(document).ready(function () {
    $.get("/Business/GetBusinessAdminToken", null, function (dataToken) {
        if (dataToken != "" && dataToken != null) {
            UserToken_Global = dataToken;
            initializeDataTable_Notification();

            getAllActiveStudentsLists();
            GetAllCreatedNotifications();
            //StopLoading();
        }
        else {
            $.get("/Staff/GetStaffToken", null, function (dataToken) {
                if (dataToken != "" && dataToken != null) {
                    UserToken_Global = dataToken;
                    initializeDataTable_Notification();

                    getAllActiveStudentsLists();
                    GetAllCreatedNotifications();
                    //StopLoading();
                }
                else {
                    StopLoading();
                }
            });
        }
    });

    $('#btnAddNotification').click(function () {
        $('#sectionAddUpdateNotification').show();
        $('#btnAddNotification').hide();
        $('#sectionViewNotification').hide();
        document.getElementById("pageTextchange").innerHTML = "Add Notification";
        document.getElementById("pageStageChange").innerHTML = "Add Notification";
    });
});

function GetAllCreatedNotifications() {
    // ---------------- Pagination Data Table --------------------
    var _url_val = "/api/Notification/GetAllByPagination";
    _notificationsTable.clear().destroy();
    _notificationsTable = $("#tblNotifications").DataTable({
        "processing": true,
        "serverSide": true,
        "filter": true,
        "orderMulti": false,
        "ordering": true,
        "paginate": true,
        "order": [[3, "desc"]],
        "ajax": {
            "headers": {
                "Authorization": "Bearer " + UserToken_Global
            },
            "url": _url_val,
            "type": "POST",
            //"data": { "_Params": JSON.stringify(_Params) },
            //"datatype": "json",
            "error": function (result) {
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
        },
        "columns": [
            {
                "data": "SerialNumber", "name": "SerialNumber", "autoWidth": true
                //,"render": function (data, type, row) {
                //    return '';
                //}
            }
            , { "data": "NotificationTitle", "name": "NotificationTitle", "autoWidth": true }
            , { "data": "NotificationText", "name": "NotificationText", "autoWidth": true }
            , {
                "data": "CreatedOn", "name": "CreatedOn", "autoWidth": true
                , "render": function (data, type, row) {
                    return `<span style="display:none;">${moment(row.CreatedOn_FormatDate).format('YYYY-MM-DD')}</span> ${row.CreatedOn_FormatDate}`;
                }
              }
            //, {
            //    "data": null, "": "Action", "autoWidth": true,
            //    "render": function (data, type, row) {
            //        //_edit = '<img src="/Content/Images/edit_icon.png" style="width:25px;height:25px;cursor:pointer;" title="Edit Expenditure-Information" onclick="EditExpenditureInfo(' + row.Id + ');" />';
                    
            //        _action = `<div class="edbt">
            //                    <a href="javascript:GetNotificationRecordDetailById(${row.Id});"><i class="fas fa-edit"></i></a>
            //                    <a href="javascript:ConfirmDeleteNotification(${row.Id});"><i class="fas fa-trash "></i></a>
            //                </div>`;
            //        return _action;
            //    }
            //  }
        ],
        //"fnRowCallback": function (nRow, aData, iDisplayIndex) {
        //    var info = _expenditureTable.page.info();
        //    $("td:first", nRow).html(info.start + iDisplayIndex + 1);
        //    return nRow;
        //},
        "responsive": true,
        "autoWidth": false,
        //"dom": "<'row my-3'<'col-sm-12'B>><'row'<'col-sm-6'l><'col-sm-6'f>><'row'<'col-sm-12'tr>><'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",
        "columnDefs": [{
            "targets": [0], // column index (start from 0)
            "orderable": false, // set orderable false for selected columns
        }]
    });

    // for enabling search box only send requet on pressing enter
    $('#tblNotifications_filter input').unbind();
    $('#tblNotifications_filter input').bind('keyup', function (e) {
        if (e.keyCode == 13 || (e.keyCode == 8 && $(this).val() == '')) {
            _notificationsTable.search(this.value).draw();
        }
    });

    // ----------------  Pagination Data Table ------------------
}


function getAllActiveStudentsLists() {
    let _url = API_Base_URL + "/api/Business/GetAllStudents";

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

            var _selectList = '';
            $("#ddlSelectedUsers").html('');
            for (var i = 0; i < response.data.length; i++) {
                var _studentName = '(' + response.data[i].Id + ')' + response.data[i].FirstName + ' ' + response.data[i].LastName;
                _selectList += '<option value="' + response.data[i].UserLoginId + '">' + _studentName + '</option>';
            }
            $("#ddlSelectedUsers").html('').append(_selectList); //.select2();

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

function btnSubmitFormClick() {
    let is_valid = true;
    $(".error-class").html('');

    var _mode = (NotificationId_Global <= 0) ? 1 : 2;

    let _notificationUsers = $("#ddlSelectedUsers").val();
    let _error_notificationUsers = $("#error_ddlSelectedUsers");
    let _notificationTitle = $("#txtNotificationTitle").val().trim();
    let _error_notificationTitle = $("#error_txtNotificationTitle");
    let _notificationMessage = $("#txtNotificationMessage").val().trim();
    let _error_notificationMessage = $("#error_txtNotificationMessage");

    if (_notificationUsers.length <= 0) {
        _error_notificationUsers.html('Please select the users!');
    }
    if (validate_IsEmptyStringInputFieldValue(_notificationTitle)) {
        is_valid = false;
        _error_notificationTitle.html('Enter Notification Title!');
    }
    if (validate_IsEmptyStringInputFieldValue(_notificationMessage)) {
        is_valid = false;
        _error_notificationMessage.html('Enter Notification Message!');
    }

    if (is_valid) {

        var data = new FormData();
        _notificationUsers = _notificationUsers.join(',');
        data.append("Id", NotificationId_Global);
        data.append("NotificationUsersList", _notificationUsers);
        data.append("NotificationTitle", _notificationTitle);
        data.append("NotificationText", _notificationMessage);
        data.append("Mode", _mode);

        $.ajax({
            url: '/api/Notification/AddUpdate',
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

                ResetAddView();
                StopLoading();
                GetAllCreatedNotifications();
            },
            error: function (result) {
                StopLoading();
                GetAllCreatedNotifications();

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

function EditNotification() {
    $('#sectionAddUpdateNotification').show();
    $('#sectionViewNotification').hide();
    document.getElementById("myText").innerHTML = "Edit Notification";
    document.getElementById("ChangeUpdateText").innerHTML = "Update";
    document.getElementById("pageTextchange").innerHTML = "Edit Notification";
    document.getElementById("pageStageChange").innerHTML = "Edit Notification";
    $('#ChangeUpdateText').show();
    $('#btnAddNotification').hide();
    $(".error-class").html('');
};

function ShowingViewNotificationList() {
    $('#sectionViewNotification').show();
}

function ResetAddView() {
    $(".error-class").html('');

    $("#ddlSelectedUsers").val('').trigger('change');
    $("#txtNotificationTitle").val('');
    $("#txtNotificationMessage").val('');

    NotificationId_Global = 0;

    $('#sectionViewNotification').show();
    $('#sectionAddUpdateNotification').hide();
    $('#btnAddNotification').show();
    document.getElementById("pageTextchange").innerHTML = "Notification";
    document.getElementById("pageStageChange").innerHTML = "Notification";

}


function GetNotificationRecordDetailById(id) {
    var _url = "/api/Notification/GetById/" + id;

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

            $('.error-class').html('');

            var item = response.data;

            $("#ddlSelectedUsers").val('').trigger('change');
            $("#ddlSelectedUsers").val(item.NotificationUserIdList).trigger('change');
            $("#txtNotificationTitle").val(item.NotificationTitle);
            $("#txtNotificationMessage").val(item.NotificationText);

            NotificationId_Global = response.data.Id;

            EditNotification();

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
function ConfirmDeleteNotification(sid) {
    swal({
        title: "Delete Notification",
        text: "Are you sure to delete this Notification?",
        type: "warning",
        buttons: {
            cancel: true,
            confirm: "Yes",
        }
    })
    .then((willDelete) => {
        if (willDelete) {
            DeleteNotification(sid);
        } else {
            //swal("Your imaginary file is safe!");
        }
    });
}

function DeleteNotification(sid) {
    StartLoading();
    $.ajax({
        type: "POST",
        url: "/api/Notification/Delete/" + sid,
        headers: {
            "Authorization": "Bearer " + UserToken_Global,
            "Content-Type": "application/json"
        },
        contentType: 'application/json',
        success: function (dataResponse) {
            StopLoading();

            //--Check if staff successfully deleted
            if (dataResponse.status == 1) {
                //setTimeout(function () {
                swal("Success!", dataResponse.message, "success");
                GetAllCreatedNotifications();
                //}, 100);
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
            GetAllCreatedNotifications();

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
