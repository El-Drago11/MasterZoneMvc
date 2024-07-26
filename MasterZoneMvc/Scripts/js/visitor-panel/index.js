var UserToken_Global = "";
var arrSearchBy = ["name", "location", "currentlocation"];

var SearchParameters_Global = {
    SearchKeyword: "",
    Latitude: "",
    Longitude: "",
    BusinessCategoryId: 0,
    LastRecordId: 0,
    SearchBy: arrSearchBy[0]
};

var UserToken_Global = "";

$(document).ready(function () {
    //StartLoading();
    $.get("/Business/GetBusinessAdminToken", null, function (dataToken) {
        if (dataToken != "" && dataToken != null) {

            UserToken_Global = dataToken;

            //StopLoading();
        }
        else {
            $.get("/Staff/GetStaffToken", null, function (dataToken) {
                if (dataToken != "" && dataToken != null) {
                    UserToken_Global = dataToken;
                    //StopLoading();
                }
                else {
                    StopLoading();
                }
            });
        }
    });
});

$(document).ready(function () {
    StartLoading();
    GetAllClassCategoryTypeList();  
    getAllCertificates_HomePage();
    getAllTrainings_HomePage();
    getAllInstructors_HomePage();
    getAllSponsors_HomePage();
    //applySlickOnCertificates();

    getAllActiveMenuItems();
    GettAllEventList();
    GetAllActiveBusinessPlans(); 
    getAllSubClassCategoryTypesForHomePage();

    getAllB2BSubCategories_HomePage();

    getAllClassesListsForSearching();
    getAllFeaturedCardSections_HomePage();
    getAllMultipleItemSections_HomePage();
    getClassCategorySections_HomePage();
    getFeaturedVideos_HomePage();
    getAllBannerSlides();
    //getAllExamFormList();

});

function sortByKeyAsc(array, key) {
    return array.sort(function (a, b) {
        var x = a[key]; var y = b[key];
        return ((x < y) ? -1 : ((x > y) ? 1 : 0));
    });
}

function getAllActiveMenuItems() {
    let _url = "/api/Menu/GetAllActiveVisitorMenuItems";

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

            var res_Categories = '';
            // --------------------- append parent categories in dropdown

            var menuOrderdList = sortByKeyAsc(response.data, "SortOrder");

            for (var i = 0; i < menuOrderdList.length &&  i < 9 && menuOrderdList[i].IsShowOnHomePage == 1; i++) {
                res_Categories += `
                    <div class="link-cards">
                        <img src="${menuOrderdList[i].MenuImageWithPath}">
                        <a href="${menuOrderdList[i].PageLink}">${menuOrderdList[i].Name}</a>
                    </div>
                `;
            }

            res_Categories += `
                <div class="link-cards link-cards-jkljlkkl text-center">
                    <a href="/Home/MoreCategories/" class="s-jkljlkkl>More Categories">More Categories</a>
                </div>
            `;

            $("#categoryLinkList").html('').append(res_Categories);
            // --------------------- append parent categories in dropdown

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


function getAllCertificates_HomePage() {
    let _url = "/api/Certificate/GetAllCertificatesForHomePage";

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

            var res_Categories = '';

            var dataList = response.data;

            for (var i = 0; i < dataList.length; i++) {
                var _link = (dataList[i].Link != '') ? dataList[i].Link : "#";
                res_Categories += `
                    <div class="d-flex flex-column">
                        <div class="logo-inr">
                            <a target="_blank" href="${_link}">
                                <img src="${dataList[i].CertificateIconWithPath}">
                            </a>
                        </div>
                        <div class="w-100 h6 mt-3 text-center" title="${dataList[i].Name}" style="white-space: nowrap; overflow: hidden; text-overflow: ellipsis;  color: black;">${dataList[i].Name}</div>
                    </div>
                `;
            }

            if (dataList.length <= 0) {

                res_Categories = `
                   <div class="text-muted">
                       No Certificates
                    </div>
                `;

                $('#section-certificates').hide();
            }

            $("#listCertificates").html(res_Categories);
            if (dataList.length > 0) {
                applySlickOnCertificates();
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

function getAllB2BSubCategories_HomePage() {
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

            var res_Categories = '<button class="button mb-3 is-checked" data-filter="*" onclick="getAllB2BBusinessesBySubCategory_HomePage(0)"> All</button>';

            var dataList = response.data;

            for (var i = 0; i < dataList.length; i++) {
                res_Categories += `
                    <button class="button mb-3" data-filter="*" onclick="getAllB2BBusinessesBySubCategory_HomePage(${dataList[i].Id})">${dataList[i].Name}</button>
                `;
            }

            if (dataList.length <= 0) {
                $('#dataB2BCategory').html('No Records!');
                //$('#section-certificates').hide();
            }

            $("#navB2BCategoryList").html(res_Categories);
            $('#navB2BCategoryList button:first-child').click();

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

function getAllB2BBusinessesBySubCategory_HomePage(subCategoryId) {
    let _url = "/api/BusinessCategory/GetAllB2BBusinessListBySubCategoryForHomePage?subCategoryId="+subCategoryId;

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

            var res_Categories = '';

            var dataList = response.data;

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

                res_Categories += `
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

            if (dataList.length <= 0) {
                res_Categories = '<div class="col-12"><div class="w-100 text-muted text-center">No Records!</div></div>';
                //$('#section-certificates').hide();
            }

            $("#dataB2BCategory").html(res_Categories);

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

function getAllSponsors_HomePage() {
    let _url = "/api/SuperAdminSponsor/GetAllSponsorsForHomePage";

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

            var _htmlData = '';

            var dataList = response.data;

            for (var i = 0; i < dataList.length; i++) {
                var _sponsorLink = (dataList[i].SponsorLink != null && dataList[i].SponsorLink != '') ? dataList[i].SponsorLink : '#';
                _htmlData += `
                    <div class="d-flex flex-column mt-2" style="width:150px">
                        <a href="${_sponsorLink}">
                            <div class="sponsor-logo-sec">
                                <img src="${dataList[i].SponsorImageWithPath}" class="sponsor-img">
                            </div>
                        </a>
                    </div>
                `;
            }

            if (dataList.length <= 0) {

                _htmlData = `
                   <div class="text-muted">
                       No Sponsors
                    </div>
                `;

                $('#section-sponsors').hide();
            }

            $("#listSponsors").html(_htmlData);
            if (dataList.length > 0) {
                applySlickOnSponsors();
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

function getTruncatedText(text, charLength) {
    var _truncatedText = text;
    if (text == null)
        return '';

    if (text.length > charLength) {
        _truncatedText = text.substring(0, charLength) + "...";
    }

    return _truncatedText;
}

function getAllTrainings_HomePage() {
    let _url = "/api/Training/GetAllTrainingsForHomePage";

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

            var _htmlData = '';

            var dataList = response.data;

            for (var i = 0; i < dataList.length; i++) {
                var _item = dataList[i];
                var _trainingTitle = getTruncatedText(_item.TrainingName, 18);
                var _trucatedDesc = getTruncatedText(_item.ShortDescription, 30);
                _htmlData += `
                    <div class="training-img">
                        <a class="hvr" href="/Home/Certification?id=${_item.Id}">
                            <img src="${_item.TrainingImageWithPath}">
                        </a>
                        <div class="text pt-3">
                            <h6>${_trainingTitle}</h6>
                            <p>${_trucatedDesc}</p>
                            <div class="text-center register-now-btn"> <a class="btn btn-masterzone" href="/Home/Certification?id=${_item.Id}" tabindex="0">Register Now</a></div>
                        </div>
                    </div>
                `;
            }

            if (dataList.length <= 0) {

                _htmlData = `
                   <div class="text-muted">
                       No Trainings
                    </div>
                `;

                $('#section-1').hide();
            }

            $("#listTrainings").html(_htmlData);
            if (dataList.length > 0) {
                applySlickOnTrainings();
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

function getAllInstructors_HomePage() {
    let _url = "/api/Business/GetAllInstructorsForHomePage";

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

            var _htmlData = '';

            var dataList = response.data;

            for (var i = 0; i < dataList.length; i++) {
                var _item = dataList[i];
                var _socialIcons = '';

                _socialIcons += (_item.FacebookProfileLink == null || _item.FacebookProfileLink == '') ? `<div class="social-inr">
                                        <a href="javascript:void(0);" class="disabled">
                                            <i class="fa-brands fa-facebook-f text-muted"></i>
                                        </a>
                                    </div>`

                    : `<div class="social-inr">
                                        <a href="${_item.FacebookProfileLink}">
                                            <i class="fa-brands fa-facebook-f"></i>
                                        </a>
                                    </div>`;

                _socialIcons += (_item.InstagramProfileLink == null || _item.InstagramProfileLink == '') ? `<div class="social-inr">
                                        <a href="javascript:void(0);"  class="disabled">
                                            <i class="fa-brands fa-instagram text-muted"></i>
                                        </a>
                                    </div>`
                    : `<div class="social-inr">
                                        <a href="${_item.InstagramProfileLink}">
                                            <i class="fa-brands fa-instagram"></i>
                                        </a>
                                    </div>`;

                _socialIcons += (_item.TwitterProfileLink == null || _item.TwitterProfileLink == '') ? `<div class="social-inr">
                                        <a href="javascript:void(0);" class="disabled">
                                            <i class="fa-brands fa-twitter text-muted"></i>
                                        </a>
                                    </div>`
                    : `<div class="social-inr">
                                        <a href="${_item.TwitterProfileLink}">
                                            <i class="fa-brands fa-twitter"></i>
                                        </a>
                                    </div>`;

                _socialIcons += (_item.LinkedInProfileLink == null || _item.LinkedInProfileLink == '') ? `<div class="social-inr">
                                        <a href="javascript:void(0);" class="disabled">
                                             <i class="fa-brands fa-linkedin-in text-muted"></i>
                                        </a>
                                    </div>`
                    : `<div class="social-inr">
                                        <a href="${_item.LinkedInProfileLink}">
                                             <i class="fa-brands fa-linkedin-in"></i>
                                        </a>
                                    </div>`;

                var ratingStars = '';
                for (var j = 1; j <= 5; j++) {
                    if (j <= _item.AverageRating) {
                        ratingStars += '<i class="fa-solid fa-star rating-star-filled"></i>';
                    } else {
                        ratingStars += `<i class="fa-solid fa-star rating-star-empty"></i>`;
                    }
                }

                var _verified = (_item.Verified == 1) ? '<i class="fas fa-check-circle text-success" title="Verified"></i>' : '';
                var _certified = (_item.IsCertified == 1) ? '<div class="badge badge-success px-3">Certified</div>' : '';

                _htmlData += `
                    <div class="yoga-items">
                        <div class="items-card">
                            <div class="yoga-card-img">
                                <img src="${_item.CoverImageWithPath}">
                            </div>
                            <div class="img-soical-icons">
                                <div class="profile-img-text">
                                    <img src="${_item.ProfileImageWithPath}">
                                    <div class="profile-name">
                                        <a target="_blank" href="/Home/BusinessProfile?businessOwnerLoginId=${_item.UserLoginId}"><h6>${_item.Name} ${_verified}</h6></a>
                                    </div>
                                    <div class="text-muted font-weight-bold text-uppercase" style="font-size:13px;">${_item.BusinessSubCategoryName}</div>
                                    <div class="mt-1">${_certified}</div>
                                    <div class="mt-2">${ratingStars}</div>
                                </div>
                                <div class="social-follow">
                                    ${_socialIcons}
                                </div>
                            </div>
                        </div>
                    </div>
                `;
            }

            if (dataList.length <= 0) {

                _htmlData = `
                   <div class="text-muted">
                       No Instructors
                    </div>
                `;

                $('#section-2').hide();
            }

            $("#listInstructors").html(_htmlData);
            if (dataList.length > 0) {
                applySlickOnInstructors();
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


function getClassCategorySections_HomePage() {
    $("#featuredCardSections").html('');

    let _url = "/api/ManageHomePage/ClassCategorySection/GetDetailForHomePage";

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

            var res_html = '';


            if (response.data.classCategorySection != null) {
                $('#section-class-category .data-card-title').html(response.data.ParentClassCategoryDetail.Name);
                for (var i = 0; i < response.data.SubCategories.length; i++) {
                    var _sub = response.data.SubCategories[i];
                    var _description = getTruncatedText(_sub.Description, 100);

                    res_html += `
                        <div class="dance-img mt-5">
                            <a href="/Home/InstructorResearch?location=&itemType=classes&categoryId=${_sub.Id}">
                                    <img src="${_sub.ImageWithPath}">
                            </a>
                            <div class="w-100 h6 mt-3 text-center" title="${_sub.Name}" style="white-space: nowrap; overflow: hidden; text-overflow: ellipsis;  color: black;">${_sub.Name}</div>
                            <div class="w-100 h6 mt-3 text-center text-muted" title="${_sub.Description}">${_description}</div>
                        </div>
                    `;
                    
                    //res_html += `
                    //    <div class="d-flex flex-column" style="width:150px">
                    //        <a href="#">
                    //            <div class="dance-img">
                    //                <img src="${_sub.ImageWithPath}" class="sponsor-img">
                    //            </div>
                    //             <div class="w-100 h6 mt-3 text-center" title="${_sub.Name}" style="white-space: nowrap; overflow: hidden; text-overflow: ellipsis;  color: black;">${_sub.Name}</div>
                    //        </a>
                    //    </div>
                    //`;
                }

                
                $('#classCategoriesList').html(res_html);

                $('#classCategoriesList').slick({
                    dots: true,
                    arrows: false,
                    infinite: false,
                    speed: 300,
                    slidesToShow: 5,
                    slidesToScroll: 1,
                    responsive: [
                        {
                            breakpoint: 1024,
                            settings: {
                                arrows: false,
                                slidesToShow: 5,
                                slidesToScroll: 5,
                                infinite: true,
                                dots: true
                            }
                        },
                        {
                            breakpoint: 600,
                            settings: {
                                arrows: false,
                                dots: true,
                                slidesToShow: 2,
                                slidesToScroll: 2
                            }
                        },
                        {
                            breakpoint: 480,
                            settings: {
                                dots: true,
                                arrows: false,
                                slidesToShow: 1,
                                slidesToScroll: 1
                            }
                        }
                    ]
                });


                $('#section-class-category').removeClass('d-none');
            }
            else {
                $('#section-class-category').addClass('d-none');
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


function getFeaturedVideos_HomePage() {
    $("#section-featured-videos .row").html('');

    let _url = "/api/ManageHomePage/FeaturedVideo/GetAllForHomePage";

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

            var res_html = '';


            if (response.data != null) {
                for (var i = 0; i < response.data.length; i++) {
                    var _item = response.data[i];
                    var _title = (_item.Title == '') ? '' : `<h4 class="my-2 px-3">${_item.Title}</h4>`;
                    var _description = (_item.Description == '') ? '' : `<p class="px-3">${_item.Description}</p>`;
                    res_html += `
                        <div class="col-sm-12 pb-2 col-md-6">
                            <div class="featured-video-card">
                                <video width="100%" height="400px" controls>
                                      <source src="${_item.VideoWithPath}" type="video/mp4">
                                    Your browser does not support the video tag.
                                </video>
                                ${_title}
                                ${_description}
                            </div>
                        </div>
                    `;
                }


                $('#section-featured-videos .row').html(res_html);

                $('#section-featured-videos').removeClass('d-none');
            }
            else {
                $('#section-featured-videos').addClass('d-none');
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


function getAllFeaturedCardSections_HomePage() {
    $("#featuredCardSections").html('');

    let _url = "/api/ManageHomePage/FeaturedCard/GetAll";

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

            var res_html = '';

            var dataCardImage = response.data.ImageCard;
            var dataCardVideo = response.data.VideoCard;

            if (dataCardImage != null) {
                if (dataCardImage.Thumbnail != '')
                    $('#featuredCardImage .home-page-image-section').css('background-image', `url(${dataCardImage.ThumbnailImageWithPath})`); // dataCardImage.Title);

                $('#featuredCardImage .data-card-title').html(dataCardImage.Title);
                $('#featuredCardImage .data-card-description').html(dataCardImage.Description);
                //$('#featuredCardImage .data-card-media').html(`
                //    <img src="${dataCardImage.ThumbnailImageWithPath}" />
                //`);

                if (dataCardImage.ButtonText != '') {
                    $('#featuredCardImage .data-card-button').html(dataCardImage.ButtonText);
                    $('#featuredCardImage .data-card-button').attr('href', dataCardImage.ButtonLink);
                }
                else {
                    $('#featuredCardImage .data-card-button').remove();
                }

                $('#featuredCardImage').removeClass('d-none');
            }
            else {
                $('#featuredCardImage').addClass('d-none');
            }

            
            if (dataCardVideo != null) {
                $('#featuredCardVideo .data-card-title').html(dataCardVideo.Title);
                $('#featuredCardVideo .data-card-description').html(dataCardVideo.Description);
                $('#featuredCardVideo .data-card-media').html(`
                    <iframe width="100%" src="${"https://www.youtube.com/embed/" + dataCardVideo.Video}"
                        title="YouTube video player" frameborder="0"
                        allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share"
                        allowfullscreen></iframe>
                `);

                if (dataCardVideo.ButtonText != '') {
                    $('#featuredCardVideo .data-card-button').html(dataCardVideo.ButtonText);
                    $('#featuredCardVideo .data-card-button').attr('href', dataCardVideo.ButtonLink);
                }
                else {
                    $('#featuredCardVideo .data-card-button').remove();
                }


                $('#featuredCardVideo').removeClass('d-none');
            }
            else {
                $('#featuredCardVideo').addClass('d-none');
            }



            $("#featuredCardSections").html(res_html);

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


function getAllMultipleItemSections_HomePage() {
    $("#featuredCardSections").html('');

    let _url = "/api/ManageHomePage/MultipleItem/GetAllForHomePage";

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


            if (response.data.MultipleImageList != null) {
                bindMultipleImageItemSection(response.data.MultipleImageList)
            }
            
            if (response.data.MultipleVideoList != null) {
                bindMultipleVideoItemSection(response.data.MultipleVideoList)
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

function bindMultipleImageItemSection(dataList) {
    var _htmlContent = '';

    for (var i = 0; i < dataList.length; i++) {
        var _item = dataList[i];
        //var _truncatedDesc = getTruncatedText(_item.Description, 200);
            //<div class="col-sm-12 pb-2 col-md-3 mb-3 ">
        _htmlContent += `
            <div class="mx-2 mt-5">
                <div class="d-flex flex-column text-center">
                    <a href="${_item.Link}">
                        <div class="img" style="background: url('${_item.ThumbnailImageWithPath}') no-repeat center center /cover;">
                        </div>
                    </a>
                    <a href="${_item.Link}">
                        <h4 class="my-3 title">${_item.Title}</h4>
                    </a>
                    <div class="">${_item.Description}</div>
                </div>
            </div>
        `;
    }

    $('#multipleImagesList').html(_htmlContent);

    if (dataList.length > 0) {
        $('#multipleImagesList').slick({
            dots: true,
            arrows: false,
            infinite: false,
            speed: 300,
            slidesToShow: 4,
            slidesToScroll: 4,
            responsive: [
                {
                    breakpoint: 1024,
                    settings: {
                        arrows: false,
                        slidesToShow: 4,
                        slidesToScroll: 1,
                        infinite: true,
                        dots: true
                    }
                },
                {
                    breakpoint: 600,
                    settings: {
                        dots: true,
                        arrows: false,
                        slidesToShow: 2,
                        slidesToScroll: 1
                    }
                },
                {
                    breakpoint: 480,
                    settings: {
                        dots: true,
                        arrows: false,
                        slidesToShow: 1,
                        slidesToScroll: 1
                    }
                }
            ]
        });
    }
}

function bindMultipleVideoItemSection(dataList) {
    var _htmlContent = '';

    for (var i = 0; i < dataList.length; i++) {
        var _item = dataList[i];
        //var _truncatedDesc = getTruncatedText(_item.Description, 200);
        _htmlContent += `
            <div class="mx-2 mt-5">
                <a href="${_item.Link}">
                    <div class="position-relative">
                        <div class="img" style="background: url('${_item.ThumbnailImageWithPath}') no-repeat center center /cover;">
                        </div>
                        <div class="d-flex flex-column py-3 px-3 cstm-overlay-block">
                            <div class="font-weight-bold">${_item.Title}</div>
                            <div class="text-uppercase">${_item.Description}</div>
                        </div>
                    </div>
                </a>
            </div>
        `;
    }

    $('#multipleVideosList').html(_htmlContent);

    if (dataList.length > 0) {
        $('#multipleVideosList').slick({
            dots: true,
            arrows: false,
            infinite: false,
            speed: 300,
            slidesToShow: 3,
            slidesToScroll: 1,
            responsive: [
                {
                    breakpoint: 1024,
                    settings: {
                        arrows: false,
                        slidesToShow: 3,
                        slidesToScroll: 1,
                        infinite: true,
                        dots: true
                    }
                },
                {
                    breakpoint: 600,
                    settings: {
                        dots: true,
                        arrows: false,
                        slidesToShow: 1,
                        slidesToScroll: 1
                    }
                },
                {
                    breakpoint: 480,
                    settings: {
                        dots: true,
                        arrows: false,
                        slidesToShow: 1,
                        slidesToScroll: 1
                    }
                }
            ]
        });
    }
}

function getAllActiveParentBusinessCategories() {
    let _url = "/api/BusinessCategory/Parent/GetAllActive";

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

            var res_Categories = '';
            // --------------------- append parent categories in dropdown

            for (var i = 0; i < response.data.length; i++) {
                //res_Categories += '<option value="' + response.data[i].Id + '">' + response.data[i].Name + '</option>';
                res_Categories += `
                    <div class="link-cards">
                        <img src="${response.data[i].CategoryImageWithPath}">
                        <a href="/Home/Category?categoryId=${response.data[i].Id}">${response.data[i].Name}</a>
                    </div>
                `;
            }

            res_Categories += `
                <div class="link-cards link-cards-jkljlkkl text-center">
                    <a href="category.html " class="s-jkljlkkl>More Categories">More Categories</a>
                </div>
            `;
            $("#categoryLinkList").html('').append(res_Categories);
            // --------------------- append parent categories in dropdown

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



//////------------------------GEO Location------------------------------------------------
var locationErrorMessage = "";

function getLocation() {

    if (navigator.geolocation) {
        var options = {
            // enableHighAccuracy: true,
            // timeout: 20000,
            // maximumAge: 120000,
            // showLocationDialog: true
        };
        navigator.geolocation.getCurrentPosition(setCurrentLocation, geoError, options);
        $('#txtSearchKeywords_Search').val('Current Location');
    } else {
        showLocationErrorMessage("Geolocation is not supported by this browser.");
        StopLoading();
    }
}

function geoError(error) {
    switch (error.code) {
        case error.PERMISSION_DENIED:
            locationErrorMessage = "User denied the request for Geolocation."
            break;
        case error.POSITION_UNAVAILABLE:
            locationErrorMessage = "Location information is unavailable."
            break;
        case error.TIMEOUT:
            locationErrorMessage = "The request to get user location timed out."
            break;
        case error.UNKNOWN_ERROR:
            locationErrorMessage = "An unknown error occurred."
            break;
    }
    showLocationErrorMessage(locationErrorMessage);
    StopLoading();
}

function setCurrentLocation(position) {
    var logMessage = 'got position! \n';
    logMessage += "Latitude: " + position.coords.latitude +
        " \n Longitude: " + position.coords.longitude + " \n Timestamp: " + position.timestamp + " \n DateTime: " + new Date(position.timestamp);
    console.log = logMessage;

    SearchParameters_Global.Latitude = position.coords.latitude;
    SearchParameters_Global.Longitude = position.coords.longitude;
    SearchParameters_Global.SearchBy = arrSearchBy[2];
}

function showLocationErrorMessage(message) {
    $.iaoAlert({
        msg: message,
        type: "error",
        mode: "dark",
    });
}
//////------------------------ GEO Location ------------------------------------------------

// SEARCH FUNCITONALITY STARTS -----------------------------------------------------------------------

function onChangeSearchBySelection() {
    var _checkboxChecked = $('#checkboxSearchBy').is(':checked');
    if (_checkboxChecked) {
        //console.log('by name!');
        SearchParameters_Global.SearchBy = arrSearchBy[0];
        $('#btnCurrentLocation').addClass('d-none');
    }
    else {
        //console.log('by location!');
        SearchParameters_Global.SearchBy = arrSearchBy[1];
        $('#btnCurrentLocation').removeClass('d-none');
    }
}

function setSearchByInputValue() {
    var _location = $('#txtLocation_Search').val();
    var _search = $('#txtSearchKeywords_Search').val();

    if (_location != '') {
        SearchParameters_Global.SearchBy = arrSearchBy[1];
        return 1;
    }
    else if (_search != '') {
        SearchParameters_Global.SearchBy = arrSearchBy[0];
        return 1;
    }
    else {
        return -1;
    }
}

function searchClasses() {
    var _isValid = true;

    var _location = $('#txtLocation_Search').val();
    var _categoryId = $('#ddlFilterTypeCategory').val();
    var _days = $("#ddlFilterDays").val();

    if (_isValid) {
        StartLoading();
        window.location.href = `/Home/InstructorResearch?location=${_location}&itemType=classes&categoryId=${_categoryId}&days=${_days}`;
        StopLoading();
    }

}

function SearchBusinessOwners() {
    var status = setSearchByInputValue();

    if (status == -1) {
        $.iaoAlert({
            msg: "Please input values in search fields!",
            type: "error",
            mode: "dark",
        });
        return;
    }

    // if search-By Name
    if (SearchParameters_Global.SearchBy == arrSearchBy[0]) {
        searchByName();
    }
    // search-By Location
    else {
        searchByLocation();
    }
}

function searchByName() {

    var _isValid = true;
    var _serachKeywords = $('#txtSearchKeywords_Search').val();
    if (_serachKeywords == "") {
        _isValid = false;
        $.iaoAlert({
            msg: "Please enter business name!",
            type: "error",
            mode: "dark",
        });
    }
    else {
        SearchParameters_Global.SearchKeyword = _serachKeywords;
    }

    if (_isValid) {
        StartLoading();
        window.location.href = `/Home/Search?searchKeyword=${SearchParameters_Global.SearchKeyword}&searchBy=${SearchParameters_Global.SearchBy}`
        StopLoading();
    }
}

function searchByLocation() {

    var _isValid = true;

    var _searchKeywords = $('#txtLocation_Search').val();

    if (_searchKeywords == "") {
        _isValid = false;
        $.iaoAlert({
            msg: "Please enter location!",
            type: "error",
            mode: "dark",
        });
    }
    else {
        SearchParameters_Global.SearchKeyword = _searchKeywords;
    }

    if (_searchKeywords == "Current Location")
        SearchParameters_Global.SearchBy = arrSearchBy[2];
    else
        SearchParameters_Global.SearchBy = arrSearchBy[1];

    // Search by location keywords
    if (SearchParameters_Global.SearchBy == arrSearchBy[1]) {

        if (_isValid) {
            StartLoading();
            window.location.href = `/Home/Search?searchKeyword=${SearchParameters_Global.SearchKeyword}&searchBy=${SearchParameters_Global.SearchBy}`;
            StopLoading();
        }
    }
    // search by current location
    else if (SearchParameters_Global.SearchBy == arrSearchBy[2]) {

        if (SearchParameters_Global.Lattitude == "" || SearchParameters_Global.Longitude == "") {
            _isValid = false;
            $.iaoAlert({
                msg: "Your current location is not set!",
                type: "error",
                mode: "dark",
            });
        }

        if (_isValid) {
            StartLoading();
            window.location.href = `/Home/Search?searchBy=${SearchParameters_Global.SearchBy}&latitude=${SearchParameters_Global.Latitude}&longitude=${SearchParameters_Global.Longitude}`;
            StopLoading();
        }
    }

}

// SEARCH FUNCITONALITY ENDS -----------------------------------------------------------------------

// logo slider
function applySlickOnCertificates() {

    $('#listCertificates').slick({
        dots: true,
        arrows: false,
        infinite: false,
        speed: 300,
        slidesToShow: 10,
        slidesToScroll: 10,
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    arrows: false,
                    slidesToShow: 3,
                    slidesToScroll: 1,
                    infinite: true,
                    dots: true
                }
            },
            {
                breakpoint: 600,
                settings: {
                    dots: true,
                    arrows: false,
                    slidesToShow: 3,
                    slidesToScroll: 1
                }
            },
            {
                breakpoint: 480,
                settings: {
                    dots: true,
                    arrows: false,
                    slidesToShow: 2,
                    slidesToScroll: 1
                }
            }
        ]
    });
}

function applySlickOnSponsors() {

    $('#listSponsors').slick({
        dots: true,
        arrows: false,
        infinite: false,
        speed: 300,
        slidesToShow: 6,
        slidesToScroll: 6,
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    arrows: false,
                    slidesToShow: 3,
                    slidesToScroll: 1,
                    infinite: true,
                    dots: true
                }
            },
            {
                breakpoint: 600,
                settings: {
                    dots: true,
                    arrows: false,
                    slidesToShow: 2,
                    slidesToScroll: 1
                }
            },
            {
                breakpoint: 480,
                settings: {
                    dots: true,
                    arrows: false,
                    slidesToShow: 1,
                    slidesToScroll: 1
                }
            }
        ]
    });
}

function applySlickOnTrainings() {
    $('#listTrainings').slick({
        dots: true,
        arrows: false,
        infinite: false,
        speed: 300,
        slidesToShow: 4,
        slidesToScroll: 4,
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    arrows: false,
                    slidesToShow: 3,
                    slidesToScroll: 1,
                    infinite: true,
                    dots: true
                }
            },
            {
                breakpoint: 600,
                settings: {
                    dots: true,
                    arrows: false,
                    slidesToShow: 2,
                    slidesToScroll: 1
                }
            },
            {
                breakpoint: 480,
                settings: {
                    dots: true,
                    arrows: false,
                    slidesToShow: 1,
                    slidesToScroll: 1
                }
            }
        ]
    });

}

function applySlickOnInstructors() {
    $('#listInstructors').slick({
        dots: true,
        arrows: false,
        infinite: false,
        speed: 300,
        slidesToShow: 5,
        slidesToScroll: 1,
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    arrows: false,
                    slidesToShow: 3,
                    slidesToScroll: 1,
                    infinite: true,
                    dots: true
                }
            },
            {
                breakpoint: 600,
                settings: {

                    dots: true,
                    arrows: false,
                    slidesToShow: 2,
                    slidesToScroll: 1
                }
            },
            {
                breakpoint: 480,
                settings: {

                    dots: true,
                    arrows: false,
                    slidesToShow: 1,
                    slidesToScroll: 1
                }
            }
        ]
    });
}

// external js: isotope.pkgd.js

// init Isotope
var $grid = $('.grid').isotope({
    itemSelector: '.element-item',
    layoutMode: 'fitRows'
});
// filter functions
var filterFns = {};
// bind filter button click
$('.filters-button-group').on('click', 'button', function () {
    var filterValue = $(this).attr('data-filter');
    // use filterFn if matches value
    filterValue = filterFns[filterValue] || filterValue;
    $grid.isotope({ filter: filterValue });
});
// change is-checked class on buttons
$('.button-group').each(function (i, buttonGroup) {
    var $buttonGroup = $(buttonGroup);
    $buttonGroup.on('click', 'button', function () {
        $buttonGroup.find('.is-checked').removeClass('is-checked');
        $(this).addClass('is-checked');
    });
});


function GettAllEventList() {
    //var id = $("#hiddenEventId").val();
    //let _url = "/api/Event/GetAllList";
    let _url = "/api/Event/GetAllEventsForHomePage";

    StartLoading();
    $.ajax({
        type: "GET",
        url: _url,
        //headers: {
        //    "Authorization": "Bearer " + UserToken_Global,
        //    "Content-Type": "application/json"
        //},
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
            //for (var i = 0; i < response.data.EventList.length; i++) {
            //var dataLength = Math.min(response.data.length, 15);
            for (var i = 0; i < response.data.length; i++) {
                var Event = response.data[i];
                var _shortDescription = getTruncatedText(Event.ShortDescription, 50);
                var eventHtml = `
                <div class="events-img">
                    <a href = "/Home/Event?eventId=${Event.Id}" class="text-decoration-none">
                        <img src="${Event.EventImageWithPath}" >
                        <div class="w-100 h6 mt-3 text-center" title="${Event.Title}" style="white-space: nowrap; overflow: hidden; text-overflow: ellipsis;  color: black;">${Event.Title}</div>
                        <div class="w-100 mt-3 text-center text-muted" title="${_shortDescription}">${_shortDescription}</div>
                    </a>
                </div> `;

                $("#GetAllEventList").append(eventHtml);
            }

            if (response.data.length <= 0) {
                $("#GetAllEventList").html(`
                   <div class="text-muted">
                       No Events
                    </div>
                `);

                $('#section-3').hide();
            }
            else {

                $('.events-slider .events-slide').slick({
                    dots: true,
                    arrows: false,
                    infinite: false,
                    speed: 300,
                    slidesToShow: 5,
                    slidesToScroll: 2,
                    responsive: [
                        {
                            breakpoint: 1024,
                            settings: {
                                arrows: false,
                                slidesToShow: 3,
                                slidesToScroll: 1,
                                infinite: true,
                                dots: true
                            }
                        },
                        {
                            breakpoint: 600,
                            settings: {
                                dots: true,
                                arrows: false,
                                slidesToShow: 2,
                                slidesToScroll: 1
                            }
                        },
                        {
                            breakpoint: 480,
                            settings: {
                                dots: true,
                                arrows: false,
                                slidesToShow: 1,
                                slidesToScroll: 1
                            }
                        }
                    ]
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

function GetAllActiveBusinessPlans() {
    //var _url = "/api/BusinessPlan/GetAllForUser";
    var _url = "/api/SuperAdminPlan/GetAllActivePlanList";

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

            $('#sectionAllPackagesList').html('');
            var cardsData = "";
            //for (var i = 0; i < response.data.length; i++) {
            var dataLength = Math.min(response.data.length, 15);
            for (var i = 0; i < dataLength; i++) {
                cardsData += bindPlanDataToCard(response.data[i]);
            }

            if (response.data.length <= 0) {
                cardsData += `<div class="w-100 text-center"><i> No Plan/Packages </i><div>`;
            }

            $('#sectionAllPackagesList').html(cardsData);

            if (response.data.length > 0) {
                $('#sectionAllPackagesList').slick({
                    dots: true,
                    rows: 1,
                    arrows: false,
                    infinite: false,
                    speed: 300,
                    slidesToShow: 3,
                    slidesToScroll: 3,
                    responsive: [
                        {
                            breakpoint: 1024,
                            settings: {
                                arrows: false,
                                slidesToShow: 3,
                                slidesToScroll: 1,
                                infinite: true,
                                dots: true
                            }
                        },
                        {
                            breakpoint: 600,
                            settings: {
                                dots: true,
                                arrows: false,
                                slidesToShow: 2,
                                slidesToScroll: 1
                            }
                        },
                        {
                            breakpoint: 480,
                            settings: {
                                dots: true,
                                arrows: false,
                                slidesToShow: 1,
                                slidesToScroll: 1
                            }
                        }
                    ]
                });
            }

            //StopLoading();
        },
        error: function (result) {
            //StopLoading();

            if (result["status"] == 401) {
                $.iaoAlert({
                    msg: 'Unauthorized !',
                    type: "error",
                    mode: "dark",
                });
            }
            else {
                $.iaoAlert({
                    msg: 'There is some technical error!',
                    type: "error",
                    mode: "dark",
                });
            }
        }
    });
}

function convertToPlain2(html) {

    // Create a new div element
    var tempDivElement = document.createElement("div");

    // Set the HTML content with the given value
    tempDivElement.innerHTML = html;

    // Retrieve the text property of the element 
    return tempDivElement.textContent || tempDivElement.innerText || "";
}

function convertToPlain(_html) {
    //$('#planDescriptionContent').html(_html);
    document.getElementById('planDescriptionContent').innerHTML = _html;
    el = document.getElementById('planDescriptionContent');

    var sel, range, innerText = "";
    if (typeof document.selection != "undefined" && typeof document.body.createTextRange != "undefined") {
        range = document.body.createTextRange();
        range.moveToElementText(el);
        innerText = range.text;
    } else if (typeof window.getSelection != "undefined" && typeof document.createRange != "undefined") {
        sel = window.getSelection();
        sel.selectAllChildren(el);
        innerText = "" + sel;
        sel.removeAllRanges();
    }

    $('#planDescriptionContent').html('');
    //tempDivElement.remove();
    return innerText;
}


function bindPlanDataToCard(businessPlan) {
    var compareAtPriceHTML = (businessPlan.CompareAtPrice <= 0) ? '' : `<p class="h5 mb-2 text-danger"><strike>Rs. ${businessPlan.CompareAtPrice}</strike></p>`;
    var truncatedDescription = convertToPlain(businessPlan.Description);
    var maxLength = 150;
    if (truncatedDescription.length > maxLength) {
        truncatedDescription = truncatedDescription.substr(0, maxLength) + '...';
    }

    var cardData = `
        <div class="mx-2 mt-3">
                <div class="card">
                    <div class="card-body pt-5 pb-5 d-flex flex-column justify-content-around text-center w-100" style="width:250px;height:400px;">
                        
                        <div class="price-top-part">
                            <img src="${businessPlan.PlanImageWithPath}" class="img-size-md object-fit-contain m-auto"/>
                            <!--<i class="iconsminds-male large-icon"></i>-->
                            <h5 class="mb-0 font-weight-semibold color-theme-1 mb-4">${businessPlan.Name}</h5>
                            ${compareAtPriceHTML}
                            <p class="text-large mb-2 text-default">Rs. ${businessPlan.Price}</p>
                            <p class="text-muted text-small">${businessPlan.PlanDurationTypeKeyName}</p>
                        </div>
                        <div class="pl-3 pr-3 pt-3 pb-0 d-flex price-feature-list flex-column flex-grow-1">
                            <ul class="list-unstyled">
                                <li>
                                    <p class="mb-0 ">
                                        ${truncatedDescription}
                                    </p>
                                </li>
                            </ul>
                        </div>
                        <div class="align-self-content-end">
                            <a target="_blank" href="/Business/Login" class="btn btn-md btn-outline-info">Buy Now</a>
                        </div>
                    </div>
                </div>
            </div>
    `;
    
    //return cardData;

    var _newCard = `<div class="member-item mt-3">
            <div class="member-img">
                <img src="${businessPlan.PlanImageWithPath}" alt="img" loading="lazy">
            </div>
            <div class="pricing-title one">
                <h5>${businessPlan.Name}</h5>
            </div>
            <div class="pricing-info">
                <ul>
                    <li>${truncatedDescription}</li>
                </ul>
            </div>
            <div class="price-amount">
                <h2>${businessPlan.Price} /- <span class="text-uppercase">${businessPlan.PlanDurationTypeKeyName}</span></h2>
            </div>
            <div class="sign-btn">
                
               <a href="/Booking/Checkout?itemId=${businessPlan.Id}&itemType=userplan">Buy Now</a>
            </div>
        </div>

    `;
    
    return _newCard;

    //return _newCard + _newCard + _newCard;
}


// Business Login Check 
//function CheckBusinessLogin(Id) {
//    $("#btnBusinessUserLoginPoup").click();
//    if (UserToken_Global != "") {
//        window.location.href = ' <a href="/Booking/Checkout?itemId=7&itemType=Class">Buy Now</a>?itemId=' +Id;
//    }

//}




function getAllSubClassCategoryTypesForHomePage() {
    //let _url = "/api/Class/GetAllList";
    //let _url = "/api/Class/GetAllClassessForHomePage";
    let _url = "/api/ClassCategoryType/GetAllClassSubCategoriesForHomePage";

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

            var classHtml = '';


            for (var i = 0; i < response.data.length; i++) {
                var Class = response.data[i];
                var classDescription = Class.Description;
                if (classDescription.length > 50) {

                    classDescription = classDescription.substring(0, 50) + "...";
                } else {
                    classDescription;
                }

                classHtml = `
                    <a target="_blank" href="/Home/InstructorResearch?location=&itemType=classes&categoryId=${Class.Id}">
                        <div class="card mx-2 mb-2 mt-3 class-detail-card">
                            <div class="card-body w-100">
                                <div class="d-flex flex-column text-center w-100 h-100 justify-content-around">
                                    <img src="${Class.ImageWithPath}" class="mb-3"/>
                                    <h4 class="font-weight-semibold color-theme-1 item-title">${Class.Name}</h4>
                                    <p class="flex-grow-1 item-description my-1">
                                        ${classDescription}
                                    </p>
                                    <!--<div class="align-self-content-end">
                                        <a target="_blank" href="#"  class="btn btn-md cstm-btn-bottom">View</a>
                                    </div>-->
                                </div>
                            </div>
                        </div>  
                    </a>
                `;

                $("#GetAllSheduleClassesList").append(classHtml);
            }

            if (response.data.length <= 0) {

                var _dataEmpty = `
                   <div class="text-muted">
                       No Instructors
                    </div>
                `;
                $('#GetAllSheduleClassesList').html(_dataEmpty);
                $('#section-classes').hide();
            }
            else {
                
                $('#GetAllSheduleClassesList').slick({
                    dots: true,
                    rows: 1,
                    arrows: false,
                    infinite: false,
                    speed: 300,
                    slidesToShow: 4,
                    slidesToScroll: 4,
                    responsive: [
                        {
                            breakpoint: 1024,
                            settings: {
                                arrows: false,
                                slidesToShow: 3,
                                slidesToScroll: 1,
                                infinite: true,
                                dots: true
                            }
                        },
                        {
                            breakpoint: 600,
                            settings: {
                                arrows: false,
                                dots: true,
                                slidesToShow: 2,
                                slidesToScroll: 2
                            }
                        },
                        {
                            breakpoint: 480,
                            settings: {
                                dots: true,
                                arrows: false,
                                slidesToShow: 1,
                                slidesToScroll: 1
                            }
                        }
                    ]
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


function getAllClassesListsForSearching() {

    let _url = "/api/Class/GetAllList";

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

            $("#ddl_AllClassesList").html('');
            var _selectList = '<option value="0">All Classes</option>';
            for (var i = 0; i < response.data.ClassList.length; i++) {
                var Class = response.data.ClassList[i];
                _selectList += '<option value="' + Class.Id + '">' + Class.Name + '</option>';
            }
            $("#ddl_AllClassesList").html('').append(_selectList); //.select2();

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


function getAllExamFormList() {
    let _url = "/api/Exam/GetAllExamFormList";

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

            var ExamHtml = '';


            //for (var i = 0; i < response.data.length; i++) {
            var dataLength = Math.min(response.data.length, 15);
            for (var i = 0; i < dataLength; i++) {
                var Exam = response.data[i];
                var ExamDescription = "...";
                if (ExamDescription.length > 50) {

                    ExamDescription = ExamDescription.substring(0, 50) + "...";
                } else {
                    ExamDescription;
                }
                ExamHtml = `
<div class="mx-2 mb-2" style="float:left; ">
                        <div class="card">
                            <div class="card-body pt-5 pb-5 d-flex flex-lg-column flex-md-row flex-sm-row flex-column text-center" style="Width:250px;height: 300px;">
                                <div class="price-top-part">
                                    <h5 class="mb-0 font-weight-semibold color-theme-1 mb-4">${Exam.Title}</h5>
                                    <p class="text-large mb-2 text-default">0.0 Rs</p>
                                   
                                </div>
                                <div class="pl-3 pr-3 pt-3 pb-0 d-flex price-feature-list flex-column flex-grow-1">
                                    <ul class="list-unstyled">
                                        <li>
                                            <p class="mb-0 ">
                                               ${ExamDescription}
                                            </p>
                                        </li>
                                    </ul>
                                    <div class="d-flex justify-content-around">
                                        <a target="_blank" href="/Home/SubmitExamForm?examId=${Exam.Id}" class="btn btn-md btn-outline-info">View Details</a>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
</div>
`;

                $("#GetAllExamFormList").append(ExamHtml);
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

// -------------------------------- Banner slider ----------------------------

var slideWrapper = $(".main-slider"),
    iframes = slideWrapper.find('.embed-player'),
    lazyImages = slideWrapper.find('.slide-image'),
    lazyCounter = 0;


// POST commands to YouTube or Vimeo API
function postMessageToPlayer(player, command) {
    if (player == null || command == null) return;
    player.contentWindow.postMessage(JSON.stringify(command), "*");
}

// When the slide is changing
function playPauseVideo(slick, control) {
    var currentSlide, slideType, startTime, player, video;

    currentSlide = slick.find(".slick-current");
    slideType = currentSlide.attr("class").split(" ")[1];
    player = currentSlide.find("iframe").get(0);
    startTime = currentSlide.data("video-start");

    //if (slideType === "vimeo") {
    //    switch (control) {
    //        case "play":
    //            if ((startTime != null && startTime > 0) && !currentSlide.hasClass('started')) {
    //                currentSlide.addClass('started');
    //                postMessageToPlayer(player, {
    //                    "method": "setCurrentTime",
    //                    "value": startTime
    //                });
    //            }
    //            postMessageToPlayer(player, {
    //                "method": "play",
    //                "value": 1
    //            });
    //            break;
    //        case "pause":
    //            postMessageToPlayer(player, {
    //                "method": "pause",
    //                "value": 1
    //            });
    //            break;
    //    }
    //} else

        if (slideType === "youtube") {
        switch (control) {
            case "play":
                postMessageToPlayer(player, {
                    "event": "command",
                    "func": "mute"
                });
                postMessageToPlayer(player, {
                    "event": "command",
                    "func": "playVideo"
                });
                break;
            case "pause":
                postMessageToPlayer(player, {
                    "event": "command",
                    "func": "pauseVideo"
                });
                break;
        }
    }
    //else if (slideType === "video") {
    //    video = currentSlide.children("video").get(0);
    //    if (video != null) {
    //        if (control === "play") {
    //            video.play();
    //        } else {
    //            video.pause();
    //        }
    //    }
    //}
}

// Resize player
function resizePlayer(iframes, ratio) {
    if (!iframes[0]) return;
    var win = $(".main-slider"),
        width = win.width(),
        playerWidth,
        height = win.height(),
        playerHeight,
        ratio = ratio || 16 / 9;

    iframes.each(function () {
        var current = $(this);
        if (width / ratio < height) {
            playerWidth = Math.ceil(height * ratio);
            current.width(playerWidth).height(height).css({
                left: (width - playerWidth) / 2,
                top: 0
            });
        } else {
            playerHeight = Math.ceil(width / ratio);
            current.width(width).height(playerHeight).css({
                left: 0,
                top: (height - playerHeight) / 2
            });
        }
    });
}

//// DOM Ready
$(function () {
    //// Initialize
    //slideWrapper.on("init", function (slick) {
    //    slick = $(slick.currentTarget);
    //    setTimeout(function () {
    //        playPauseVideo(slick, "play");
    //    }, 1000);
    //    resizePlayer(iframes, 16 / 9);
    //});

    slideWrapper.on("beforeChange", function (event, slick) {
        slick = $(slick.$slider);
        playPauseVideo(slick, "pause");
    });
    slideWrapper.on("afterChange", function (event, slick) {
        slick = $(slick.$slider);
        playPauseVideo(slick, "play");
    });
    slideWrapper.on("lazyLoaded", function (event, slick, image, imageSource) {
        lazyCounter++;
        if (lazyCounter === lazyImages.length) {
            lazyImages.addClass('show');
            // slideWrapper.slick("slickPlay");
        }
    });

    ////start the slider
    //slideWrapper.slick({
    //    // fade:true,
    //    // autoplay: true,
    //    autoplaySpeed: 4000,
    //    // lazyLoad:"progressive",
    //    speed: 600,
    //    arrows: true,
    //    dots: false,
    //    cssEase: "cubic-bezier(0.87, 0.03, 0.41, 0.9)"
    //});
});

// Resize event
$(window).on("resize.slickVideoPlayer", function () {
    resizePlayer(iframes, 16 / 9);
});

function getAllBannerSlides() {
    let _url = "/api/ManageHomePage/BannerItem/GetAllForHomePage";

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

            var _html = '';


            var dataLength = response.data.length;
            for (var i = 0; i < dataLength; i++) {
                var _item = response.data[i];

                var _caption = (_item.Text == null || _item.Text == '') ? `` : `<figcaption class="caption h4">${_item.Text}</figcaption></a>`;

                if (_item.Link == null || _item.Link == "")
                { }
                else {
                    _caption = `<a target="_blank" href="${_item.Link}">` + _caption + `</a>`;
                }

                if (_item.Type == "Image") {
                    _html += `
                        <div class="item image">
                            <figure>
                                <div class="slide-image slide-media" style="background-image:url('${_item.ImageWithPath}');">
                                    <img data-lazy="${_item.ImageWithPath}" class="image-entity" />
                                </div>
                                ${_caption}
                            </figure>
                        </div>
                    `;
                }
                else if (_item.Type == "Video") {
                    _html += `
                        <div class="item youtube">
                            <iframe class="embed-player slide-media" width="980" height="520" src="${_item.Video}?enablejsapi=1&controls=0&fs=0&iv_load_policy=3&rel=0&showinfo=0&loop=1&start=1" frameborder="0" allowfullscreen></iframe>
                            ${_caption}
                        </div>
                    `;
                }

            }

            $(".main-slider").html('').append(_html);

            iframes = slideWrapper.find('.embed-player'),
                lazyImages = slideWrapper.find('.slide-image'),
                lazyCounter = 0;

            // Initialize
            slideWrapper.on("init", function (slick) {
                slick = $(slick.currentTarget);
                setTimeout(function () {
                    playPauseVideo(slick, "play");
                }, 1000);
                resizePlayer(iframes, 16 / 9);
            });
            

            //start the slider
            slideWrapper.slick({
                // fade:true,
                // autoplay: true,
                autoplaySpeed: 4000,
                // lazyLoad:"progressive",
                speed: 600,
                arrows: true,
                dots: false,
                cssEase: "cubic-bezier(0.87, 0.03, 0.41, 0.9)"
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

function GetAllClassCategoryTypeList() {

    let _url = "/api/ClassCategoryType/GetAllClasssCategoryDetailDropDown";

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
                    msg: '@(Resources.ErrorMessage.UnAuthorizedErrorMessage)',
                    type: "error",
                    mode: "dark",
                });
                return;
            }

            var res_Categories = '<option value="0">Select Class category</option>';

            for (var parentCategoryId in response.data) {
                res_Categories += '<optgroup label="' + response.data[parentCategoryId].Name + '">';

                response.data[parentCategoryId].SubClassCategory.forEach(function (item) {
                    res_Categories += '<option value="' + item.Id + '">' + item.Name + '</option>';
                });

                res_Categories += '</optgroup>';
            }

            $("#ddlFilterTypeCategory").html('').append(res_Categories);

        },
        error: function (result) {
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

function initMap() {

    const input = document.getElementById("txtLocation_Search");
    const options = {
        types: ["(regions)"],
        //fields: ["formatted_address", "geometry", "name"],
        strictBounds: false,
    };

    const autocomplete = new google.maps.places.Autocomplete(input, options);

    const infowindow = new google.maps.InfoWindow();
    const infowindowContent = document.getElementById("infowindow-content");

    infowindow.setContent(infowindowContent);

    autocomplete.addListener("place_changed", () => {
        infowindow.close();

        const place = autocomplete.getPlace();

        //if (!place.geometry || !place.geometry.location) {
        //    // User entered the name of a Place that was not suggested and
        //    // pressed the Enter key, or the Place Details request failed.
        //    window.alert("No details available for input: '" + place.name + "'");
        //    return;
        //}

        //infowindowContent.children["place-name"].textContent = place.name;
        //infowindowContent.children["place-address"].textContent =
        //    place.formatted_address;
        console.log(place);
    });

    //// Sets a listener on a radio button to change the filter type on Places
    //// Autocomplete.
    //function setupClickListener(id, types) {
    //    const radioButton = document.getElementById(id);

    //    radioButton.addEventListener("click", () => {
    //        autocomplete.setTypes(types);
    //        input.value = "";
    //    });
    //}

    //setupClickListener("changetype-all", []);
    //setupClickListener("changetype-address", ["address"]);
    //setupClickListener("changetype-establishment", ["establishment"]);
    //setupClickListener("changetype-geocode", ["geocode"]);
    //setupClickListener("changetype-cities", ["(cities)"]);
    //setupClickListener("changetype-regions", ["(regions)"]);

}

window.initMap = initMap;
