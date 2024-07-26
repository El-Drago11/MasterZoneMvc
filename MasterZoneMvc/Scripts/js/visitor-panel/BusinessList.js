var BusinessList_LastRecordId_Global = 0;

var subCategoryId_global;
$(document).ready(function () {
    StartLoading();
    getAllB2BSubCategories_BusinessListPage();


});

//------------get  alll b2b business category name ---------------
function getAllB2BSubCategories_BusinessListPage() {

    let _url = "/api/BusinessCategory/GetAllB2BSubBusinessCategoryListForHomePage";

    $.ajax({
        type: "GET",
        url: _url,
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

            var res_Categories = '<button class="button mb-3 is-checked" data-filter="*" onclick="getAllB2BBusinessListPage(0)"> All</button>';

            var dataList = response.data;

            for (var i = 0; i < dataList.length; i++) {
                res_Categories += `
                    <button class="button mb-3 " data-filter="*" onclick="getAllB2BBusinessListPage(${dataList[i].Id})">${dataList[i].Name}</button>
                `;
            }

            if (dataList.length <= 0) {
                $('#BusinessList').html('No Records!');
            }

            $("#navB2BBusinessCategoryList").html(res_Categories);
            $('#navB2BBusinessCategoryList button:first-child').click();

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


//-------get all b2b business list for view -------------

function getAllB2BBusinessListPage(subCategoryId) {
 
    let _url = "/api/BusinessCategory/GetAllB2BBusinessListBySubCategoryForBusinessListPage?subCategoryId=" + subCategoryId + "&lastRecordId=0";

    subCategoryId_global = subCategoryId;
    $.ajax({
        type: "GET",
        url: _url,
        contentType: 'application/json',
        success: function (response) {

            if (response.status < 1) {
                $.iaoAlert({
                    msg: '@(Resources.ErrorMessage.UnAuthorizedErrorMessage)',
                    type: "error",
                    mode: "dark",
                });

                return;
            }

            var BusinessList = '';

            if (response.data.BusinessList.length <= 0) {
                $('#BusinessList').html('<div class="w-100 text-black-50 text-center mt-4"><i>No Record!</i></div>');
                $('#BtnViewMoreBusinessList').addClass('d-none');
                StopLoading();
                return;
            }

            if (response.data.BusinessList.length <= 0) {
                $('#BtnViewMoreBusinessList').addClass('d-none');
            }
            else {
                BusinessList_LastRecordId_Global = response.data.BusinessList[response.data.BusinessList.length - 1].Id;
                $('#BtnViewMoreBusinessList').removeClass('d-none');
            }
            BusinessList = getHTMLBindedBusinessListData(response.data.BusinessList);
            $('#BusinessList').html(BusinessList);


            StopLoading();
        },

        error: function (result) {
            StopLoading();
            if (result["status"] == 401) {
                $.iaoAlert({
                    msg: '@(Resources.ErrorMessage.UnAuthorizedErrorMessage)',
                    type: "error",
                    mode: "dark",
                });
            }
            else {
                $.iaoAlert({
                    msg: '@(Resources.ErrorMessage.TechincalErrorMessage)',
                    type: "error",
                    mode: "dark",
                });
            }
        }
    });
}

//--------view more function for business list -----

function ViewMoreBusinessList() {

    let _url = "/api/BusinessCategory/GetAllB2BBusinessListBySubCategoryForBusinessListPage?lastRecordId=" + BusinessList_LastRecordId_Global + "&subCategoryId=" + subCategoryId_global;

    $.ajax({
        type: "GET",
        url: _url,

        contentType: 'application/json',
        success: function (response) {

            if (response.status < 1) {
                $.iaoAlert({
                    msg: '@(Resources.ErrorMessage.UnAuthorizedErrorMessage)',
                    type: "error",
                    mode: "dark",
                });

                return;
            }
            // $('#BusinessList').html('');
            var BusinessList = '';

            if (response.data.BusinessList.length <= 0) {
                $('#BtnViewMoreBusinessList').addClass('d-none');
            }
            else {
                BusinessList_LastRecordId_Global = response.data.BusinessList[response.data.BusinessList.length - 1].Id;
                $('#BtnViewMoreBusinessList').removeClass('d-none');
            }

            BusinessList = getHTMLBindedBusinessListData(response.data.BusinessList);

            $('#BusinessList').append(BusinessList);



            StopLoading();
        },
        error: function (result) {
            StopLoading();

            if (result["status"] == 401) {
                $.iaoAlert({
                    msg: '@(Resources.ErrorMessage.UnAuthorizedErrorMessage)',
                    type: "error",
                    mode: "dark",
                });
            }
            else {
                $.iaoAlert({
                    msg: '@(Resources.ErrorMessage.TechincalErrorMessage)',
                    type: "error",
                    mode: "dark",
                });
            }
        }
    });
}

//-----html view card of business list--------
function getHTMLBindedBusinessListData(BusinessList) {
    var htmlData = '';
    var dataList = BusinessList;
    for (var i = 0; i < dataList.length; i++) {
        var _item = dataList[i];
        var _businessTrademark = '';
        if (_item.Verified == 1) {
            _businessTrademark = '<i class="fas fa-check-circle text-success" title="Verified"></i>';
        }
        else if (_item.Verified == 2) {
            _businessTrademark = '&reg;';
        }
        else if (_item.Verified == 3) {
            _businessTrademark = '&trade;';
        }

        htmlData += `
                    <div class="col-md-4 col-sm-12 element-item transition ${_item.SubCategoryId}" data-category="${_item.SubCategoryId}">
                        <a target="_blank" href="/Home/BusinessProfile?businessOwnerLoginId=${_item.UserLoginId}">
                            <div class="d-flex flex-column w-100" style="position:relative;">
                                <div class="img-inr">
                                    <img src="${_item.CoverImageWithPath}">
                                </div>
                                <div class="d-flex flex-column py-3 px-3 cstm-overlay-block">
                                    <div class="d-flex align-items-center w-100 mb-2" style="overflow:hidden;">
                                        <img src="${_item.BusinessLogoWithPath}" class="img-size-sm mr-2" />
                                        <span class="font-weight-bold">${_item.BusinessName} ${_businessTrademark}</span>
                                    </div>
                                    <div class="font-weight-bold text-uppercase">${_item.SubCategoryName}</div>
                                </div>
                            </div>
                        </a>
                    </div>
                `;
    }

    return htmlData;
}

// change is-checked class on buttons
$('.button-group').each(function (i, buttonGroup) {
    var $buttonGroup = $(buttonGroup);
    $buttonGroup.on('click', 'button', function () {
        $buttonGroup.find('.is-checked').removeClass('is-checked');
        $(this).addClass('is-checked');
    });
});