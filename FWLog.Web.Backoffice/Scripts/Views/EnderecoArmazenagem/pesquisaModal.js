(function () {

    var validator = $("#form-datatable-modal").validate({
        ignore: ".ignore"
    });

    $("#Filtros_Codigo").mask("#0.S.#0.#0");

    let actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                action: 'select',
                visible: true,
                attrs: { 'data-select': full.IdEnderecoArmazenagem, 'name-select': full.Codigo }
            }
        ];
    });

    $('#dataTableModal').DataTable({
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
            { data: 'Codigo' },
            { data: 'LimitePeso' },
            { data: 'Fifo' },
            { data: 'EstoqueMinimo' },
            { data: 'EstoqueMaximo' },
            actionsColumn
        ],
        "bInfo": false
    });


    $('#dataTableModal').on('click', '[data-select]', function () {
        selecionarEnderecoArmazenagem($(this).attr('data-select'), $(this).attr("name-select"));
    });

    dart.dataTables.loadFormFilterEvents($("#form-datatable-modal"));
 
})();