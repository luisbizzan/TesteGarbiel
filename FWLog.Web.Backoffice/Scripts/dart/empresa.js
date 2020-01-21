$(function () {

    BuscarIdEmpresa();

    $('#ddlEmpresa').change(function (e) {
        var id = $('#ddlEmpresa').val() === "" ? 0 : $('#ddlEmpresa').val();
        dart.modalAjaxConfirm.open({
            title: 'Empresa',
            message: "Deseja realmente mudar a empresa?",
            url: HOST_URL + "Empresa/MudarEmpresa/" + id,
            onCancel: cancelMudarEmpresa,
            onConfirm: confirmMudarEmpresa,

        });
    });

    $('[data-toggle="tooltip"]').on('click', function () {
        $(this).tooltip('hide')
    });

    $('#ddlPerfilImpressora').change(function (e) {
        var valor = e.target.value;

        $.ajax({
            type: "POST",
            url: HOST_URL + "PerfilImpressora/DefinePerfilImpressoraSessao",
            data: { idPerfil: valor }
        });
    });

    $('[data-toggle="tooltip"]').tooltip({
        trigger: 'hover'
    });
});

var cancelMudarEmpresa = BuscarIdEmpresa;
var confirmMudarEmpresa = Confirm;

function BuscarIdEmpresa() {
    $.post(HOST_URL + "Empresa/BuscarIdEmpresa", function (result) {
        if (result === "0") {
            $("#ddlEmpresa").val($("#ddlEmpresa option:first").val());
        }
        else {
            $("#ddlEmpresa").val(result);
        }
    });
}

function Confirm(result) {
    window.location.href = result.Data;
}