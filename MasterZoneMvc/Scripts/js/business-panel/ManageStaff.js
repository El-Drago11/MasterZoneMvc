var StaffId_Global = 0;
var UserToken_Global = "";
var btnUpdateCredentialValue_Global = 0;
var SelectedPermissionArr_Global = [];

$(document).ready(function () {

    $.get("/Business/GetBusinessAdminToken", null, function (dataToken) {
        if (dataToken != "" && dataToken != null) {
            UserToken_Global = dataToken;
            GetAllStaff();
            //StopLoading();
        }
        else {
            $.get("/Staff/GetStaffToken", null, function (dataToken) {
                if (dataToken != "" && dataToken != null) {
                    UserToken_Global = dataToken;
                    GetAllStaff();
                    //StopLoading();
                }
                else {
                    StopLoading();
                }
            });
        }
    });

    $('#btnAddStaff').click(function () {
        $('#sectionAddStaff').show();
        $('#btnAddStaff').hide();
        $('#sectionViewStaff').hide();
        document.getElementById("pageTextchange").innerHTML = "Add Staff";
        document.getElementById("pageStageChange").innerHTML = "Add Staff";
        $('#btnUpdateCredential').addClass('d-none');
    });

});

$(document).on('change', '.chk_permission', function () {
    var $checkbox = $(this);
    var _checkboxProp = false;
    if ($checkbox.is(':checked')) {
        //console.log('checked!');
        _checkboxProp = true;
    }
    else {
        //console.log('unchecked!');
        _checkboxProp = false;
    }
    var _parentCheckboxId = $checkbox.attr('data-parent-id');
    var _id = $checkbox.attr('data-id');

    // if this checkbox is parent checkbox then check/uncheck all sub checkboxes
    if (_parentCheckboxId == 0) {
        var subPermissionCheckboxes = getAllSubPermissionCheckboxes(_id);
        if (subPermissionCheckboxes.length > 0) {
            subPermissionCheckboxes.prop('checked', _checkboxProp);
        }
    }
    // else it is a sub-checkbox then check the parent if atleaset one is selected and uncheck if all are unselected
    else {
        var subPermissionCheckboxes = getAllSubPermissionCheckboxes(_parentCheckboxId);
        var subPermissionCheckboxes_Checked = getAllCheckedSubPermissionCheckboxes(_parentCheckboxId);
        var flag_check_parent = false;
        if (subPermissionCheckboxes_Checked.length > 0) {
            flag_check_parent = true;
        }
        else {
            flag_check_parent = false;
        }
        $('#chk_permission_'+_parentCheckboxId).prop('checked', flag_check_parent);
    }

    // update global array of selections
    onPermissionSelectionChange();
});

function btnClickUpdateCredentials() {
    btnUpdateCredentialValue_Global = (btnUpdateCredentialValue_Global == 0) ? 1 : 0;

    if (btnUpdateCredentialValue_Global == 1) {
        enableDisableUpdateCredentialFields(false);
    }
    else {
        enableDisableUpdateCredentialFields(true);
    }
}

function enableDisableUpdateCredentialFields(value) {
    $("#txtEmail").prop('disabled', value);
    $("#txtPassword").prop('disabled', value);
    $("#txtConfirmPassword").prop('disabled', value);
}

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

            var _table = $('#tblStaff_ManageStaff').DataTable();
            _table.destroy();

            var sno = 0;
            var _status = '';
            var _action = '';
            var _staffImage = '';

            var data = [];
            for (var i = 0; i < response.data.length; i++) {
                sno++;
                _staffImage = (response.data[i].ProfileImageWithPath == "") ? '' : '<img src="' + response.data[i].ProfileImageWithPath + '" class="image" />';
                _action = '<div class="edbt">';
                _action += '<a href="javascript:EditStaff(' + response.data[i].Id + ');"><i class="fas fa-edit"></i></a>';
                _action += `<a href="javascript:confrimDelete_ManageStaff(${response.data[i].Id}, '${response.data[i].ParentBusinessCategoryId}', '${response.data[i].FirstName} ${response.data[i].LastName}');"><i class="fas fa-trash "></i></a>`;
                _action += '</div>';

                //---Check Staff-Status
                if (response.data[i].Status == 1) {
                    _status = '<button class="btn btn-success btn-sm" style="width:80px;" onclick="ConfirmChangeStatusStaff(' + response.data[i].Id + ');">Active</button>';
                }
                else {
                    _status = '<button class="btn btn-danger text-white btn-sm" style="width:80px;" onclick="ConfirmChangeStatusStaff(' + response.data[i].Id + ');">Inactive</button>';
                }

                data.push([
                    sno,
                    response.data[i].StaffCategoryName,
                    _staffImage,
                    response.data[i].FirstName + " " + response.data[i].LastName,
                    _status,
                    _action
                ]);
            }

            $('#tblStaff_ManageStaff').DataTable({
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
            GetAllActiveStaffCategories();
        },
        error: function (result) {
            StopLoading();
            GetAllActiveStaffCategories();

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

function GetAllActiveStaffCategories() {
    var _url = "/api/Staff/Categories/GetAllActive";

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
                    msg: 'Unauthorized! Invalid Token!',
                    type: "error",
                    mode: "dark",
                });
                return;
            }

            // --------------------- append parent categories in dropdown
            $("#ddlStaffCategory_ManageStaff").html('');
            var res_Categories = '<option value="0">Select Category</option>';

            for (var i = 0; i < response.data.length; i++) {
                res_Categories += '<option value="' + response.data[i].Id + '">' + response.data[i].Name + '</option>'; 
            }

            $("#ddlStaffCategory_ManageStaff").html('').append(res_Categories); //.select2();
            
            StopLoading();
            GetAllBusinessPanelPermissions();
        },
        error: function (result) {
            StopLoading();
            GetAllBusinessPanelPermissions();

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

function showEditStaffPage() {
    $('#sectionAddStaff').show();
    $('#sectionViewStaff').hide();
    document.getElementById("myText").innerHTML = "Edit Staff";
    document.getElementById("ChangeUpdateText").innerHTML = "Update";
    document.getElementById("pageTextchange").innerHTML = "Edit Staff";
    document.getElementById("pageStageChange").innerHTML = "Edit Staff";
    $('#ChangeUpdateText').show();
    $('#btnAddStaff').hide();
    $(".error-class").html('');

    $('#btnUpdateCredential').removeClass('d-none');
    enableDisableUpdateCredentialFields(true);
};

function EditStaff(id) {
    var _url = "/api/Staff/GetById/"+id;

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

            $("#ddlStaffCategory_ManageStaff").val(response.data.StaffCategoryId);
            $("#txtEmail").val(response.data.Email);
            $("#txtStaffFirstName").val(response.data.FirstName);
            $("#txtStaffLastName").val(response.data.LastName);
            $("#txtPassword").val(response.data.Password);
            $("#txtConfirmPassword").val(response.data.Password);

            //---Check Staff-Status
            if (response.data.Status == 1) {
                $('#chkIsActive_ManageStaff').prop('checked', true);
            }
            else {
                $('#chkIsActive_ManageStaff').prop('checked', false);
            }

            $("#fileProfileImage_ManageStaff").val('');
            if (response.data.ProfileImageWithPath != "") {
                $("#previewImage").attr('src', response.data.ProfileImageWithPath);
                $("#previewImage").removeClass('d-none');
            }
            StaffId_Global = response.data.Id;

            resetPermissionSelectionsFromObj(response.data.Permissions);
            updateCheckboxes();

            showEditStaffPage();

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

function ShowingViewStaffList() {

    $('#sectionViewStaff').show();
}

function ResetAddView() {
    // Reset data
    $("#ddlStaffCategory_ManageStaff").val('0');
    $("#txtEmail").val('');
    $("#txtStaffFirstName").val('');
    $("#txtStaffLastName").val('');
    $("#txtPassword").val('');
    $("#txtConfirmPassword").val('');
    $('#chkIsActive_ManageStaff').prop('checked', true);
    $("#fileProfileImage_ManageStaff").val('');
    $("#previewImage").attr('src', '');
    $("#previewImage").addClass('d-none');
    StaffId_Global = 0;
    clearPermissionCheckboxes();

    enableDisableUpdateCredentialFields(false);

    // show hide panel
    $('#sectionViewStaff').show();
    $('#sectionAddStaff').hide();
    $('#btnAddStaff').show();
    document.getElementById("pageTextchange").innerHTML = "View Staff";
    document.getElementById("pageStageChange").innerHTML = "View Staff";
    $(".error-class").html('');
}

function btnAddUpdateStaff() {
    let is_valid = true;
    $(".error-class").html('');

    var _Mode = (StaffId_Global > 0) ? 2 : 1;

    let _ManageStaffCategoryId = $("#ddlStaffCategory_ManageStaff").val();
    let _email = $("#txtEmail").val().trim();
    let _staffFirstName = $("#txtStaffFirstName").val().trim();
    let _staffLastName = $("#txtStaffLastName").val().trim();
    let _password = $("#txtPassword").val().trim();
    let _confirmPassword = $("#txtConfirmPassword").val().trim();

    let _error_ManageStaffCategoryId = $("#error_ddlStaffCategory");
    let _error_email = $("#error_txtEmail");
    let _error_staffFirstName = $("#error_txtStaffFirstName");
    let _error_staffLastName = $("#error_txtStaffLastName");
    let _error_password = $("#error_txtPassowrd");
    let _error_confirmPassword = $("#error_txtConfirmPassword");

    var TestEmail = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    //var phone_test = /^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$/im;


    if (validate_IsEmptySelectInputFieldValue(_ManageStaffCategoryId) || parseInt(_ManageStaffCategoryId) < 0) {
        is_valid = false;
        _error_ManageStaffCategoryId.html('Please select a category!');
    }

    if (validate_IsEmptyStringInputFieldValue(_staffFirstName)) {
        is_valid = false;
        _error_staffFirstName.html('Enter First Name!');
    }

    if (validate_IsEmptyStringInputFieldValue(_staffLastName)) {
        is_valid = false;
        _error_staffLastName.html('Enter Last Name!');
    }

    if (validate_IsEmptyStringInputFieldValue(_password)) {
        is_valid = false;
        _error_password.html('Enter Password!');
    }
    if (validate_IsEmptyStringInputFieldValue(_confirmPassword)) {
        is_valid = false;
        _error_confirmPassword.html('Enter Confirm Password!');
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

    var _isActiveStaff = 0;
    if ($('#chkIsActive_ManageStaff').is(':checked')) {
        // checked
        _isActiveStaff = 1;
    }

    if (is_valid) {
        StartLoading();

        var data = new FormData();
        data.append("Id", StaffId_Global);
        data.append("StaffCategoryId", _ManageStaffCategoryId);
        data.append("Email", _email);
        data.append("Password", _password);
        data.append("FirstName", _staffFirstName);
        data.append("LastName", _staffLastName);
        data.append("Status", _isActiveStaff);
        data.append("Mode", _Mode);
        data.append("PermissionIds", SelectedPermissionArr_Global.join(","));

        var _staffImageFile_MS = $("#fileProfileImage_ManageStaff").get(0);
        var _staffImageFiles = _staffImageFile_MS.files;
        data.append('StaffProfileImage', _staffImageFiles[0]);

        $.ajax({
            url: '/api/Staff/AddUpdate',
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
                //ShowingViewStaffList();
                GetAllStaff();
                //removeBtnLoading(btnSelector);
                //enableContinueBtn();
            },
            error: function (result) {
                StopLoading();
                //removeBtnLoading(btnSelector);
                //enableContinueBtn();

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

function confrimDelete_ManageStaff(sid) {
    swal({
        title: "Delete Staff",
        text: "Are you sure to delete this Staff-Member?",
        type: "warning",
        buttons: {
            cancel: true,
            confirm: "Yes",
        }
    })
    .then((willDelete) => {
        if (willDelete) {
            DeleteStaff(sid);
        } else {
            //swal("Your imaginary file is safe!");
        }
    });
}

function DeleteStaff(sid) {
    StartLoading();
    $.ajax({
        type: "POST",
        url: "/api/Staff/Delete/" + sid,
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
                    GetAllStaff();
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

function ConfirmChangeStatusStaff(sid) {
    swal({
        title: "Change Status",
        text: "Are you sure to change status of this Staff-Member?",
        type: "warning",
        buttons: true,
        cancel: {
            text: "Cancel",
            value: null,
            visible: false,
            className: "",
            closeModal: true,
        }
    })
    .then((willDelete) => {
        if (willDelete) {
            ChangeStatusStaff(sid);
        } else {
            //swal("Your imaginary file is safe!");
        }
    });
}

function ChangeStatusStaff(sid) {
    StartLoading();
    $.ajax({
        type: "POST",
        url: "/api/Staff/ChangeStatus/" + sid,
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
                    GetAllStaff();
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

// ------------- PERMISSIONS FUNCTIONALITY --------------------
function GetAllBusinessPanelPermissions() {
    var _url = "/api/Permisssions/GetAll/BusinessPanel";

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
                    msg: 'Unauthorized! Invalid Token!',
                    type: "error",
                    mode: "dark",
                });
                return;
            }

            //console.log(response.data);

            $("#BusinessPanelPermissions_ManageStaff").html('');
            var checkboxesData = '';

            for (var i = 0; i < response.data.length; i++) {
                var permission = response.data[i];

                // print parent Permission
                checkboxesData += `
                    <div class="form-check">
                        <input type="checkbox" class="form-check-input chk_permission" data-id="${permission.Id}" data-parent-id="0" id="chk_permission_${permission.Id}">
                        <label class="form-check-label" for="chk_permission_${permission.Id}" style="cursor:pointer;">${permission.TextValue}</label>
                    </div>
                `;

                if (permission.SubPermissions != null && permission.SubPermissions.length > 0) {
                    for (var spi = 0; spi < permission.SubPermissions.length; spi++) {
                        var subPermission = permission.SubPermissions[spi];
                        checkboxesData += `
                            <div class="ml-4 form-check">
                                <input type="checkbox" class="form-check-input chk_permission" data-id="${subPermission.Id}" data-parent-id="${permission.Id}" id="chk_permission_${subPermission.Id}">
                                <label class="form-check-label" for="chk_permission_${subPermission.Id}" style="cursor:pointer;">${subPermission.TextValue}</label>
                            </div>
                        `;
                    }
                }

            }

            $("#BusinessPanelPermissions_ManageStaff").html('').append(checkboxesData); //.select2();

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

function clearPermissionsArray() {
    SelectedPermissionArr_Global = [];
}

function onPermissionSelectionChange() {
    clearPermissionsArray();
    // readd all checked permissions
    $(".chk_permission").each(function () {
        var self = $(this);
        if (self.is(':checked')) {
            SelectedPermissionArr_Global.push(self.attr("data-id"));
        }
    });
}


function resetPermissionSelectionsFromObj(permissions) {
    clearPermissionsArray();
    for (var i = 0; i < permissions.length; i++) {
        SelectedPermissionArr_Global.push(permissions[i].Id);
    }
}

function updateCheckboxes() {
    clearPermissionCheckboxes();
    for (var i = 0; i < SelectedPermissionArr_Global.length; i++) {
        $('.chk_permission[data-id="' + SelectedPermissionArr_Global[i] + '"]').prop('checked', true);
    }
}

function clearPermissionCheckboxes() {
    $('.chk_permission').prop('checked', false);
}

function getAllSubPermissionCheckboxes(parentId) {
    return $('.chk_permission[data-parent-id="' + parentId + '"]');
}
function getAllCheckedSubPermissionCheckboxes(parentId) {
    return $('.chk_permission[data-parent-id="' + parentId + '"]:checked');
}

// ---------------------------------------------------------------


// Image File Preview -------------------------------------------------------------
document.getElementById('fileProfileImage_ManageStaff').addEventListener('change', handleImageUpload);

function handleImageUpload(event) {
    const file = event.target.files[0];
    const fileSize = file.size / 1024; // size in kilobytes
    const maxSize = 10*1024*1024; // maximum size in kilobytes
    const fileType = file.type;
    const validImageTypes = ['image/jpeg', 'image/png'];

    if (!validImageTypes.includes(fileType)) {
        $.iaoAlert({
            msg: 'Invalid image type. Please select a JPEG, PNG image.',
            type: "error",
            mode: "dark",
        });
        event.target.value = null; // clear the file input element
        $('#previewImage').addClass('d-none'); // hide the preview image
        return;
    }
    if (fileSize > maxSize) {
        $.iaoAlert({
            msg: 'Image size too large. Please select a smaller image(< 10 MB).',
            type: "error",
            mode: "dark",
        });
        event.target.value = null; // clear the file input element
        $('#previewImage').addClass('d-none'); // hide the preview image

        return;
    }

    // image size is within the limit, display the preview image
    const reader = new FileReader();
    reader.onload = function (event) {
        document.getElementById('previewImage').src = event.target.result;
        $('#previewImage').removeClass('d-none');
    }

    reader.readAsDataURL(file);
}
// Image File Preview -------------------------------------------------------------



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
