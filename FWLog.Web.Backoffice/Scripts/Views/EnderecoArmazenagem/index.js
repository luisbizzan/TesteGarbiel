(function () {    
    var actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                action: 'details',
                href: view.urlDetalhes + '/' + full.IdEnderecoArmazenagem,
                visible: view.destalhesVisivel
            },
            {
                action: 'edit',
                href: view.urlEditar + '/' + full.IdEnderecoArmazenagem,
                visible: view.editarVisivel
            },
            {
                action: 'delete',
                attrs: { 'data-delete-url': view.urlExcluir + '/' + full.IdEnderecoArmazenagem },
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
        order: [[2, "DESC"]],
        columns: [
            { data: 'NivelArmazenagem' },
            { data: 'PontoArmazenagem' },
            { data: 'Codigo' },
            { data: 'Fifo' },
            { data: 'PontoSeparacao' },
            { data: 'EstoqueMinimo' },
            { data: 'Status' },
            actionsColumn
        ]
    };

    $('#dataTable').DataTable(options);
    dart.dataTables.loadFormFilterEvents();

    $('#dataTable').on('click', '[data-delete-url]', function () {
        var table = $(this).closest('table');

        dart.modalAjaxDelete.open({
            title: 'Excluir Endereco de Armazenagem',
            message: 'Somente Enderecos de Armazenagem não utilizados podem ser excluídos. Deseja continuar?',
            deleteUrl: $(this).attr('data-delete-url'),
            onConfirm: function () {
                table.DataTable().ajax.reload(null, false);
            }
        });
    });

    $("#pesquisarNivelArmazenagem").click(function () {
        $("#modalPesquisaNivelArmazenagem").load("NivelArmazenagem/PesquisaModal", function () {
            $("#modalPesquisaNivelArmazenagem").modal();
        });
    });

    $("#limparNivelArmazenagem").click(function () {
        $("#Filtros_DescricaoNivelArmazenagem").val("");
        $("#Filtros_IdNivelArmazenagem").val("");
    });

    $("#pesquisarPontoArmazenagem").click(function () {
        $("#modalPesquisaPontoArmazenagem").load("PontoArmazenagem/PesquisaModal", function () {
            $("#modalPesquisaPontoArmazenagem").modal();
        });
    });

    $("#limparPontoArmazenagem").click(function () {
        $("#Filtros_DescricaoPontoArmazenagem").val("");
        $("#Filtros_IdPontoArmazenagem").val("");
    });
})();

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