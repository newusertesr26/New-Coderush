$(document).ready(function () {

    $('#btninsert').click(function () {
        var id = $("#hednid").val();
        var userid = $("#leavedrpdwn").val();
        var fromdate = $("#levfrmdate").val();
        var todate = $("#levtodate").val();
        var count = $("#levcount").val();
        var EmployeeDescription = $("#levdescription").val();
        var HrDescription = $("#HrDescription").val();
        var isapprove = $("#isaprv").val();
        var approveDate = $("#apprvdate").val();
        var formdata = new FormData();
        formdata.append('Id', id);
        formdata.append('Userid', userid);
        formdata.append('Fromdate', fromdate);
        formdata.append('Todate', todate);
        formdata.append('Count', count);
        formdata.append('EmployeeDescription', EmployeeDescription);
        formdata.append('HrDescription', HrDescription);
        formdata.append('Isapprove', isapprove);
        formdata.append('ApproveDate', approveDate);
        $.ajax({
            url: '/Leavecount/SubmitForm',
            type: "POST",
            contentType: false,
            processData: false,
            data: formdata,
            success: function (result) {
                alert(result.message);
                Bindtablegrid(userid, "");
            },
            error: function (err) {
                alert("Fail!");
            }
        });
    });



    $("#btninsert").unbind().click(function () {

        var userid = $("#leavedrpdwn").val();
        if (userid == null || userid == "" || userid == undefined) {
            /* alert("Please select drowpdown menu first !!");*/
            alert("Please select user");
            return false;
        }
        $(".text-danger").hide();
    });

    var Bindtablegrid = function (id, username, userid) {
        $.ajax({
            url: "/Leavecount/BindGridData?id=" + id + "&UserName=" + username + "&Userid=" + userid,
            method: 'Get',
            data: {},
            success: function (data) {
                console.log(data);
                $("#levtblbdy").empty();
                if (data.list != null && data.list.length > 0) {
                    var innerHtml = '';
                    var arrya = new Array();
                    $.each(data.list, function (i, v) {
                        var rowdata = data.list[i]
                        innerHtml += "<tr>";
                        //innerHtml += "<td><i class='fa fa-edit' style='font-size:20px' id='btnedit' data-id = " + rowdata.id + "></i></td>";
                        //innerHtml += "<td><i class='fa fa-trash' ></i></td>";
                        //innerHtml += "<td><a href='/Leavecount/Form?id=" + rowdata.id + "'&userid='" + rowdata.userid +"'><i class='fa fa-edit'></i></a></td>";
                        if (rowdata.adminRole) {
                            innerHtml += "<td><a href='javascript:void(0)' class='editleave'data-id='" + rowdata.id + "'data-userid='" + rowdata.userid + "'><i class='fa fa-edit'></i></a></td>";
                        }
                        //innerHtml += "<td><a href='/Leavecount/Delete/" + rowdata.id + "'><i class='fa fa-trash'></i></a></td>";
                        innerHtml += "<td scope='col' id='User ID'>" + rowdata.userid + "</td>";
                        innerHtml += "<td scope='col' id='From Date'>" + moment(rowdata.fromdate).format('LL') + "</td>";
                        innerHtml += "<td scope='col' id='To Date'>" + moment(rowdata.todate).format('LL') + "</td>";
                        innerHtml += "<td scope='col' id='Count'>" + rowdata.count + "</td>";
                        innerHtml += "<td scope='col' id='Description'>" + rowdata.employeeDescription + "</td>";
                        if (rowdata.adminRole) {
                            innerHtml += "<td scope='col' id='Description'>" + rowdata.hrDescription + "</td>";
                        }
                        /*innerHtml += "<td scope='col' id='IsApprove'>" + rowdata.isapprove + "</td>";*/
                        if (rowdata.isapprove == true) {
                            innerHtml += "<td scope='col' > <input type='checkbox' data-approveId='1' class='clsChecked' id='IsChecked" + rowdata.id + "' disabled /></td>";
                            arrya.push(rowdata.id);
                        } else {
                            innerHtml += "<td scope='col' > <input type='checkbox' data-approveId='0' class='clsChecked' id='IsChecked" + rowdata.id + "' disabled /></td>";
                        }
                        innerHtml += "<td scope='col' id='Approve Date'>" + moment(rowdata.approveDate).format('LL') + "</td>";
                        //innerHtml += '<td>@Html.ActionLink("Download", "DownloadFile", new { fileName = item.FileUpload })</td>';
                        innerHtml += "<td><a class='cladownload' data-downloadFile='/document/Leave/" + rowdata.filename + "'><i class='fa fa-download'></i></a></td>";
                        if (rowdata.adminRole) {
                            innerHtml += "<td><input type='button' value='approve' class='clsAprove' data-aproveid='" + rowdata.id + "' id='btn_" + rowdata.id + "' ></td>";
                        }
                        innerHtml += "</tr>"
                    });
                    $("#levtblbdy").html(innerHtml);

                    $(".editleave").unbind().click(function () {
                        var id = $(this).data('id');
                        var userid = $(this).data('userid');
                        window.location.href = '/LeaveCount/Form?id=' + id + '&userid=' + userid;
                    });

                    for (var i = 0; i < arrya.length; i++) {
                        console.log(arrya[i]);
                        $('#IsChecked' + arrya[i]).prop('checked', true);
                    }
                }
                else {
                    var innerhtml = '<tr><td colspan="9" class="text-center">No record found.</td></tr>';
                    $("#levtblbdy").html(innerhtml);
                }
            }
        });
    }

    $(document).on("click", ".clsAprove", function () {
        var Id = $(this).data('aproveid');
        $('#myModal').modal('show');
        $("#leaveId").val(Id);
        clearTextBox();
    })

    $(document).on("click", ".Approvcheck", function () {
        $('.Approvcheck').not(this).prop('checked', false);
    })

    $(document).on("click", "#saveLeave", function () {
        var Id = $('#leaveId').val();
        var HRDescription = $('#notes').val();
        if (HRDescription == '') {
            alert("HRDescription is Required");
            return false;
        }
        var Isapprove = $('.Approvcheck:checked').val() == "1" ? true : false;
        $.ajax({
            url: "/Leavecount/leavepopuop?Id=" + Id + "&HRDescription=" + HRDescription + "&Isapprove=" + Isapprove,
            type: "POST",
            contentType: false,
            processData: false,
            contentType: "application/json",
            success: function (res) {
                var id = $("#leavedrpdwn option:selected").val();
                var username = $("#leavedrpdwn option:selected").text();
                Bindtablegrid(id, username);
            },
            error: function (res, err) {
            }
        });
    })

    var BinddrpdwnData = function () {
        $.ajax({
            url: "/Leavecount/BinddrpdwnData",
            type: "GET",
            contentType: false,
            processData: false,
            contentType: "application/json",
            success: function (res) {
                var Project = "#leavedrpdwn";
                $(Project).empty();
                $(Project).append($("<option></option>").val("").html("--Select--"));
                $.each(res, function (index, object) {
                    $(Project).append($("<option></option>").val(object.value).html(object.text));
                    //Bindtablegrid(object.value, object.text);
                });

                //var Make = $(Project).data();
                //if (Make != null && Make != undefined) {
                //    $(Project).val(Make.make).trigger('change');
                //}

            },
            error: function (res, err) {

            }
        });
    };

    $("#leavedrpdwn").on('change', function () {
        var id = $("#leavedrpdwn option:selected").val();
        var username = $("#leavedrpdwn option:selected").text();
        Bindtablegrid(id, username);
        $("#grid").show();
    });
    $("#btninsert").unbind().click(function () {
        var userid = $("#leavedrpdwn").val();
        if (userid == null || userid == "" || userid == undefined) {

            var username = $("#leavedrpdwn option:selected").text();
            alert("Please select user");
            return false;
        }
        

        var userid = $("#leavedrpdwn").val();
        window.location.href = '/LeaveCount/Form?id=' + 0 + '&userid=' + userid;
    });

    $('body').on('click', '#btnedit', function () {
        var id = $(this).data('id');
        $.ajax({
            url: '/Leavecount/Form?id=' + id,
            type: "GET",
            contentType: false,
            processData: false,
            success: function (result) {
                $("#hednid").val(result.id);
                $("#levuserid").val($("#leavedrpdwn option:selected").text());
                $("#levfrmdate").val(result.fromdate);
                $("#levtodate").val(result.todate);
                $("#levcount").val(result.count);
                $("#levdescription").val(result.EmployeeDescription);
                $("#isaprv").val(result.isapprove);
                $("#apprvdate").val(result.approveDate);
                $("#AddLeave").modal("show");
                $(".text-danger").hide();
            },
            error: function (err) {
            }
        });
    });

    BinddrpdwnData();

    Bindtablegrid("","");

    $(document).on("click", ".cladownload", function () {
        var fileName = $(this).attr('data-downloadFile');
        //var path = "/document/Leave/";
        window.open(window.origin + fileName, '_blank'); // open the pdf in a new window/tab
    })


    function clearTextBox() {
        $('#notes').val("");
        $('#notes').css('border-color', 'lightgrey');
        $('#IsApprove').css('border-color', 'lightgrey');
        $('#IsReject').css('border-color', 'light');
        $("#IsApprove").prop("checked", false);
        $("#IsReject").prop("checked", false);

    }
});

