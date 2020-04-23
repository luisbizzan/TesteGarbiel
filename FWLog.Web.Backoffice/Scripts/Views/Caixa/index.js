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
            { data: 'Nome' },
            { data: 'TextoEtiqueta' },
            { data: 'PesoMaximo' },
            { data: 'Cubagem' },
            { data: 'Sobra' },
            { data: 'CaixaTipoDescricao' },
            { data: 'PesoCaixa' },
            { data: 'Prioridade' },
            { data: 'Status' },
            actionsColumn
        ]
    };

    $('#dataTable').DataTable(options);
    dart.dataTables.loadFormFilterEvents();

    $('#dataTable').on('click', '[data-delete-url]', function () {
        var table = $(this).closest('table');

        dart.modalAjaxDelete.open({
            title: 'Excluir Caixa',
            message: 'Somente caixas não utilizadas podem ser excluídas. Deseja continuar?',
            deleteUrl: $(this).attr('data-delete-url'),
            onConfirm: function () {
                table.DataTable().ajax.reload(null, false);
            }
        });
    });
})();