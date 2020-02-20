(function () {

    let actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        debugger
        return [
            {
                action: 'select',
                visible: true,
                attrs: { 'data-select': full.IdCliente, 'name-select': full.RazaoSocial }
            }
        ];
    });


    $('#dataTableClienteModal').DataTable({
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
            dart.dataTables.addEventsForDropdownAutoposition($('#dataTableClienteModal'));

        },
        columns: [
            { data: 'IdCliente' },
            { data: 'RazaoSocial' },
            { data: 'NomeFantasia' },
            { data: 'CNPJCPF' },
            { data: 'Classificacao' },
            { data: 'Status' },
            actionsColumn
        ],
        "bInfo": false
    });

    $('#dataTableClienteModal').on('click', '[data-select]', function () {
        setCliente($(this).attr('data-select'), $(this).attr("name-select"));
    });

    dart.dataTables.loadFormFilterEvents($("#form-datatable-modal"));
    
})();