(function () {
    $('.onlyNumber').mask('0#');

    $("#adicionarLinhaTabela").click(function () {
        var idProduto = $("#IdProduto").val();
        var referencia = $("#ReferenciaProduto").val();
        var descricaoProduto = $("#DescricaoProduto").val();
        var nroVolume = $("#NroVolume").val();
        var idPedidoVendaVolumeOrigem = $("#IdPedidoVendaVolumeOrigem").val();
        var qtdOrigem = parseInt($("#QuantidadeOriginal").val());
        var qtd = parseInt($("#Quantidade").val());

        if (!idProduto || !idPedidoVendaVolumeOrigem || !qtd) {
            PNotify.warning({ text: "Preencha os campos volume, produto e quantidade." });
            return;
        }

        if (qtd <= 0) {
            PNotify.warning({ text: "Preencha o campo quantidade." });
            return;
        }

        if (qtd > qtdOrigem) {
            PNotify.warning({ text: "A quantidade não pode ser maior que a quantidade original" });
            return;
        }

        if (validarItemExisteTabela(idPedidoVendaVolumeOrigem, idProduto)) {
            PNotify.warning({ text: "O volume e produto já foram adicionados na tabela." });
            return;
        }

        if (!ConsultarEValidarDadosProduto(idPedidoVendaVolumeOrigem, idProduto, qtd)) {
            return;
        }

        var markup =
            "<tr>" +
            "<td><input type='checkbox' name='record'></td>" +
            "<td class='hide'>" + idPedidoVendaVolumeOrigem + "</td>" +
            "<td>" + nroVolume + "</td>" +
            "<td>" + referencia + "</td>" +
            "<td class='hide'>" + idProduto + "</td>" +
            "<td>" + descricaoProduto + "</td>" +
            "<td>" + qtdOrigem + "</td>" +
            "<td>" + qtd + "</td>" +
            "</tr>";

        $("table tbody").append(markup);
        limparProduto();
        limpaQuantidade();
    });

    $("#selecionarPedido").click(function () {
        var nroPedido = $("#NroPedido").val();

        dart.modalAjaxConfirm.open({
            title: 'Pedido',
            message: "Deseja realmente selecionar o Pedido: " + nroPedido + "? Após a confirmação não será possivel trocar o pedido do cadastro.",
            onConfirm: ConsultarPedidoVenda,

        });
    });

    $("#removerLinhaTabela").click(function () {
        $("table tbody").find('input[name="record"]').each(function () {
            if ($(this).is(":checked")) {
                $(this).parents("tr").remove();
            }
        });
    });

    $("#salvarVolumes").click(function () {
        salvarVoloumes();
    });

    $("#pesquisarProduto").on('click', function () {
        if ($("#IdPedidoVendaVolumeOrigem").val() > 0) {
            if (!$(this).attr('disabled')) {
                $("#modalProduto").load(HOST_URL + "Produto/SearchModal/?idPedidoVendaVolume=" + $("#IdPedidoVendaVolumeOrigem").val(), function () {
                    $("#modalProduto").modal();
                });
            }
        }
        else {
            PNotify.warning({ text: "Selecione o volume para abrir o filtro de Produtos." });
        }
    });

    $("#limparProduto").click(function () {
        if (!$(this).attr('disabled')) {
            limparProduto();
        }
    });

    $("#pesquisarVolume").on('click', function () {
        if ($("#PedidoSelecionado").val() === "true" && $("#NroPedido").val() > 0) {
            if (!$(this).attr('disabled')) {
                $("#modalVolume").load(HOST_URL + "PedidoVendaVolume/SearchModal/" + $("#NroPedido").val(), function () {
                    $("#modalVolume").modal();
                });
            }
        }
        else {
            PNotify.warning({ text: "Selecione o número do pedido para abrir o filtro de Volumes." });
        }
    });

    $("#limparVolume").click(function () {
        if (!$(this).attr('disabled')) {
            limparVolume();
        }
    });
})();

function validarItemExisteTabela(idPedidoVendaVolume, idProduto) {
    var existe = false;

    $('table tbody tr').each(function (i, el) {
        var _idPedidoVendaVolume = $(el).children().eq(1).text();
        var _idProduto = $(el).children().eq(3).text();
        if (_idPedidoVendaVolume === idPedidoVendaVolume && _idProduto === idProduto) {
            existe = true;
        }
    });

    return existe;
}

function salvarVoloumes() {
    var resposta = {};
    resposta.ProdutosVolumes = new Array();

    resposta.NroPedido = $("#NroPedido").val();
    resposta.IdPedidoVendaVolume = $("#IdPedidoVendaVolume").val();
    resposta.IdGrupoCorredorArmazenagem = $("#IdGrupoCorredorArmazenagem").val();

    $("table tbody tr").each(function () {
        var row = $(this);
        var produto = {};
        produto.IdPedidoVendaVolumeOrigem = row.find("td").eq(1).html();
        produto.IdProduto = row.find("td").eq(4).html();
        produto.Quantidade = row.find("td").eq(7).html();
        resposta.ProdutosVolumes.push(produto);
    });

    $.ajax({
        url: HOST_URL + CONTROLLER_PATH + "GerenciarVolumes",
        method: "POST",
        data: JSON.stringify(resposta),
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            if (result.Success) {
                PNotify.success({ text: result.Message });
                setTimeout(redirecionar, 2000);
            }
            else {
                PNotify.error({ text: result.Message });
            }
        },
        error: function (data) {
            PNotify.error({ text: "Ocorreu um erro para salvar as alterações dos volumes." });
            NProgress.done();
        }
    });
}

function redirecionar() {
    window.location.href = HOST_URL + CONTROLLER_PATH + "RelatorioPedidos"
}

function setProduto(idProduto, descricao, referencia) {
    $("#IdProduto").val(idProduto);
    $("#DescricaoProduto").val(descricao);
    $("#ReferenciaProduto").val(referencia);
    $("#modalProduto").modal("hide");
    $("#modalProduto").empty();
    limpaQuantidade();
    ConsultarVolumeProdutoQtd($("#IdPedidoVendaVolumeOrigem").val(), idProduto);
}

function limpaQuantidade() {
    $("#QuantidadeOriginal").val("");
    $("#Quantidade").val("");
}

function setVolume(idPedidoVendaVolume, nroVolume) {
    $("#IdPedidoVendaVolumeOrigem").val(idPedidoVendaVolume);
    $("#NroVolume").val(nroVolume);
    $("#modalVolume").modal("hide");
    $("#modalVolume").empty();
    limpaQuantidade();
    limparProduto();
}

function limparProduto() {
    $("#IdProduto").val("");
    $("#DescricaoProduto").val("");
}

function limparVolume() {
    $("#NroVolume").val("");
    $("#IdPedidoVendaVolumeOrigem").val("");
}

function ConsultarEValidarDadosProduto(idPedidoVendaVolume, idProduto, qtd) {
    var retorno = false;
    $.ajax({
        url: HOST_URL + "PedidoVendaVolume/ConsultarDadosProduto",
        method: "POST",
        async: false,
        data: {
            idPedidoVendaVolume: idPedidoVendaVolume,
            idProduto: idProduto
        },
        success: function (result) {
            if (result.Success) {
                var model = JSON.parse(result.Data);

                if (qtd < model.MultiploVenda || (qtd % model.MultiploVenda) !== 0) {
                    retorno = false;
                    PNotify.error({ text: "Quantidade digitada fora do múltiplo de venda do produto. Múltiplo de venda: " + model.MultiploVenda });
                    return;
                }

                var grupo = $("input[name=IdGrupoCorredorArmazenagem]:hidden").val();
                if (grupo !== "0" && grupo !== model.IdGrupoCorredorArmazenagem.toString()) {
                    retorno = false;
                    PNotify.error({ text: "O Produto selecionado não pertence ao intervalo de Corredor: " + $("input[name=CorredorInicio]:hidden").val() + " até " + $("input[name=CorredorFim]:hidden").val() });
                    return;
                }
                else if (grupo === undefined || grupo === null || grupo === "0") {
                    $("input[name=IdGrupoCorredorArmazenagem]:hidden").val(model.IdGrupoCorredorArmazenagem);
                    $("input[name=CorredorInicio]:hidden").val(model.CorredorInicio);
                    $("input[name=CorredorFim]:hidden").val(model.CorredorFim);
                }

                retorno = true;
            }
            else {
                retorno = false;
                PNotify.error({ text: result.Message });
                return;
            }
        },
        error: function (data) {
            retorno = false;
            PNotify.error({ text: "Ocorreu um erro na consulta de dados do volume." });
            NProgress.done();
        }
    });

    return retorno;
}

function ConsultarVolumeProdutoQtd(idPedidoVendaVolume, idProduto) {
    $.ajax({
        url: HOST_URL + "PedidoVendaVolume/ConsultarVolumeProdutoQtd",
        method: "POST",
        async: false,
        data: {
            idPedidoVendaVolume: idPedidoVendaVolume,
            idProduto: idProduto
        },
        success: function (result) {
            if (result.Success) {
                $("#QuantidadeOriginal").val(result.Data)
            }
            else {
                PNotify.error({ text: result.Message });
            }
        },
        error: function (data) {
            PNotify.error({ text: "Ocorreu um erro na consulta de quantidade do volume." });
            NProgress.done();
        }
    });
}

function ConsultarPedidoVenda() {
    var nroPedido = $("#NroPedido").val();

    $.ajax({
        url: HOST_URL + "PedidoVenda/ValidarPedidoVenda/" + nroPedido,
        method: "POST",
        async: false,        
        success: function (result) {
            if (result.Success) {
                $("#selecionarPedido").prop('disabled', true);
                $("#NroPedido").prop('disabled', true);
                $("#PedidoSelecionado").val(true);
            }
            else {
                PNotify.error({ text: result.Message });
                $("#PedidoSelecionado").val(false);
            }
        },
        error: function (data) {
            PNotify.error({ text: "Ocorreu um erro na consulta de quantidade do volume." });
            NProgress.done();
        }
    });
}