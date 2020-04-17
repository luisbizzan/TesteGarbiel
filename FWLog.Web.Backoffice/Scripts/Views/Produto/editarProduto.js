(function () {
    $("#pesquisarNivelArmazenagem").click(function () {
        $("#modalPesquisaNivelArmazenagem").empty();
        $("#modalPesquisaPontoArmazenagem").empty();
        $("#modalPesquisaEnderecoArmazenagem").empty();

        $("#modalPesquisaNivelArmazenagem").load("/NivelArmazenagem/PesquisaModal", function () {
            $("#modalPesquisaNivelArmazenagem").modal();
        });
    });

    $("#limparNivelArmazenagem").click(function () {
        $("#modalPesquisaNivelArmazenagem").empty();
        $("#modalPesquisaPontoArmazenagem").empty();
        $("#modalPesquisaEnderecoArmazenagem").empty();
        
        $("#DescricaoNivelArmazenagem").val("");
        $("#IdNivelArmazenagem").val("");
    });
    


    $("#pesquisarPontoArmazenagem").click(function () {
        $("#modalPesquisaNivelArmazenagem").empty();
        $("#modalPesquisaPontoArmazenagem").empty();
        $("#modalPesquisaEnderecoArmazenagem").empty();

        let id = $("#IdNivelArmazenagem").val();

        $("#modalPesquisaPontoArmazenagem").load("/PontoArmazenagem/PesquisaModal/" + id, function () {
            $("#modalPesquisaPontoArmazenagem").modal();
        });
    });

    $("#limparPontoArmazenagem").click(function () {
        $("#modalPesquisaPontoArmazenagem").empty();
        $("#modalPesquisaEnderecoArmazenagem").empty();

        $("#DescricaoPontoArmazenagem").val("");
        $("#IdPontoArmazenagem").val("");
    });

    $("#pesquisarEnderecoArmazenagem").click(function () {
        $("#modalPesquisaNivelArmazenagem").empty();
        $("#modalPesquisaPontoArmazenagem").empty();
        $("#modalPesquisaEnderecoArmazenagem").empty();

        let id = $("#IdPontoArmazenagem").val();
        $("#modalPesquisaEnderecoArmazenagem").load("/EnderecoArmazenagem/PesquisaModal/" + id, function () {
            $("#modalPesquisaEnderecoArmazenagem").modal();
        });
    });

    $("#limparEnderecoArmazenagem").click(function () {
        $("#modalPesquisaEnderecoArmazenagem").empty();
        $("#CodigoEnderecoArmazenagem").val("");
        $("#IdEnderecoArmazenagem").val("");
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

