$(document).ready(function () {

    $('#btnprojectsave').click(function () {
        var id = $("#hidenid").val();
        var projectName = $("#prjctname").val();
        var technologies = $("#ddltechnologiesList option:selected").val();
        var description = $("#prjctdescription").val();
        var managerName = $("#prjctmanager").val();
        var developerName = $("#prjctdeveloper").val();
        var paymenttype = $("#prjctpayment").val();
        var projectamount = $("#prjctamount").val();
        var currency = $("#prjcurrency").val();
        var isactive = $("#chkisactive").is(":checked");


        var Isvalid = true;
        if (projectName == null || projectName == undefined || projectName == "") {
            $(".prospanname").show();
            Isvalid = false;
        }

        if (technologies == null || technologies == undefined || technologies == "") {
            $(".prospantechlogies").show();
            Isvalid = false;
        }

        if (description == null || description == undefined || description == "") {
            $(".prospandescription").show();
            Isvalid = false;
        }

        if (managerName == null || managerName == undefined || managerName == "") {
            $(".prospanprjmanager").show();
            Isvalid = false;
        }

        if (developerName == null || developerName == undefined || developerName == "") {
            $(".prospanprjdeveloper").show();
            Isvalid = false;
        }

        if (paymenttype == null || paymenttype == undefined || paymenttype == "") {
            $(".prospanpyment").show();
            Isvalid = false;
        }

        if (projectamount == null || projectamount == undefined || projectamount == "") {
            $(".prospanprjamount").show();
            Isvalid = false;
        }

        if (currency == null || currency == undefined || currency == "") {
            $(".prospanprjcurncy").show();
            Isvalid = false;
        }

        if (!Isvalid) {
            return false;
        }
        $(".text-danger").hide();

        var fileData = new FormData();
        fileData.append('Id', id);
        fileData.append('ProjectName', projectName);
        fileData.append('Technologies', technologies);
        fileData.append('Description', description);
        fileData.append('ManagerName', managerName);
        fileData.append('DeveloperName', developerName);
        fileData.append('paymenttype', paymenttype);
        fileData.append('projectamount', projectamount);
        fileData.append('currency', currency);
        fileData.append('isactive', isactive);


        $.ajax({
            url: '/ProjectMaster/AddOrEdit',
            type: "POST",
            contentType: false,
            processData: false,
            data: fileData,
            success: function (result) {
                $("#AddProjects").modal('hide');
                alert(result.message);
                window.location.reload();
                $(".spanprjcurncy").hide();
            },
            error: function (err) {
                alert("Fail!");
            }
        });
    });

    $("#prjcurrency").on("blur", function () {
        var amunt = $(this).val();
        var filter = /^\d*(?:\.\d{1,2})?$/;

        if (filter.test(amunt)) {
            if (amunt == null || amunt == undefined || amunt == "") {
                $(".spanprjcurncy").show();
                $(".prjcurncyvalid").hide();

                return false;
            }
        }
        else {
            $(".prjcurncyvalid").show();
            $(".spanprjcurncy").hide();
            return false;
        }
        $(".prjcurncyvalid").hide();
    });

    $("#prjctamount").on("blur", function () {
        var amunt = $(this).val();
        var filter = /^\d*(?:\.\d{1,2})?$/;

        if (filter.test(amunt)) {
            if (amunt == null || amunt == undefined || amunt == "") {
                $(".prjamountvalid").hide();
                $(".spanprjamount").show();

                return false;
            }
        }
        else {
            $(".spanprjamount").hide();
            $(".prjamountvalid").show();
            return false;
        }
        $(".prjamountvalid").hide();
    });

    $("#btncprojectlose, #probtncls").click(function () {
        $("#hidenid").val("");
        $("#prjctname").val("");
        $("#ddltechnologiesList").val("");
        $("#prjctdescription").val("");
        $("#prjctmanager").val("");
        $("#prjctdeveloper").val("");
        $("#prjctpayment").val("");
        $("#prjctamount").val("");
        $("#prjcurrency").val("");
        $("#chkisactive").prop("checked", false);
        $("#AddProjects").modal("hide");
    });

    $("#btnaddprojectpopup").click(function () {
        $(".text-danger").hide();
        $("#AddProjects").modal("show");
    });

});


function Editdata(id) {
    $.ajax({
        url: '/ProjectMaster/EditData?id=' + id,
        type: "GET",
        contentType: false,
        processData: false,
        success: function (result) {
            $("#hidenid").val(result.id);
            $("#prjctname").val(result.projectName);
            $("#ddltechnologiesList").val(result.technologies);
            $("#prjctdescription").val(result.description);
            $("#prjctmanager").val(result.managerName);
            $("#prjctdeveloper").val(result.developerName);
            $("#prjctpayment").val(result.paymenttype);
            $("#prjctamount").val(result.projectamount);
            $("#prjcurrency").val(result.currency);
            $("#chkisactive").val('checked', result.isactive);
            $("#AddProjects").modal("show");
            $(".text-danger").hide();
        },
        error: function (err) {
            /*alert(result.message);*/
        }
    });
}

function Delete(id) {
    var result = confirm("Are you sure want to delete?");
    if (result) {
        $.ajax({
            url: '/ProjectMaster/Delete?id=' + id,
            type: "POST",
            contentType: false,
            processData: false,
            success: function (result) {
                alert(result.message);
                window.location.reload();
            },
            error: function (err) {

            }
        });
    }

}
