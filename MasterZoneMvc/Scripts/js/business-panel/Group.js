var GroupId_Global = 0;
var UserToken_Global = "";
var _groupsTable;
var GroupDetail_Global = {};

function initializeDataTable_Groups() {
    _groupsTable = $("#tblGroups").DataTable();
}

$(document).ready(function () {
    $.get("/Business/GetBusinessAdminToken", null, function (dataToken) {
        if (dataToken != "" && dataToken != null) {
            UserToken_Global = dataToken;
            initializeDataTable_Groups();
            GetAllBusinessGroups();

            //StopLoading();
        }
        else {
            $.get("/Staff/GetStaffToken", null, function (dataToken) {
                if (dataToken != "" && dataToken != null) {
                    UserToken_Global = dataToken;
                    initializeDataTable_Groups();
                    GetAllBusinessGroups();

                    //StopLoading();
                }
                else {
                    StopLoading();
                }
            });
        }
    });

    $('#btnAddGroup').click(function () {
        $('#sectionAddGroup').show();
        $('#btnAddGroup').hide();
        $('#sectionViewGroup').hide();
        document.getElementById("pageTextchange").innerHTML = "Add Group";
        document.getElementById("pageStageChange").innerHTML = "Add Group";
    });

    $('#addGroupMemberModal').on('hide.bs.modal', function () {
        ResetAddGroupMemberModal();
    });
    
    $('#viewGroupDetialsModal').on('hide.bs.modal', function () {
        ResetViewGroupDetialModal();
    });

    $('#txtSearchBusinessStudents_Modal').on('keyup', function (e) {
        var searchKeywords = $(this).val();

        //if (e.which < 65 || e.which > 90) //65=a, 90=z
        //    return false;
        if ((e.which != 8 || e.which != 13) && searchKeywords.length == 0) {
            getAllBusinessStudentsLists();
        }
        else if (searchKeywords.length >= 3) {
            searchBusinessStudentsLists(searchKeywords);
        }
    });
});

function ResetAddGroupMemberModal() {
    GroupId_Global = 0;
    $('#txtSearchBusinessStudents_Modal').val('');
    $('#businessStudentsList_Modal').html('');
}

function ResetViewGroupDetialModal() {
    GroupId_Global = 0;
    $('#groupDetails_GroupDetailModal').html('');
}

function addGroupMembers(groupId) {
    GroupId_Global = groupId;
    getGroupDetailWithMembers(getAllBusinessStudentsLists);
}

function getGroupDetailWithMembers(callbackFunction) {
    let _url = "/api/Group/GetGroupDetialsWithMembers?id="+GroupId_Global;
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

function getAllBusinessStudentsLists() {
    let _url = API_Base_URL + "/api/Business/GetAllStudents";
    //StartLoading();
    showSpinnerInAddMemberModal();
    $('#addGroupMemberModal').modal('show');
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
            bindAddMemberListData(response.data);

            //StopLoading();
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

function searchBusinessStudentsLists(searchKeywords) {
    let _url = API_Base_URL + "/api/Business/SearchStudents?searchKeywords=" + searchKeywords;
    //StartLoading();
    showSpinnerInAddMemberModal();

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
            
            bindAddMemberListData(response.data);

            StopLoading();
        },
        error: function (result) {
            $('#businessStudentsList_Modal').html('');
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

function showSpinnerInAddMemberModal() {
    // show spinner 
    $('#businessStudentsList_Modal').html(`<div class="w-100 text-center"><div class="spinner-border" role="status">
                        <span class="sr-only">Loading...</span>
                    </div></div>`);
}

function bindAddMemberListData(data) {
    var _studentList = '';
    for (var i = 0; i < data.length; i++) {
        var _studentName = data[i].FirstName + ' ' + data[i].LastName;

        _addButton = ``;

        if (GroupDetail_Global.GroupStudents.filter(s => s.StudentUserLoginId == data[i].StudentUserLoginId).length <= 0) {
            _addButton = `
                    <button class="btn btn-sm btn-primary" id="btnAddMember${data[i].StudentUserLoginId}" onclick="AddMemberInGroup(${data[i].StudentUserLoginId})"><i class="fas fa-plus"></i> Add</button>
                `;
        }
        _studentList += `
                <div class="d-flex align-items-center">
                    <div><img src="${data[i].ProfileImageWithPath}" class="profile-img" /></div>
                    <div class="flex-grow-1 p-3 font-weight-bold">${_studentName}</div>
                    <div>
                        ${_addButton}
                    </div>
                </div>`;
    }
    if (data.length <= 0) {
        _studentList = '<div class="w-100 text-center font-italic">No Records Found!</div>';
    }
    $("#businessStudentsList_Modal").html('').append(_studentList);
}

function GetAllBusinessGroups() {
    // ---------------- Pagination Data Table --------------------
    var _url_val = "/api/Group/ByBusiness/GetAllByPagination";
    _groupsTable.clear().destroy();
    _groupsTable = $("#tblGroups").DataTable({
        "processing": true,
        "serverSide": true,
        "filter": true,
        "orderMulti": false,
        "ordering": true,
        "paginate": true,
        "order": [[4, "desc"]],
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
            , {
                "data": "GroupImage", "name": "GroupImage", "autoWidth": true
                , "render": function (data, type, row) {
                    return `<img src="${row.GroupImageWithPath}" class="image" />`
                }
            }
            , { "data": "Name", "name": "Name", "autoWidth": true }
            , { "data": "Description", "name": "Description", "autoWidth": true }
            , {
                "data": "CreatedOn", "name": "CreatedOn", "autoWidth": true
                , "render": function (data, type, row) {
                    return row.CreatedOn_FormatDate; //`<span style="display:none;">${moment(row.CreatedOn_FormatDate).format('YYYY-MM-DD')}</span> ${row.CreatedOn_FormatDate}`;
                }
            }
            , {
                "data": null, "": "Action", "autoWidth": true,
                "render": function (data, type, row) {
                    
                    var _view = `<a href="javascript:viewGroupDetail(${row.Id});"><i class="fas fa-eye" title="View Group Details"></i></a>`;
                    var _edit = `<a href="javascript:EditGroupById(${row.Id});"><i class="fas fa-edit" title="Edit Group"></i></a>`;
                    var _addMembers = `<a href="javascript:addGroupMembers(${row.Id});"><i class="fas fa-plus" title="Add Members" ></i></a>`;
                    var _delete = `<a href="javascript:ConfirmDeleteGroup(${row.Id});"><i class="fas fa-trash" title="Delete Group"></i></a>`;

                    var _action = `<div class="edbt">
                                ${_addMembers}
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
            "targets": [0,5], // column index (start from 0)
            "orderable": false, // set orderable false for selected columns
        }]
    });

    // for enabling search box only send requet on pressing enter
    $('#tblGroups_filter input').unbind();
    $('#tblGroups_filter input').bind('keyup', function (e) {
        if (e.keyCode == 13 || (e.keyCode == 8 && $(this).val() == '')) {
            _groupsTable.search(this.value).draw();
        }
    });

    // ----------------  Pagination Data Table ------------------
}

function btnSubmitGroupForm() {
    
    let is_valid = true;
    $(".error-class").html('');
    var _mode = (GroupId_Global <= 0) ? 1 : 2;

    let _name = $("#txtGroupName").val().trim();
    let _descriptionMessage = $("#txtDescriptionMessage").val().trim();

    let _error_name = $("#error_txtGroupName");
    let _error_descriptionMessage = $("#error_txtDescriptionMessage");

    if (validate_IsEmptyStringInputFieldValue(_name)) {
        is_valid = false;
        _error_name.html('Enter Group Name!');
    }
    if (validate_IsEmptyStringInputFieldValue(_descriptionMessage)) {
        is_valid = false;
        _error_descriptionMessage.html('Enter description !');
    }

    if (is_valid) {
        var data = new FormData();
        data.append("Id", GroupId_Global);
        data.append("Name", _name);
        data.append("Description", _descriptionMessage);
        data.append("Mode", _mode);

        var _groupImageFile_MS = $("#fileGroupImage_ManageGroup").get(0);
        var _groupImageFiles = _groupImageFile_MS.files;
        data.append('GroupImage', _groupImageFiles[0]);

        $.ajax({
            url: '/api/Group/AddUpdate',
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
                StopLoading();

                //--Parse into Json of response-json-string
                dataResponse = JSON.parse(dataResponse);

                //--If successfully added/updated
                if (dataResponse.status === 1) {
                    swal("Success!", dataResponse.message, "success");
                    ResetAddUpdateForm();
                    GetAllBusinessGroups();
                }
                else {
                    swal({
                        title: 'Error!',
                        icon: 'error',
                        text: dataResponse.message
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
}

function EditGroupView() {
    $('#sectionAddGroup').show();
    $('#sectionViewGroup').hide();
    document.getElementById("myText").innerHTML = "Edit Group";
    document.getElementById("ChangeUpdateText").innerHTML = "Update";
    document.getElementById("pageTextchange").innerHTML = "Edit Group";
    document.getElementById("pageStageChange").innerHTML = "Edit Group";
    $('#ChangeUpdateText').show();
    $('#btnAddGroup').hide();
    $(".error-class").html('');
};

function ResetAddUpdateForm() {
    $("#txtGroupName").val('');
    $("#txtDescriptionMessage").val('');
    $('#sectionViewGroup').show();
    $('#sectionAddGroup').hide();
    $('#btnAddGroup').show();
    document.getElementById("pageTextchange").innerHTML = "Group";
    document.getElementById("pageStageChange").innerHTML = "Group";
    $(".error-class").html('');
    GroupId_Global = 0;
}

function EditGroupById(id) {
    var _url = "/api/Group/GetById?id=" + id;

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

            $('.error-class').html('');

            var item = response.data;

            $("#txtGroupName").val(item.Name);
            $("#txtDescriptionMessage").val(item.Description);
            $("#fileGroupImage_ManageGroup").val('');
            if (response.data.GroupImageWithPath != "") {
                $("#previewGroupImage").attr('src', response.data.GroupImageWithPath);
                $("#previewGroupImage").removeClass('d-none');
            }

            GroupId_Global = item.Id;

            EditGroupView();

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

function AddMemberInGroup(memberLoginId) {
    let _url = API_Base_URL + "/api/Group/AddMember";
    StartLoading();
    //showSpinnerInAddMemberModal();
    //$('#addGroupMemberModal').modal('show');
    data = { GroupId: GroupId_Global, MemberLoginId: memberLoginId };
    $.ajax({
        type: "POST",
        url: _url,
        data: JSON.stringify(data),
        headers: {
            "Authorization": "Bearer " + UserToken_Global,
            "Content-Type": "application/json"
        },
        contentType: 'application/json',
        success: function (dataResponse) {
            if (dataResponse.status < 1) {
                $.iaoAlert({
                    msg: dataResponse.message,
                    type: "error",
                    mode: "dark",
                });
                return;
            }

            //if (response.status == 1) {
            swal("Success!", dataResponse.message, "success");
            $('#btnAddMember' + memberLoginId).remove();
            //}

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

function RemoveMemberFromGroup(memberLoginId) {
    let _url = API_Base_URL + "/api/Group/RemoveMember";
    StartLoading();
    //showSpinnerInAddMemberModal();
    //$('#addGroupMemberModal').modal('show');
    data = { GroupId: GroupId_Global, MemberLoginId: memberLoginId };
    $.ajax({
        type: "POST",
        url: _url,
        data: JSON.stringify(data),
        headers: {
            "Authorization": "Bearer " + UserToken_Global,
            "Content-Type": "application/json"
        },
        contentType: 'application/json',
        success: function (dataResponse) {
            if (dataResponse.status < 1) {
                $.iaoAlert({
                    msg: response.message,
                    type: "error",
                    mode: "dark",
                });
                return;
            }

            //if (response.status == 1) {
            swal("Success!", dataResponse.message, "success");
            getGroupDetailWithMembers(callbackGroupDetailDataModal);
            //}

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

function viewGroupDetail(groupId) {
    GroupId_Global = groupId;
    $('#groupDetails_GroupDetailModal').html('');
    getGroupDetailWithMembers(callbackGroupDetailDataModal);
}

function callbackGroupDetailDataModal(apiResponse) {
    GroupId_Global = apiResponse.data.Id;
    var _htmlData = ``;
    _htmlData += `
        <div class="col-sm-12">
            <div class="d-flex mb-1">
                <img src="${apiResponse.data.GroupImageWithPath}" class="profile-img mr-3 border" />
                <div>
                    <h4>${apiResponse.data.Name}</h4>
                    <p>${apiResponse.data.Description}</p>
                </div>
            </div>
        </div>
        <hr/>
        <div class="col-sm-12">
        `;

    for (var i = 0; i < apiResponse.data.GroupStudents.length; i++) {
        var student = apiResponse.data.GroupStudents[i];
        var _studentName = student.FirstName + ' ' + student.LastName;

        _htmlData += `
            <div class="d-flex align-items-center">
                <div><img src="${student.ProfileImageWithPath}" class="profile-img" /></div>
                <div class="flex-grow-1 p-3 font-weight-bold">${_studentName}</div>
                <div>
                    <button class="btn btn-sm btn-danger" onclick="RemoveMemberFromGroup(${student.StudentUserLoginId})"><i class="fas fa-trash"></i> Remove</button>
                </div>
            </div>
        `;
    }
    if (apiResponse.data.GroupStudents.length <= 0) {
        _htmlData += `<div class="w-100 font-italic text-center text-black-50 py-2">No members in this group.</div>`;
    }
    _htmlData += `</div>`;
    $('#groupDetails_GroupDetailModal').html(_htmlData);
    $('#viewGroupDetialsModal').modal('show');
}


function ConfirmDeleteGroup(id) {
    swal({
        title: "Delete Group",
        text: "Are you sure to delete this Group",
        type: "warning",
        buttons: {
            cancel: true,
            confirm: "Yes",
        }
    })
        .then((willDelete) => {
            if (willDelete) {
                DeleteGroup(id);
            } else {
                //swal("Your imaginary file is safe!");
            }
        });
}

function DeleteGroup(id) {
    StartLoading();
    $.ajax({
        type: "POST",
        url: "/api/Group/Delete?id=" + id,
        headers: {
            "Authorization": "Bearer " + UserToken_Global,
            "Content-Type": "application/json"
        },
        contentType: 'application/json',
        success: function (dataResponse) {
            StopLoading();

            //--Check if group successfully deleted
            if (dataResponse.status == 1) {
                setTimeout(function () {
                    swal("Success!", dataResponse.message, "success");
                    //--Get group List
                    GetAllBusinessGroups();
                }, 100);
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

// Image File Preview -------------------------------------------------------------
document.getElementById('fileGroupImage_ManageGroup').addEventListener('change', handleImageUpload);

function handleImageUpload(event) {
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
        $('#previewGroupImage').addClass('d-none'); // hide the preview image
        return;
    }
    if (fileSize > maxSize) {
        $.iaoAlert({
            msg: 'Image size too large. Please select a smaller image(< 10 MB).',
            type: "error",
            mode: "dark",
        });
        event.target.value = null; // clear the file input element
        $('#previewGroupImage').addClass('d-none'); // hide the preview image

        return;
    }

    // image size is within the limit, display the preview image
    const reader = new FileReader();
    reader.onload = function (event) {
        document.getElementById('previewGroupImage').src = event.target.result;
        $('#previewGroupImage').removeClass('d-none');
    }

    reader.readAsDataURL(file);
}
// Image File Preview -------------------------------------------------------------


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
