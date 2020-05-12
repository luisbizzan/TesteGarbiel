(function () {
    $.validator.setDefaults({ ignore: [] });

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

    var idEnderecoArmazenagemAntigo = $("#IdEnderecoArmazenagem").val();

    $('#form-editar-produto').submit(function (e) {

        e.preventDefault();

        var idEnderecoArmazenagemNovo = $("#IdEnderecoArmazenagem").val();

        if ((idEnderecoArmazenagemAntigo && idEnderecoArmazenagemAntigo != idEnderecoArmazenagemNovo) && $(this).valid()) {

            dart.modalAjaxConfirm.open({
                title: 'Confirmação de edição',
                message: "Para mudança de endereço de produto todas as etiquetas devem ser impressas novamente. Deseja realmente continuar?",
                onConfirm: confirmaEdicao
            });

        } else {
            confirmaEdicao();
        }
    });
})();

function confirmaEdicao() {

    let form = document.getElementById('form-editar-produto');

    $.ajax({
        url: form.action,
        type: "POST",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: new FormData(form),
        processData: false,
        contentType: false,
        success: function (result) {

            if (result.Success) {
                window.location = HOST_URL + "Produto"
            } else {
                new PNotify({
                    title: 'Erro',
                    text: result.Message,
                    type: 'error'
                });
            }
        }
    });

    return false;
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

function selecionarEnderecoArmazenagem(IdEnderecoArmazenagem, codigo) {
    $("#CodigoEnderecoArmazenagem").val(codigo);
    $("#IdEnderecoArmazenagem").val(IdEnderecoArmazenagem);
    $("#modalPesquisaEnderecoArmazenagem").modal("hide");
    $("#modalPesquisaEnderecoArmazenagem").empty();
}