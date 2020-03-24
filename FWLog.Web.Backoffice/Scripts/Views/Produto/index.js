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
        ];
    });

    $("#dataTable").on('click', "[action='detailsUrl']", detalhesEntradaConferencia);

    $('#dataTable').DataTable({
        ajax: {
            "url": view.pageDataUrl,
            "type": "POST",
            "data": function (data) {
                dart.dataTables.saveFilterToData(data);
            }
        },
        //initComplete: function (settings, json) {
        //    dart.dataTables.addEventsForDropdownAutoposition($('#dataTable'));
        //},
        //stateSaveParams: function (settings, data) {
        //    dart.dataTables.saveFilterToData(data);
        //},
        //stateLoadParams: function (settings, data) {
        //    dart.dataTables.loadFilterFromData(data);
        //},
        order: [[1, "desc"]],
        columns: [
            { data: 'Referencia', },
            { data: 'Descricao', },
            { data: 'Peso', },
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
                ProdutoStatus: $("#Filtros_ProdutoStatus").val()
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

    dart.dataTables.loadFormFilterEvents();
})();

function detalhesEntradaConferencia() {
    var id = $(this).data("id");
    let modalDetalhesProduto = $("#modalDetalhesProduto");

    modalDetalhesProduto.load(CONTROLLER_PATH + "DetalhesProduto/" + id, function () {
        modalDetalhesProduto.modal();
    });
}

function imprimir(acao, id) {
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
                    ProdutoStatus: $("#Filtros_ProdutoStatus").val()
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
    }
}

function mensagemImpressao(result) {
    if (result.Success) {
        PNotify.success({ text: result.Message });
    } else {
        PNotify.error({ text: result.Message });
    }
}
