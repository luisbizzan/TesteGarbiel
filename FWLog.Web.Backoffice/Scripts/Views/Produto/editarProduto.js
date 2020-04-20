(function () {
    //$.validator.setDefaults({ ignore: [] });

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

    //$('#formEditarProduto').submit(function () {

    //    //e.preventDefault();

    //    if ($(this).valid()) {

    //        dart.modalAjaxConfirm.open({
    //            title: 'Confirmação de edição',
    //            message: "Para mudança de endereço de produto todas as etiquetas devem ser impressas novamente. Deseja realmente continuar?",
    //            //url: HOST_URL + "Empresa/MudarEmpresa/" + id,
    //            onCancel: cancelaEdicao,
    //            onConfirm: confirmaEdicao,
    //        });
    //    }

    //    return false;
    //});

})();


//function cancelaEdicao() {
//    return false;
//}

////function confirmaEdicao() {
////    return true;
////}

//function confirmaEdicao() {

//    var form = $('#formEditarProduto');

//    alert(form.serialize());

//    $.ajax({
//        url: form.action,
//        type: "POST",
//        dataType: "json",
//        contentType: "application/json; charset=utf-8",
//        data: new FormData(form),
//        processData: false,
//        contentType: false,
//        success: function (result) {

//            alert("Cheguei");

//            return true;

//            //new PNotify({
//            //    title: (result.Success) ? 'Sucesso' : 'Erro',
//            //    text: result.Message,
//            //    type: (result.Success) ? 'success' : 'error'
//            //});

//            //if (result.Success) {
//            //    form.reset();
//            //    $(".modal").modal("hide");
//            //}
//        }
//    });

//    return false;
//}

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