(function () {
    $(".onlyNumber").mask("0#");

    $(".quantidade-mais").blur(function () {
        if ($(this).val() !== "") {
            $(this).css("border-color", "");
            $(this).closest('tr').css("background-color", "");
            $(this).closest('tr').find(".quantidade-menos").css("border-color", "");
            $(this).closest('tr').find(".quantidade-menos").val("");
            $(".validacao-confirmar").text("");
        }
    });

    $(".quantidade-menos").blur(function () {
        var quantidadeMenos = $(this).val();
        var quantidadeNota = $(this).closest('tr').find(".quantidade-nota").val();

        if (quantidadeMenos !== "") {
            if (quantidadeNota - quantidadeMenos < 0) {
                PNotify.error({ text: "A quantidade a MENOS não pode maior que o total da nota." });
                $(this).focus();
            }

            $(this).css("border-color", "");
            $(this).closest('tr').css("background-color", "");
            $(this).closest('tr').find(".quantidade-mais").css("border-color", "");
            $(this).closest('tr').find(".quantidade-mais").val("");
            $(".validacao-confirmar").text("");
        }
    });

    $("#tratarDivergenciasRecebimento").click(function () {
        var isVAlid = true;
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
            $(".validacao-confirmar").text("Existem divergências não tratadas.");
            return;
        }

        $.ajax({
            url: HOST_URL + "BORecebimentoNota/TratarDivergencia",
            cache: false,
            method: "POST",
            data: $("#divergencias").serialize(),
            success: function (result) {
                if (!result.Success) {
                    $(".close").click();
                    $("#dataTable").DataTable().ajax.reload();
                } else {
                    PNotify.success({ text: "Todas as divergências foram tratadas." });
                    $(".close").click();
                    $("#dataTable").DataTable().ajax.reload();
                }
            }
        });
    });
})();