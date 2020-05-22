(function () {
    $('input[type=radio]').iCheck({
        checkboxClass: 'icheckbox_flat-green',
        radioClass: 'iradio_flat-green'
    });

    $('.onlyNumber').mask('0#');

    $("#submit").click(function (e) {
        e.preventDefault();

        var dados = $("#locacaoEtiqueta").serializeArray();

        $.ajax({
            url: "LocacaoValidaImpressao",
            method: "POST",
            cache: false,
            data: dados,
            success: function (result) {
                if (result.Success) {
                    $("#modalImpressoras").load(HOST_URL + "BOPrinter/Selecionar?idImpressaoItem=2", function () {
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

    $("#pesquisarPontoArmazenagem").click(function () {
        $("#modalPesquisaNivelArmazenagem").empty();
        $("#modalPesquisaPontoArmazenagem").empty();
        $("#modalPesquisaEnderecoArmazenagem").empty();

        let id = $("#IdNivelArmazenagem").val();

        $("#modalPesquisaPontoArmazenagem").load("/PontoArmazenagem/PesquisaModal/" + id, function () {
            $("#modalPesquisaPontoArmazenagem").modal();
        });
    });

    $("#limparPontoArmazenagem").click(function () {
        $("#modalPesquisaPontoArmazenagem").empty();
        $("#modalPesquisaEnderecoArmazenagem").empty();

        $("#DescricaoPontoArmazenagem").val("");
        $("#IdPontoArmazenagem").val("");
    });
})();

function imprimir(acao, id) {
    var idImpressora = $("#IdImpressora").val();

    var dados = $("#locacaoEtiqueta").serializeArray();
    dados.push({ name: "IdImpressora", value: idImpressora });

    $.ajax({
        url: "LocacaoImprimir",
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

function selecionarPontoArmazenagem(idPontoArmazenagem, descricao) {
    $("#DescricaoPontoArmazenagem").val(descricao);
    $("#IdPontoArmazenagem").val(idPontoArmazenagem);
    $("#modalPesquisaPontoArmazenagem").modal("hide");
    $("#modalPesquisaPontoArmazenagem").empty();
}
