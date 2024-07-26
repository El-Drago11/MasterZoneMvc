function CheckLogin() {

    var _email = $("#Email").val();
    var _masterId = $("#MasterId").val();
    var _password = $("#Password").val();
    var TestEmail = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    var _is_valid = true;

    $(".error-class").html('');

    //if (_email == "" || _email.replace(/\s/g, "") == "") {
    //    _is_valid = false;
    //    $("#email_error_Login").html('Please enter email!');
    //}
    if (_masterId == "" || _masterId.replace(/\s/g, "") == "") {
        _is_valid = false;
        $("#masterId_error_Login").html('Please enter Master Id!');
    }
    //if (!TestEmail.test(_email) && _email != "" && _email.replace(/\s/g, "") != "") {
    //    _is_valid = false;
    //    $("#email_error_Login").html('Please enter the valid email address!');
    //}

    if (_password == "") {
        _is_valid = false;
        $("#password_error_Login").html('Please enter password!');
    }

    if (_is_valid == false) {
        return false;
    }
    else {
        StartLoading();
        return true;
    }
}