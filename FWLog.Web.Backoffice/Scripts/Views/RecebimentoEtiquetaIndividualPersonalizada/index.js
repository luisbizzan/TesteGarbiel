(function () {
    $('.onlyNumber').mask('0#');

    $("#pesquisarProduto").click(function () {
        $("#modalProduto").load(HOST_URL + "Produto/SearchModal", function () {
            $("#modalProduto").modal();
        });
    });

    function limparProduto() {
        $("#IdProduto").val("");
        $("#DescricaoProduto").val("");
    }

    $("#limparProduto").click(function () {
        limparProduto();
    });

    $("#submit").click(function (e) {
        e.preventDefault();

        var dados = $("#recebimentoEtiquetaIndividualPersonalizada").serializeArray();

        $.ajax({
            url: CONTROLLER_PATH + "ValidaImpressao",
            method: "POST",
            cache: false,
            data: dados,
            success: function (result) {
                if (result.Success) {
                    $("#modalImpressoras").load("BOPrinter/Selecionar?tipo=zebra", function () {
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

function setProduto(idProduto, descricao) {
    $("#IdProduto").val(idProduto);
    $("#DescricaoProduto").val(descricao);
    $("#modalProduto").modal("hide");
    $("#modalProduto").empty();
}

function imprimir(acao, id) {
    var idImpressora = $("input[name='IdImpressora']:checked").val();

    var dados = $("#recebimentoEtiquetaIndividualPersonalizada").serializeArray();
    dados.push({ name: "IdImpressora", value: idImpressora });

    $.ajax({
        url: CONTROLLER_PATH + "Imprimir",
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