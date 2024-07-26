var UserToken_Global = "";
var CategoryId_Global = "0";
var LastBusinessRecordId_Global = 0;

$(document).ready(function () {
    //CategoryId_Global = $('#hiddenCategoryId_CategoryLisitng').val();
    console.log(SearchParameters_Global);

    // set parent category name 
    $('#pageHeading').html('<a class="h2 text-dark" href="javascript:window.location.reload();">Search Results for: "' + SearchParameters_Global.SearchKeyword + '"</a>');

    getAllBusinessOwnersBySearch();
});

function getAllBusinessOwnersBySearch(forceRefresh = false) {
    if (forceRefresh == true) {
        // -- reset list view
        $('#businessRecordsList').html('');
        SearchParameters_Global.LastRecordId = 0;
    }

    let _url = "/api/Business/GetAllBySearch"; //+categoryId+"&lastRecordId="+LastBusinessRecordId_Global;
    var requestData = SearchParameters_Global;
    StartLoading();
    $.ajax({
        type: "POST",
        url: _url,
        data: JSON.stringify(requestData),
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
                var businessName = (item.BusinessName != "") ? item.BusinessName : item.FirstName + ' ' + item.LastName;
                res_content += `
                    <div class="search-r-outer d-flex justify-content-between align-items-center mt-4 border-bottom pb-3">
                        <div class="busi-sc d-flex">
                            <a href="#"> <img src="${item.ProfileImageWithPath}"></a>
                            <div class="bus-txt-sc">
                                <div class="bus-head-i ">
                                    <a href="#" class="d-flex align-items-center text-dark">
                                        <div class="Bus-txt-1 b-title">${businessName}</div>
                                    </a>
                                </div>
                                <div class="bus-addr-txt b-addrs text-black-50">Category: ${item.BusinessCategoryName}</div>
                            </div>
                        </div>
                        <a class="btn btn-outline-dark btn-sm btn-md-lg cstm-btn w-auto" href="#">View Profile</a>
                    </div>
                `;
            }

            if (response.data.length > 0) {
                SearchParameters_Global.LastRecordId = response.data[response.data.length - 1].Id;
                $('#btnViewMore').removeClass('d-none');
            }
            else {
                SearchParameters_Global.LastRecordId = 0;
                res_content = '<div class="w-100 text-center text-black-50"> No records found!</div > ';
                $('#btnViewMore').addClass('d-none');
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
    //CategoryId_Global = categoryId;
    SearchParameters_Global.BusinessCategoryId = categoryId;
    getAllBusinessOwnersBySearch(categoryId, true);
}

function parentCategoryClick(categoryId) {
    // -- reset list view
    $('#businessRecordsList').html('');
    $('#subCategoriesList a p').removeClass('text-primary');
    // get and bind data
    CategoryId_Global = categoryId;
    getAllBusinessOwnersBySearch(categoryId, true);
}

function btnViewMoreClick() {
    // sub category stored in global
    getAllBusinessOwnersBySearch();
}