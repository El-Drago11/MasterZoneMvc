var UserToken_Global_Layout = "";

$(document).ready(function () {
    $(".commonNavLinkClass").parent('li').removeClass("active");
    $(".commonSubNavLinkClass").parent('li').removeClass("active");
    //$("#li_ManageReports_Sidebar").removeClass("menu-open");

    $.get("/Business/GetBusinessAdminToken", null, function (dataAdminToken) {
        if (dataAdminToken != "" && dataAdminToken != null) {

            UserToken_Global_Layout = dataAdminToken;

            //--Set Active Link in Sidebar
            SetSideBarLink();

            ////--Get Business-Profile Info
            GetBusinessAdminProfileDetail_Layout();
        }
        else {
                
            $.get("/Business/GetStaffToken", null, function (dataToken) {
                if (dataToken != "" && dataToken != null) {
                    UserToken_Global_Layout = dataToken;
                    GetStaffProfileDetail_Layout();
                    //StopLoading();
                }
                else {
                    StopLoading();
                }
            });
        }
    });
});

function GetBusinessAdminProfileDetail_Layout() {
    $("#login_profileImage").attr("src", "");
    $("#login_userName").html("");
    $("#login_businessLogo").attr("src", "");

    $.ajax({
        type: "GET",
        url: "/api/Business/GetBasicProfileDetail",
        headers: {
            "Authorization": "Bearer " + UserToken_Global_Layout,
            "Content-Type": "application/json"
        },
        contentType: 'application/json',
        success: function (response) {
            if (response.data != null) {
                $("#login_profileImage").attr("src", response.data.ProfileImageWithPath);
                $("#login_userName").html(response.data.FirstName + " " + response.data.LastName);
                $("#login_businessLogo").attr("src", response.data.BusinessLogoWithPath);
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
    $("#login_profileImage").attr("src", "");
    $("#login_userName").html("");

    $.ajax({
        type: "GET",
        url: "/api/Staff/GetBasicProfileDetail",
        headers: {
            "Authorization": "Bearer " + UserToken_Global_Layout,
            "Content-Type": "application/json"
        },
        contentType: 'application/json',
        success: function (response) {

            if (response.data != null) {

                $("#login_profileImage").attr("src", response.data.ProfileImageWithPath);
                $("#login_userName").html(response.data.FirstName + " " + response.data.LastName);

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

function makeListItemActive(selector) {
    $(selector).parent('li').addClass("active");
}

function selectMenuItems(mainMenuSelector = "", subMenuSelector = "") {
    $(".commonNavLinkClass").parent('li').removeClass("active");
    $(".commonSubNavLinkClass").parent('li').removeClass("active");

    if (mainMenuSelector != "") {
        makeListItemActive(mainMenuSelector);

        // main menu scroll -----------------------
        scrollVerticalNav('.main-menu .scroll',mainMenuSelector);
    }   

    if (subMenuSelector != "") {
        makeListItemActive(subMenuSelector);

        // sub menu scroll -----------------------
        scrollVerticalNav('.sub-menu .scroll', subMenuSelector);
    }
}

function scrollVerticalNav(scrollContainer, id) {
    var container = $(scrollContainer);
    var scrollTo = $(id);

    // Calculating new position of scrollbar
    var position = scrollTo.offset().top
        - container.offset().top
        + container.scrollTop();

    // Setting the value of scrollbar
    //container.scrollLeft(position);
    $(container).animate({ 'scrollTop': position }, 1000);
}

function SetSideBarLink() {
    //--Get Selected Sidebar Page Link Value
    $.get("/Business/GetSidebarCookieDetail", null, function (dataSidebar) {

        // Dashboard
        if (dataSidebar == "dashboard") {
            selectMenuItems("#a_Sidebar_Dashboard");
        }
        //--Manage Staff -View Staff
        else if (dataSidebar == "manageViewStaff") {
            selectMenuItems("#a_Sidebar_ManageStaff", "#a_Sidebar_Sub_ViewStaff");
        }
        //--Manage Staff -Salary
        else if (dataSidebar == "manageSalary") {
            selectMenuItems("#a_Sidebar_ManageStaff", "#a_Sidebar_Sub_StaffSalaries");
        }
        //--Manage Staff-Attendance
        else if (dataSidebar == "manageStaffAttendance") {
            selectMenuItems("#a_Sidebar_ManageStaff", "#a_Sidebar_Sub_StaffAttendance");
        }

        //--Manage My Account
        else if (dataSidebar == "myAccount") {
            selectMenuItems("#a_Sidebar_MyAccount");
        }
        //--Manage Notification
        else if (dataSidebar == "manageNotification") {
            selectMenuItems("#a_Sidebar_Miscellaneous", "#a_Sidebar_Sub_Notifications");
        }
        //--Manage Advertiesement
        else if (dataSidebar == "manageAdvertisement") {
            selectMenuItems("#a_Sidebar_ManageAdvertisements");
        }
        //--Manage Group
        else if (dataSidebar == "manageGroup") {
            selectMenuItems("#a_Sidebar_ManageGroups");
        }
        //--Manage Enquiry
        else if (dataSidebar == "manageEnquiry") {
            selectMenuItems("#a_Sidebar_ManageEnquiries");
        }
        //--Manage Enquiry Follow Up
        else if (dataSidebar == "manageEnquiryFollowsup") {
            selectMenuItems("#a_Sidebar_ManageEnquiries", "#a_Sidebar_ManageFollowUp");
        }
        //--Manage Queries
        else if (dataSidebar == "manageQueries") {
            selectMenuItems("#a_Sidebar_ManageQueries");
        }
        //--Manage Event
        else if (dataSidebar == "manageEvents") {
            selectMenuItems("#a_Sidebar_ManageEvents");
        }
        //--Manage Discount/Coupon
        else if (dataSidebar == "manageDiscount") {
            selectMenuItems("#a_Sidebar_ManageDiscounts");
        }
        //--Manage Student
        else if (dataSidebar == "manageStudent") {
            selectMenuItems("#a_Sidebar_ManageStudent", "#a_Sidebar_Sub_ManageStudent");
        }
        //--View Student with Class Instructor
        else if (dataSidebar == "manageStudentWithClassInstructor") {
            selectMenuItems("#a_Sidebar_ManageStudent", "#a_Sidebar_Sub_ViewStudentWithClassInstructor");
        }
        //--Manage Plan/Package
        else if (dataSidebar == "managePackage") {
            selectMenuItems("#a_Sidebar_Packages", "#a_Sidebar_Sub_ViewPackages");
        }

        //--Manage Training
        else if (dataSidebar == "manageTraining") {
            selectMenuItems("#a_Sidebar_Packages", "#a_Sidebar_Sub_ManageTraining");
        }
        //--View Messages
        else if (dataSidebar == "viewMessages") {
            selectMenuItems("#a_Sidebar_Packages", "#a_Sidebar_Sub_ManageTraining");
        }
        //--Manage Message
        else if (dataSidebar == "manageMessage") {
            selectMenuItems("#a_Sidebar_Miscellaneous", "#a_Sidebar_Sub_ViewMessages");
        }
        //--Manage Reviews
        else if (dataSidebar == "manageReviews") {
            selectMenuItems("#a_Sidebar_Miscellaneous", "#a_Sidebar_Sub_ManageReviews");
        }
        // Manage Payment/Transactions
        else if (dataSidebar == "paymentsTransactionDetailsBusiness") {
            selectMenuItems("#a_Sidebar_ManagePayments");
        }
        //--Manage Business Videos
        else if (dataSidebar == "manageBusinessVideos") {
            selectMenuItems("#a_Sidebar_Miscellaneous", "#a_Sidebar_Sub_ManageVideos");
        }
        //--Manage Business Images
        else if (dataSidebar == "manageBusinessImages") {
            selectMenuItems("#a_Sidebar_Miscellaneous", "#a_Sidebar_Sub_ManageImages");
        }
        //---Manage Business Service
        else if (dataSidebar == "manageBusinessService") {
            selectMenuItems("#a_Sidebar_Miscellaneous", "#a_Sidebar_Sub_ManageBusinessService");
        }
        //----Manage Sponsors
        else if (dataSidebar == "manageSponsors") {
            selectMenuItems("#a_Sidebar_Miscellaneous", "#a_Sidebar_Sub_ManageSponsors");
        }
        //----Manage Profile Page
        else if (dataSidebar == "manageProfilePage") {
            selectMenuItems("#a_Sidebar_Miscellaneous", "#a_Sidebar_Sub_ManageProfilePage");
        }
        //----Manage Manage Expense
        else if (dataSidebar == "manageExpense") {
            selectMenuItems("#a_Sidebar_Miscellaneous", "#a_Sidebar_ManageExpense");
        }

        //--Manage Exam
        else if (dataSidebar == "manageExam") {
            selectMenuItems("#a_Sidebar_ManageExams");
        }
        //--Manage Business-Timing
        else if (dataSidebar == "manageTimeSlot") {
            selectMenuItems("#a_Sidebar_Miscellaneous", "#a_Sidebar_Sub_BusinessTiming");
        }
        //--Manage Transfer-Package
        else if (dataSidebar == "managePackageTransfer") {
            selectMenuItems("#a_Sidebar_Packages", "#a_Sidebar_Sub_TransferPackages");
        }
        //--Manage Backup
        else if (dataSidebar == "manageBackup") {
            selectMenuItems("#a_Sidebar_Backup");
        }
        //--Manage Certificate
        else if (dataSidebar == "manageCertificate") {
            selectMenuItems("#a_Sidebar_ManageCertifications", "#a_Sidebar_Sub_CertificateVerify");
        }

        //--Transfer Notifications
        else if (dataSidebar == "manageTransferPackageNotification") {
            selectMenuItems("#a_Sidebar_Packages", "#a_Sidebar_Sub_TransferNotification");
        }
        ////--Manage Certificate Requests
        //else if (dataSidebar == "manageCertificateRequests") {
        //    selectMenuItems("#a_Sidebar_ManageCertifications", "#a_Sidebar_Sub_CertificateRequests");
        //}
        //--Manage Book Certificate-License
        else if (dataSidebar == "manageBookCertificateLicense") {
            selectMenuItems("#a_Sidebar_ManageCertifications", "#a_Sidebar_Sub_BookCertificateLicense");
        }
        //--My Booked Licenses
        else if (dataSidebar == "manageMyBookedLicenses") {
            selectMenuItems("#a_Sidebar_ManageCertifications", "#a_Sidebar_Sub_MyBookedLicenses");
        }
        //--Manage Apartment
        else if (dataSidebar == "manageApartment") {
            selectMenuItems("#a_Sidebar_SprotsClub", "#a_Sidebar_Sub_ManageApartments");
        }
        //--Manage Apartment Booking
        else if (dataSidebar == "manageApartmentBooking") {
            selectMenuItems("#a_Sidebar_SprotsClub", "#a_Sidebar_Sub_ManageApartmentBooking");
        }
        //--View Apartment Booking
        else if (dataSidebar == "viewApartmentBookingAllDetail") {
            selectMenuItems("#a_Sidebar_SprotsClub", "#a_Sidebar_Sub_ViewApartmentBooking");
        }
        // view booking request
        else if (dataSidebar == "tennisBooking") {
            selectMenuItems("#a_Sidebar_SprotsClub", "#a_Sidebar_Sub_BookingRequest");
        }
        // Manage Branches 
        else if (dataSidebar == "manageBranches") {
            selectMenuItems("#a_Sidebar_ManageBranches", "#a_Sidebar_ManageBranches");
        }
     
        // Manage BranchEvent
        else if (dataSidebar == "branchEvent") {
            selectMenuItems("#a_Sidebar_BranchEvent", "#a_Sidebar_BranchEvent");
        }
        //Manage BranchStaff
        else if (dataSidebar == "branchStaff") {
            selectMenuItems("#a_Sidebar_BranchStaff", "#a_Sidebar_BranchStaff");
        }
        // Manage BranchStudent
        else if (dataSidebar == "branchStudent") {
            selectMenuItems("#a_Sidebar_BranchStudent", "#a_Sidebar_BranchStudent");
        }
        //Manage BranchTansactions
        else if (dataSidebar == "branchTansactions") {
            selectMenuItems("#a_Sidebar_BranchTansactions", "#a_Sidebar_BranchTansactions");
        }

        //--Manage Schedule/Class
        else if (dataSidebar == "manageScheduleClass") {
            selectMenuItems("#a_Sidebar_ScheduleClasses", "#a_Sidebar_Sub_ManageClass");
        }
        //Manage Pause Class Detail 
        else if (dataSidebar == "pauseClassDetail") {
            console.log("hello");
            selectMenuItems("#a_Sidebar_ScheduleClasses", "#a_Sidebar_PauseClassDetail");

        }
        // Manage Course Form Detail
        else if (dataSidebar == "scheduleCourses") {
            selectMenuItems("#a_Sidebar_scheduleCourse", "#a_Sidebar_scheduleCourse");
        }


        //--Manage Class-Batch
        else if (dataSidebar == "manageBatch") {
            selectMenuItems("#a_Sidebar_ScheduleClasses", "#a_Sidebar_Sub_ManageBatch");
        }
        // Manage Custom Form Detail
        else if (dataSidebar == "customForm") {
            selectMenuItems("#a_Sidebar_Sub_CustomForm", "#a_Sidebar_Sub_CustomForm");
        }
        // Manage View Custom Form Detail 
        else if (dataSidebar == "viewCustomForm") {
            selectMenuItems("#a_Sidebar_CustomForm", "#a_Sidebar_Sub_ViewCustomForm");
        }
        // Manage Transfer Custom Form Detail 
        else if (dataSidebar == "businessTransferForm") {
            selectMenuItems("#a_Sidebar_Sub_BusinessTransferForm", "#a_Sidebar_Sub_BusinessTransferForm");
        }
        // Manage My Booking Detail
        else if (dataSidebar == "myBooking") {
            selectMenuItems("#a_Sidebar_myBooking", "#a_Sidebar_myBooking");
        }


    });
}



function LogoutUser() {
    StartLoading();
    $.get("/Login/LogoutUser", null, function () {
        window.location = "/Login/Index";
    });
}