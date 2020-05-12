(function () {
    $("#btnConfirmar").click(function () {
        var idEnderecoArmazenagem = $("#IdEnderecoArmazenagem").val();
        var idProduto = $("#IdProduto").val();

        var $modal = $("#confirmarImpressao");

        $modal.modal("hide");
        $modal.empty();

        $("#modalImpressoras").load(HOST_URL + "BOPrinter/Selecionar?idImpressaoItem=11&acao=etiquetaPicking&id=" + idEnderecoArmazenagem + "&id2=" + idProduto, function () {
            $("#modalImpressoras").modal();
        });
    });
})();