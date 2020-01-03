(function () {
    $("#btnVoltarConferencia").click(function () {
        $("#modalConferencia").load(HOST_URL + CONTROLLER_PATH + "EntradaConferencia/" + $("#IdNotaFiscal").val(), function (result) {
            $("#modalConferencia").modal();
        });
    });

    $("#btnFinalizarConferencia").click(function () {
        $.ajax({
            url: HOST_URL + CONTROLLER_PATH + "FinalizarConferencia/" + $("#IdLote").val(),
            cache: false,
            method: "POST",
            data: { },
            success: function (result) {
                if (result.Success) {
                    $(".close").click();
                    $("#dataTable").DataTable().ajax.reload();
                    PNotify.info({ text: result.Message });
                } else {
                    PNotify.info({ text: result.Message });
                }
            }
        });
    });
})();