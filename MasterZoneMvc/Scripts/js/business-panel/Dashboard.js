var UserToken_Global = "";

$(document).ready(function () {
    //console.log("Initial: " + UserToken_Global);
    //StartLoading();
    $.get("/Business/GetBusinessAdminToken", null, function (dataToken) {
        if (dataToken != "" && dataToken != null) {

            UserToken_Global = dataToken;
            getDataForDashBoard();
            GetAllCreatedNotifications();
            //StopLoading();
        }
        else {
            $.get("/Staff/GetStaffToken", null, function (dataToken) {
                if (dataToken != "" && dataToken != null) {
                    UserToken_Global = dataToken;
                    //getDataForDashBoard();
                    //StopLoading();
                }
                else {
                    StopLoading();
                }
            });
        }
        //console.log("Final:" + UserToken_Global);
    });
});

function getDataForDashBoard() {
    StartLoading();
    var _url = '/api/Business/DashboardData/Get';
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
            } else {
               //console.log("UserDashBoard", response);
                
                if (response.data != null) {
                    var dashDetails = response.data;
                    $('#CurrYearEnrolledStudent').html(dashDetails.CurrYearEnrolledStudent)
                    $('#OnGoingCourse').html(dashDetails.OnGoingCourse)
                    $('#TotalCourseBuyed').html(dashDetails.TotalCourseBuyed)
                    $('#TotalEnrolledStudents').html(dashDetails.TotalEnrolledStudents)
                    $('#TotalRevenueGenrated').html('Rs. ' + (dashDetails.TotalRevenueGenrated !== null ? dashDetails.TotalRevenueGenrated + '.00' : '0.00'));
                }
            }
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


function GetAllCreatedNotifications() {
    // ---------------- Pagination Data Table --------------------
    var _url = "/api/BusinessNotification/GetAllByPagination";
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
            } else {
               // console.log("notification", response.data);
                var dashboardList = '';
                for (var i = 0; i < response.data.length; i++){
                    if (response.data[i].ItemTable == "ClassBookings") {
                        var data = `<tr>
                                        <td>
                                            <span class="log-indicator border-theme-1 align-middle"></span>
                                        </td>
                                        <td>
                                            <a href="/Business/MyPurchaseClassBookingDetail?classId=${response.data[i].ItemId}">
                                                <span class="font-weight-medium">${response.data[i].NotificationTitle}</span>
                                             </a>
                                        </td>
                                        <td class="text-right">
                                            <span class="text-muted">${response.data[i].CreatedOn_FormatDate}</span>
                                        </td>
                                    </tr>`
                    } else if (response.data[i].ItemTable == "EventBookings") {
                        var data = `<tr>
                                        <td>
                                            <span class="log-indicator border-theme-2 align-middle"></span>
                                        </td>
                                        <td>
                                           <a href="/Business/MyEventPurchaseDetails?eventId=${response.data[i].ItemId}">
                                                <span class="font-weight-medium">${response.data[i].NotificationTitle}</span>
                                             </a>
                                        </td>
                                        <td class="text-right">
                                            <span class="text-muted">${response.data[i].CreatedOn_FormatDate}</span>
                                        </td>
                                    </tr>`
                    }
                    else if (response.data[i].ItemTable == "PlanBookings") {
                        var data = `<tr>
                                        <td>
                                            <span class="log-indicator border-danger align-middle"></span>
                                        </td>
                                        <td>
                                           <a href="/Business/MyPlanPurchaseDetails?planId=${response.data[i].ItemId}">
                                                <span class="font-weight-medium">${response.data[i].NotificationTitle}</span>
                                             </a>
                                        </td>
                                        <td class="text-right">
                                            <span class="text-muted">${response.data[i].CreatedOn_FormatDate}</span>
                                        </td>
                                    </tr>`
                    }
                    else if (response.data[i].ItemTable == "TrainingBookings") {
                        var data = `<tr>
                                        <td>
                                            <span class="log-indicator border-warning align-middle"></span>
                                        </td>
                                        <td>
                                           <a href="/Business/MyTrainingPurchaseDetails?trainingId=${response.data[i].ItemId}">
                                                <span class="font-weight-medium">${response.data[i].NotificationTitle}</span>
                                             </a>
                                        </td>
                                        <td class="text-right">
                                            <span class="text-muted">${response.data[i].CreatedOn_FormatDate}</span>
                                        </td>
                                    </tr>`
                    } else if (response.data[i].ItemTable == "CourseBookings") {
                        var data = `<tr>
                                        <td>
                                            <span class="log-indicator border-primary align-middle"></span>
                                        </td>
                                        <td>
                                            <a href="/Business/MyCoursePurchaseDetails?courseId=${response.data[i].ItemId}">
                                                <span class="font-weight-medium">${response.data[i].NotificationTitle}</span>
                                             </a>
                                        </td>
                                        <td class="text-right">
                                            <span class="text-muted">${response.data[i].CreatedOn_FormatDate}</span>
                                        </td>
                                    </tr>`
                    }
                    else{
                        var data = `<tr>
                                        <td>
                                            <span class="log-indicator border-danger align-middle"></span>
                                        </td>
                                        <td>
                                            <a href="/Business/MyPurchaseClassBookingDetail?classId=${response.data[i].ItemId}">
                                                <span class="font-weight-medium">${response.data[i].NotificationTitle}</span>
                                             </a>
                                        </td>
                                        <td class="text-right">
                                            <span class="text-muted">${response.data[i].CreatedOn_FormatDate}</span>
                                        </td>
                                    </tr>`
                    }
                    dashboardList += data;

                }
                $("#dashBoardNotification").html('').append(dashboardList); 
            }
            StopLoading();
        },
    });

}