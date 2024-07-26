var UserToken_Global_Layout = "";

$(document).ready(function () {
    $(".commonNavLinkClass").parent('li').removeClass("active");

    $('#li_LoginSignUp').removeClass('d-none');
    $('#loginUser_HomeLayout').addClass('d-none');

    $.get("/Home/GetStudentToken", null, function (dataAdminToken) {
        if (dataAdminToken != "" && dataAdminToken != null) {

            UserToken_Global_Layout = dataAdminToken;

            //--Set Active Link in Sidebar
            //SetSideBarLink();

            ////--Get Student-Profile Info
            GetStudentProfileDetail_Layout();
            GetPlanDetailsForShowDate();
        }
        else {
        }
    });
});

function GetStudentProfileDetail_Layout() {
    //$("#login_profileImage").attr("src", "");
    //$("#login_userName").html("");
    //$("#login_email").html("");
    
    $(".login_profileImage").attr("src", "");
    $(".login_userName").html("");
    $(".login_email").html("");

    $.ajax({
        type: "GET",
        url: "/api/Student/GetBasicProfileDetail",
        headers: {
            "Authorization": "Bearer " + UserToken_Global_Layout,
            "Content-Type": "application/json"
        },
        contentType: 'application/json',
        success: function (response) {
            if (response.data != null) {
                var fullName = response.data.FirstName + " " + response.data.LastName;
                //$("#login_profileImage").attr("src", response.data.ProfileImageWithPath);
                //$("#login_userName").html(fullName);
                //$("#login_email").html(response.data.Email);
                $(".login_profileImage").attr("src", response.data.ProfileImageWithPath);
                $(".login_userName").html(fullName);
                $(".login_email").html(response.data.Email);

                var loginUserSection_HomeLayout = $('#loginUser_HomeLayout');
                if (loginUserSection_HomeLayout.length > 0) {
                    $('#li_LoginSignUp').addClass('d-none');
                    loginUserSection_HomeLayout.removeClass('d-none');
                    loginUserSection_HomeLayout.find('a').html(fullName);
                }
                else {
                    $('#li_LoginSignUp').removeClass('d-none');
                    $('#loginUser_HomeLayout').addClass('d-none');
                }
            }
            //--Set Active Link in Sidebar
            SetSideBarLink();
        },
        error: function (result) {

            //--Set Active Link in Sidebar
            SetSideBarLink();

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
function GetPlanDetailsForShowDate() {
    
    $.ajax({
        type: "GET",
        url: '/api/Student/GetPlanDetailsForDate',
        headers: {
            "Authorization": "Bearer " + UserToken_Global_Layout,
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
            $(".planexpiredays").html('');

            if (response.data.length <= 0) {
                $(".planexpiredays").html('');
                StopLoading();
                return;
            }

            for (var i = 0; i < response.data.length; i++) {

                
                var EndDate = new Date(response.data[i].Startdate_FormatDates);

                if (response.data[i].PlanDurationTypeName == 'Weekly' || response.data[i].PlanDurationTypeName == 'weekly') {
                    EndDate.setDate(EndDate.getDate() + 7);
                }
                else if (response.data[i].PlanDurationTypeName == 'per_monthly' || response.data[i].PlanDurationTypeName == 'Monthly' || response.data.PlanDurationTypeName == 'monthly') {
                    EndDate.setDate(EndDate.getDate() + 30);

                }
                else if (response.data[i].PlanDurationTypeName == 'Yearly' || response.data[i].PlanDurationTypeName == 'yearly' || response.data.PlanDurationTypeName == 'per_Yearly') {
                    EndDate.setDate(EndDate.getDate() + 365);

                }

                const currentDate = new Date();
                const endDate = EndDate;
                const diffTime = Math.abs(endDate - currentDate);
                const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));

                var days = ``;
                if (diffDays > 1) {

                    days = `<img src="/Content/visitor-panel/image/security.png" loading="lazy" alt="img"><span class="prime-text">Prime user ${diffDays} days left</span>`;
                }
             
                else {
                    days = `<img src="/Content/visitor-panel/image/security.png" loading="lazy" alt="img"><span class="prime-text">Prime user ${diffDays} days left</span>`;
                }
                $(".planexpiredays").append(days);
            }
            StopLoading();
        },
        error: function (result) {
            // StopLoading();

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

function selectMenuItems(mainMenuSelector = "") {
    $(".commonNavLinkClass").parent('li').removeClass("active");

    $(mainMenuSelector).parent('li').addClass("active");
}

function SetSideBarLink() {
    //--Get Selected Sidebar Page Link Value
    $.get("/Home/GetSidebarCookieDetail", null, function (dataSidebar) {
        // My Class
        if (dataSidebar == "myClass") {
            selectMenuItems(".a_Sidebar_MyClass");
        }
        //My Training
        else if (dataSidebar == "myTraining") {
            selectMenuItems(".a_Sidebar_MyTraining");
        }
        //My Event
        else if (dataSidebar == "myEvent") {
            selectMenuItems(".a_Sidebar_MyEvent");
        }
        // Saved Instructor
        else if (dataSidebar == "savedInstructors") {
            selectMenuItems(".a_Sidebar_SavedInstructors");
        }
        // Manage Queries
        else if (dataSidebar == "manageQueries") {
            selectMenuItems(".a_Sidebar_Queries");
        }
        // My Notifications
        else if (dataSidebar == "notifications") {
            selectMenuItems(".a_Sidebar_Notifications");
        }
        // My Coupons
        else if (dataSidebar == "myCoupons") {
            selectMenuItems(".a_Sidebar_MyCoupons");
        }
        // Messages
        else if (dataSidebar == "messages") {
            selectMenuItems(".a_Sidebar_Messages");
        }
            // group Messages
        else if (dataSidebar == "groupmessages") {
            selectMenuItems(".a_Sidebar_GroupMessages");
        }
        else if (dataSidebar == "groupMessageChat") {
            selectMenuItems(".a_Sidebar_GroupMessages");
        }
        // Transfer Package
        else if (dataSidebar == "transferPackage") {
            selectMenuItems(".a_Sidebar_TransferPackage");
        }
        // ManageImages
        else if (dataSidebar == "manageImages") {
            selectMenuItems(".a_Sidebar_ManageImages");
        }
        //// ManageVideos
        //else if (dataSidebar == "manageVedios") {
        //    selectMenuItems("#a_Sidebar_ManageVedios");
        //}
        // Manage License
        else if (dataSidebar == "license") {
            selectMenuItems(".a_Sidebar_License");
        }

        // Manage VedioGallery
        else if (dataSidebar == "vedioGallery") {
            selectMenuItems(".a_Sidebar_vedioGallery");
        }
        // View Student Pause class Requests 
        else if (dataSidebar == "studentPauseClassRequest") {
            selectMenuItems(".a_Sidebar_studentPauseClassRequest");
        }
        //My Courses
        else if (dataSidebar == "myCourses") {
            selectMenuItems(".a_Sidebar_MyCourses");
        }
        //My Courses
        else if (dataSidebar == "sportBooking") {
            selectMenuItems(".a_Sidebar_SportBooking");
        }
        //Manage My Booking Detail 
        else if (dataSidebar == "myBooking") {
            selectMenuItems(".a_Sidebar_mybooking");
        }
        else if (dataSidebar == "myaccount") {
            selectMenuItems(".a_Sidebar_MyAccount");
        }
    });
}

function LogoutUser() {
    StartLoading();
    $.get("/Login/LogoutUser", null, function () {
        window.location = "/Login/Index";
    });
}




