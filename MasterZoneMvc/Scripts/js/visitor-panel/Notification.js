var UserToken_Global = "";
var NotificationList_LastRecordId_Global = 0;

$(document).ready(function () {
    StartLoading();
    $.get("/Home/GetStudentToken", null, function (dataToken) {
        if (dataToken != "" && dataToken != null) {
            UserToken_Global = dataToken;
            getAllNotification();
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

function getAllNotification() {
    let _url = "/api/Notification/GetAllNotification?lastRecordId=0";

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

            var NotificationList = '';

            // if no enquiries found and not a hit from ViewMore.
            if (response.data.NotificationList.length <= 0) {
                $('#NotificationList').html('<div class="w-100 text-black-50 text-center mt-4"><i>You don\'t have any notification!</i></div>');
                $('#BtnViewMoreNotification').addClass('d-none');
                StopLoading();
                return;
            }

            if (response.data.NotificationList.length <= 0) {
                $('#BtnViewMoreNotification').addClass('d-none');
            }
            else {
                NotificationList_LastRecordId_Global = response.data.NotificationList[response.data.NotificationList.length - 1].NotificationId;
                $('#BtnViewMoreNotification').removeClass('d-none');
            }

            NotificationList = getHTMLBindedNotificationData(response.data.NotificationList);

            $('#NotificationList').html(NotificationList);

            //getAllBusinessOwnerLists();

            StopLoading();
        },
        error: function (result) {
            StopLoading();
            //getAllBusinessOwnerLists();
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

function ViewMoreNotification() {

    let _url = "/api/Notification/GetAllNotification?lastRecordId=" + NotificationList_LastRecordId_Global;

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

            var NotificationList = '';

            if (response.data.NotificationList.length <= 0) {
                $('#BtnViewMoreNotification').addClass('d-none');
            }
            else {
                NotificationList_LastRecordId_Global = response.data.NotificationList[response.data.NotificationList.length - 1].NotificationId;
                $('#BtnViewMoreNotification').removeClass('d-none');
            }

            NotificationList = getHTMLBindedNotificationData(response.data.NotificationList);

            $('#NotificationList').append(NotificationList);

            //getAllBusinessOwnerLists();

            StopLoading();
        },
        error: function (result) {
            StopLoading();
            //getAllBusinessOwnerLists();
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

function getHTMLBindedNotificationData(NotificationList) {
    var htmlData = '';

    for (var i = 0; i < NotificationList.length; i++) {
        var Notification = NotificationList[i];
        //${ moment(enquiry.CreatedOn).format('YYYY-MM-DD') }

        var _onclickEvent = '';
        var _notificationIcon = '<i class="fas fa-circle" aria-hidden="true"style="color: antiqueWhite;"></i>';

        if (Notification.IsRead == 0) {
            _notificationIcon = `<i class="fas fa-circle" aria-hidden="true"></i>`;
            _onclickEvent = `onclick="btnMarkAsReadOrUnread(${Notification.NotificationId});"`;
        }

        htmlData += `
                    <div class="enq-cnt-liuijl mt-0">
                        <div class="accordion" id="accordionNotification${Notification.NotificationId}">
                            <div class="card border-0 border-bottom">
                                <div id="dv_AccordianHeader_${Notification.NotificationId}"  class="card-header card-header-rri collapsed" data-toggle="collapse" data-target="#collapse${Notification.NotificationId}" aria-expanded="false"  ${_onclickEvent}>
                                    <p class="title"><span class="notification_icon">${_notificationIcon}</span>  ${Notification.NotificationTitle}</p>
                                    <span class="accicon"><i class="fas fa-angle-down rotate-icon"></i></span>
                                </div>
                                <div id="collapse${Notification.NotificationId}" class="collapse" data-parent="#accordionNotification${Notification.NotificationId}">
                                    <div class="card-body">
                                        ${Notification.NotificationText}
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
         `;
    }

    return htmlData;
}


function btnMarkAsReadOrUnread(id) {

    if ($("#dv_AccordianHeader_" + id).hasClass("collapsed")) {
        //style = "background-color: aqua;"
        $("#dv_AccordianHeader_").style = "background-color: aqua";
        var _Params = {
            "NotificationId": id,
            "ReadStatus": 1
        };
        $.ajax({
            url: '/api/Notification/MarkAsReadUnread',
            data: JSON.stringify(_Params),
            processData: false,
            headers: {
                "Authorization": "Bearer " + UserToken_Global,
                "Content-Type": "application/json"
            },

            contentType: 'application/json',
            type: 'POST',
            success: function (dataResponse) {
                //--If successfully added/updated
                if (dataResponse.status === 1) {
                    //swal("Success!", dataResponse.message, "success");
                    $("#dv_AccordianHeader_" + id + ' .notification_icon').html('<i class="fas fa-circle" aria-hidden="true"style="color: antiqueWhite;"></i>');

                    $("#dv_AccordianHeader_" + id).removeAttr('onclick');
                }
                else {
                    swal({
                        title: '',
                        imageUrl: '/Content/svg/error.svg',
                        text: dataResponse.message
                    });

                    //StopLoading();

                }

            },
            error: function (result) {
                //StopLoading();

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

}

