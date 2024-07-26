﻿var UserToken_Global = "";
var AdvertisementId_Global = 0;

var _advertisementsTable;

function initializeDataTable_Advertisement() {
    _advertisementsTable = $("#tblAdvertisementViewForSuperAdmin").DataTable();
}

$(document).ready(function () {
    StartLoading();
    $.get("/SuperAdmin/GetSuperAdminToken", null, function (dataAdminToken) {
        if (dataAdminToken != "" && dataAdminToken != null) {
            initializeDataTable_Advertisement();
            UserToken_Global = dataAdminToken;
            GetAllAdvertisementsPagination();
            StopLoading();
        }
        else {

            //$.get("/SubAdmin/GetSubAdminCookieDetail", null, function (dataSubAdminToken) {
            //    if (dataSubAdminToken != "" && dataSubAdminToken != null) {

            //        UserToken_Global = dataSubAdminToken;

            //    }
            //    else {

            //    }
            //});
        }
        //StopLoading();
    });
});

function GetAllAdvertisementsPagination() {
    // ---------------- Pagination Data Table --------------------
    var _url_val = "/api/Advertisement/SuperAdmin/GetAllByPagination";
    _advertisementsTable.clear().destroy();
    _advertisementsTable = $("#tblAdvertisementViewForSuperAdmin").DataTable({
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
            }
            , {
                "data": null, "": "Image", "autoWidth": true
                , "render": function (data, type, row) {
                    //---Check Advertisement-Status
                    var _Image = (row.ImageWithPath == '') ? '' : '<img src="' + row.ImageWithPath + '" class="Image" />';
                    return _Image;
                }
            }
            , { "data": "AdvertisementCategory", "name": "AdvertisementCategory", "autoWidth": true }
            , { "data": "ImageOrientationType", "name": "ImageOrientationType", "autoWidth": true }
            , {
                "data": "CreatedOn", "name": "CreatedOn", "autoWidth": true
                , "render": function (data, type, row) {
                    return `<span style="display:none;">${moment(row.CreatedOn_FormatDate).format('YYYY-MM-DD')}</span> ${row.CreatedOn_FormatDate}`;
                }
            }
            , {
                "data": null, "": "Status", "autoWidth": true
                , "render": function (data, type, row) {
                    //---Check Advertisement-Status
                    var _status = '';
                    if (row.Status == 1) {
                        _status = '<a class="btn btn-success text-white btn-sm" style="width:max-content;" onclick="ConfirmChangeStatusSuperAdminAdvertisement(' + row.Id + ');">Active</a>';
                    }
                    else {
                        _status = '<a class="btn btn-danger text-white btn-sm" style="width:max-content;" onclick="ConfirmChangeStatusSuperAdminAdvertisement(' + row.Id + ');">In-Active</a>';
                    }
                    return _status;
                }
            }
            , {
                "data": null, "": "Action", "autoWidth": true,
                "render": function (data, type, row) {
                    var _action = '<div class="edbt">';
                    //_action += '<a href="javascript:EditNewUserAdvertisement(' + response.data[i].Id + ');"><i class="fas fa-edit"></i></a>';
                    _action += '<a href="javascript:ConfirmDeleteAdvertisementForSuperAdmin(' + row.Id + ');"><i class="fas fa-trash "></i></a>';
                    _action += '</div>';
                    return _action;
                }
              }
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
            "targets": [0,1,5,6], // column index (start from 0)
            "orderable": false, // set orderable false for selected columns
        }]
    });

    // for enabling search box only send requet on pressing enter
    $('#tblAdvertisementViewForSuperAdmin_filter input').unbind();
    $('#tblAdvertisementViewForSuperAdmin_filter input').bind('keyup', function (e) {
        if (e.keyCode == 13 || (e.keyCode == 8 && $(this).val() == '')) {
            _advertisementsTable.search(this.value).draw();
        }
    });

    // ----------------  Pagination Data Table ------------------
}

// ------------ NOT IN USE -----------------
function EditNewUserAdvertisement(id) {
    $('#AddNewAdvertisement').show();
    $('#AllAdvertisement').hide();
    document.getElementById("ToChangeUpdateText").innerHTML = "Update";
    document.getElementById("ToChangeTextTitle").innerHTML = "Edit Advertisements";
    document.getElementById("TochangeHeader").innerHTML = "Edit Advertisements";
    document.getElementById("pageUpTextChange").innerHTML = "Edit Advertisements";
    $(".error-class").html('');
    $('#AddAdvertisement').click(function () {
        document.getElementById("ToChangeTextTitle").innerHTML = "Add Advertisements";
        document.getElementById("TochangeHeader").innerHTML = "Add Advertisements";
        document.getElementById("pageUpTextChange").innerHTML = "Add Advertisements";
        document.getElementById("ToChangeUpdateText").innerHTML = "Save";
        $(".error-class").html('');
    });
    GetAdvertisementDataById(id);
}

function btnChangeView() {
    $("#fileProfileImage_ManageAdvertisement").val('');
    $('#ProfileImage').addClass('d-none');
    $("#ProfileImage").attr('src', '');
    $("#ddlAdvertisement_ManageCategoryForSuperAdmin").val('0');
    $("#ddlAdvertisement_ManageAdvertisementForSuperAdmin").val('0');
    $('#AllAdvertisement').show();
    $('#AddNewAdvertisement').hide();
    $(".error-class").html('');
}

//$('#ResetCancelbtn').click(function () {
//    $("#fileProfileImage_ManageAdvertisement").val('');
//    $('#ProfileImage').addClass('d-none');
//    $("#ProfileImage").attr('src', '');
//    $("#ddlAdvertisement_ManageCategoryForSuperAdmin").val(0);
//    $("#ddlAdvertisement_ManageAdvertisementForSuperAdmin").val(0);
//    $('#AllAdvertisement').show();
//    $('#AddNewAdvertisement').hide();
//    $(".error-class").html('');
//});


$('#AddAdvertisement').click(function () {
    $('#AddNewAdvertisement').show();
    $('#AllAdvertisement').hide();
});

function btnToSubmitAdvertisementInfoForSuperAdmin() {
    let is_valid = true;
    $(".error-class").html('');
    var _mode = (AdvertisementId_Global > 0) ? 2 : 1;

    let _manageCategoryies = $("#ddlAdvertisement_ManageCategoryForSuperAdmin").val();
    let _manageadvertisements = $("#ddlAdvertisement_ManageAdvertisementForSuperAdmin").val();


    let _error_advertisementImage = $("#error_fileProfileImage_ManageAdvertisement");
    let _error_manageCategoryies = $("#error_ddlAdvertisement_ManageCategoryForSuperAdmin");
    let _error_manageadvertisements = $("#error_ddlAdvertisement_ManageAdvertisementForSuperAdmin");


    if ($("#fileProfileImage_ManageAdvertisement").val() == '') {
        is_valid = false;
        _error_advertisementImage.html('Please select Image!');
    }


    if (validate_IsEmptySelectInputFieldValue(_manageCategoryies) || parseInt(_manageCategoryies) <= 0) {
        is_valid = false;
        _error_manageCategoryies.html('Please select a Category!');
    }

    if (validate_IsEmptySelectInputFieldValue(_manageadvertisements) || parseInt(_manageadvertisements) <= 0) {
        is_valid = false;
        _error_manageadvertisements.html('Please select a Advertisement!');
    }


    var _isActiveCategory = 0;
    if ($('#chkIsActive_ManageAdvertisementForSuperAdmin').is(':checked')) {
        // checked
        _isActiveCategory = 1;
    }

    if (is_valid) {
        var data = new FormData();
        data.append("Id", AdvertisementId_Global);
        data.append("ImageOrientationType", _manageadvertisements);
        data.append("AdvertisementCategory", _manageCategoryies);
        data.append("IsActive", _isActiveCategory);

        var _advertisementImageFile_MS = $("#fileProfileImage_ManageAdvertisement").get(0);
        var _advertisementImageFiles = _advertisementImageFile_MS.files;
        data.append('ProfileImage', _advertisementImageFiles[0]);
        data.append('mode', _mode);

        $.ajax({
            url: '/api/Advertisement/AddUpdate',
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
                    GetAllAdvertisementsPagination();
                    btnChangeView();
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

// Image File Preview -------------------------------------------------------------
document.getElementById('fileProfileImage_ManageAdvertisement').addEventListener('change', handleProfileImageUpload);

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

function getAllActiveAdvertisementListsSuperAdmin() {
    let _url = "/api/Advertisement/GetAllAdvertisement";

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

            var _table = $('#tblAdvertisementViewForSuperAdmin').DataTable();
            _table.destroy();

            var sno = 0;
            var _Image = '';
            var _Imagetype = '';
            var _OrientationType = '';
            var _status = '';
            var _action = '';

            var data = [];
            for (var i = 0; i < response.data.length; i++) {
                sno++;
                _Image = (response.data[i].ImageWithPath == '') ? '' : '<img src="' + response.data[i].ImageWithPath + '" class="Image" />';
                _Imagetype = response.data[i].AdvertisementCategory;
                _OrientationType = response.data[i].ImageOrientationType;
                _action = '<div class="edbt">';
                //_action += '<a href="javascript:EditNewUserAdvertisement(' + response.data[i].Id + ');"><i class="fas fa-edit"></i></a>';
                _action += '<a href="javascript:ConfirmDeleteAdvertisementForSuperAdmin(' + response.data[i].Id + ');"><i class="fas fa-trash "></i></a>';
                _action += '</div>';

                //---Check Staff-Status
                if (response.data[i].IsActive == 1) {
                    _status = '<a class="btn btn-success text-white btn-sm" style="width:max-content;" onclick="ConfirmChangeStatusSuperAdminAdvertisement(' + response.data[i].Id + ');">Active</a>';
                }
                else {
                    _status = '<a class="btn btn-danger text-white btn-sm" style="width:max-content;" onclick="ConfirmChangeStatusSuperAdminAdvertisement(' + response.data[i].Id + ');">In-Active</a>';
                }

                data.push([
                    sno,
                    _Image,
                    _Imagetype,
                    _OrientationType,
                    _status,
                    _action
                ]);
            }

            $('#tblAdvertisementViewForSuperAdmin').DataTable({
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

function ConfirmDeleteAdvertisementForSuperAdmin(sid) {
    swal({
        title: "Delete Advertisement",
        text: "Are you sure to delete this Advertisement?",
        type: "warning",
        buttons: true,
        cancel: {
            text: "Cancel",
            value: null,
            visible: false,
            className: "",
            closeModal: true,
        }
    }).then((willDelete) => {
        if (willDelete) {
            DeleteAdvertisementForSuperAdmin(sid);
        } else {
            //swal("Your imaginary file is safe!");
        }
    });
}

function DeleteAdvertisementForSuperAdmin(sid) {
    StartLoading();
    $.ajax({
        type: "POST",
        url: "/api/Advertisement/Delete/" + sid,
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
                    //--Get AllActiveAdvertisementListsSuperAdmin List
                    GetAllAdvertisementsPagination();
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

// -- NOT IN USE
function GetAdvertisementDataById(id) {
    var _url = "/api/Advertisement/GetSingleAdvertisement/" + id;

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

            $("#ProfileImage").attr('src', response.data.ImageWithPath);
            $("#ProfileImage").removeClass('d-none');
            $("#ddlAdvertisement_ManageCategoryForSuperAdmin").val(response.data.AdvertisementCategory);
            $("#ddlAdvertisement_ManageAdvertisementForSuperAdmin").val(response.data.ImageOrientationType);
            //--Category-Status
            if (response.data.IsActive == 1) {
                $('#chkIsActive_ManageAdvertisementForSuperAdmin').prop('checked', true);
            }
            else {
                $('#chkIsActive_ManageAdvertisementForSuperAdmin').prop('checked', false);
            }
            //$("#textAddress").val(response.data.Address);
            //AdvertisementId_Global = id;

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

function ConfirmChangeStatusSuperAdminAdvertisement(sid) {
    swal({
        title: "Change Status",
        text: "Are you sure to change status of this Advertisement?",
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
            ChangeStatusSuperAdminAdvertisement(sid);
        } else {
            //swal("Your imaginary file is safe!");
        }
    });
}

function ChangeStatusSuperAdminAdvertisement(sid) {
    StartLoading();
    $.ajax({
        type: "POST",
        url: "/api/Advertisement/ChangeStatus/" + sid,
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
                    GetAllAdvertisementsPagination();
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