﻿var UserToken_Global = "";
var BusinessCategoryId_Global = 0;
var _parentCategoriesHashMap = new Map();
var _CategoriesList_Global = [];

$(document).ready(function () {
    StartLoading();
    $.get("/SuperAdmin/GetSuperAdminToken", null, function (dataAdminToken) {
        if (dataAdminToken != "" && dataAdminToken != null) {

            UserToken_Global = dataAdminToken;
            getAllBusinessCategories();
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

function getAllBusinessCategories() {
    let _url = "/api/BusinessCategory/GetAll";

    $.ajax({
        type: "GET",
        url: _url,
        headers: {
            "Authorization": "Bearer " + UserToken_Global,
            "Content-Type": "application/json"
        },
        //crossDomain: true,
        //xhrFields: {
        //    withCredentials: true
        //},
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
            _CategoriesList_Global = response.data;

            var res_Categories = '<option value="0">Select Category</option>';

            for (var i = 0; i < response.data.length; i++) {
                if (response.data[i].ParentBusinessCategoryId == 0) {
                    _parentCategoriesHashMap.set(response.data[i].Id, response.data[i].Name);
                    res_Categories += '<option value="' + response.data[i].Id + '">' + response.data[i].Name + '</option>';
                }
            }

            $("#ddlParentBusinessCategory_ManageCategories").html('').append(res_Categories).select2();
            // --------------------- append parent categories in dropdown

            var _table = $('#tblBusinessCategories').DataTable();
            _table.destroy();

            var sno = 0;
            var _parentCategoryData = '';
            var _status = '';
            var _action = '';
            var _categoryIcon = '';
            var data = [];
            for (var i = 0; i < response.data.length; i++) {
                sno++;
                _categoryIcon = (response.data[i].CategoryImageWithPath == '') ? '': '<img src="' + response.data[i].CategoryImageWithPath +'" class="categoryIcon" />';
                _parentCategoryData = '-';
                if (response.data[i].ParentBusinessCategoryId > 0) {
                    if (_parentCategoriesHashMap.has(response.data[i].ParentBusinessCategoryId))
                        _parentCategoryData = _parentCategoriesHashMap.get(response.data[i].ParentBusinessCategoryId);
                }
                _action = '<div class="edbt">';
                _action += '<a href="javascript:EditBusinessCategory(' + response.data[i].Id + ');"><i class="fas fa-edit"></i></a>';
                _action += `<a href="javascript:confrimDelete_ManageCategory(${response.data[i].Id}, '${response.data[i].ParentBusinessCategoryId}', '${response.data[i].Name}');"><i class="fas fa-trash "></i></a>`;
                _action += '</div>';

                //---Check Staff-Status
                if (response.data[i].IsActive == 1) {
                    _status = '<a class="btn btn-success btn-sm" style="width:80px;" onclick="ConfirmChangeStatusCategory(' + response.data[i].Id + ');">Active</a>';
                }
                else {
                    _status = '<a class="btn btn-danger btn-sm" style="width:80px;" onclick="ConfirmChangeStatusCategory(' + response.data[i].Id + ');">In-Active</a>';
                }

                data.push([
                    sno,
                    _parentCategoryData,
                    _categoryIcon,
                    response.data[i].Name,
                    _status,
                    _action
                ]);
            }
            
            $('#tblBusinessCategories').DataTable({
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

function showAddUpdateCategorySection() {
    $('#sectionViewCategories_ManageCategories').addClass('d-none');
    $('#sectionAddUpdateCategory_ManageCategories').removeClass('d-none');
}

function showViewCategorySection() {
    $('#sectionAddUpdateCategory_ManageCategories').addClass('d-none');
    $('#sectionViewCategories_ManageCategories').removeClass('d-none');
}

function resetAddUpdateCategoryForm() {
    $('.error-class').html('');
    // Reset Fields
    $('#ddlParentBusinessCategory_ManageCategories').prop('disabled', false);
    $("#ddlParentBusinessCategory_ManageCategories").val(0).select2();
    $("#txtCategoryName_ManageCategories").val('');
    $('#chkIsActive_ManageCategories').prop('checked', false);
    $("#fileCategoryImage_ManageCategories").val('');
    $("#previewImage").attr('src', '');
    $("#previewImage").addClass('d-none');

    BusinessCategoryId_Global = 0;
    $('#sectionAddUpdateCategory_ManageCategories .card-title').html('Add Category');
}

function EditBusinessCategory(id) { 
    if (id <= 0)
        return;

    StartLoading();
    var _url = "/api/BusinessCategory/GetById/" +id;

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

            let _category = response.data;
            $("#ddlParentBusinessCategory_ManageCategories").val(_category.ParentBusinessCategoryId).trigger('change');
            $("#txtCategoryName_ManageCategories").val(_category.Name);
            $("#previewImage").attr('src', _category.CategoryImageWithPath);
            $("#previewImage").removeClass('d-none');

            //--Category-Status
            if (_category.IsActive == 1) {
                $('#chkIsActive_ManageCategories').prop('checked', true);
            }
            else {
                $('#chkIsActive_ManageCategories').prop('checked', false);
            }

            // disable if this is parent category and has subcategories
            let subCategories = _CategoriesList_Global.filter(c => c.ParentBusinessCategoryId == _category.Id);
            if (subCategories.length > 0) {
                $('#ddlParentBusinessCategory_ManageCategories').prop('disabled', true);
            }

            BusinessCategoryId_Global = id;
            $('#sectionAddUpdateCategory_ManageCategories .card-title').html('Edit Category');
            showAddUpdateCategorySection();
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

function AddUpdate_ManageCategories() {
    var is_valid = true;
    $(".error-class").html('');
    var _mode = (BusinessCategoryId_Global > 0) ? 2 : 1;

    var _parentBusinessCategoryId = $("#ddlParentBusinessCategory_ManageCategories").val();
    var _categoryName = $("#txtCategoryName_ManageCategories").val().trim();

    var _error_parentBusinessCategoryId = $("#error_ddlParentBusinessCategory_ManageCategories");
    var _error_categoryName = $("#error_txtCategoryName_ManageCategories");
    var _error_categoryImage = $("#error_fileCategoryImage_ManageCategories");

    //if (validate_IsEmptySelectInputFieldValue(_businessCategoryId) || parseInt(_businessCategoryId) < 0) {
    //    is_valid = false;
    //    _error_businessCategoryId.html('Please select a category!');
    //}

    if (validate_IsEmptyStringInputFieldValue(_categoryName)) {
        is_valid = false;
        _error_categoryName.html('Enter Category Name!');
    }

    if (_mode == 1) {
        if ($("#fileCategoryImage_ManageCategories").val() == '') {
            is_valid = false;
            _error_categoryImage.html('Please select category icon!');
        }
    }

    var _isActiveCategory = 0;
    if ($('#chkIsActive_ManageCategories').is(':checked')) {
        // checked
        _isActiveCategory = 1;
    }

    if (is_valid) {
        StartLoading();
        var formdata = new FormData();

        formdata.append("Id", BusinessCategoryId_Global);
        formdata.append("ParentBusinessCategoryId", _parentBusinessCategoryId);
        formdata.append("IsActive", _isActiveCategory);
        formdata.append("Name", _categoryName);
        formdata.append("Mode", _mode);

        var _categoryImageFile_MC = $("#fileCategoryImage_ManageCategories").get(0);
        var _categoryImageFiles = _categoryImageFile_MC.files;
        formdata.append('CategoryImage', _categoryImageFiles[0]);

        $.ajax({
            url: '/api/BusinessCategory/AddUpdate',
            headers: {
                "Authorization": "Bearer " + UserToken_Global
            },
            data: formdata,
            processData: false,
            contentType: false,
            mimeType: 'multipart/form-data',
            //contentType: 'application/json',
            type: 'POST',
            dataType: "json",
            success: function (dataResponse) {

                //--If successfully added
                if (dataResponse.status == 1) {
                    swal("Success!", dataResponse.message, "success");

                    getAllBusinessCategories();

                    resetAddUpdateCategoryForm();
                    showViewCategorySection();
                }
                else {
                    $.iaoAlert({
                        msg: dataResponse.message,
                        type: "error",
                        mode: "dark",
                    });
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

function btnClickCancel_ManageCategories() {
    showViewCategorySection();
    resetAddUpdateCategoryForm();
}

function confrimDelete_ManageCategory(categoryId, ParentCategoryId, categoryName) {
    let textMessage = `Delete category: ${categoryName} `;
    if (ParentCategoryId == 0) {
        textMessage = `Delete Category and its Sub-Categories of "${categoryName}" `;
    }
    swal({
        title: "Are you sure?",
        text: textMessage,
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
    .then((willDelete) => {
        if (willDelete) {
            ajaxDeleteCategory(categoryId);
        } else {
            //swal("Your imaginary file is safe!");
        }
    });
}

function ajaxDeleteCategory(categoryId) {
    StartLoading();
    _url =  '/api/BusinessCategory/Delete/' + categoryId;
    $.ajax({
        type: "POST",
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

            if (response.status == 1) {
                swal("Category has been deleted successfully!", {
                    icon: "success",
                });
            }
            
            //StopLoading();
            getAllBusinessCategories();
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


function ConfirmChangeStatusCategory(sid) {
    swal({
        title: "Change Status",
        text: "Are you sure to change status of this Category?",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: '#DD6B55',
        confirmButtonText: 'Yes',
        cancelButtonText: "No"
    })
        .then((willDelete) => {
            if (willDelete) {
                ChangeStatusCategory(sid);
            } else {
                //swal("Your imaginary file is safe!");
            }
        });
}

function ChangeStatusCategory(sid) {
    StartLoading();
    
    $.ajax({
        type: "POST",
        url: "/api/BusinessCategory/ChangeStatus/" + sid,
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
                    getAllBusinessCategories();

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

document.getElementById('fileCategoryImage_ManageCategories').addEventListener('change', handleImageUpload);

function handleImageUpload(event) {
    const file = event.target.files[0];
    const fileSize = file.size / 1024; // size in kilobytes
    const maxSize = 1024*1024; // maximum size in kilobytes
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
            msg: 'Image size too large. Please select a smaller image(< 1 MB).',
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