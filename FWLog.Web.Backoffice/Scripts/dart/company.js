$(function () {

    GetCompanyId();

    $('#ddlCompany').change(function (e) {
        dart.modalAjaxConfirm.open({
            title: 'Empresa',
            message: "Deseja realmente mudar a empresa?",
            url: HOST_URL + "Company/ChangeCompany?companyId=" + $('#ddlCompany').val(),
            onCancel: cancelChangeCompany
        });
    });
});

var cancelChangeCompany = GetCompanyId;

function GetCompanyId() {
    $.post(HOST_URL + "Company/GetCompanyId", function (result) {
        $("#ddlCompany").val(result);
    });
}