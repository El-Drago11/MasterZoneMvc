var UserToken_Global_Layout = "";

$(document).ready(function () {
    $(".commonNavLinkClass").removeClass("active");
    $(".commonSubNavLinkClass").removeClass("active");
    //$("#li_ManageReports_Sidebar").removeClass("menu-open");

    $.get("/SuperAdmin/GetSuperAdminToken", null, function (dataAdminToken) {
        if (dataAdminToken != "" && dataAdminToken != null) {

            UserToken_Global_Layout = dataAdminToken;

            //--Set Active Link in Sidebar
            SetSideBarLink();
            GetSuperAdminProfileDetail_Layout();
            ////--Get Staff-Profile Info
            //GetAdminProfileDetail_Layout();
        }
        else {
                
            $.get("/SubAdmin/GetSubAdminToken", null, function (dataAdminToken) {
                if (dataAdminToken != "" && dataAdminToken != null) {

                    UserToken_Global_Layout = dataAdminToken;

                    //--Get SubAdmin-Profile Info
                    GetSubAminProfileDetail_Layout();
                }
                else {

                }
            });
        }
    });
});

function GetSuperAdminProfileDetail_Layout() {
    $("#loginsuperadmin_profileImage").attr("src", "");
    $("#loginsuperadmin_userName").html("");
    $.ajax({
        type: "GET",
        url: "/api/SuperAdmin/DashoardProfileDetailGet",
        headers: {
            "Authorization": "Bearer " + UserToken_Global_Layout,
            "Content-Type": "application/json"
        },
        contentType: 'application/json',
        success: function (response) {
            if (response.data != null) {
                $("#loginsuperadmin_profileImage").attr("src", response.data.ProfileImageWithPath);
                $("#loginsuperadmin_userName").html(response.data.FirstName + " " + response.data.LastName);
            }
            else {
                $("#loginsuperadmin_profileImage").attr("src", "/Content/images/defaultProfileImg.png");
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

function GetSubAminProfileDetail_Layout() {
    $("#loginsubadmin_profileImage").attr("src", "");
    $("#loginsubadmin_userName").html("");
    $.ajax({
        type: "GET",
        url: "/api/SubAdmin/GetSubAdminDetail",
        headers: {
            "Authorization": "Bearer " + UserToken_Global_Layout,
            "Content-Type": "application/json"
        },
        contentType: 'application/json',
        success: function (response) {
            if (response.data != null) {
                $("#loginsubadmin_profileImage").attr("src", response.data.ProfileImageWithPath);
                $("#loginsubadmin_userName").html(response.data.FirstName + " " + response.data.LastName);
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




function GetAdminProfileDetail_Layout() {

    $("#img_Admin_AdminLayout").attr("src", "");
    $("#a_Admin_AdminLayout").html("");

    $.ajax({
        type: "GET",
        url: "/GetAdminProfile",
        headers: {
            "Authorization": "Bearer " + UserToken_Global_Layout,
            "Content-Type": "application/json"
        },
        contentType: 'application/json',
        success: function (dataAdminLayout) {
            var str = dataAdminLayout.data.Admin;
            if (dataAdminLayout.data.Admin != null) {

                $("#img_Admin_AdminLayout").attr("src", "/Content/AdminImages/" + dataAdminLayout.data.Admin.ProfileImage);
                $("#a_Admin_AdminLayout").html(dataAdminLayout.data.Admin.FirstName + " " + dataAdminLayout.data.Admin.LastName);
                $("#li_InstituteName_AdminLayout").html(dataAdminLayout.data.Admin.InstituteName);

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

function GetStaffProfileDetail_Layout() {

    $("#img_Staff_StaffLayout").attr("src", "");
    $("#a_Staff_StaffLayout").html("");
    $("#li_BranchName_StaffLayout").html("");

    $.ajax({
        type: "GET",
        url: "/GetStaffProfile",
        headers: {
            "Authorization": "Bearer " + UserToken_Global_Layout,
            "Content-Type": "application/json"
        },
        contentType: 'application/json',
        success: function (dataStaffLayout) {

            if (dataStaffLayout.data.staff != null) {

                $("#li_BranchName_StaffLayout").html(dataStaffLayout.data.staff.BranchName);

                $("#img_Staff_StaffLayout").attr("src", "/Content/StaffImages/" + dataStaffLayout.data.staff.ProfileImage);
                $("#a_Staff_StaffLayout").html(dataStaffLayout.data.staff.FirstName + " " + dataStaffLayout.data.staff.LastName);

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

function SetSideBarLink() {
    //--Get Selected Sidebar Page Link Value
    $.get("/SuperAdmin/GetSidebarCookieDetail", null, function (dataSidebar) {

        //$('.commonNavLinkClass').removeClass('active');

        // Dashboard
        if (dataSidebar == "dashboard") {
            $("#a_Dashboard_Sidebar").addClass("active");
        }
        //--Manage Categories
        else if (dataSidebar == "manageCategories") {
            $("#a_ManageCategories_Sidebar").addClass("active");
        }
        //--Manage Menu
        else if (dataSidebar == "manageMenu") {
            $("#a_ManageMenu_Sidebar").addClass("active");
        }
        //--Manage Advertisements
        else if (dataSidebar == "manageAdvertisements") {
            $("#a_ManageAdvertisements_Sidebar").addClass("active");
        }
        // Manage Business Video Detail 
        else if (dataSidebar == "manageVideo") {
            $("#a_ManageVideo_Sidebar").addClass("active");
        }
        //--Manage Pay-Fee
        else if (dataSidebar == "managePayFee") {
            $("#a_ManagePayFee_Sidebar").addClass("active");
            $("#a_ManageFees_Sidebar").addClass("active");

            $("#li_ManageFees_Sidebar").addClass("menu-open");
        }
        // Change Password
        else if (dataSidebar == "changePassword") {
            $("#a_ChangePassword_Sidebar").addClass("active");
        }
        // Manage Backup
        else if (dataSidebar == "manageBackup") {
            $("#a_ManageBackup_Sidebar").addClass("active");
        }
        // Manage AccountSettings
        else if (dataSidebar == "manageAccountSettings") {
            $("#a_ManageAccountSettings_Sidebar").addClass("active");
        }
        // Manage Profile
        else if (dataSidebar == "manageProfile") {
            //$("#a_ManageDiscount_Sidebar").addClass("active");
        }
        // Manage Business
        else if (dataSidebar == "manageBusiness") {
            $("#a_ManageBusiness_Sidebar").addClass("active");
        }
        //--Manage Pay-Other-Charges
        else if (dataSidebar == "managePayOtherCharges") {
            $("#a_ManagePayOtherCharges_Sidebar").addClass("active");
            $("#a_ManageFees_Sidebar").addClass("active");

            $("#li_ManageFees_Sidebar").addClass("menu-open");
        }
        // Manage Plan/Package
        else if (dataSidebar == "managePackage") {
            $("#a_Packages_Sidebar").addClass("active");
        }
        // Manage Payment/Transactions
        else if (dataSidebar == "managePayments") {
            $("#a_Payments_Sidebar").addClass("active");
        }
        // Manage UserAccount
        else if (dataSidebar == "userAccount") {
            $("#a_ManageUser_Sidebar").addClass("active");
        }
        // Manage SubAdmin
        else if (dataSidebar == "manageSubAdmin") {
            $("#a_ManageSubAdmin_Sidebar").addClass("active");
        }
        // Manage Certificates
        else if (dataSidebar == "manageCertificates") {
            $("#a_ManageCertificates_Sidebar").addClass("active");
        }
        // Manage License Bookings
        else if (dataSidebar == "manageLicenseBookings") {
            $("#a_ManageLicenseBookings_Sidebar").addClass("active");
        }
        // Manage Sponsor
        else if (dataSidebar == "manageSponsors") {
            $("#a_ManageSponsors_Sidebar").addClass("active");
        }
        // Manage Class Category
        else if (dataSidebar == "manageClassCategory") {
            $("#a_ManageClassCategory_Sidebar").addClass("active");
        }
        // Manage All Classes
        else if (dataSidebar == "manageAllClasses") {
            $("#a_ManageAllClasses_Sidebar").addClass("active");
        }
        // Manage All Trainings
        else if (dataSidebar == "manageAllTrainings") {
            $("#a_ManageAllTrainings_Sidebar").addClass("active");
        }
        // Manage All Events
        else if (dataSidebar == "manageAllEvents") {
            $("#a_ManageAllEvents_Sidebar").addClass("active");
        }
        // Manage All Instructors
        else if (dataSidebar == "manageAllInstructors") {
            $("#a_ManageAllInstructors_Sidebar").addClass("active");
        }
        // Manage Home Page
        else if (dataSidebar == "manageHomePage") {
            $("#a_ManageHomePage_Sidebar").addClass("active");
        }
        // Manage Language
        else if (dataSidebar == "manageLanguage") {
            $("#a_ManageLanguage_Sidebar").addClass("active");
        }
        // Manage Language
        else if (dataSidebar == "manageUniversity") {
            $("#a_ManageUniversity_Sidebar").addClass("active");
        }
        // Manage Course Category
        else if (dataSidebar == "manageCourseCategory") {
            $("#a_ManageCourseCategory_Sidebar").addClass("active");
        }
        // Custom Form
        else if (dataSidebar == "customForm") {
            $("#a_CustomForm_Sidebar").addClass("active");
        }
        // View Custom Form
        else if (dataSidebar == "viewCustomForm") {
            $("#a_ViewCustomForm_Sidebar").addClass("active");
        }
        // Transfer custom Form Detail
        else if (dataSidebar == "transferForm") {
            $("#a_TransferForm_Sidebar").addClass("active");
        }
        // Manage Event Category
        else if (dataSidebar == "manageEventCategory") {
            $("#a_Sidebar_manageEventCategory").addClass("active");
        }
        // Manage Staff Category
        else if (dataSidebar == "manageStaffCategory") {
            $("#a_Sidebar_manageStaffCategory").addClass("active");
        }
        // About Detail 
        else if (dataSidebar == "about") {
            $("#a_ManageAbout_Sidebar").addClass("active");
        }
       
        
    });
}

function LogoutUser() {
    StartLoading();
    $.get("/Login/LogoutUser", null, function () {
        window.location = "/Login/Index";
    });
}