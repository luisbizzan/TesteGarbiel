(function () {

    $("#pesquisarTransportadora").click(function () {
        $("#modalTransportadora").load(HOST_URL + "Transportadora/SearchModal", function () {
            $("#modalTransportadora").modal();
        });
    });


    $("#limparTransportadora").click(function () {
        limparTransportadora();
    });

    $("#pesquisarEnderecoArmazenagem").click(function () {
        $("#modalPesquisaNivelArmazenagem").empty();
        $("#modalPesquisaPontoArmazenagem").empty();
        $("#modalPesquisaEnderecoArmazenagem").empty();

        let id = $("#IdPontoArmazenagem").val();
        let buscarTodos = true;

        $("#modalPesquisaEnderecoArmazenagem").load(HOST_URL + "EnderecoArmazenagem/PesquisaModal" + "?id=" + id + "&buscarTodos=" + buscarTodos, function () {
            $("#modalPesquisaEnderecoArmazenagem").modal();
        });
    });

    $("#limparEnderecoArmazenagem").click(function () {
        $("#modalPesquisaEnderecoArmazenagem").empty();

        $("#CodigoEnderecoArmazenagem").val("");
        $("#IdEnderecoArmazenagem").val("");
    });

    $("#pesquisarNivelArmazenagem").click(function () {
        $("#modalPesquisaNivelArmazenagem").empty();
        $("#modalPesquisaPontoArmazenagem").empty();
        $("#modalPesquisaEnderecoArmazenagem").empty();

        $("#modalPesquisaNivelArmazenagem").load(HOST_URL + "NivelArmazenagem/PesquisaModal", function () {
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
        $("#modalPesquisaPontoArmazenagem").load(HOST_URL + "PontoArmazenagem/PesquisaModal/" + id, function () {
            $("#modalPesquisaPontoArmazenagem").modal();
        });
    });

    $("#limparPontoArmazenagem").click(function () {
        $("#modalPesquisaPontoArmazenagem").empty();
        $("#modalPesquisaEnderecoArmazenagem").empty();

        $("#DescricaoPontoArmazenagem").val("");
        $("#IdPontoArmazenagem").val("");
    });

})();


function limparTransportadora() {
    let razaoSocial = $("#RazaoSocialTransportadora");
    let cliente = $("#IdTransportadora");
    razaoSocial.val("");
    cliente.val("");
}

function setTransportadora(idTransportadora, nomeFantasia) {
    $("#RazaoSocialTransportadora").val(nomeFantasia);
    $("#IdTransportadora").val(idTransportadora);
    $("#modalTransportadora").modal("hide");
    $("#modalTransportadora").empty();
}

function selecionarEnderecoArmazenagem(IdEnderecoArmazenagem, codigo) {
    $("#CodigoEnderecoArmazenagem").val(codigo);
    $("#IdEnderecoArmazenagem").val(IdEnderecoArmazenagem);
    $("#modalPesquisaEnderecoArmazenagem").modal("hide");
    $("#modalPesquisaEnderecoArmazenagem").empty();
}

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

