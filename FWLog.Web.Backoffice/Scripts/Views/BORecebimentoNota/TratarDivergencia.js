(function () {
    var $observacao = $('#ObservacaoDivergencia');
    $(".onlyNumber").mask("0#");

    $observacao.blur(function () {
        if (!$(this).val()) {
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
                PNotify.error({ text: "A quantidade a MENOS não pode maior que o total da nota." });
            }

            $(this).css("border-color", "");
            $(this).closest('tr').css("background-color", "");
            $(this).closest('tr').find(".quantidade-mais").css("border-color", "");
            $(this).closest('tr').find(".quantidade-mais").val("");
        }
    });

    $("#tratarDivergenciasRecebimento").click(function () {
        var isVAlid = true;

        if (!$observacao.val()) {
            $observacao.css("border-color", "#a94442");
            PNotify.error({ text: "Obervação é obrigatório." });
            return;
        }

        $(".linha-divergencia").each(function () {
            var inputs = $(this).find("input[type=text]");

            if ($(inputs[0]).val() === "" && $(inputs[1]).val() === "") {
                isVAlid = false;
                $(this).css("background-color", "#ffeded");
                $(inputs[0]).css("border-color", "#a94442");
                $(inputs[1]).css("border-color", "#a94442");
            }
        });

        if (!isVAlid) {
            PNotify.error({ text: "Existem divergências não tratadas." });
            return;
        }

        $.ajax({
            url: HOST_URL + "BORecebimentoNota/TratarDivergencia",
            cache: false,
            method: "POST",
            data: $("#divergencias").serialize(),
            success: function (result) {
                if (!result.Success) {
                    PNotify.error({ text: result.Message });
                } else {
                    PNotify.success({ text: result.Message });
                    VerificarStatusLote($("#IdNotaFiscal").val());
                }
            }
        });
    });
})();

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