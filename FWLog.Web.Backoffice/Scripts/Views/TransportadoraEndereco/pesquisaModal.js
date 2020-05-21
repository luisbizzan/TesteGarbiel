(function () {

    //var validator = $("#form-datatable-modal").validate({
    //    ignore: ".ignore"
    //});

    $("#Filtros_CodigoEnderecoArmazenagem").mask("#0.S.#0.#0");

    let actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                action: 'select',
                visible: true,
                attrs: { 'data-select': full.IdTransportadoraEndereco, 'endereco-select': full.Codigo }
            }
        ];
    });

    $('#dataTableTransportadoraEnderecoModal').DataTable({
        ajax: {
            "url": view_modal.pageDataUrl,
            "type": "POST",
            "data": function (data) {
                dart.dataTables.saveFilterToData(data, $("#form-datatable-modal"));
            }
        },
        lengthChange: false,
        initComplete: function (settings, json) {
            dart.dataTables.addEventsForDropdownAutoposition($('#dataTableTransportadoraEnderecoModal'));
        },
        columns: [
            { "defaultContent": "", width: '40%' },
            { data: 'Codigo' },
            actionsColumn
        ],
        order: [[1, 'asc']],
        rowGroup: {
            dataSrc: 'DadosTransportadora'
        },
        "bInfo": false
    });

    $('#dataTableTransportadoraEnderecoModal').on('click', '[data-select]', function () {
        selecionarTransportadoraEndereco($(this).attr('data-select'), $(this).attr("endereco-select"));
    });

    dart.dataTables.loadFormFilterEvents($("#form-datatable-modal"));
})();