(function () {
    $("#pesquisarEnderecoArmazenagem").click(function () {
        let id = $("#IdPontoArmazenagem").val();
        $("#modalPesquisaEnderecoArmazenagem").load(HOST_URL + "EnderecoArmazenagem/PesquisaModal/" + id, function () {
            $("#modalPesquisaEnderecoArmazenagem").modal();
        });
    });

    $("#limparEnderecoArmazenagem").click(function () {
        $("#CodigoEnderecoArmazenagem").val("");
        $("#IdEnderecoArmazenagem").val("");
    });

    $("#pesquisarNivelArmazenagem").click(function () {
        $("#modalPesquisaNivelArmazenagem").load(HOST_URL + "NivelArmazenagem/PesquisaModal", function () {
            $("#modalPesquisaNivelArmazenagem").modal();
        });
    });

    $("#limparNivelArmazenagem").click(function () {
        $("#DescricaoNivelArmazenagem").val("");
        $("#IdNivelArmazenagem").val("");
    });

    $("#pesquisarPontoArmazenagem").click(function () {
        let id = $("#IdNivelArmazenagem").val();
        $("#modalPesquisaPontoArmazenagem").load(HOST_URL + "PontoArmazenagem/PesquisaModal/" + id, function () {
            $("#modalPesquisaPontoArmazenagem").modal();
        });
    });

    $("#limparPontoArmazenagem").click(function () {
        $("#DescricaoPontoArmazenagem").val("");
        $("#IdPontoArmazenagem").val("");
    });

})();

function selecionarNivelArmazenagem(idNivelArmazenagem, descricao) {
    $("#DescricaoNivelArmazenagem").val(descricao);
    $("#IdNivelArmazenagem").val(idNivelArmazenagem);
    $("#modalPesquisaNivelArmazenagem").modal("hide");
    $("#modalPesquisaNivelArmazenagem").empty();
}

function selecionarPontoArmazenagem(idPontoArmazenagem, descricao) {
    $("#DescricaoPontoArmazenagem").val(descricao);
    $("#IdPontoArmazenagem").val(idPontoArmazenagem);
    $("#modalPesquisaPontoArmazenagem").modal("hide");
    $("#modalPesquisaPontoArmazenagem").empty();
}

function selecionarEnderecoArmazenagem(IdEnderecoArmazenagem, codigo) {
    $("#CodigoEnderecoArmazenagem").val(codigo);
    $("#IdEnderecoArmazenagem").val(IdEnderecoArmazenagem);
    $("#modalPesquisaEnderecoArmazenagem").modal("hide");
    $("#modalPesquisaEnderecoArmazenagem").empty();
}

