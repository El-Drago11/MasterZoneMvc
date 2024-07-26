var UserToken_Global = "";

$(document).ready(function () {
    StartLoading();
    $.get("/Home/GetStudentToken", null, function (dataToken) {
        if (dataToken != "" && dataToken != null) {
            UserToken_Global = dataToken;
            getAllCouponList();
        }
        else {
            $.iaoAlert({
                msg: 'Unauthorized! Invalid Token!',
                type: "error",
                mode: "dark",
            });
            window.location.href = '/home/login';
            StopLoading();
        }
    });
});

function getAllCouponList() {
    let _url = "/api/Coupon/GetByAll";

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

            var _table = $('#ViewForCoupon').DataTable();
            _table.destroy();

            var sno = 0;
            var _Name = '';
            var _Description = '';
            var _Code = '';
            var _EndDate = '';
            var _businessname = '';

            var data = [];
            for (var i = 0; i < response.data.length; i++) {
                sno++;
                _Name = response.data[i].Name;
                _Description = response.data[i].Description;
                _Code = response.data[i].Code;
                _EndDate = response.data[i].EndDate;
                _businessname = response.data[i].BusinessName;

                //DiscountId_Global = response.data[i].Id;
                data.push([
                    sno,
                    _Name,
                    _Description,
                    _Code,

                    _EndDate,
                    _businessname,


                ]);
            }

            $('#ViewForCoupon').DataTable({
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

