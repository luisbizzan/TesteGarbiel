(function () {
    $("#btnConfirmar").click(function () {
        var id = $("#IdPedidoVendaVolume").val();

        var $modal = $("#confirmarReimpressao");

        $modal.modal("hide");
        $modal.empty();

        $("#modalImpressoras").load(HOST_URL + "BOPrinter/Selecionar?idImpressaoItem=12&acao=ReimprimirEtiquetaVolume&id=" + id, function () {
            $("#modalImpressoras").modal();
        });
    });
})();