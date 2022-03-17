$(document).ready(function () {

    $('#btninsert').click(function () {
        var id = $("#hednid").val();
        var userid = $("#leavedrpdwn").val();
        var fromdate = $("#levfrmdate").val();
        var todate = $("#levtodate").val();
        var count = $("#levcount").val();
        var description = $("#levdescription").val();
        var isapprove = $("#isaprv").val();
        var approveDate = $("#apprvdate").val();


        ////var Isvalid = true;
        ////if (userid == null || userid == undefined || userid == "") {
        ////    $(".spansuerid").show();
        ////    Isvalid = false;
        ////}

        ////if (fromdate == null || fromdate == undefined || fromdate == "") {
        ////    $(".spanfrmdate").show();
        ////    Isvalid = false;
        ////}

        ////if (todate == null || todate == undefined || todate == "") {
        ////    $(".spantodate").show();
        ////    Isvalid = false;
        ////}

        ////if (count == null || count == undefined || count == "") {
        ////    $(".spancount").show();
        ////    Isvalid = false;
        ////}
        ////if (description == null || description == undefined || description == "") {
        ////    $(".spandescription").show();
        ////    Isvalid = false;
        ////}


        //if (!Isvalid) {
        //    return false;
        //}
        //$(".text-danger").hide();
        var formdata = new FormData();
        formdata.append('Id', id);
        formdata.append('Userid', userid);
        formdata.append('Fromdate', fromdate);
        formdata.append('Todate', todate);
        formdata.append('Count', count);
        formdata.append('Description', description);
        formdata.append('Isapprove', isapprove);
        formdata.append('ApproveDate', approveDate);



        $.ajax({
            url: '/Leavecount/SubmitForm',
            type: "POST",
            contentType: false,
            processData: false,
            data: formdata,
            success: function (result) {
                //$("#AddLeave").modal('hide');
                alert(result.message);
                Bindtablegrid(userid, "");
            },
            error: function (err) {
                alert("Fail!");
            }
        });
    });


    //$("#btncls, #levbtncls").unbind().click(function () {
    //    $("#hednid").val("");
    //    $("#levuserid").val("");
    //    $("#levfrmdate").val("");
    //    $("#levtodate").val("");
    //    $("#levcount").val("");
    //    $("#levdescription").val("");
    //    $("#isaprv").val("");
    //    $("#apprvdate").val("");
    //    $("#AddLeave").modal("hide");
    //});


    $("#btninsert").unbind().click(function () {

        var userid = $("#leavedrpdwn").val();
        if (userid == null || userid == "" || userid == undefined) {
            alert("Please select drowpdown menu first !!");
            return false;
        }
        $(".text-danger").hide();
        //$("#AddLeave").modal("show");
        //var BinddrpdwnData = $("#leavedrpdwn option:selected").text();
        //$("#levuserid").val(BinddrpdwnData);
    });

    var Bindtablegrid = function (id, username) {
        $.ajax({
            url: "/Leavecount/BindGridData?id=" + id + "&UserName=" + username,
            method: 'Get',
            data: {},
            success: function (data) {
                debugger
                /*console.log(data);*/
                $("#levtblbdy").empty();
                if (data.list != null && data.list.length > 0) {
                    var innerHtml = '';
                    $.each(data.list, function (i, v) {
                        var rowdata = data.list[i]
                        innerHtml += "<tr>";
                        debugger
                        //innerHtml += "<td><i class='fa fa-edit' style='font-size:20px' id='btnedit' data-id = " + rowdata.id + "></i></td>";
                        //innerHtml += "<td><i class='fa fa-trash' ></i></td>";
                        innerHtml += "<td><a href='/Leavecount/Form/" + rowdata.id + "'><i class='fa fa-edit'></i></a></td>";
                        innerHtml += "<td><a href='/Leavecount/Delete/" + rowdata.id + "'><i class='fa fa-trash'></i></a></td>";
                        innerHtml += "<td scope='col' id='User ID'>" + rowdata.userid + "</td>";
                        /*innerHtml += "<td scope='col' id='User ID'>" + rowdata.firstname + rowdata.lastname + "</td>";*/
                        innerHtml += "<td scope='col' id='From Date'>" + rowdata.fromdate + "</td>";
                        innerHtml += "<td scope='col' id='To Date'>" + rowdata.todate + "</td>";
                        innerHtml += "<td scope='col' id='Count'>" + rowdata.count + "</td>";
                        innerHtml += "<td scope='col' id='Description'>" + rowdata.description + "</td>";
                        innerHtml += "<td scope='col' id='IsApprove'>" + rowdata.isapprove + "</td>";
                        innerHtml += "<td scope='col' id='Approve Date'>" + rowdata.approveDate + "</td>";
                        //innerHtml += '<td>@Html.ActionLink("Download", "DownloadFile", new { fileName = item.FileUpload })</td>';
                        innerHtml += "<td><a href='/Leavecount/DownloadFile/" + rowdata.filename + "'><i class='fa fa-download'></i></a></td>";
                        innerHtml += "</tr>"
                    });
                    $("#levtblbdy").html(innerHtml);
                }
                else {
                    var innerhtml = '<tr><td colspan="9" class="text-center">No record found.</td></tr>';
                    $("#levtblbdy").html(innerhtml);
                }
            }

        });
    }

    //$('#levtodate').on('change', function () {
    //    var fromdate = $('#levfrmdate').val();
    //    var todate = $('#levtodate').val();
    //    if (todate < fromdate) {
    //        alert('To date should be greater than From date.');
    //        $('#levtodate').val('');
    //    }
    //});

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
                });

                var Make = $(Project).data();
                if (Make != null && Make != undefined) {
                    $(Project).val(Make.make).trigger('change');
                }

                //$(Project).on('change', function () {
                //    var id = $(this).val();
                //    Bindtablegrid(id);
                //    $("#grid").show();
                //})
            },
            error: function (res, err) {

            }

        });
    };



    $("#leavedrpdwn").on('change', function () {
        debugger
        //var id = $(this).val();
        var id = $("#leavedrpdwn option:selected").val();
        var username = $("#leavedrpdwn option:selected").text();
        Bindtablegrid(id, username);
        $("#grid").show();
    });




    $('body').on('click', '#btnedit', function () {
        debugger
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
                $("#levdescription").val(result.description);
                $("#isaprv").val(result.isapprove);
                $("#apprvdate").val(result.approveDate);
                //$("#AddLeave").modal("show");
                $(".text-danger").hide();
            },
            error: function (err) {
                /*alert(result.message);*/
            }
        });
    });

    BinddrpdwnData();

});


//$(function () {
//    debugger
//    $("#levfrmdate").datepicker({
//        minDate: new Date(),
//        numberOfMonths: 2,
//        onSelect: function (selected) {
//            var dt = new Date(selected);
//            dt.setDate(dt.getDate() + 1);
//            $("#levtodate").datepicker("option", "minDate", dt);
//        }
//    });
//    $("#levtodate").datepicker({
//        numberOfMonths: 2,
//        onSelect: function (selected) {
//            var dt = new Date(selected);
//            dt.setDate(dt.getDate() - 1);
//            $("#levfrmdate").datepicker("option", "maxDate", dt);
//        }
//    });
//});