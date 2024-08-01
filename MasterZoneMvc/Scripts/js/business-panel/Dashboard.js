var UserToken_Global = "";

$(document).ready(function () {
    console.log("Initial: " + UserToken_Global);
    //StartLoading();
    $.get("/Business/GetBusinessAdminToken", null, function (dataToken) {
        if (dataToken != "" && dataToken != null) {

            UserToken_Global = dataToken;
            getDataForDashBoard();
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
        console.log("Final:" + UserToken_Global);
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
                console.log("UserDashBoard", response);
                
                if (response.data != null) {
                    var dashDetails = response.data;
                    $('#CurrYearEnrolledStudent').html(dashDetails.CurrYearEnrolledStudent)
                    $('#OnGoingCourse').html(dashDetails.OnGoingCourse)
                    $('#TotalCourseBuyed').html(dashDetails.TotalCourseBuyed)
                    $('#TotalEnrolledStudents').html(dashDetails.TotalEnrolledStudents)
                    $('#TotalRevenueGenrated').html('Rs. '+dashDetails.TotalRevenueGenrated+'.00')

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