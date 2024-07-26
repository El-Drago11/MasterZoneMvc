var UserToken_Global = "";

$(document).ready(function () {
    StartLoading();
    $.get("/Business/GetBusinessAdminToken", null, function (dataToken) {
        if (dataToken != "" && dataToken != null) {
            UserToken_Global = dataToken;
            getAllActiveStudentsLists();
            //StopLoading();
        }
        else {
            $.get("/Staff/GetStaffToken", null, function (dataToken) {
                if (dataToken != "" && dataToken != null) {
                    UserToken_Global = dataToken;
                    getAllActiveStudentsLists();
                }
                else {
                    StopLoading();
                }
            });
        }
    });
});

function getAllActiveStudentsLists() {
    let _url = API_Base_URL + "/api/Business/GetAllStudents";

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
                    msg: 'Unauthorized! Invalid Token!',
                    type: "error",
                    mode: "dark",
                });
                return;
            }

            var _table = $('#tblBusinessStudent').DataTable();
            _table.destroy();

            var sno = 0;
            var _FirstName = '';
            var _LastName = '';
            var _Email = '';
            var _status = '';

            var data = [];
            for (var i = 0; i < response.data.length; i++) {
                sno++;
                _FirstName = response.data[i].FirstName;
                _LastName = response.data[i].LastName;
                _Email = response.data[i].Email;
                //---Check Staff-Status
                if (response.data[i].Status == 1) {
                    _status = '<a class="btn btn-success  btn-sm" style="width:80px; color: white;" onclick="ConfirmChangeStatusStudent(' + response.data[i].Id + ');">Active</a>';
                }
                else {
                    _status = '<a class="btn btn-danger  btn-sm" style="width:fit-content; color: white;" onclick="ConfirmChangeStatusStudent(' + response.data[i].Id + ');">In-Active</a>';
                }
                data.push([
                    sno,
                    _FirstName,
                    _LastName,
                    _Email,
                    _status

                ]);
            }

            $('#tblBusinessStudent').DataTable({
                "data": data,
                "paging": true,
                "lengthChange": true,
                "searching": true,
                "ordering": true,
                "info": true,
                "autoWidth": false,
                "responsive": true,
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
