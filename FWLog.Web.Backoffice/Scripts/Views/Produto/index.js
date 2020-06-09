(function () {

    var actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                text: "Detalhes",
                attrs: { 'data-id': full.IdProduto, 'action': 'detailsUrl' },
                icon: 'fa fa-eye',
                visible: view.detalhesVisivel
            },
            {
                text: "Inserir/Editar",
                action: 'edit',
                href: view.editarProdutoUrl + '/' + full.IdProduto,
                visible: view.edicaoEInsercaoVisivel
            },
            {
                action: 'imprimir',
                icon: 'fa fa-print',
                text: "Imprimir Etiqueta Picking",
                attrs: { 'data-id': full.IdEnderecoArmazenagem, 'data-id-produto': full.IdProduto, 'action': 'imprimir' },
                visible: true
            },
        ];
    });

    $("#dataTable").on('click', "[action='detailsUrl']", detalhesEntradaConferencia);
    $("#dataTable").on('click', "[action='imprimir']", imprimirEtiquetaPicking);
    dart.dataTables.loadFormFilterEvents();

    $('#dataTable').DataTable({
        ajax: {
            "url": view.pageDataUrl,
            "type": "POST",
            "data": function (data) {
                dart.dataTables.saveFilterToData(data);
            }
        },
        initComplete: function (settings, json) {
            dart.dataTables.addEventsForDropdownAutoposition($('#dataTable'));
        },
        stateSaveParams: function (settings, data) {
            dart.dataTables.saveFilterToData(data);
        },
        stateLoadParams: function (settings, data) {
            dart.dataTables.loadFilterFromData(data);
        },
        order: [[1, "desc"]],
        columns: [
            { data: 'Referencia', },
            { data: 'Descricao', },
            { data: 'Peso', },
            { data: 'Saldo', },
            { data: 'Largura', },
            { data: 'Altura', },
            { data: 'Comprimento', },
            { data: 'Unidade', },
            { data: 'Multiplo', },
            { data: 'Endereco', },
            { data: 'Status', },
            actionsColumn
        ]
    });

    $("#downloadRelatorioProduto").click(function () {
        $.ajax({
            url: "/Produto/DownloadRelatorioProduto",
            method: "POST",
            data: {
                Referencia: $("#Filtros_Referencia").val(),
                Descricao: $("#Filtros_Descricao").val(),
                CodigoDeBarras: $("#Filtros_CodigoDeBarras").val(),
                ProdutoStatusId: $("#Filtros_ProdutoStatus").val(),
                ProdutoStatus: $("#Filtros_ProdutoStatus option:selected").text(),
                LocacaoSaldoId: $("#Filtros_LocacaoSaldo").val(),
                LocacaoSaldo: $("#Filtros_LocacaoSaldo option:selected").text(),
                IdPontoArmazenagem: $("#Filtros_IdPontoArmazenagem").val(),
                IdNivelArmazenagem: $("#Filtros_IdNivelArmazenagem").val(),
                IdEnderecoArmazenagem: $("#Filtros_IdEnderecoArmazenagem").val()
            },
            xhrFields: {
                responseType: 'blob'
            },
            success: function (data) {
                var a = document.createElement('a');
                var url = window.URL.createObjectURL(data);

                a.href = url;
                a.download = 'Relatório de Produtos.pdf';
                document.body.append(a);
                a.click();
                a.remove();
                window.URL.revokeObjectURL(url);
            }
        });
    });

    $("#imprimirRelatorioProduto").click(function () {
        $("#modalImpressoras").load("BOPrinter/Selecionar?idImpressaoItem=1&acao=produtos", function () {
            $("#modalImpressoras").modal();
        });
    });

    $("#pesquisarEnderecoArmazenagem").click(function () {
        $("#modalPesquisaNivelArmazenagem").empty();
        $("#modalPesquisaPontoArmazenagem").empty();
        $("#modalPesquisaEnderecoArmazenagem").empty();

        let id = $("#Filtros_IdPontoArmazenagem").val();
        let buscarTodos = true;

        $("#modalPesquisaEnderecoArmazenagem").load(HOST_URL + "EnderecoArmazenagem/PesquisaModal" + "?id=" + id + "&buscarTodos=" + buscarTodos, function () {
            $("#modalPesquisaEnderecoArmazenagem").modal();
        });
    });

    $("#limparEnderecoArmazenagem").click(function () {
        $("#modalPesquisaEnderecoArmazenagem").empty();

        $("#Filtros_CodigoEnderecoArmazenagem").val("");
        $("#Filtros_IdEnderecoArmazenagem").val("");
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

        $("#Filtros_DescricaoNivelArmazenagem").val("");
        $("#Filtros_IdNivelArmazenagem").val("");
    });

    $("#pesquisarPontoArmazenagem").click(function () {
        $("#modalPesquisaNivelArmazenagem").empty();
        $("#modalPesquisaPontoArmazenagem").empty();
        $("#modalPesquisaEnderecoArmazenagem").empty();

        let id = $("#Filtros_IdNivelArmazenagem").val();
        $("#modalPesquisaPontoArmazenagem").load(HOST_URL + "PontoArmazenagem/PesquisaModal/" + id, function () {
            $("#modalPesquisaPontoArmazenagem").modal();
        });
    });

    $("#limparPontoArmazenagem").click(function () {
        $("#modalPesquisaPontoArmazenagem").empty();
        $("#modalPesquisaEnderecoArmazenagem").empty();

        $("#Filtros_DescricaoPontoArmazenagem").val("");
        $("#Filtros_IdPontoArmazenagem").val("");
    });

    function imprimirEtiquetaPicking() {
        let idEnderecoArmazenagem = $(this).data("id");
        let idProduto = $(this).data("id-produto");

        if (!idEnderecoArmazenagem || !idProduto) {
            PNotify.warning({ text: "Para imprimir a etiqueta é necessário relcionar um endereço." });
        }
        else {
            let $modal = $("#confirmarImpressao");

            $modal.load(HOST_URL + CONTROLLER_PATH + "ConfirmarImpressao?idEnderecoArmazenagem=" + idEnderecoArmazenagem + "&idProduto=" + idProduto, function () {
                $modal.modal();
            });
        }
    }
})();

function detalhesEntradaConferencia() {
    var id = $(this).data("id");
    let modalDetalhesProduto = $("#modalDetalhesProduto");

    modalDetalhesProduto.load(CONTROLLER_PATH + "DetalhesProduto/" + id, function () {
        modalDetalhesProduto.modal();
    });
}

function imprimir(acao, id, id2, id3) {
    switch (acao) {
        case 'produtos':
            $.ajax({
                url: "/Produto/ImprimirRelatorioProdutos",
                method: "POST",
                data: {
                    IdImpressora: $("#IdImpressora").val(),
                    Referencia: $("#Filtros_Referencia").val(),
                    Descricao: $("#Filtros_Descricao").val(),
                    CodigoDeBarras: $("#Filtros_CodigoDeBarras").val(),
                    ProdutoStatusId: $("#Filtros_ProdutoStatus").val(),
                    LocacaoSaldoId: $("#Filtros_LocacaoSaldo").val(),
                    LocacaoSaldo: $("#Filtros_LocacaoSaldo option:selected").text(),
                    ProdutoStatus: $("#Filtros_ProdutoStatus option:selected").text(),
                    IdPontoArmazenagem: $("#Filtros_IdPontoArmazenagem").val(),
                    IdNivelArmazenagem: $("#Filtros_IdNivelArmazenagem").val(),
                    IdEnderecoArmazenagem: $("#Filtros_IdEnderecoArmazenagem").val()
                },
                success: function (result) {
                    mensagemImpressao(result);
                    $('#modalImpressoras').modal('toggle');
                    waitingDialog.hide();
                }
            });
            break;
        case 'detalheproduto':
            $.ajax({
                url: "/Produto/ImprimirDetalhesProduto",
                method: "POST",
                data: {
                    IdImpressora: $("#IdImpressora").val(),
                    IdProduto: id
                },
                success: function (result) {
                    mensagemImpressao(result);
                    $('#modalImpressoras').modal('toggle');
                    waitingDialog.hide();
                }
            });
            break;
        case 'etiquetaPicking':
            var idImpressora = $("#IdImpressora").val();


            $.ajax({
                url: HOST_URL + CONTROLLER_PATH + "ImprimirEtiqueta",
                method: "POST",
                cache: false,
                data: {
                    IdImpressora: idImpressora,
                    IdEnderecoArmazenagem: id,
                    IdProduto: id2,
                    TipoImpressaoEtiqueta: id3
                },
                success: function (result) {
                    if (result.Success) {
                        PNotify.success({ text: result.Message });

                        fechaModal();
                    } else {
                        PNotify.error({ text: result.Message });
                    }
                },
                error: function (data) {
                    PNotify.error({ text: "Ocorreu um erro na impressão." });
                    NProgress.done();
                }
            });
            break;
    }
}

function mensagemImpressao(result) {
    if (result.Success) {
        PNotify.success({ text: result.Message });
    } else {
        PNotify.error({ text: result.Message });
    }
}

function selecionarNivelArmazenagem(idNivelArmazenagem, descricao) {
    $("#Filtros_DescricaoNivelArmazenagem").val(descricao);
    $("#Filtros_IdNivelArmazenagem").val(idNivelArmazenagem);
    $("#modalPesquisaNivelArmazenagem").modal("hide");
    $("#modalPesquisaNivelArmazenagem").empty();
}

function selecionarPontoArmazenagem(idPontoArmazenagem, descricao) {
    $("#Filtros_DescricaoPontoArmazenagem").val(descricao);
    $("#Filtros_IdPontoArmazenagem").val(idPontoArmazenagem);
    $("#modalPesquisaPontoArmazenagem").modal("hide");
    $("#modalPesquisaPontoArmazenagem").empty();
}

function selecionarEnderecoArmazenagem(IdEnderecoArmazenagem, codigo) {
    $("#Filtros_CodigoEnderecoArmazenagem").val(codigo);
    $("#Filtros_IdEnderecoArmazenagem").val(IdEnderecoArmazenagem);
    $("#modalPesquisaEnderecoArmazenagem").modal("hide");
    $("#modalPesquisaEnderecoArmazenagem").empty();
}

function fechaModal() {
    var $modal = $("#modalImpressoras");

    $modal.modal("hide");
    $modal.empty();
}