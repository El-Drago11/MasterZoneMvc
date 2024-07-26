﻿var UserToken_Global = "";
var CategoryId_Global = "0";
var LastBusinessRecordId_Global = 0;

$(document).ready(function () {
    CategoryId_Global = $('#hiddenCategoryId_CategoryLisitng').val();

    getAllActiveSubCategoriesByParentCategory(CategoryId_Global);
    getAllBusinessOwnersByCategory(CategoryId_Global);
});

function getAllActiveSubCategoriesByParentCategory(id) {
    let _url = "/api/BusinessCategory/Parent/"+id+"/GetAllActiveSubCategories";

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

            if (response.data) {

                // set parent category name 
                $('#categoryHeading').html('<a class="h2 text-dark" href="javascript:parentCategoryClick('+id+');">' + response.data.Name  + '</a>');
                $('title').html(response.data.Name);

                var res_Categories = '';
                // --------------------- append parent categories in dropdown

                for (var i = 0; i < response.data.SubCategories.length; i++) {
                    var item = response.data.SubCategories[i];

                    res_Categories += `
                    <div class="img-w-txt">
                        <a href="javascript:getBySubCategory(${item.Id})" data-id="${item.Id}">
                            <img style="width: 60px;" src="${item.CategoryImageWithPath}" alt="${item.Name}" class="img-tx">
                            <p class="txt-img text-center">${item.Name}</p>
                        </a>
                    </div>
                `;
                }

                $("#subCategoriesList").html('').append(res_Categories);
                // --------------------- append parent categories in dropdown
            }
            else {
                $.iaoAlert({
                    msg: 'Could not get data!',
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

function getAllBusinessOwnersByCategory(categoryId, forceRefresh = false) {
    if (forceRefresh == true) {
        // -- reset list view
        $('#businessRecordsList').html('');
        LastBusinessRecordId_Global = 0;
    }

    let _url = "/api/Business/GetAllByCategory?categoryId="+categoryId+"&lastRecordId="+LastBusinessRecordId_Global;
    StartLoading();
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

            var res_content = '';
            // --------------------- append parent categories in dropdown

            for (var i = 0; i < response.data.length; i++) {
                var item = response.data[i];
                res_content += `
                    <div class="search-r-outer d-flex justify-content-between align-items-center mt-4 border-bottom pb-3">
                        <div class="busi-sc d-flex">
                            <a href="#"> <img src="${item.ProfileImageWithPath}"></a>
                            <div class="bus-txt-sc">
                                <div class="bus-head-i ">
                                    <a href="#" class="d-flex align-items-center text-dark">
                                        <div class="Bus-txt-1 b-title">${item.FirstName + ' ' + item.LastName}</div>
                                    </a>
                                </div>
                                <div class="bus-addr-txt b-addrs"></div>
                            </div>
                        </div>
                        <a class="btn btn-outline-dark btn-sm btn-md-lg cstm-btn w-auto" href="#">View Profile</a>
                    </div>
                `;
            }

            if (response.data.length > 0) {
                LastBusinessRecordId_Global = response.data[response.data.length - 1].Id;
                $('#btnViewMore').show();
            }
            else {
                LastBusinessRecordId_Global = 0;
                res_content = '<div class="w-100 text-center text-black-50"> No records found!</div > ';
                $('#btnViewMore').hide();
            }

            $("#businessRecordsList").append(res_content);

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

function getBySubCategory(categoryId) {
    // -- reset list view
    $('#businessRecordsList').html('');
    $('#subCategoriesList a p').removeClass('text-primary');
    $('#subCategoriesList a[data-id="' + categoryId + '"] p').addClass('text-primary');
    // get and bind data
    CategoryId_Global = categoryId;
    getAllBusinessOwnersByCategory(categoryId, true);
}

function parentCategoryClick(categoryId) {
    // -- reset list view
    $('#businessRecordsList').html('');
    $('#subCategoriesList a p').removeClass('text-primary');
    // get and bind data
    CategoryId_Global = categoryId;
    getAllBusinessOwnersByCategory(categoryId, true);
}

function btnViewMoreClick() {
    // sub category stored in global
    getAllBusinessOwnersByCategory(CategoryId_Global);
}