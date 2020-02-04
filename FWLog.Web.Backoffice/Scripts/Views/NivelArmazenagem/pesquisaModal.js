(function () {
    let actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                action: 'select',
                visible: true,
                attrs: { 'data-select': full.IdNivelArmazenagem, 'name-select': full.Descricao }
            }
        ];
    });

    $('.dataTableModal').DataTable({
        destroy: true,
        ajax: {
            "url": view_modal.pageDataUrl,
            "type": "POST",
            "data": function (data) {
                dart.dataTables.saveFilterToData(data, $("#form-datatable-modal"));
            }
        },
        lengthChange: false,
        initComplete: function (settings, json) {
            dart.dataTables.addEventsForDropdownAutoposition($('.dataTableModal'));

        },
        columns: [
            { data: 'IdNivelArmazenagem' },
            { data: 'Descricao' },
            { data: 'Status' },
            actionsColumn
        ],
        "bInfo": false
    });

    function selecionarNivelArmazenagem_Click() {
        selecionarNivelArmazenagem($(this).attr('data-select'), $(this).attr("name-select"), view_modal.origem);
    }

    $('.dataTableModal').off('click', '[data-select]', selecionarNivelArmazenagem_Click);
    $('.dataTableModal').on('click', '[data-select]', selecionarNivelArmazenagem_Click);

    dart.dataTables.loadFormFilterEvents($("#form-datatable-modal"));

    var validator = $("#form-datatable-modal").validate({
        ignore: ".ignore"
    });
})();