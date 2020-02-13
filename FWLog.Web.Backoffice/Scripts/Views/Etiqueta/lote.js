(function () {
    $('.onlyNumber').mask('0#');

    $("#submit").click(function (e) {
        e.preventDefault();

        var dados = $("#recebimentoEtiqueta").serializeArray();

        $.ajax({
            url: "LoteValidaImpressao",
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

    $("#modalProduto").on("hidden.bs.modal", function () {
        $("#modalProduto").text('');
    });

    $("#modalLote").on("hidden.bs.modal", function () {
        $("#modalLote").text('');
    });

    $("#pesquisarProduto").on('click', function () {
        if ($("#NroLote").val() > 0) {
            if (!$(this).attr('disabled')) {
                $("#modalProduto").load(HOST_URL + "Produto/SearchModal/" + $("#NroLote").val(), function () {
                    $("#modalProduto").modal();
                });
            }
        }
        else {
            PNotify.warning({ text: "Selecione o Lote para abrir o filtro de Produtos." });
        }
    });

    $("#pesquisarLote").on('click', function () {
        if (!$(this).attr('disabled')) {
            $("#modalLote").load(HOST_URL + "BORecebimentoNota/PesquisaLote", function () {
                $("#modalLote").modal();
            });
        }
    });

    $("#limparProduto").click(function () {
        if (!$(this).attr('disabled')) {
            limparProduto();
        }
    });

    $("#limparLote").click(function () {
        if (!$(this).attr('disabled')) {
            limparLote();
        }
    });
})();

function imprimir(acao, id) {
    var idImpressora = $("#IdImpressora").val();

    var dados = $("#recebimentoEtiqueta").serializeArray();
    dados.push({ name: "IdImpressora", value: idImpressora });

    $.ajax({
        url: "LoteImprimir",
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

function setProduto(idProduto, descricao) {
    $("#IdProduto").val(idProduto);
    $("#DescricaoProduto").val(descricao);
    $("#modalProduto").modal("hide");
    $("#modalProduto").empty();
}

function setLote(idLote) {
    $("#NroLote").val(idLote);
    $("#modalLote").modal("hide");
    $("#modalLote").empty();
}

function limparLote() {
    $("#NroLote").val("");
}

function limparProduto() {
    $("#IdProduto").val("");
    $("#DescricaoProduto").val("");
}