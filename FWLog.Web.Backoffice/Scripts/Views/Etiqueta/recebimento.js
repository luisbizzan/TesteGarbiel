(function () {
    $('.onlyNumber').mask('0#');

    $("#submit").click(function (e) {
        e.preventDefault();

        var dados = $("#recebimentoEtiqueta").serializeArray();

        $.ajax({
            url: "RecebimentoValidaImpressao",
            method: "POST",
            cache: false,
            data: dados,
            success: function (result) {
                if (result.Success) {
                    $("#modalImpressoras").load(HOST_URL + "BOPrinter/Selecionar?idImpressaoItem=5", function () {
                        $("#modalImpressoras").modal();
                    });
                } else {
                    PNotify.error({ text: result.Message });
                }
            },
            error: function (data) {
                PNotify.error({ text: "Não foi possível solicitar impressão." });
                NProgress.done();
            }
        });
    });
})();

function imprimir(acao, id) {
    var idImpressora = $("#IdImpressora").val();

    var dados = $("#recebimentoEtiqueta").serializeArray();
    dados.push({ name: "IdImpressora", value: idImpressora });

    $.ajax({
        url: "RecebimentoImprimir",
        method: "POST",
        cache: false,
        data: dados,
        success: function (result) {
            if (result.Success) {
                PNotify.success({ text: result.Message });

                fechaModal();
            } else {
                PNotify.error({ text: result.Message });
            }
        },
        error: function (data) {
            PNotify.error({ text: "Ocorreu um erro na impressão." });
            NProgress.done();
        }
    });
}

function fechaModal() {
    var $modal = $("#modalImpressoras");

    $modal.modal("hide");
    $modal.empty();
}