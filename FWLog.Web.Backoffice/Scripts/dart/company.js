$(function () {

    GetCompanyId();

    $('#ddlCompany').change(function (e) {
        var id = $('#ddlCompany').val() === "" ? 0 : $('#ddlCompany').val();
        dart.modalAjaxConfirm.open({
            title: 'Empresa',
            message: "Deseja realmente mudar a empresa?",
            url: HOST_URL + "Company/ChangeCompany?companyId=" + id,
            onCancel: cancelChangeCompany,
            onConfirm: confirmChangeCompany
        });
    });
});

var cancelChangeCompany = GetCompanyId;
var confirmChangeCompany = Confirm;

function GetCompanyId() {
    $.post(HOST_URL + "Company/GetCompanyId", function (result) {
        if (result === "0") {
            $("#ddlCompany").val($("#ddlCompany option:first").val());
        }
        else {
            $("#ddlCompany").val(result);
        }
    });
}

function Confirm() {
    location.reload();
}