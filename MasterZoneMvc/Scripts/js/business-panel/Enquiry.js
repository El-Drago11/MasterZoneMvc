var EnquiryId_Global = 0;
var UserToken_Global = "";
var _enquiryTable;

function initializeDataTable_Enquiry() {
    _enquiryTable = $("#tblEnquiry").DataTable();
}


$(document).ready(function () {
    $.get("/Business/GetBusinessAdminToken", null, function (dataToken) {
        if (dataToken != "" && dataToken != null) {
            UserToken_Global = dataToken;
            initializeDataTable_Enquiry();
            GetAllBusinessEnquiries();

            StopLoading();
        }
        else {
            $.get("/Staff/GetStaffToken", null, function (dataToken) {
                if (dataToken != "" && dataToken != null) {
                    UserToken_Global = dataToken;
                    initializeDataTable_Enquiry();
                    GetAllBusinessEnquiries();

                    StopLoading();
                }
                else {
                    StopLoading();
                }
            });
        }
    });
});

$("#enquiryReplyModal").on('hide.bs.modal', function () {
    ResetReplyModal();
});

function GetAllBusinessEnquiries() {
    // ---------------- Pagination Data Table --------------------
    var _url_val = "/api/Enquiry/Business/GetAllByPagination";
    _enquiryTable.clear().destroy();
    _enquiryTable = $("#tblEnquiry").DataTable({
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
            , { "data": "StudentName", "name": "StudentName", "autoWidth": true }
            , { "data": "Title", "name": "Title", "autoWidth": true }
            , {
                "data": "Description", "name": "Description", "autoWidth": true,
                "render": function (data, type, row) {
                    if (row.Description.length > 100) { return row.Description.substr(0, 100) + '...'; }
                    else { return row.Description; }
                }
            }
            , {
                "data": "CreatedOn", "name": "CreatedOn", "autoWidth": true
                , "render": function (data, type, row) {
                    return `<span style="display:none;">${moment(row.CreatedOn_FormatDate).format('YYYY-MM-DD')}</span> ${row.CreatedOn_FormatDate}`;
                }
            }
            , {
                "data": null, "": "Action", "autoWidth": true,
                "render": function (data, type, row) {
                    var _view = `<a href="javascript:getEnquiryDetailById(${row.Id});"><i class="fas fa-eye" title="View"></i></a>`;
                    var _edit = ``;
                    if (row.IsReplied == 0) {
                        _edit = `<a href="javascript:ReplyEnquiry(${row.Id});"><i class="fas fa-reply" title="Reply" ></i></a>`;
                    }
                    var _action = `<div class="edbt">
                                ${_view}
                                ${_edit}
                            </div>`;
                    return _action;
                }
            }
        ],
        //"fnRowCallback": function (nRow, aData, iDisplayIndex) {
        //    var info = _expenditureTable.page.info();
        //    $("td:first", nRow).html(info.start + iDisplayIndex + 1);
        //    return nRow;
        //},
        "responsive": true,
        "autoWidth": false,
        //"dom": "<'row my-3'<'col-sm-12'B>><'row'<'col-sm-6'l><'col-sm-6'f>><'row'<'col-sm-12'tr>><'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",
        "columnDefs": [{
            "targets": [0, 5], // column index (start from 0)
            "orderable": false, // set orderable false for selected columns
        }]
    });

    // for enabling search box only send requet on pressing enter
    $('#tblEnquiry_filter input').unbind();
    $('#tblEnquiry_filter input').bind('keyup', function (e) {
        if (e.keyCode == 13 || (e.keyCode == 8 && $(this).val() == '')) {
            _enquiryTable.search(this.value).draw();
        }
    });

    // ----------------  Pagination Data Table ------------------
}

function btnSubmitReplyClick() {

    let is_valid = true;
    $("#enquiryReplyModal .error-class").html('');

    let _reply = $("#textareaEnquiryReply").val().trim();

    let _error_reply = $("#error_textareaEnquiryReply");

    if (validate_IsEmptyStringInputFieldValue(_reply)) {
        is_valid = false;
        _error_reply.html('Enter Reply!');
    }

    if (is_valid) {
        var data = new FormData();
        data.append("Id", EnquiryId_Global);
        data.append("ReplyBody", _reply);

        $.ajax({
            url: '/api/Enquiry/AddEnquiryReply',
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
                StopLoading();

                //--Parse into Json of response-json-string
                dataResponse = JSON.parse(dataResponse);

                //--If successfully added/updated
                if (dataResponse.status === 1) {
                    swal("Success!", dataResponse.message, "success");
                    $('#enquiryReplyModal').modal('hide');
                    GetAllBusinessEnquiries();
                }
                else {
                    swal({
                        title: 'Error!',
                        icon: 'error',
                        text: dataResponse.message
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

}


function getEnquiryDetailById(id) {
    StartLoading();
    let _url = "/api/Enquiry/GetById?id=" + id;

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

            if (response.data && response.data.EnquiryDetail != null) {
                var modalBody = `<p class="modl-txt">${response.data.EnquiryDetail.Description}</p>`;
                if (response.data.EnquiryDetail.IsReplied == 1) {
                    modalBody += `<hr/><h6 class="text-black-50"><i class="fa fa-reply"></i> Reply <span class="float-right">${response.data.EnquiryDetail.RepliedOn_FormatDate}</span></h6>
                            <p class="modl-txt">${response.data.EnquiryDetail.ReplyBody}</p>
                        `;
                }
                $('#enquiryDetailModal .modal-title').html(response.data.EnquiryDetail.Title);
                $('#enquiryDetailModal .modal-body').html(modalBody);
                $('#enquiryDetailModal').modal('show');
            }
            else {
                $.iaoAlert({
                    msg: 'Did not get data. Try Again later!',
                    type: "error",
                    mode: "dark",
                });
            }

            StopLoading();
        },
        error: function (result) {

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

function ReplyEnquiry(id) {
    //$('#btnOpenEnquiryReplyModal').click();
    StartLoading();
    let _url = "/api/Enquiry/GetById?id=" + id;

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

            if (response.data && response.data != null) {
                var modalBody = `<h3>${response.data.EnquiryDetail.Title}</h3><p class="modl-txt">${response.data.EnquiryDetail.Description}</p><hr/>`;
                
                EnquiryId_Global = response.data.EnquiryDetail.Id;
                $('#enquiryDetail_ReplyModal').html(modalBody);
                $('#enquiryReplyModal').modal('show');
            }
            else {
                $.iaoAlert({
                    msg: 'Did not get data. Try Again later!',
                    type: "error",
                    mode: "dark",
                });
            }

            StopLoading();
        },
        error: function (result) {

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

function ResetReplyModal() {
    EnquiryId_Global = 0;

    $('#enquiryReplyModal .error-class').html('');
    $('#textareaEnquiryReply').val('');

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