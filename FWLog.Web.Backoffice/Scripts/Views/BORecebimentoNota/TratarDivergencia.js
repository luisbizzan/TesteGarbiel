(function () {
    $("#tratarDivergenciasRecebimento").click(function () {
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
                    PNotify.info({ text: result.Message });
                    $(".close").click();
                    $("#dataTable").DataTable().ajax.reload();
                }
            }
        });
    });
})();