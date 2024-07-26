var UserToken_Global = "";

var _advertisementsTable;

function initializeDataTable_Advertisement() {
    _advertisementsTable = $("#tblAdvertisementView").DataTable();
}

$(document).ready(function () {
    $.get("/Business/GetBusinessAdminToken", null, function (dataToken) {
        if (dataToken != "" && dataToken != null) {
            initializeDataTable_Advertisement();
            UserToken_Global = dataToken;
            GetAllAdvertisementsForBusinessPagination();
            StopLoading();
        }
        else {
            StopLoading();
        }
    });
});

function GetAllAdvertisementsForBusinessPagination() {
    // ---------------- Pagination Data Table --------------------
    var _url_val = "/api/Advertisement/Business/GetAllByPagination";
    _advertisementsTable.clear().destroy();
    _advertisementsTable = $("#tblAdvertisementView").DataTable({
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
                        _status = '<a class="btn btn-success text-white btn-sm" style="width:max-content;" onclick="ConfirmChangeStatusAdvertisement(' + row.Id + ');">Active</a>';
                    }
                    else {
                        _status = '<a class="btn btn-danger text-white btn-sm" style="width:max-content;" onclick="ConfirmChangeStatusAdvertisement(' + row.Id + ');">In-Active</a>';
                    }
                    return _status;
                }
            }
            , {
                "data": null, "": "Action", "autoWidth": true,
                "render": function (data, type, row) {
                    var _action = '<div class="edbt">';
                    //_action += '<a href="javascript:EditNewUserAdvertisement(' + response.data[i].Id + ');"><i class="fas fa-edit"></i></a>';
                    _action += '<a href="javascript:ConfirmDeleteAdvertisement(' + row.Id + ');"><i class="fas fa-trash "></i></a>';
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
            "targets": [0, 1, 5, 6], // column index (start from 0)
            "orderable": false, // set orderable false for selected columns
        }]
    });

    // for enabling search box only send requet on pressing enter
    $('#tblAdvertisementView_filter input').unbind();
    $('#tblAdvertisementView_filter input').bind('keyup', function (e) {
        if (e.keyCode == 13 || (e.keyCode == 8 && $(this).val() == '')) {
            _advertisementsTable.search(this.value).draw();
        }
    });

    // ----------------  Pagination Data Table ------------------
}


function btnSubmitAdvertisementClick() {
    let is_valid = true;
    $(".error-class").html('');

    let _manageCategory = $("#ddlAdvertisement_ManageCategory").val().trim();
    let _manageadvertisement = $("#ddlAdvertisement_ManageAdvertisement").val().trim();

    let _error_manageCategory = $("#error_ddlAdvertisement_ManageCategory");
    let _error_manageadvertisement = $("#error_ddlAdvertisement_ManageAdvertisement");
    let _error_manageAdvertisementImage = $("#error_fileProfileImage_ManageProfile");

    if (validate_IsEmptySelectInputFieldValue(_manageCategory) || parseInt(_manageCategory) < 0) {
        is_valid = false;
        _error_manageCategory.html('Please select a Category!');
    }
    if (validate_IsEmptySelectInputFieldValue(_manageadvertisement) || parseInt(_manageadvertisement) < 0) {
        is_valid = false;
        _error_manageadvertisement.html('Please select a Category!');
    }
    if ($("#fileProfileImage_ManageAdvertisement").val() == '') {
        is_valid = false;
        _error_manageAdvertisementImage.html('Please select a Image!');
    }

    var _isActiveCategory = 0;
    if ($('#chkIsActive_ManageAdvertisement').is(':checked')) {
        // checked
        _isActiveCategory = 1;
    }

    if (is_valid) {
        var data = new FormData();
        data.append("ImageOrientationType", _manageadvertisement);
        data.append("AdvertisementCategory", _manageCategory);
        data.append("IsActive", _isActiveCategory);
        data.append("Mode", "1");

        var _advertisementImageFile_MS = $("#fileProfileImage_ManageAdvertisement").get(0);
        var _advertisementImageFiles = _advertisementImageFile_MS.files;
        data.append('ProfileImage', _advertisementImageFiles[0]);

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
                    GetAllAdvertisementsForBusinessPagination();
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

                ResetAddView()
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

$(document).ready(function () {
    // ResetAddView();
});

$('#btnAddStaff').click(function () {
    $('#sectionAddStaff').show();
    $('#btnAddStaff').hide();
    $('#sectionViewStaff').hide();
    document.getElementById("pageTextchange").innerHTML = "Add Advertisements";
    document.getElementById("pageStageChange").innerHTML = "Add Advertisements";
});


function ShowingViewStaffList() {
    $('#sectionViewStaff').show();
}

$('#ResetValues').click(function () {
    $("#ddlAdvertisement_ManageCategory").val('0');
    $("#ddlAdvertisement_ManageAdvertisement").val('0');
    $("#fileProfileImage_ManageAdvertisement").val('');
});

function ResetAddView() {
    $("#fileProfileImage_ManageAdvertisement").val('');
    $('#ProfileImage').addClass('d-none');
    $("#ProfileImage").attr('src', '');
    $('#sectionViewStaff').show();
    $('#sectionAddStaff').hide();
    $('#btnAddStaff').show();
    document.getElementById("pageTextchange").innerHTML = "Advertisements";
    document.getElementById("pageStageChange").innerHTML = "Advertisements";
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

function ConfirmDeleteAdvertisement(sid) {
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
            DeleteAdvertisement(sid);
        } else {
            //swal("Your imaginary file is safe!");
        }
    });
}

function DeleteAdvertisement(sid) {
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
                    GetAllAdvertisementsForBusinessPagination();
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

function ConfirmChangeStatusAdvertisement(sid) {
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
            ChangeStatusAdvertisement(sid);
        } else {
            //swal("Your imaginary file is safe!");
        }
    });
}

function ChangeStatusAdvertisement(sid) {
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
                    GetAllAdvertisementsForBusinessPagination();
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

function getAllActiveAdvertisementLists() {
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

            var _table = $('#tblAdvertisementView').DataTable();
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
                // _action += '<a href="javascript:EditAdvertisement(' + response.data[i].Id + ');"><i class="fas fa-edit"></i></a>';
                _action += '<a href="javascript:ConfirmDeleteAdvertisement(' + response.data[i].Id + ');"><i class="fas fa-trash "></i></a>';
                _action += '</div>';

                //---Check Staff-Status
                if (response.data[i].IsActive == 1) {
                    _status = '<a class="btn btn-success text-white btn-sm" style="width:max-content;" onclick="ConfirmChangeStatusAdvertisement(' + response.data[i].Id + ');">Active</a>';
                }
                else {
                    _status = '<a class="btn btn-danger  text-white btn-sm" style="width:max-content;" onclick="ConfirmChangeStatusAdvertisement(' + response.data[i].Id + ');">In-Active</a>';
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

            $('#tblAdvertisementView').DataTable({
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