(function () {
    var actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                action: 'details',
                href: view.urlDetalhes + '/' + full.IdCaixa,
                visible: view.destalhesVisivel
            },
            {
                action: 'edit',
                href: view.urlEditar + '/' + full.IdCaixa,
                visible: view.editarVisivel
            },
            {
                action: 'delete',
                attrs: { 'data-delete-url': view.urlExcluir + '/' + full.IdCaixa },
                visible: view.excluirVisivel
            }
        ];
    });

    var options = {
        ajax: {
            "url": view.urlDadosLista,
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
        order: [[0, "ASC"]],
        columns: [
            { data: 'NomeCaixa' },
            actionsColumn
        ]
    };

    $('#dataTable').DataTable(options);
    dart.dataTables.loadFormFilterEvents();

    $('#dataTable').on('click', '[data-delete-url]', function () {
        var table = $(this).closest('table');

        dart.modalAjaxDelete.open({
            title: 'Excluir Caixa Recusa',
            message: 'Essa opção removerá a caixa e todos os produtos. Deseja continuar?',
            deleteUrl: $(this).attr('data-delete-url'),
            onConfirm: function () {
                table.DataTable().ajax.reload(null, false);
            }
        });
    });

    $("#pesquisarProduto").on('click', function () {
        if (!$(this).attr('disabled')) {
            $("#modalProduto").load(HOST_URL + "Produto/SearchModal", function () {
                $("#modalProduto").modal();
            });
        }
    });

    function limparProduto() {
        $("#Filtros_IdProduto").val("");
        $("#Filtros_DescricaoProduto").val("");
    }

    $("#limparProduto").click(function () {
        if (!$(this).attr('disabled')) {
            limparProduto();
        }
    });

    $("#pesquisarCaixa").on('click', function () {
        if (!$(this).attr('disabled')) {
            $("#modalCaixa").load(HOST_URL + "Caixa/SearchModal", function () {
                $("#modalCaixa").modal();
            });
        }
    });

    function limparCaixa() {
        $("#Filtros_IdCaixa").val("");
        $("#Filtros_DescricaoCaixa").val("");
    }

    $("#limparCaixa").click(function () {
        if (!$(this).attr('disabled')) {
            limparCaixa();
        }
    });
})();

function setProduto(idProduto, descricao) {
    $("#Filtros_IdProduto").val(idProduto);
    $("#Filtros_DescricaoProduto").val(descricao);
    $("#modalProduto").modal("hide");
    $("#modalProduto").empty();
}

function setCaixa(idCaixa, descricao) {
    $("#Filtros_IdCaixa").val(idCaixa);
    $("#Filtros_DescricaoCaixa").val(descricao);
    $("#modalCaixa").modal("hide");
    $("#modalCaixa").empty();
}