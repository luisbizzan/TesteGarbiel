(function () {
    $('.onlyNumber').mask('0#');

    let actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                action: 'select',
                visible: true,
                attrs: { 'data-select': full.IdPedidoVenda, 'numero-select': full.NumeroPedidoVenda }
            }
        ];
    });

    $('#dataTablePedidoVendaModal').DataTable({
        ajax: {
            "url": view_modal.pageDataUrl,
            "type": "POST",
            "data": function (data) {
                dart.dataTables.saveFilterToData(data, $("#form-datatable-modal"));
            }
        },
        lengthChange: false,
        initComplete: function (settings, json) {
            dart.dataTables.addEventsForDropdownAutoposition($('#dataTablePedidoVendaModal'));
        },
        columns: [
            { data: 'NumeroPedido' },
            { data: 'NumeroPedidoVenda' },
            { data: 'ClienteNome' },
            { data: 'TransportadoraNome' },
            actionsColumn
        ],
        "bInfo": false
    });

    $('#dataTablePedidoVendaModal').on('click', '[data-select]', function () {
        selecionarPedidoVenda($(this).attr('data-select'), $(this).attr("numero-select"));
    });

    dart.dataTables.loadFormFilterEvents($("#form-datatable-modal"));
})();