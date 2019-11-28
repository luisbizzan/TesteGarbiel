(function () {
    let actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                action: 'select',
                visible: true,
                attrs: { 'data-select': full.IdPontoArmazenagem, 'name-select': full.PontoArmazenagem }
            }
        ];
    });

    $('#dataTableModal').DataTable({
        ajax: {
            "url": view_modal.pageDataUrl,
            "type": "POST",
            "data": function (data) {
                dart.dataTables.saveFilterToData(data, $("#form-datatable-modal"));
            }
        },
        lengthChange: false,
        initComplete: function (settings, json) {
            dart.dataTables.addEventsForDropdownAutoposition($('#dataTableModal'));

        },
        columns: [
            { data: 'IdPontoArmazenagem' },
            { data: 'NivelArmazenagem' },
            { data: 'PontoArmazenagem' },
            { data: 'Status' },
            actionsColumn
        ],
        "bInfo": false
    });

    $('#dataTableModal').on('click', '[data-select]', function () {
        selecionarPontoArmazenagem($(this).attr('data-select'), $(this).attr("name-select"));
    });

    dart.dataTables.loadFormFilterEvents($("#form-datatable-modal"));

    var validator = $("#form-datatable-modal").validate({
        ignore: ".ignore"
    });
})();