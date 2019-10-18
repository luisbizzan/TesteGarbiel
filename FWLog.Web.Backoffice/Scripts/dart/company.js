$(function () {  
    $("#OldCompanyId").val($('#ddlCompany').val());

    $('#ddlCompany').change(function (e) {
        dart.modalAjaxConfirm.open({
            title: 'Empresa',
            message: "Deseja realmente mudar a empresa?",
            url: HOST_URL + "Company/ChangeCompany?companyId=" + $('#ddlCompany').val()            
        });
    });    
});

//Ajustar para cancelar.

function ChangeEmpresa() {
    $.post(HOST_URL + "Company/ChangeCompany", { id: $("#ddlCompany").val() }, function (result) {
        location.reload();
    });
}