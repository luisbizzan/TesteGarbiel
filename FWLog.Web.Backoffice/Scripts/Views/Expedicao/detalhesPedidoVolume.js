(function () {
    $("#voltar").click(function () {
        $(".close").click();

        $("#modalDetalhes").empty();

        var link = $(this).attr("href");

        $("#modalDetalhes").load(link, function () {

            $("#modalDetalhes").modal();
        });
    });
})();