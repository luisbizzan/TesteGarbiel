(function () {
    $('.onlyNumber').mask('0#');
    $("#Filter_CNPJ").mask("99.999.999/9999-99");

    let actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                action: 'select',
                visible: true,
                attrs: { 'data-select': full.IdTransportadora, 'name-select': full.RazaoSocial }
            }
        ];
    });


    $('#dataTableTransportadoraModal').DataTable({
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
            dart.dataTables.addEventsForDropdownAutoposition($('#dataTableTransportadoraModal'));

        },
        columns: [
            { data: 'IdTransportadora'},
            { data: 'RazaoSocial' },
            { data: 'CNPJ'},
            { data: 'Status' },
            actionsColumn
        ],
        "bInfo": false
    });

    $('#dataTableTransportadoraModal').on('click', '[data-select]', function () {
        setTransportadora($(this).attr('data-select'), $(this).attr("name-select"));
    });

    dart.dataTables.loadFormFilterEvents($("#form-datatable-modal"));

})();