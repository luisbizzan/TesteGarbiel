(function () {
    $("#btnImprimir").click(function () {
        var acao = $("#Acao").val();
        var id = $("#Id").val();

        imprimir(acao, id);
    });
})();