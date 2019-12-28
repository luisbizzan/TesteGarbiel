(function () {
    $("#voltar").click(function () {
        $("#modalConferencia").load(HOST_URL + CONTROLLER_PATH + "EntradaConferencia/" + $("#IdNotaFiscal").val(), function (result) {
            $("#modalConferencia").modal();
        });
    });
})();