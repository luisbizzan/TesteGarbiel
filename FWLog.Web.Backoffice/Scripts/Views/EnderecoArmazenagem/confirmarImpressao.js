(function () {
    $("#btnConfirmar").click(function () {
        var id = $("#IdEnderecoArmazenagem").val();

        var $modal = $("#confirmarImpressao");

        $modal.modal("hide");
        $modal.empty();

        $("#modalImpressoras").load(HOST_URL + "BOPrinter/Selecionar?idImpressaoItem=6&acao=" + id, function () {
            $("#modalImpressoras").modal();
        });
    });
})();