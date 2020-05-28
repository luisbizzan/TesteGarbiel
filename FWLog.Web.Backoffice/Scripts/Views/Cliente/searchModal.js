(function () {
    $('.onlyNumber').mask('0#');

    var options = {
        onKeyPress: function (cpf, ev, el, op) {
            var masks = ['000.000.000-000', '00.000.000/0000-00'];
            $('.CNPJCPF').mask((cpf.length > 14) ? masks[1] : masks[0], op);
        }
    }

    $('.CNPJCPF').length > 11 ? $('.CNPJCPF').mask('00.000.000/0000-00', options) : $('.CNPJCPF').mask('000.000.000-00#', options);

    let actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
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
            { data: 'CNPJCPF' },            
            actionsColumn
        ],
        "bInfo": false
    });

    $('#dataTableClienteModal').on('click', '[data-select]', function () {
        setCliente($(this).attr('data-select'), $(this).attr("name-select"));
    });

    dart.dataTables.loadFormFilterEvents($("#form-datatable-modal"));
    
})();