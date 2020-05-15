(function () {
    $("#btnImprimir").click(function () {
        var acao = $("#Acao").val();
        var id = $("#Id").val();
        var id2 = $("#Id2").val();
        var id3 = $("#Id3").val();

        imprimir(acao, id, id2, id3);
    });
})();