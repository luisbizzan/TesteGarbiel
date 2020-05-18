(function () {
    $("#btnConfirmar").click(function () {
        var id = $("#IdEnderecoArmazenagem").val();

        var tipoImpressao = $("#TipoImpressao:checked").val();

        if (!tipoImpressao || tipoImpressao == "") {
            PNotify.error({ text: "Tipo de impressão é obrigatório" });
        } else {
            var $modal = $("#confirmarImpressao");

            $modal.modal("hide");
            $modal.empty();

            $("#modalImpressoras").load(HOST_URL + "BOPrinter/Selecionar?idImpressaoItem=6&acao=" + id + "&id=" + tipoImpressao, function () {
                $("#modalImpressoras").modal();
            });
        }
    });
})();