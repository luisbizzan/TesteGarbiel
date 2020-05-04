(function () {
    $('.onlyNumber').mask('0#');
    $.validator.setDefaults({ ignore: null });

    $.validator.addMethod('validateCorredorInicial', function (value, ele) {
        var corredorFinal = $("#CorredorFinal").val();

        if (!corredorFinal || !value) {
            return true;
        }

        value = parseInt(value);
        corredorFinal = parseInt(corredorFinal);

        if (value <= corredorFinal)
            return true;
        else
            return false;
    }, 'Corredor inicial deve ser menor que o corredor final.');

    $("#pesquisarPontoArmazenagem").click(function () {
        $("#modalPesquisaPontoArmazenagem").empty();

        let id = $("#IdNivelArmazenagem").val();
        $("#modalPesquisaPontoArmazenagem").load(HOST_URL + "PontoArmazenagem/PesquisaModal/" + id, function () {
            $("#modalPesquisaPontoArmazenagem").modal();
        });
    });

    $("#limparPontoArmazenagem").click(function () {
        $("#modalPesquisaPontoArmazenagem").empty();

        $("#DescricaoPontoArmazenagem").val("");
        $("#IdPontoArmazenagem").val("");
    });
})();

function selecionarPontoArmazenagem(idPontoArmazenagem, descricao) {
    $("#DescricaoPontoArmazenagem").val(descricao);
    $("#IdPontoArmazenagem").val(idPontoArmazenagem);
    $("#modalPesquisaPontoArmazenagem").modal("hide");
    $("#modalPesquisaPontoArmazenagem").empty();
}