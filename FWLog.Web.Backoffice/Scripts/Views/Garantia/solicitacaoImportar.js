(function () {
    console.log('teste');
    $(".divTipo").hide();

    $("#Id_Tipo").change(function () {
        let valor = $(this).val();
        $(".divTipo").hide();

        if (valor == "30" || valor == "21") {
            $("#div1").show();
        } else if (valor == "31") {
            $("#div2").show();
        } else if (valor == "32") {
            $("#div3").show();
        }
    });

    $('#formImportarSolicitacao').submit(function (e) {
        e.preventDefault();
        importar();
    });
})();

function importar() {
    var formulario = $("form#formImportarSolicitacao").serialize();

    $.ajax({
        url: HOST_URL + CONTROLLER_PATH + "SolicitacaoImportarGravar",
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