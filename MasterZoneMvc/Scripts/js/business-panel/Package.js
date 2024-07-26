var PlanId_Global = 0;
var UserToken_Global = "";

$(document).ready(function () {
    $.get("/Business/GetBusinessAdminToken", null, function (dataToken) {
        if (dataToken != "" && dataToken != null) {
            UserToken_Global = dataToken;
            GetAllBusinessPlans();
            //StopLoading();
        }
        else {
            $.get("/Staff/GetStaffToken", null, function (dataToken) {
                if (dataToken != "" && dataToken != null) {
                    UserToken_Global = dataToken;
                    GetAllBusinessPlans();
                    //StopLoading();
                }
                else {
                    StopLoading();
                }
            });
        }
    });

    $('#btnAddPackage').click(function () {
        $('#sectionAddStaff').show();
        $('#btnAddPackage').hide();
        $('#sectionViewStaff').hide();
        document.getElementById("pageTextchange").innerHTML = "Add Package";
        document.getElementById("pageStageChange").innerHTML = "Add Package";
    });
});

function GetAllBusinessPlans() {
    var _url = "/api/BusinessPlan/GetAll";

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

            $('#BusinessPlanCardList').html('');
            var cardsData = "";
            for (var i = 0; i < response.data.length; i++) {
                cardsData += bindPlanDataToCard(response.data[i]);
            }

            if (response.data.length <= 0) {
                cardsData += `<div class="w-100 text-center"><i> No Plan/Packages </i><div>`;
            }

            $('#BusinessPlanCardList').append(cardsData);

            StopLoading();
            GetAllBusinessPlanDurationTypes();
        },
        error: function (result) {
            StopLoading();
            GetAllBusinessPlanDurationTypes();

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

function bindPlanDataToCard(businessPlan) {
    var cardData = `
        <div class="col-md-12 col-lg-4 mb-4 col-item">
                <div class="card">
                    <div class="card-body pt-5 pb-5 d-flex flex-lg-column flex-md-row flex-sm-row flex-column">
                        <div class="price-top-part">
                            <!--<i class="iconsminds-male large-icon"></i>-->
                            <h5 class="mb-0 font-weight-semibold color-theme-1 mb-4">${businessPlan.Name}</h5>
                            <p class="text-large mb-2 text-default">Rs. ${businessPlan.Price}</p>
                            <p class="text-muted text-small">${businessPlan.BusinessPlanDurationTypeName}</p>
                        </div>
                        <div class="pl-3 pr-3 pt-3 pb-0 d-flex price-feature-list flex-column flex-grow-1">
                            <ul class="list-unstyled">
                                <li>
                                    <p class="mb-0 ">
                                        ${businessPlan.Description}
                                    </p>
                                </li>
                            </ul>
                            <div class="d-flex justify-content-around">
                                <button type="button" class="btn btn-md btn-outline-info" onclick="EditPackage(${businessPlan.Id})">
                                    <i class="fas fa-edit"></i> Edit 
                                </button>
                                <button type="button" class="btn btn-md btn-outline-danger" onclick="ConfirmDeletePackage(${businessPlan.Id},'${businessPlan.Name}')">
                                    <i class="fas fa-trash"></i> Delete
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
    `;
    return cardData;
}

function GetAllBusinessPlanDurationTypes() {
    var _url = "/api/BusinessPlanDurationType/GetAll";

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
            $("#ddlDurationType").html('');
            var res_DurationTypeList = '<option value="0">Select</option>';

            for (var i = 0; i < response.data.length; i++) {
                res_DurationTypeList += '<option value="' + response.data[i].Id + '">' + response.data[i].Value + '</option>';
            }

            $("#ddlDurationType").html('').append(res_DurationTypeList); //.select2();

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

    let _planName = $("#txtPlanName").val().trim();
    let _durationTypeId = $("#ddlDurationType").val().trim();
    let _price = $("#txtPrice").val().trim();
    let _description = $("#txtDescription").val().trim();

    let _error_planName = $("#error_txtPlanName");
    let _error_durationTypeId = $("#error_ddlDurationType");
    let _error_price = $("#error_txtPrice");
    let _error_description = $("#error_txtDescription");

    var _isActivePlan = 0;
    if ($('#chkIsActive').is(':checked')) {
        // checked
        _isActivePlan = 1;
    }

    if (validate_IsEmptyStringInputFieldValue(_planName)) {
        is_valid = false;
        _error_planName.html('Enter Name!');
    }
    if (validate_IsEmptySelectInputFieldValue(_durationTypeId)) {
        is_valid = false;
        _error_durationTypeId.html('Select Duration Type!');
    }
    if (validate_IsEmptyStringInputFieldValue(_price) || isNaN(_price)) {
        is_valid = false;
        _error_price.html('Enter Price!');
    }
    else if (parseFloat(_price) <= 0) {
        is_valid = false;
        _error_price.html('Price must be greater than zero!');
    }
    if (validate_IsEmptyStringInputFieldValue(_description)) {
        is_valid = false;
        _error_description.html('Enter Description!');
    }

    if (is_valid) {
        var _mode = (PlanId_Global > 0) ? 2 : 1;
        var data = new FormData();
        data.append("Id", PlanId_Global);
        data.append("Name", _planName);
        data.append("BusinessPlanDurationTypeId", _durationTypeId);
        data.append("Description", _description);
        data.append("Price", _price);
        data.append("Status", _isActivePlan);
        data.append("Mode", _mode);

        $.ajax({
            url: '/api/BusinessPlan/AddUpdate',
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

                ResetAddUpdateView();
                StopLoading();
                GetAllBusinessPlans();
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

function showEditPackageView() {
    $('#sectionAddStaff').show();
    $('#sectionViewStaff').hide();
    document.getElementById("myText").innerHTML = "Edit Package";
    document.getElementById("ChangeUpdateText").innerHTML = "Update";
    document.getElementById("pageTextchange").innerHTML = "Edit Package";
    document.getElementById("pageStageChange").innerHTML = "Edit Package";
    $('#ChangeUpdateText').show();
    $('#btnAddPackage').hide();
    $(".error-class").html('');
};

function EditPackage(id) {
    var _url = "/api/BusinessPlan/GetById/" + id;

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

            $("#txtPlanName").val(response.data.Name);
            $("#ddlDurationType").val(response.data.BusinessPlanDurationTypeId);
            $("#txtPrice").val(response.data.Price);
            $("#txtDescription").val(response.data.Description);


            //---Check Staff-Status
            if (response.data.Status == 1) {
                $('#chkIsActive').prop('checked', true);
            }
            else {
                $('#chkIsActive').prop('checked', false);
            }

            PlanId_Global = response.data.Id;

            showEditPackageView();

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

function ResetAddUpdateView() {
    $(".error-class").html('');

    $("#txtPlanName").val('');
    $("#ddlDurationType").val(0);
    $("#txtPrice").val('');
    $("#txtDescription").val('');
    $('#chkIsActive').prop('checked', true);
    PlanId_Global = 0;

    $('#sectionViewStaff').show();
    $('#sectionAddStaff').hide();
    $('#btnAddPackage').show();

    document.getElementById("pageTextchange").innerHTML = "Package";
    document.getElementById("pageStageChange").innerHTML = "Package";

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

function ConfirmDeletePackage(sid, packageName) {
    swal({
        title: "Delete Package",
        text: "Are you sure to delete this Package '"+packageName+"'?",
        type: "warning",
        buttons: {
            cancel: true,
            confirm: "Yes",
        }
    })
    .then((willDelete) => {
        if (willDelete) {
            DeletePackage(sid);
        } else {
            //swal("Your imaginary file is safe!");
        }
    });
}

function DeletePackage(sid) {
    StartLoading();
    $.ajax({
        type: "POST",
        url: "/api/BusinessPlan/Delete/" + sid,
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
                    GetAllBusinessPlans();
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
