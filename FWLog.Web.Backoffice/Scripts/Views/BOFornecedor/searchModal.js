(function () {
    $("#Filter_CNPJ").mask("99.999.999/9999-99");

    let actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                action: 'select',
                visible: true,
                attrs: { 'data-select': full.IdFornecedor, 'name-select': full.NomeFantasia }
            }
        ];
    });

    $('#dataTableModal').DataTable({
       // destroy: true,
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
            { data: 'CodigoIntegracao' },
            { data: 'RazaoSocial' },
            { data: 'NomeFantasia' },
            { data: 'CNPJ' },
            actionsColumn
        ],
        "bInfo": false
    });

    $('#dataTableModal').on('click', '[data-select]', function () {
        setFornecedor($(this).attr('data-select'), $(this).attr("name-select"));
    });

    dart.dataTables.loadFormFilterEvents($("#form-datatable-modal"));

    var validator = $("#form-datatable-modal").validate({
        ignore: ".ignore"
    });
})();