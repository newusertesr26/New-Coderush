$(document).ready(function () {

    //$('#btnsave').click(function () {
    //    debugger
    //    //var Id = $("#hideid").val();
    //    var Projectid = $("#invprojid").val();
    //    var Amount = $("#invamount").val();
    //    var Duedate = $("#invdate").val();
    //    var InvoiceNumber = $("#invinvoicenumber").val();
    //    var PendingAmount = $("#invpendingamount").val();
    //    var Isverify = $("#chkverify").is(":checked");


    //    if (Projectid == null || Projectid == undefined || Projectid == "") {
    //        $(".spantext").show();
    //        return false;
    //    }
    //    else {
    //        $(".spantext").hide();
    //    }


    //    if (Amount == null || Amount == undefined || Amount == "") {
    //        $(".spandescription").show();
    //        return false;
    //    }
    //    else {
    //        $(".spandescription").hide();
    //    }

    //    if (Duedate == null || Duedate == undefined || Duedate == "") {
    //        $(".spanSelection").show();
    //        return false;
    //    }
    //    else {
    //        $(".spanSelection").hide();
    //    }

    //    if (InvoiceNumber == null || InvoiceNumber == undefined || InvoiceNumber == "") {
    //        $(".spanSelection").show();
    //        return false;
    //    }
    //    else {
    //        $(".spanSelection").hide();
    //    }

    //    var fileData = new FormData();
    //    fileData.append('Id', Id);
    //    fileData.append('ProjectId', Projectid);
    //    fileData.append('Amount', Amount);
    //    fileData.append('Duedate', Duedate);
    //    fileData.append('InvoiceNumber', InvoiceNumber);
    //    fileData.append('PendingAmount', PendingAmount);
    //    fileData.append('Isverify', Isverify);


    //    $.ajax({
    //        url: '/InvoiceMaster/AddOrEdit',
    //        type: "POST",
    //        contentType: false,
    //        processData: false,
    //        data: fileData,
    //        success: function (result) {
    //            $("#AddInvoice").modal('hide');
    //            alert(result.message);
    //            window.location.reload();
    //        },
    //        error: function (err) {
    //            alert("Fail!");
    //        }
    //    });
    //});

    BindDropDown();

    //$("#btnclose,#invbtncls").click(function () {
    //    $("#hideid").val("");
    //    $("#invprojid").val("");
    //    $("#invamount").val("");
    //    $("#invdate").val();
    //    $("#invinvoicenumber").val("");
    //    $("#invpendingamount").val("");
    //    $("#chkverify").val("");
    //    $("#AddInvoice").modal("hide");
    //});

    //$("#btnaddinvoicepopup").click(function () {
    //    $(".text-danger").hide();
    //    $("#AddInvoice").modal("show");
    //    BindDropDown();
    //});

});


//function Editdata(id) {
//    $.ajax({
//        url: '/InvoiceMaster/EditData?id=' + id,
//        type: "GET",
//        contentType: false,
//        processData: false,
//        success: function (result) {
//            $("#hideid").val(result.id);
//            $("#invprojid").val(result.projectId);
//            $("#invamount").val(result.amount);
//            $("#invdate").val(result.duedate);
//            $("#invinvoicenumber").val(result.invoiceNumber);
//            $("#invpendingamount").val(result.pendingAmount);
//            $("#chkverify").val(result.isverify);
//            $("#AddInvoice").modal("show");
//            $(".text-danger").hide();
//        },
//        error: function (err) {
//            /*alert(result.message);*/
//        }
//    });
//}



var BindDropDown = function () {
    $.ajax({
        url: "/InvoiceMaster/BindDropDown",
        type: "GET",
        contentType: false,
        processData: false,
        contentType: "application/json",
        success: function (res) {
            var Project = "#invprojid";
            $(Project).empty();
            $(Project).append($("<option></option>").val("").html("--Select--"));
            $.each(res, function (index, object) {

                $(Project).append($("<option></option>").val(object.value).html(object.text));
            });

            var Make = $(Project).data();
            if (Make != null && Make != undefined) {
                $(Project).val(Make.make).trigger('change');
            }

            $(Project).on('change', function () {
                debugger
                var id = $(this).val();
                Bindamount(id);
            })
        },
        error: function (res, err) {

        }

    });
};
var Bindamount = function (Id) {
    debugger
    $.ajax({
        type: "GET",
        url: "/InvoiceMaster/Bindamount?ProjectId=" + Id,
        dataType: "json",
        data: {},
        contentType: "application/json",
        success: function (res) {
            //$("#txtdeviceworkphone").empty();
            $("#invamount").empty();
            $("#invamount").val(res);
            $("#invamountold").val(res);
        },
        error: function (res, err) {

        }
    });

};

$("#invamount").on('change', function () {

    debugger
    var amt = $("#invamount").val();
    var amtold = $("#invamountold").val();
    if (amt < amtold) {
        var different = amtold - amt
        $("#invpendingamount").val(different);
    }
    if (amtold < amt) {
        $("#invpendingamount").val("");
    }
})

$("#invpendingamount").on("blur", function () {
    debugger

    var amunt = $(this).val();
    var filter = /^\d*(?:\.\d{1,2})?$/;
    if (filter.test(amunt)) {
        $(".prjcurncyvalid").show();
        return false;
    }
    else {
        $(".prjcurncyvalid").hide();
        return false;
    }
});
//function Delete(id) {
//    var result = confirm("Are you sure want to delete?");
//    if (result) {
//        $.ajax({
//            url: '/InvoiceMaster/Delete?id=' + id,
//            type: "POST",
//            contentType: false,
//            processData: false,
//            success: function (result) {
//                alert(result.message);
//                window.location.reload();
//            },
//            error: function (err) {

//            }
//        });
//    }

//}
