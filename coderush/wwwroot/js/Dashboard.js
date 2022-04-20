$(document).ready(function () {

    // Bind LeaveCount Grid //
    var Bindtablegrid = function (id, username, userid) {
        debugger
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
                        innerHtml += "<tr style='background-color:" + rowdata.colouris + "'>";
                        innerHtml += "<td scope='col' id='User ID'>" + rowdata.userid + "</td>";
                        innerHtml += "<td scope='col' id='From Date'>" + moment(rowdata.fromdate).format('LL') + "</td>";
                        innerHtml += "<td scope='col' id='To Date'>" + moment(rowdata.todate).format('LL') + "</td>";
                        innerHtml += "<td scope='col' id='Count'>" + rowdata.count + "</td>";
                        innerHtml += "<td scope='col' id='Description'>" + rowdata.employeeDescription + "</td>";
                        if (rowdata.adminRole) {
                            innerHtml += "<td scope='col' id='Description'>" + rowdata.hrDescription + "</td>";
                        }
                        if (rowdata.isapprove == true) {
                            innerHtml += "<td scope='col' > <input type='checkbox' data-approveId='1' class='clsChecked' id='IsChecked" + rowdata.id + "' disabled /></td>";
                            arrya.push(rowdata.id);
                        } else {
                            innerHtml += "<td scope='col' > <input type='checkbox' data-approveId='0' class='clsChecked' id='IsChecked" + rowdata.id + "' disabled /></td>";
                        }
                        innerHtml += "<td scope='col' id='Approve Date'>" + moment(rowdata.approveDate).format('LL') + "</td>";
                        innerHtml += "</tr>"
                    });
                    $("#levtblbdy").html(innerHtml);
                    debugger
                    gridSorting();
                    debugger
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
                    debugger
                    var innerhtml = '<tr><td colspan="9" class="text-center">No record found.</td></tr>';
                    $("#levtblbdy").html(innerhtml);
                }
            }
        });
    }

    // Bind User Drop-Down //
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
            },
            error: function (res, err) {
            }
        });
    };

    // Bind Data Sorting //
    function gridSorting() {
        debugger
        $(".grid1").dataTable({
            aaSorting: [[2, 'asc']],
            bPaginate: false,
            bFilter: false,
            bInfo: false,
            bSortable: true,
            bRetrieve: true,
            aoColumnDefs: [
                { "aTargets": [0], "bSortable": true },
                { "aTargets": [1], "bSortable": true },
                { "aTargets": [2], "bSortable": true },
                //{ "aTargets": [3], "bSortable": true },
                //{ "aTargets": [4], "bSortable": true },
                //{ "aTargets": [5], "bSortable": true },
                //{ "aTargets": [6], "bSortable": true },
                //{ "aTargets": [7], "bSortable": true }
            ]
        });
    }
    debugger
    Bindtablegrid("", "", "");
    BinddrpdwnData();

    $('.grid').DataTable({
        lengthChange: false,
        info: false,
        searching: true,
        dom: 'lrtip',
        scrollX: false,
        pageLength: 25,
        paging: false,

    });

    // Drop-Dwon Change Event //
    $("#leavedrpdwn").on('change', function () {
        var id = $("#leavedrpdwn option:selected").val();
        var username = $("#leavedrpdwn option:selected").text();
        Bindtablegrid(id, username);
        $("#grid").show();
    });

});
