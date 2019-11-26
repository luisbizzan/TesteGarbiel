$(function () {

    BuscarIdEmpresa();

    $('#ddlEmpresa').change(function (e) {
        var id = $('#ddlEmpresa').val() === "" ? 0 : $('#ddlEmpresa').val();
        dart.modalAjaxConfirm.open({
            title: 'Empresa',
            message: "Deseja realmente mudar a empresa?",
            url: HOST_URL + "BOEmpresa/MudarEmpresa?idEmpresa=" + id,
            onCancel: cancelMudarEmpresa,
            onConfirm: confirmMudarEmpresa
        });
    });
});

var cancelMudarEmpresa = BuscarIdEmpresa;
var confirmMudarEmpresa = Confirm;

function BuscarIdEmpresa() {
    $.post(HOST_URL + "BOEmpresa/BuscarIdEmpresa", function (result) {
        if (result === "0") {
            $("#ddlEmpresa").val($("#ddlEmpresa option:first").val());
        }
        else {
            $("#ddlEmpresa").val(result);
        }
    });
}

function Confirm() {
    location.reload();
}