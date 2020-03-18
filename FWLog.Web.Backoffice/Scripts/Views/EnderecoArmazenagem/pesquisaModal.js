(function () {
    let actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                action: 'select',
                visible: true,
                attrs: { 'data-select': full.IdEnderecoArmazenagem, 'name-select': full.Codigo }
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
            { data: 'Codigo' },
            { data: 'LimitePeso' },
            { data: 'Fifo' },
            { data: 'EstoqueMinimo' },
            { data: 'EstoqueMaximo' },
            actionsColumn
        ],
        "bInfo": false
    });

    function selecionarEnderecoArmazenagem_Click() {
        selecionarEnderecoArmazenagem($(this).attr('data-select'), $(this).attr("name-select"));
    }

    $('.dataTableModal').off('click', '[data-select]', selecionarEnderecoArmazenagem_Click);
    $('.dataTableModal').on('click', '[data-select]', selecionarEnderecoArmazenagem_Click);

    dart.dataTables.loadFormFilterEvents($("#form-datatable-modal"));
 
})();