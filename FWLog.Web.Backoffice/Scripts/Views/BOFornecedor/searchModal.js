(function () {
    var options = {
        onKeyPress: function (cpf, ev, el, op) {
            var masks = ['000.000.000-000', '00.000.000/0000-00'];
            $('#Filter_CNPJ').mask((cpf.length > 14) ? masks[1] : masks[0], op);
        }
    }

    $('#Filter_CNPJ').length > 11 ? $('#Filter_CNPJ').mask('00.000.000/0000-00', options) : $('#Filter_CNPJ').mask('000.000.000-00#', options);

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
            { data: 'IdFornecedor' },
            { data: 'RazaoSocial' },
            { data: 'NomeFantasia' },
            { data: 'CNPJ' },
            actionsColumn
        ],
        "bInfo": false
    });

    $('#dataTableModal').on('click', '[data-select]', function () {
        setFornecedor($(this).attr('data-select'), $(this).attr("name-select"), view_modal.origem);
    });

    dart.dataTables.loadFormFilterEvents($("#form-datatable-modal"));

    var validator = $("#form-datatable-modal").validate({
        ignore: ".ignore"
    });
})();