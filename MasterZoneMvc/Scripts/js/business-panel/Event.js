var EventId_Global = 0;
var UserToken_Global = "";
var _eventTable;
var GroupDetail_Global = {};
var EventSponsorId_Global = 0;

function initializeDataTable_Events() {
    _eventTable = $("#tblEvent").DataTable();
    $('#tblEventSponsor').DataTable();
}
$(document).ready(function () {
    $.get("/Business/GetBusinessAdminToken", null, function (dataToken) {
        if (dataToken != "" && dataToken != null) {
            UserToken_Global = dataToken;
            initializeDataTable_Events();
            GetAllEvents();
            $("#sectionViewEventSponsor").hide();

            //StopLoading();
        }
        else {
            $.get("/Staff/GetStaffToken", null, function (dataToken) {
                if (dataToken != "" && dataToken != null) {
                    UserToken_Global = dataToken;
                    initializeDataTable_Events();
                    GetAllEvents();
                    $("#sectionViewEventSponsor").hide();
                    //StopLoading();
                }
                else {
                    StopLoading();
                }
            });
        }
    });



    $('#addGroupMemberModal').on('hide.bs.modal', function () {
        ResetAddGroupMemberModal();
    });

    $('#viewGroupDetialsModal').on('hide.bs.modal', function () {
        ResetViewGroupDetialModal();
    });

    $('#txtSearchBusinessStudents_Modal').on('keyup', function (e) {
        var searchKeywords = $(this).val();

        //if (e.which < 65 || e.which > 90) //65=a, 90=z
        //    return false;
        if ((e.which != 8 || e.which != 13) && searchKeywords.length == 0) {
            getAllBusinessStudentsLists();
        }
        else if (searchKeywords.length >= 3) {
            searchBusinessStudentsLists(searchKeywords);
        }
    });
});








$(document).ready(function () {
    $('#btnAddEvent').click(function () {
        $('#sectionAddEvent').show();
        $('#btnAddEvent').hide();
        $('#sectionViewEvent').hide();

        document.getElementById("pageTextchange").innerHTML = "Add Event";
        document.getElementById("pageStageChange").innerHTML = "Add Event";

    });
});


//function EditEvent() {
//    $('#sectionAddStaff').show();
//    $('#sectionViewStaff').hide();
//    document.getElementById("myText").innerHTML = "Edit Event";
//    document.getElementById("ChangeUpdateText").innerHTML = "Update";
//    document.getElementById("pageTextchange").innerHTML = "Edit Event";
//    document.getElementById("pageStageChange").innerHTML = "Edit Event";
//    $('#ChangeUpdateText').show();
//    $('#btnAddStaff').hide();
//    $(".error-class").html('');
//    $('#btnAddStaff').click(function () {
//        document.getElementById("myText").innerHTML = "Add Event";
//        document.getElementById("ChangeUpdateText").innerHTML = "Save";
//        $(".error-class").html('');
//    });
//};




/* function ResetAddView() {
     $("#ddlManageStaff").val('');
     $("#txtEmail").val('');
     $("#txtStaffName").val('');
     $("#txtPassword").val('');
     $("#txtConfirmPassword").val('');
     $('#ShowingViewStaffList').show();
}*/
function ResetAddView() {

    $("#txtTitle_Event").val('');
    $("#txtLocationURL_Event").val('');
    $("#StartDate_Event").val('');
    $("#EndDate_Event").val('');
    $("#StartTime_Event").val('');
    $("#EndTime_Event").val('');
    $("#ShortDecription_Event").val('');
    $("#Price_Event").val('');
    $("#FeaturedImage_Event").val('');
    $("#previewEventImage").attr('src', '');
    $("#IsPaidRadioYes_Event").prop('checked', false);
    $("#IsPaidRadioNo_Event").prop('checked', false);
    $("#WalkingRadioYes_Event").prop('checked', false);
    $("#WalkingRadioNo_Event").prop('checked', false);
    $("#About_Event").val('');
    $("#AdditonalInforamtion_Event").val('');
    $("#TicketInformation_Event").val('');
    $('#sectionViewEvent').show();
    $('#sectionAddEvent').hide();
    $('#btnAddEvent').show();
    document.getElementById("pageTextchange").innerHTML = "Event";
    document.getElementById("pageStageChange").innerHTML = "Event";
    $(".error-class").html('');
}


////// -----------    FIELD VALIDATION HANDLER FUNCTIONS  --------------------------

const validate_IsEmptyStringInputFieldValue = function (inputFieldValue) {
    if (inputFieldValue == '' || inputFieldValue.replace(/\s/g, "") == "")
        return true;
    return false;
}

const validate_IsEmptySelectInputFieldValue = function (inputFieldValue) {
    if (inputFieldValue == undefined || inputFieldValue == null || inputFieldValue == '' || inputFieldValue == 0)
        return true;
    return false;
}
////// -----------    FIELD VALIDATION HANDLER FUNCTIONS  --------------------------
function ConfirmDeleteEventSponsor(id, EventId) {
    swal({
        title: "Delete Event Sponsor",
        text: "Are you sure to delete this Event Sponsor?",
        type: "warning",
        buttons: {
            cancel: true,
            confirm: "Yes",
        }
    })
        .then((willDelete) => {
            if (willDelete) {
                DeleteEventSponsor(id, EventId);
            } else {
                //swal("Your imaginary file is safe!");
            }
        });
}

function DeleteEventSponsor(id, EventId) {
    StartLoading();
    $.ajax({
        type: "POST",
        url: "/api/Event/Sponsor/DeleteById?id=" + id + "&EventId=" + EventId,
        headers: {
            "Authorization": "Bearer " + UserToken_Global,
            "Content-Type": "application/json"
        },
        contentType: 'application/json',
        success: function (dataResponse) {
            StopLoading();

            //--Check if staff successfully deleted
            if (dataResponse.status == 1) {
                setTimeout(function () {
                    swal("Success!", dataResponse.message, "success");
                    //--Get Event Sponsor List
                    getAllEventSponsor(EventId);

                }, 100);
            }
            else {
                $.iaoAlert({
                    msg: 'There is some technical error, please try again!',
                    type: "error",
                    mode: "dark",
                });
            }
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

function ConfirmChangeStatusStaff(sid) {
    swal({
        title: "Change Status",
        text: "Are you sure to change status of this Staff-Member?",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: '#DD6B55',
        confirmButtonText: 'Yes',
        cancelButtonText: "No"
    }, function (isConfirm) {
        if (!isConfirm) return;
        ChangeStatusStaff(sid);
    });
}

function ChangeStatusStaff(sid) {
    StartLoading();
    $.ajax({
        type: "GET",
        url: "/ChangeStaffStatusById?staffID=" + sid,
        headers: {
            "Authorization": "Bearer " + UserToken_Global,
            "Content-Type": "application/json"
        },
        contentType: 'application/json',
        success: function (dataResponse) {
            StopLoading();

            //--Check if staff-status successfully updated
            if (dataResponse.status == 1) {
                setTimeout(function () {
                    swal("Success!", dataResponse.message, "success");
                    //--Get Staff List
                    //GetStaffList();
                    FilterStaffByBranch();
                }, 100);
            }
            else {
                $.iaoAlert({
                    msg: 'There is some technical error, please try again!',
                    type: "error",
                    mode: "dark",
                });
            }

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
///
function swl01() {
    swal({
        title: "Are you sure?",
        text: "Once deleted, you will not be able to recover this imaginary file!",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                swal("Oof! Your imaginary file has been deleted!", {
                    icon: "success",
                });
            } else {
                swal("Your imaginary file is safe!");
            }
        });

}
function ValidateFields_Event() {
    var _eventTitle = $("#txtTitle_Event").val().trim();
    var _eventStartDate = $("#StartDate_Event").val().trim();
    var _eventEndDate = $("#EndDate_Event").val().trim();
    var _eventStartTime = $("#StartTime_Event").val().trim();
    var _eventEndTime = $("#EndTime_Event").val().trim();
    var _eventIsPaid = $('input[name="IsPaidRadio_Event"]:checked').val();
    var _eventWalking = $('input[name="WalkingRadio_Event"]:checked').val();
    var _eventLocationURL = $("#txtLocationURL_Event").val().trim();
    var _eventShortDescription = $("#ShortDecription_Event").val().trim();
    var _eventAbout = $("#About_Event").val().trim();
    var _eventAdditionalInformation = $("#AdditonalInforamtion_Event").val().trim();
    var _eventFeaturedImage = $("#FeaturedImage_Event").get(0).files;
    var _eventTicketInformation = $("#TicketInformation_Event").val().trim();
    var _eventPrice = $("#Price_Event").val().trim();
    var _is_valid = true;
    $(".error-class").html('');

    //Event-Title
    if (_eventTitle == "" || _eventTitle.replace(/\s/g, "") == "") {
        _is_valid = false;
        $("#error_txtTtile_Event").html('Please enter Title!');
    }
    //Event-Start Date
    if (_eventStartDate == "" || _eventStartDate.replace(/\s/g, "") == "") {
        _is_valid = false;
        $("#error_EventStartDate").html('Please enter Start Date!');
    }
    //Event End Date
    if (_eventEndDate == "" || _eventEndDate.replace(/\s/g, "") == "") {
        _is_valid = false;
        $("#error_EventEndDate").html('Please enter End Date!');
    }
    // Event Start Time
    if (_eventStartTime == "" || _eventStartTime.replace(/\s/g, "") == "") {
        _is_valid = false;
        $("#error_EventStartTime").html('Please enter Start Time!');
    }
    // Event End Time
    if (_eventEndTime == "" || _eventEndTime.replace(/\s/g, "") == "") {
        _is_valid = false;
        $("#error_EventEndTime").html('Please enter End Time!');
    }
    // Event Is Paid
    //if (_eventIsPaid.length <= 0) {
    //    _is_valid = false;
    //    $("#error_IsPaid_Event").html('Please select Is Paid !');
    //}
    // Event Walking
    //if (_eventWalking.length <= 0) {
    //    _is_valid = false;
    //    $("#error_Walking_Event").html('Please select Walking !');
    //}
    // Event Location URL
    if (_eventLocationURL == "" || _eventLocationURL.replace(/\s/g, "") == "") {
        _is_valid = false;
        $("#error_txtLocationURL_Event").html('Please enter Location URL!');
    }
    // Event Short Decription
    if (_eventShortDescription == "" || _eventShortDescription.replace(/\s/g, "") == "") {
        _is_valid = false;
        $("#error_EventShortDecription").html('Please Enter Short Description !');
    }
    // Event About
    if (_eventAbout == "" || _eventAbout.replace(/\s/g, "") == "") {
        _is_valid = false;
        $("#error_About_Event").html('Please enter About !');
    }
    // Event Additional Inforamtion
    if (_eventAdditionalInformation == "" || _eventAdditionalInformation.replace(/\s/g, "") == "") {
        _is_valid = false;
        $("#error_AdditonalInforamtion_Event").html('Please enter Additional Inforamtion !');
    }
    //-- image
    if (_eventFeaturedImage.length <= 0) {
        _is_valid = false;
        $("#error_FeaturedImage_Event").html('please select simage!');
    }
    // Event Ticket Inforamtion
    if (_eventTicketInformation == "" || _eventTicketInformation.replace(/\s/g, "") == "") {
        _is_valid = false;
        $("#error_TicketInformation_Event").html('Please enter Ticket Inforamtion !');
    }
    if (!validate_IsEmptyStringInputFieldValue(_eventPrice) && parseFloat(_eventPrice) <= 0) {
        _is_valid = false;
        $("#error_Price_Event").html('event price must be greater than 0');
    }
    return _is_valid;

}
function AddUpdateEvent() {
    var _eventTitle = $("#txtTitle_Event").val();
    var _eventStartDate = $("#StartDate_Event").val();
    var _eventEndDate = $("#EndDate_Event").val();
    var _eventStartTime = $("#StartTime_Event").val();
    var _eventEndTime = $("#EndTime_Event").val();
    var _eventIsPaid = $('input[name="IsPaidRadio_Event"]:checked').val();
    var _eventWalking = $('input[name="WalkingRadio_Event"]:checked').val();
    var _eventLocationURL = $("#txtLocationURL_Event").val();
    var _eventShortDescription = $("#ShortDecription_Event").val();
    var _eventAbout = $("#About_Event").val();
    var _eventAdditionalInformation = $("#AdditonalInforamtion_Event").val();
    var _eventTicketInformation = $("#TicketInformation_Event").val();
    var _eventPrice = $("#Price_Event").val().trim();
    var _eventFeaturedImage_MS = $("#FeaturedImage_Event").get(0);
    var _eventFeaturedImage = _eventFeaturedImage_MS.files;
    var _mode = (EventId_Global <= 0) ? 1 : 2;

    if (ValidateFields_Event()) {
        StartLoading();

        var data = new FormData();

        data.append("id", EventId_Global);
        data.append("title", _eventTitle);
        data.append("startDate", _eventStartDate);
        data.append("endDate", _eventEndDate);
        data.append("startTime", _eventStartTime);
        data.append("endTime", _eventEndTime);
        data.append("isPaid", _eventIsPaid);
        data.append("walking", _eventWalking);
        data.append("locationURL", _eventLocationURL);
        data.append("shortDescription", _eventShortDescription);
        data.append("additionalInforamtion", _eventAdditionalInformation);
        data.append("ticketInforamtion", _eventTicketInformation);
        data.append("about", _eventAbout);
        data.append("featuredImage", _eventFeaturedImage[0]);
        data.append("price", _eventPrice);
        data.append("mode", _mode);


        $.ajax({
            url: '/api/Event/AddUpdate',
            headers: {
                "Authorization": "Bearer " + UserToken_Global
            },
            data: data,
            processData: false,
            mimeType: 'multipart/form-data',
            contentType: false,
            //contentType: 'application/json',
            type: 'POST',
            success: function (dataResponse) {

                //--Parse into Json of response-json-string
                dataResponse = JSON.parse(dataResponse);

                //--If successfully added/updated
                if (dataResponse.status === 1) {
                    swal("Success!", dataResponse.message, "success");
                    //Hide section add event
                    $('#sectionAddEvent').hide();
                    // show section view event
                    $('#btnAddEvent').show();
                    $('#sectionViewEvent').show();

                    //get all event list
                    GetAllEvents();
                }
                else {
                    swal({
                        title: 'Error!',
                        icon: 'error',
                        text: dataResponse.message
                    });
                    StopLoading();
                    return;
                }



                StopLoading();


            },
            error: function (result) {
                StopLoading();
                //removeBtnLoading(btnSelector);
                //enableContinueBtn();

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

function GetAllEvents() {
    // ---------------- Pagination Data Table --------------------
    var _url_val = "/api/Event/GetAllByPagination";
    _eventTable.clear().destroy();
    _eventTable = $("#tblEvent").DataTable({
        "processing": true,
        "serverSide": true,
        "filter": true,
        "orderMulti": false,
        "ordering": true,
        "paginate": true,
        "order": [[4, "desc"]],
        "ajax": {
            "headers": {
                "Authorization": "Bearer " + UserToken_Global
            },
            "url": _url_val,
            "type": "POST",
            //"data": { "_Params": JSON.stringify(_Params) },
            //"datatype": "json",
            "error": function (result) {
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
        },
        "columns": [
            {
                "data": "SerialNumber", "name": "SerialNumber", "autoWidth": true
                //,"render": function (data, type, row) {
                //    return '';
                //}
            }
            //, {
            //    "data": "GroupImage", "name": "GroupImage", "autoWidth": true
            //    , "render": function (data, type, row) {
            //        return `<img src="${row.GroupImageWithPath}" class="image" />`
            //    }
            //}
            , { "data": "Title", "name": "Title", "autoWidth": true }
            , { "data": "StartDateTime", "name": "StartDateTime", "autoWidth": true }
            , { "data": "EndDateTime", "name": "EndDateTime", "autoWidth": true }
            , { "data": "Price", "name": "Price", "autoWidth": true }
            //, {
            //    "data": "CreatedOn", "name": "CreatedOn", "autoWidth": true
            //    , "render": function (data, type, row) {
            //        return row.CreatedOn_FormatDate; //`<span style="display:none;">${moment(row.CreatedOn_FormatDate).format('YYYY-MM-DD')}</span> ${row.CreatedOn_FormatDate}`;
            //    }
            //}
            , {
                "data": null, "": "Action", "autoWidth": true,
                "render": function (data, type, row) {

                    /*var _view = `<a href="javascript:viewGroupDetail(${row.Id});"><i class="fas fa-eye" title="View Group Details"></i></a>`;*/
                    var _edit = `<a href="javascript:EditEventById(${row.Id});"><i class="fas fa-edit" title="Edit Group"></i></a>`;
                    var _addMembers = `<a href="javascript:addGroupMembers(${row.Id});"><i class="fas fa-plus" title="Add Members" ></i></a>`;
                    var _delete = `<a href="javascript:ConfirmDeleteGroup(${row.Id});"><i class="fas fa-trash" title="Delete Group"></i></a>`;

                    var _action = `<div class="edbt">
                                ${_addMembers}
                               
                                ${_edit}
                                ${_delete}
                            </div>`;
                    return _action;
                }
            }

        ],

        "responsive": true,
        "autoWidth": false,
        //"dom": "<'row my-3'<'col-sm-12'B>><'row'<'col-sm-6'l><'col-sm-6'f>><'row'<'col-sm-12'tr>><'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",
        "columnDefs": [{
            "targets": [0, 5], // column index (start from 0)
            "orderable": false, // set orderable false for selected columns
        }]
    });

    // for enabling search box only send requet on pressing enter
    $('#tblEvent_filter input').unbind();
    $('#tblEvent_filter input').bind('keyup', function (e) {
        if (e.keyCode == 13 || (e.keyCode == 8 && $(this).val() == '')) {
            _eventTable.search(this.value).draw();
        }
    });

    // ----------------  Pagination Data Table ------------------
}
function EditEventById(id) {
    var _url = "/api/Event/GetById?id=" + id;

    StartLoading();
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
            }

            $('.error-class').html('');

            var item = response.data;

            $("#txtTitle_Event").val(item.Title);
            $("#txtLocationURL_Event").val(item.EventLocationURL);
            $("#StartDate_Event").val(item.StartDate);
            $("#EndDate_Event").val(item.EndDate);
            $("#StartTime_Event").val(item.StartTime_24HF);
            $("#EndTime_Event").val(item.EndTime_24HF);
            $("#ShortDecription_Event").val(item.ShortDescription);
            $("#Price_Event").val(item.Price);
            $("#FeaturedImage_Event").val('');
            if (response.data.EventImageWithPath != "") {
                $("#previewEventImage").attr('src', response.data.EventImageWithPath);
                $("#previewEventImage").removeClass('d-none');
            }
            else if (item.IsPaid == 1) {
                $("#IsPaidRadioYes_Event").prop('checked', true);
            }
            else if (item.IsPaid == 0) {
                $("#IsPaidRadioNo_Event").prop('checked', true);
            }
            else if (item.Walkings == 1) {
                $("#WalkingRadioYes_Event").prop('checked', true);
            }
            else if (item.Walkings == 0) {
                $("#WalkingRadioNo_Event").prop('checked', true);
            }
            $("#About_Event").val(item.AboutEvent);
            $("#AdditonalInforamtion_Event").val(item.AdditionalInformation);
            $("#TicketInformation_Event").val(item.TicketInformation);


            EventId_Global = item.Id;

            EditEventView();
            getAllEventSponsor(EventId_Global);
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
function EditEventView() {
    $('#sectionAddEvent').show();
    $('#sectionViewEvent').hide();
    $("#sectionViewEventSponsor").show();
    document.getElementById("myText").innerHTML = "Edit Event";
    document.getElementById("ChangeUpdateText").innerHTML = "Update";
    document.getElementById("pageTextchange").innerHTML = "Edit Event";
    document.getElementById("pageStageChange").innerHTML = "Edit Event";
    $('#ChangeUpdateText').show();
    $('#btnAddEvent').hide();
    $(".error-class").html('');
}
function ShowAddUpdateEventSponsor() {
    $("#AddUpdateEventSponsorModal").modal('show');
    document.getElementById("ChangeUpdateText_EventSponsor").innerHTML = "Submit";
    document.getElementById("Modal_Title").innerHTML = "Add Event Sponsor";
}
function AddUpdateEventSponor() {

    var _eventSponsorTitle = $("#txtTitle_EventSponsor").val().trim();
    var _eventSpnsorLink = $("#EventSponsorLink").val().trim();
    var _eventSponsorIcon_MS = $("#Icon_EventSponsor").get(0);
    var _eventSponsorIcon = _eventSponsorIcon_MS.files;
    var _mode = (EventSponsorId_Global <= 0) ? 1 : 2;
    let is_valid = true;
    $(".error-class").html('');

    if (validate_IsEmptyStringInputFieldValue(_eventSponsorTitle)) {
        is_valid = false;
        $("#error_txtTtile_EventSponsor").html('Enter  Title!');
    }
    if (validate_IsEmptyStringInputFieldValue(_eventSpnsorLink)) {
        is_valid = false;
        $("#error_EventSponsorLink").html('Enter  Link!');
    }
    if (validate_IsEmptySelectInputFieldValue(_eventSponsorIcon)) {
        is_valid = false;
        $("#error_Icon_EventSponsor").html('Select Icon!');
    }

    if (is_valid) {

        var data = new FormData();
        data.append("id", EventSponsorId_Global);
        data.append("EventId", EventId_Global);
        data.append("Title", _eventSponsorTitle);
        data.append("Link", _eventSpnsorLink);
        data.append("Icon", _eventSponsorIcon[0]);
        data.append("mode", _mode);

        $.ajax({
            url: '/api/Event/Sponsor/AddUpdate',
            headers: {
                "Authorization": "Bearer " + UserToken_Global
            },
            data: data,
            processData: false,
            mimeType: 'multipart/form-data',
            contentType: false,
            //contentType: 'application/json',
            type: 'POST',
            success: function (dataResponse) {

                //--Parse into Json of response-json-string
                dataResponse = JSON.parse(dataResponse);

                //--If successfully added/updated
                if (dataResponse.status === 1) {
                    swal("Success!", dataResponse.message, "success");
                    $("#AddUpdateEventSponsorModal").modal('hide');
                    getAllEventSponsor(EventId_Global);
                }
                else {
                    swal({
                        title: 'Error!',
                        icon: 'error',
                        text: dataResponse.message
                    });
                    StopLoading();
                    return;
                }



                StopLoading();


            },
            error: function (result) {
                StopLoading();
                //removeBtnLoading(btnSelector);
                //enableContinueBtn();

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

function getAllEventSponsor(EventId_Global) {
    let _url = API_Base_URL + "/api/Event/Sponsor/GetAllList?id=" + EventId_Global;

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

            var _table = $('#tblEventSponsor').DataTable();
            _table.destroy();

            var sno = 0;
            var _sponsorTitle = '';
            var _sponsorIcon = '';
            var _sponsorLink = '';
            var _action = '';

            var data = [];
            for (var i = 0; i < response.data.length; i++) {
                sno++;
                _sponsorTitle = response.data[i].SponsorTitle;
                _sponsorLink = response.data[i].SponsorLink;
                _sponsorIcon = (response.data[i].EventSponsorWithPath == "") ? '' : '<img src="' + response.data[i].EventSponsorWithPath + '" class="image" />';
                _action = '<div class="edbt">';
                /* _action += `<a href="javascript:EditEventSponsor(${response.data[i].Id}, ${response.data[i].EventId});"><i class="fas fa-edit"></i></a>`;*/
                _action += `<a href="javascript:ConfirmDeleteEventSponsor(${response.data[i].Id}, ${response.data[i].EventId});"><i class="fas fa-trash "></i></a>`;
                _action += '</div>';
                data.push([
                    sno,
                    _sponsorTitle,
                    _sponsorLink,
                    _sponsorIcon,
                    _action

                ]);

            }

            $('#tblEventSponsor').DataTable({
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

function EditEventSponsor(id, EventId) {
    var _url = "/api/Event/Sponsor/GetById?id=" + id + "&EventId=" + EventId;

    StartLoading();
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
            }

            $('.error-class').html('');

            var item = response.data;

            $("#txtTitle_EventSponsor").val(item.SponsorTitle);
            $("#EventSponsorLink").val(item.SponsorLink);
            $("#Icon_EventSponsor").val('');
            if (response.data.EventSponsorWithPath != "") {
                $("#previewSponsorEventImage").attr('src', response.data.EventSponsorWithPath);
                $("#previewSponsorEventImage").removeClass('d-none');
            }
            EventSponsorId_Global = item.Id;

            EditEventSponsorViewModel();
            //getAllEventSponsor(EventId_Global);
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
function EditEventSponsorViewModel() {
    $("#AddUpdateEventSponsorModal").modal('show');
    document.getElementById("ChangeUpdateText_EventSponsor").innerHTML = "Update";
    document.getElementById("Modal_Title").innerHTML = "Update Event Sponsor";

}
