(function () {
    $(".onlyNumber").mask("0#");

    let $observacao = $('#ObservacaoDivergencia');
    let $linhasDivergencias = $(".linha-tratarDivergencia");

    $observacao.blur(function () {
        if (!!$(this).val()) {
            $(this).css("border-color", "");
        }
    });

    $(".quantidade-mais").blur(function () {
        if ($(this).val() !== "") {
            $(this).css("border-color", "");
            $(this).closest('tr').css("background-color", "");
            $(this).closest('tr').find(".quantidade-menos").css("border-color", "");
            $(this).closest('tr').find(".quantidade-menos").val("");
        }
    });

    $(".quantidade-menos").blur(function () {
        var quantidadeMenos = $(this).val();
        var quantidadeNota = $(this).closest('tr').find(".quantidade-nota").val();

        if (quantidadeMenos !== "") {
            if (quantidadeNota - quantidadeMenos < 0) {
                PNotify.error({ text: "Quantidade a MENOS não pode exceder o total da nota." });
            }

            $(this).css("border-color", "");
            $(this).closest('tr').css("background-color", "");
            $(this).closest('tr').find(".quantidade-mais").css("border-color", "");
            $(this).closest('tr').find(".quantidade-mais").val("");
        }
    });

    $("#tratarDivergenciasRecebimento").click(function () {
        try {
            var _model = { isValid: true, Message: '' };

            if (!$observacao.val()) {
                $observacao.css("border-color", "#a94442");

                throw new divergenciaValidation('Campo Obervação é obrigatório.');
            }

            $linhasDivergencias.each(function () {
                var $quantidadeNota      = $('.quantidade-nota', this);
                var $quantidadeMais      = $('.quantidade-mais', this);
                var $quantidadeMenos     = $('.quantidade-menos', this);
                var $quantidadeDevolucao = $('.quantidade-devolucao', this);

                if (!$quantidadeMais.val() && !$quantidadeMenos.val()) {
                    _model = { isValid: false, Message: 'Existem divergências não tratadas.' };

                    $(this).css("background-color", "#ffeded");

                    $quantidadeMais.css("border-color"     , "#a94442");
                    $quantidadeMenos.css("border-color"    , "#a94442");
                    $quantidadeDevolucao.css("border-color", "#a94442");

                    return;
                }

                if ($quantidadeNota.val() - $quantidadeMenos.val() < 0) {
                    _model = { isValid: false, Message: 'Quantidade a MENOS não pode exceder o total da nota.' };

                    $(this).css("background-color", "#ffeded");

                    $quantidadeMenos.css("border-color", "#a94442");
                }
            });
         
            if (!_model.isValid) {
                throw new divergenciaValidation(_model.Message);
            }

            $.ajax({
                url: HOST_URL + "BORecebimentoNota/TratarDivergencia",
                cache: false,
                method: "POST",
                data: $("#tratarDivergencias").serialize(),
                success: function (result) {
                    if (!result.Success) {
                        PNotify.error({ text: result.Message });
                        $("#dataTable").DataTable().ajax.reload();
                        $("#modalDivergencia").modal("hide");
                    } else {
                        PNotify.success({ text: result.Message });
                        VerificarStatusLote($("#IdNotaFiscal").val());
                    }
                }
            });
        } catch (e) {
            if (!!e.texto) {
                PNotify.error({ text: e.texto });
            } else {
                throw e;
            }
        }
    });
})();

function divergenciaValidation(message) {
    this.texto = message;
}

function VerificarStatusLote(id) {
    $.ajax({
        url: HOST_URL + "BORecebimentoNota/ContinuarProcessamentoLote/" + id,
        cache: false,
        method: "POST",
        success: function (result) {
            if (!result.Success) {
                PNotify.error({ text: result.Message });
            } else {
                $(".close").click();
                $("#dataTable").DataTable().ajax.reload();

                if (result.Data !== "True") {
                    return;
                }

                PNotify.info({ text: "Continuando processo de finalização da tratativa de divergência..." });
                let $modal = $("#modalProcessamentoTratativaDivergencia");
                $modal.load(HOST_URL + CONTROLLER_PATH + "ResumoProcessamentoDivergencia/" + id, function () {
                    $modal.modal();
                    $('input').iCheck({ checkboxClass: 'icheckbox_flat-green' });
                    FinalizarTratativa();
                });
            }
        }
    });
}