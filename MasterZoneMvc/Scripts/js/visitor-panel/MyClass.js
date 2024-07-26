var UserToken_Global = "";

$(document).ready(function () {
    StartLoading();
    $.get("/Home/GetStudentToken", null, function (dataToken) {
        if (dataToken != "" && dataToken != null) {
            UserToken_Global = dataToken;
            //getAllActiveStudentsLists();
            StopLoading();
        }
        else {
            StopLoading();
        }
    });

});
