(function () {
    $('#formRemessaCriar').submit(function (e) {
        e.preventDefault();
        criarRemessa();
    });
})();

function criarRemessa() {
    var formulario = $("form#formImportarSolicitacao").serialize();

    $.ajax({
        url: HOST_URL + CONTROLLER_PATH + "RemessaCriarGravar",
        method: "POST",
        data: formulario,
        success: function (result) {
            if (result.Success) {
                $("#modalVisualizar").modal('hide');
                PNotify.success({ text: result.Message });
            } else {
                PNotify.error({ text: result.Message });
            }
        },
        error: function (request, status, error) {
            PNotify.warning({ text: result.Message });
        }
    });
}