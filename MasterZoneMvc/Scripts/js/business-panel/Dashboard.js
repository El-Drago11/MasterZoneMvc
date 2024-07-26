var UserToken_Global = "";

$(document).ready(function () {
    console.log("Initial: " + UserToken_Global);
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
        console.log("Final:" + UserToken_Global);
    });
});