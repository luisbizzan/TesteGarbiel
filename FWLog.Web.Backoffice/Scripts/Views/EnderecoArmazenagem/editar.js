(function () {
    $.validator.setDefaults({ ignore: [] });

    $("#Codigo").mask("#0.S.#0.#0", { reverse: false });

    $("#pesquisarNivelArmazenagem").click(function () {
        $("#modalPesquisaNivelArmazenagem").load("/NivelArmazenagem/PesquisaModal", function () {
            $("#modalPesquisaNivelArmazenagem").modal();
        });
    });

    $("#limparNivelArmazenagem").click(function () {
        $("#DescricaoNivelArmazenagem").val("");
        $("#IdNivelArmazenagem").val("");
        $("#IdNivelArmazenagem").valid();

        if ($("#IdPontoArmazenagem").val() !== "") {
            $("#DescricaoPontoArmazenagem").val("");
            $("#IdPontoArmazenagem").val("");
            $("#IdPontoArmazenagem").valid();
        }
    });

    $("#pesquisarPontoArmazenagem").click(function () {
        if ($("#IdNivelArmazenagem").val() === "") {
            $("#IdNivelArmazenagem").valid();
            return;
        }

        $("#modalPesquisaPontoArmazenagem").load("/PontoArmazenagem/PesquisaModal/" + $("#IdNivelArmazenagem").val(), function () {
            $("#modalPesquisaPontoArmazenagem").modal();
        });
    });

    $("#limparPontoArmazenagem").click(function () {
        $("#DescricaoPontoArmazenagem").val("");
        $("#IdPontoArmazenagem").val("");
        $("#IdPontoArmazenagem").valid();
    });
})();

function selecionarNivelArmazenagem(idNivelArmazenagem, descricao) {
    $("#DescricaoNivelArmazenagem").val(descricao);
    $("#IdNivelArmazenagem").val(idNivelArmazenagem);
    $("#IdNivelArmazenagem").valid();
    $("#modalPesquisaNivelArmazenagem").modal("hide");
    $("#modalPesquisaNivelArmazenagem").empty();
    $("#DescricaoPontoArmazenagem").val("");
    $("#IdPontoArmazenagem").val("");
}

function selecionarPontoArmazenagem(idPontoArmazenagem, descricao) {
    $("#DescricaoPontoArmazenagem").val(descricao);
    $("#IdPontoArmazenagem").val(idPontoArmazenagem);
    $("#IdPontoArmazenagem").valid();
    $("#modalPesquisaPontoArmazenagem").modal("hide");
    $("#modalPesquisaPontoArmazenagem").empty();
}