(function () {
    $('.onlyNumber').mask('0#');

    let actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                action: 'select',
                visible: true,
                attrs: { 'data-select': full.IdPedidoVendaVolume, 'name-select': full.NroVolume }
            }
        ];
    });


    $('#dataTableVolumeModal').DataTable({
        stateSave: false,
        ajax: {
            "url": view_modal.pageDataUrl,
            "type": "POST",
            "data": function (data) {
                dart.dataTables.saveFilterToData(data, $("#form-datatable-modal"));
            }
        },
        lengthChange: false,
        initComplete: function (settings, json) {
            dart.dataTables.addEventsForDropdownAutoposition($('#dataTableVolumeModal'));

        },
        columns: [
            { data: 'NroPedido' },
            { data: 'NroVolume' },
            { data: 'DescricaoStatus' },
            actionsColumn
        ],
        "bInfo": false
    });

    $('#dataTableVolumeModal').on('click', '[data-select]', function () {
        setVolume($(this).attr('data-select'), $(this).attr("name-select"));
    });

    dart.dataTables.loadFormFilterEvents($("#form-datatable-modal"));
    
})();