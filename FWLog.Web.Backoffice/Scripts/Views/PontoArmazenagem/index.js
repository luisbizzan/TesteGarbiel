(function () {    
    var actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                action: 'details',
                href: view.urlDetalhes + '/' + full.IdPontoArmazenagem,
                visible: view.destalhesVisivel
            },
            {
                action: 'edit',
                href: view.urlEditar + '/' + full.IdPontoArmazenagem,
                visible: view.editarVisivel
            },
            {
                action: 'delete',
                attrs: { 'data-delete-url': view.urlExcluir + '/' + full.IdPontoArmazenagem },
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
        order: [[0, "DESC"]],
        columns: [
            { data: 'IdPontoArmazenagem' },
            { data: 'NivelArmazenagem' },
            { data: 'Descricao' },
            { data: 'TipoArmazenagem' },
            { data: 'TipoMovimentacao' },
            { data: 'LimitePesoVertical' },
            { data: 'Status' },
            actionsColumn
        ]
    };

    $('#dataTable').DataTable(options);
    dart.dataTables.loadFormFilterEvents();
})();