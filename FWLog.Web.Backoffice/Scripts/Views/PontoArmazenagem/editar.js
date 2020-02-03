(function () {
    $.validator.setDefaults({ ignore: [] });

    $("#pesquisarNivelArmazenagem").click(function () {
        $("#modalPesquisaNivelArmazenagem").load("/NivelArmazenagem/PesquisaModal", function () {
            $("#modalPesquisaNivelArmazenagem").modal();
        });
    });

    $("#limparNivelArmazenagem").click(function () {
        $("#DescricaoNivelArmazenagem").val("");
        $("#IdNivelArmazenagem").val("");
        $("#IdNivelArmazenagem").valid();
    });

    $('#LimitePesoVertical').mask('#000,00', { reverse: true });

})();

function selecionarNivelArmazenagem(idNivelArmazenagem, descricao) {
    $("#DescricaoNivelArmazenagem").val(descricao);
    $("#IdNivelArmazenagem").val(idNivelArmazenagem);
    $("#IdNivelArmazenagem").valid();
    $("#modalPesquisaNivelArmazenagem").modal("hide");
    $("#modalPesquisaNivelArmazenagem").empty();
}