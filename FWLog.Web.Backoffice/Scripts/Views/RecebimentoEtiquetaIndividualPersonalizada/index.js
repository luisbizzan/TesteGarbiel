(function () {
    $('input[type=radio]').iCheck({
        checkboxClass: 'icheckbox_flat-green',
        radioClass: 'iradio_flat-green'
    });

    $('.onlyNumber').mask('0#');

    $('input[type=radio]').on('ifChanged', function () {
        if (this.value == 4) {
            limparProduto();
            $("#limparProduto").attr("disabled", true);
            $("#pesquisarProduto").attr("disabled", true);

        }
        else {
            $("#limparProduto").attr("disabled", false);
            $("#pesquisarProduto").attr("disabled", false);
        }
    });

    $("#pesquisarProduto").on('click', function () {
        if (!$(this).attr('disabled')) {
            $("#modalProduto").load(HOST_URL + "Produto/SearchModal", function () {
                $("#modalProduto").modal();
            });
        }
    });

    function limparProduto() {
        $("#IdProduto").val("");
        $("#DescricaoProduto").val("");
    }

    $("#limparProduto").click(function () {
        if (!$(this).attr('disabled')) {
            limparProduto();
        }
    });

    $("#submit").click(function (e) {
        e.preventDefault();

        var dados = $("#recebimentoEtiquetaIndividualPersonalizada").serializeArray();

        $.ajax({
            url: HOST_URL + CONTROLLER_PATH + "ValidaImpressao",
            method: "POST",
            cache: false,
            data: dados,
            success: function (result) {
                if (result.Success) {
                    var tipo = $('input[type=radio]:checked').val();
                    var idImpressaoItem;

                    switch (parseInt(tipo)) {
                        case 4:
                            idImpressaoItem = 5;
                            break;
                        case 1:
                            idImpressaoItem = 3;
                            break;
                        case 2:
                            idImpressaoItem = 1; // TODO: definir o tipo correto de impressão
                            break;

                        default:
                    }

                    $("#modalImpressoras").load(HOST_URL + "BOPrinter/Selecionar?idImpressaoItem=" + idImpressaoItem, function () {
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
    var idImpressora = $("#IdImpressora").val();

    var dados = $("#recebimentoEtiquetaIndividualPersonalizada").serializeArray();
    dados.push({ name: "IdImpressora", value: idImpressora });

    $.ajax({
        url: HOST_URL + CONTROLLER_PATH + "Imprimir",
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