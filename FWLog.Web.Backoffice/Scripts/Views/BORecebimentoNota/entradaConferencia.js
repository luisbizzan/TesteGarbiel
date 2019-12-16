(function () {
    debugger;
    $("#Referencia").focus();
})();

function carregarDadosReferencia() {
    let idLote = $("#IdLote").val();
    let referencia = $("#Referencia").val();
    let $modal = $("#modalTratarDivergencia");

    $.ajax({
        url: HOST_URL + "BORecebimentoNota/CarregarDadosReferenciaConferencia",
        cache: false,
        method: "POST",
        data: {
            idLote: idLote,
            codigoBarras: referencia
        },
        success: function (result) {
            if (!!result.Success) {
                $modal.load(result, function () {
                    $modal.modal();
                });
            } else {
                PNotify.info({ text: result.Message });
            }
        }
    });
}