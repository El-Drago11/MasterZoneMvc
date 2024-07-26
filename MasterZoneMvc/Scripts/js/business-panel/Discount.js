var UserToken_Global = "";
var DiscountId_Global = 0;
var _discountTable;

function initializeDataTable_Discount() {
    _discountTable = $("#ViewForDiscount").DataTable();
}

$(document).ready(function () {
    $.get("/Business/GetBusinessAdminToken", null, function (dataToken) {
        if (dataToken != "" && dataToken != null) {
            UserToken_Global = dataToken;
            // getAllDiscountList();
            getAllActiveStudentsLists();
            initializeDataTable_Discount();

            GetAllCreatedDiscounts();
            StopLoading();
        }

        else {
            $.get("/Staff/GetStaffToken", null, function (dataToken) {
                if (dataToken != "" && dataToken != null) {
                    UserToken_Global = dataToken;
                    initializeDataTable_Discount();

                    getAllActiveStudentsLists();
                    GetAllCreatedDiscounts();
                    //StopLoading();
                }
                else {
                    StopLoading();
                }
            });
        }
    });
});

$('#viewDiscountDetialsModal').on('hide.bs.modal', function () {
    ResetViewDiscountDetialModal();
});


function ResetViewDiscountDetialModal() {
    DiscountId_Global = 0;
    $('#discountDetails_DiscountDetailModal').html('');
}

function viewgroupdetail(Id) {
    DiscountId_Global = Id;
    $('#discountDetails_DiscountDetailModal').html('');
    getDiscountDetail(callbackDiscountDetailDataModal);
}
function callbackDiscountDetailDataModal(apiResponse) {
    DiscountId_Global = apiResponse.data.Id;
    var _htmlData = ``;
    _htmlData += `
        <div class="col-sm-12">
            
                    <h4>${apiResponse.data.Name}</h4>
                    <p>${apiResponse.data.Description}</p>
                </div>
            </div>
        </div>
        <hr/>
        <div class="col-sm-12">
        `;

    for (var i = 0; i < apiResponse.data.DiscountStudent.length; i++) {
        var student = apiResponse.data.DiscountStudent[i];
        var _studentName = student.FirstName + ' ' + student.LastName;

        _htmlData += `
            <div class="d-flex align-items-center">
                <div><img src="${student.ProfileImageWithPath}" class="profile-img" /></div>
                <div class="flex-grow-1 p-3 font-weight-bold">${_studentName}</div>
               
            </div>
        `;
    }
    if (apiResponse.data.DiscountStudent.length <= 0) {
        _htmlData += `<div class="w-100 font-italic text-center text-black-50 py-2">No members in this group.</div>`;
    }
    _htmlData += `</div>`;
    $('#discountDetails_DiscountDetailModal').html(_htmlData);
    $('#viewDiscountDetialsModal').modal('show');
}

function getDiscountDetail(callbackFunction) {
    let _url = "/api/Discount/GetDiscountDetialsWith?id=" + DiscountId_Global;
    StartLoading();
    //showSpinnerInAddMemberModal();
    //$('#addGroupMemberModal').modal('show');
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

            GroupDetail_Global = response.data;

            callbackFunction(response);

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

function GetAllCreatedDiscounts() {
    // ---------------- Pagination Data Table --------------------
    var _url_val = "/api/Discount/ByBusiness/GetAllByPagination";
    _discountTable.clear().destroy();
    _discountTable = $("#ViewForDiscount").DataTable({
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
                "data": "SerialNumber", "name": "CreatedOn", "autoWidth": true
                //,"render": function (data, type, row) {
                //    return '';
                //}
            }
            , { "data": "Name", "name": "Name", "autoWidth": true }
            , { "data": "Description", "name": "Description", "autoWidth": true }
            , { "data": "Code", "name": "Code", "autoWidth": true }
            , {
                "data": "StartDate", "name": "StartDate", "autoWidth": true
            }

            , {
                "data": "EndDate", "name": "EndDate", "autoWidth": true
            }
            , {
                "data": "IsFixedAmount", "name": "IsFixedAmount", "autoWidth": true
                , "render": function (data, type, row) {
                    return `${row.AmountTypeName}`;
                }
            }

            , { "data": "DiscountValue", "name": "DiscountValue", "autoWidth": true }

            , {
                "data": "DiscountFor", "name": "DiscountFor", "autoWidth": true
                , "render": function (data, type, row) {
                    return `${row.DiscountForName}`;
                }
            }
            , { "data": "TotalUsed", "name": "TotalUsed", "autoWidth": true }
            , {
                "data": "CreatedOn", "name": "CreatedOn", "autoWidth": true
                , "render": function (data, type, row) {
                    return `<span style="display:none;">${moment(row.CreatedOn_FormatDate).format('YYYY-MM-DD')}</span> ${row.CreatedOn_FormatDate}`;
                }
            }

            , {
                "data": null, "": "action", "autowidth": true,
                "render": function (data, type, row) {

                    var _view = `<a href="javascript:viewgroupdetail(${row.Id});"><i class="fas fa-eye" title="view group details"></i></a>`;
                    var _edit = `<a href="javascript:EditDiscount(${row.Id});"><i class="fas fa-edit" title="edit group"></i></a>`;

                    var _delete = `<a href="javascript:confirmdeletediscount(${row.Id});"><i class="fas fa-trash" title="delete group"></i></a>`;

                    var _action = `<div class="edbt">
                               
                                ${_view}
                                ${_edit}
                                ${_delete}
                            </div>`;
                    return _action;
                }
            }


        ],

        "responsive": true,
        "autoWidth": false,
        //"dom": "<'row my-3'<'col-sm-12'B>><'row'<'col-sm-6'l><'col-sm-6'f>><'row'<'col-sm-12'tr>><'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",
        "columnDefs": [{
            "targets": [0, 11], // column index (start from 0)
            "orderable": false, // set orderable false for selected columns
        }]
    });

    // for enabling search box only send requet on pressing enter
    $('#ViewForDiscount_filter input').unbind();
    $('#ViewForDiscount_filter input').bind('keyup', function (e) {
        if (e.keyCode == 13 || (e.keyCode == 8 && $(this).val() == '')) {
            _discountTable.search(this.value).draw();
        }
    });

    // ----------------  Pagination Data Table ------------------
}





function getAllActiveStudentsLists() {
    let _url = "/api/Business/GetAllStudents";

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
            //'<option value="' + response.data[i].StudentUserLoginId + _studentName +'">' + _studentName + '</option>';
            var _selectList = '';
            $("#ddlSelectedStudentsForDiscount").html('');
            for (var i = 0; i < response.data.length; i++) {
                var _studentName = '(' + response.data[i].StudentId + ')' + response.data[i].FirstName + ' ' + response.data[i].LastName;
                _selectList += '<option value="' + response.data[i].StudentUserLoginId + '">' + _studentName + '</option>';
            }
            $("#ddlSelectedStudentsForDiscount").html('').append(_selectList); //.select2();

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

function btnSubmitToDiscount() {
    let is_valid = true;
    $(".error-class").html('');
    var _mode = (DiscountId_Global > 0) ? 2 : 1;


    let _name = $("#txtName").val().trim();
    let _description = $("#txtDescription").val().trim();
    let _startdate = $("#txtStartDate").val().trim();
    let _enddate = $("#txtEndDate").val().trim();
    let _IsFixedamount = $("#ddlDiscount_ManageDiscount").val();
    let _discountvalue = $("#txtDiscountValue").val().trim();
    let _selecteddiscount = $("#ddlDiscount_SelectedDiscount").val().trim();
    let _selecteddiscountforstudent = $("#ddlSelectedStudentsForDiscount").val();


    let _error_name = $("#error_txtName");
    let _error_description = $("#error_txtDescription");
    let _error_startdate = $("#error_txtStartDate");
    let _error_enddate = $("#error_txtEndDate");
    let _error_IsFixedamount = $("#error_ddlDiscount_ManageDiscount");
    let _error_discountvalue = $("#error_txtDiscountValue");
    let _error_selecteddiscount = $("#error_ddlDiscount_SelectedDiscount");
    let _error_selecteddiscountforstudent = $("#error_ddlSelectedStudentsForDiscount");


    // var TestEmail = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    //var phone_test = /^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$/im;



    if (validate_IsEmptyStringInputFieldValue(_name)) {
        is_valid = false;
        _error_name.html('Enter  Name!');
    }
    if (validate_IsEmptyStringInputFieldValue(_description)) {
        is_valid = false;
        _error_description.html('Enter description!');
    }
    if (validate_IsEmptyStringInputFieldValue(_startdate)) {
        is_valid = false;
        _error_startdate.html('Enter Start Date!');
    }


    if (validate_IsEmptyStringInputFieldValue(_enddate)) {
        is_valid = false;
        _error_enddate.html('Enter End Date!');
    }
    if (validate_IsEmptySelectInputFieldValue(_IsFixedamount)) {
        is_valid = false;
        _error_IsFixedamount.html('Enter Select IsFixedAmount!');
    }


    if (validate_IsEmptyStringInputFieldValue(_discountvalue)) {
        is_valid = false;
        _error_discountvalue.html('Enter Discount Value!');
    }
    if (validate_IsEmptySelectInputFieldValue(_selecteddiscount)) {
        is_valid = false;
        _error_selecteddiscount.html('Enter Discount Value!');
    }

    if (_selecteddiscountforstudent.length <= 0) {
        _error_selecteddiscountforstudent.html('Please select the Selected Students!');
    }

    if (is_valid) {
        var data = new FormData();
        _selecteddiscountforstudent = _selecteddiscountforstudent.join(',');
        data.append("Id", DiscountId_Global);
        data.append("Name", _name);
        data.append("Description", _description);
        data.append("StartDate", _startdate);
        data.append("EndDate", _enddate);
        data.append("IsFixedAmount", _IsFixedamount);
        data.append("DiscountValue", _discountvalue);
        data.append("DiscountFor", _selecteddiscount);
        data.append("SelectedStudent", _selecteddiscountforstudent);
        data.append('Mode', _mode);


        $.ajax({
            url: '/api/BusinessAdmin/AddUpdateDiscountDetail',
            headers: {
                "Authorization": "Bearer " + UserToken_Global,

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
                    ResetAddView();
                    GetAllCreatedDiscounts();
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

            },
            error: function (result) {
                //StopLoading();


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
        $('#btnAddStaff').hide();
        $('#sectionViewStaff').hide();
        getAllActiveStudentsLists();
        document.getElementById("pageTextchange").innerHTML = "Add Discount";
        document.getElementById("pageStageChange").innerHTML = "Add Discount";

    });
});


function EditDiscount(Id) {

    $('#sectionAddStaff').show();
    $('#sectionViewStaff').hide();
    document.getElementById("myText").innerHTML = "Edit Discount";
    document.getElementById("ChangeUpdateText").innerHTML = "Update";
    document.getElementById("pageTextchange").innerHTML = "Edit Discount";
    document.getElementById("pageStageChange").innerHTML = "Edit Discount";
    $('#ChangeUpdateText').show();
    $('#btnAddStaff').hide();
    $(".error-class").html('');
    $('#btnAddStaff').click(function () {
        document.getElementById("myText").innerHTML = "Add Discount";
        document.getElementById("ChangeUpdateText").innerHTML = "Save";
        $(".error-class").html('');
    });
    var _url = "/api/Discount/GetById/" + Id;

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

            $("#txtName").val(response.data.Name);
            $("#txtDescription").val(response.data.Description);
            $("#txtStartDate").val(response.data.StartDate);
            $("#txtEndDate").val(response.data.EndDate);
            $("#ddlDiscount_ManageDiscount").val(response.data.IsFixedAmount);
            $("#txtDiscountValue").val(response.data.DiscountValue);
            $("#ddlDiscount_SelectedDiscount").val(response.data.DiscountFor);
            $("#ddlSelectedStudentsForDiscount").val(response.data.SelectedStudent);
            DiscountId_Global = response.data.Id;

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
};

function ShowingViewStaffList() {

    $('#sectionViewStaff').show();

}
$('#Closed').click(function () {
    $('#ddlSelectedStudentsForDiscount').val('0');
})

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
    $("#txtDescription").val('');
    $("#txtStartDate").val('');
    $("#txtEndDate").val('');
    $("#ddlDiscount_ManageDiscount").val('');
    $("#txtDiscountValue").val('');
    $("#ddlDiscount_SelectedDiscount").val('0').trigger('change');
    $("#ddlSelectedStudentsForDiscount").val('').trigger('change');
    $('#sectionViewStaff').show();
    $('#sectionAddStaff').hide();
    $('#btnAddStaff').show();
    document.getElementById("pageTextchange").innerHTML = "Discount";
    document.getElementById("pageStageChange").innerHTML = "Discount";
    $(".error-class").html('');
}

function onChangeDiscountForField() {
    if ($('#ddlDiscount_SelectedDiscount').val() == 2) {
        $("#ddlSelectedStudentsForDiscount").prop('disabled', false);
    }
    else {
        $("#ddlSelectedStudentsForDiscount").prop('disabled', true);
        $("#ddlSelectedStudentsForDiscount").val('').trigger('change');
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
function confirmdeletediscount(id) {
    swal({
        title: "Delete Discount",
        text: "Are you sure to delete this Discount",
        type: "warning",
        buttons: {
            cancel: true,
            confirm: "Yes",
        }
    })
        .then((willDelete) => {
            if (willDelete) {
                Deletediscount(id);
            } else {
                //swal("Your imaginary file is safe!");
            }
        });
}

function Deletediscount(id) {
    StartLoading();
    $.ajax({
        type: "POST",
        url: "/api/discount/DeletediscountById/" + id,
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

function GetDiscountDataById(Id) {
    $('#sectionAddStaff').show();
    var _url = "/api/Discount/GetById?id=" + Id;

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

            $("#txtName").val(response.data.Name);
            $("#txtDescription").val(response.data.Description);
            $("#txtStartDate").val(response.data.StartDate);
            $("#txtEndDate").val(response.data.EndDate);
            $("#ddlDiscount_ManageDiscount").val(response.data.IsFixedAmount);
            $("#txtDiscountValue").val(response.data.DiscountValue);
            $("#ddlDiscount_SelectedDiscount").val(response.data.DiscountFor);
            $("#ddlSelectedStudentsForDiscount").val(response.data.SelectedStudent);


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

