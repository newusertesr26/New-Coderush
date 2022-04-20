$(document).ready(function () {
    // Bind LeaveCount Grid //
    var UNBindtablegrid = function (id, username, userid) {
        debugger
        $.ajax({
            url: "/Leavecount/BindGridData?id=" + id + "&UserName=" + username + "&Userid=" + userid,
            method: 'Get',
            data: {},
            success: function (data) {
                console.log(data);
                $("#UNlevtblbdy").empty();
                if (data.list != null && data.list.length > 0) {
                    var innerHtml = '';
                    var arrya = new Array();
                    $.each(data.list, function (i, v) {
                        var rowdata = data.list[i]
                        innerHtml += "<tr>"
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
                        innerHtml += "<td><a class='cladownload' data-downloadFile='/document/Leave/" + rowdata.filename + "'><i class='fa fa-download'></i></a></td>";
                        innerHtml += "</tr>"
                    });
                    $("#UNlevtblbdy").html(innerHtml);
                    gridSorting();
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
                    $("#UNlevtblbdy").html(innerhtml);
                }
            }
        });
    }

    // Bind Data Sorting //
    function gridSorting() {
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
            ]
        });
    }
    UNBindtablegrid("", "", "");

    $(document).on("click", ".cladownload", function () {
        var fileName = $(this).attr('data-downloadFile');
        if (fileName) {
            window.open(window.origin + fileName, '_blank');
        }
    })

    $('.grid').DataTable({
        lengthChange: false,
        info: false,
        searching: true,
        dom: 'lrtip',
        scrollX: false,
        pageLength: 25,
        paging: false,

    });

    $(document).on("click", "#unpaidleavespopup", function () {
        debugger
        var id = $(this).attr('data-id');
        var username = $(this).attr('data-username')
        if (id && username) {
            UNBindtablegrid(id, username, "");
            $("#grid").show();
        }
    })
});
