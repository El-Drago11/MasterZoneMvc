var UserToken_Global = "";
var PlanId_Global = "0";

$(document).ready(function () {
    PlanId_Global = $("#hiddenPackageId").val();

    StartLoading();
    $.get("/Home/GetStudentToken", null, function (dataToken) {
        if (dataToken != "" && dataToken != null) {
            UserToken_Global = dataToken;
            getPlanDetails();
        }
        else {
            $.iaoAlert({
                msg: 'Please login to book the class!',
                type: "error",
                mode: "dark",
            });
            getPlanDetails();
            StopLoading();
        }
    });
});

function getPlanDetails() {
    let _url = "/api/BusinessPlan/DetialForUser?planId=" + PlanId_Global;

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
                var item = response.data;
                $('#PlanName').html(item.Name);
                $('#PlanDescription').html(item.Description);
                $('#PlanDurationName').html(item.BusinessPlanDurationTypeName);
                $('#PlanCompareAtPrice').html(item.CompareAtPrice);
                $('#PlanPrice').html(item.Price);
                $('#TotalPrice').html(item.Price);
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