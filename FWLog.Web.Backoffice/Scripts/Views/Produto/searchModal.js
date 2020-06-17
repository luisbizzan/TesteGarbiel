(function () {
    let actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                action: 'select',
                visible: true,
                attrs: { 'data-select': full.IdProduto, 'name-select': full.Descricao, 'referencia-select': full.Referencia  }
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
            { data: 'Referencia' },
            { data: 'Descricao' },
            { data: 'Status' },
            actionsColumn
        ],
        "bInfo": false
    });

    $('#dataTableModal').on('click', '[data-select]', function ()
    {
        debugger;
        if ($("#Filter_ExibirReferenciaProduto").val() === "True" || $("#ExibirReferenciaProduto").val() === true) {
            setProduto($(this).attr('data-select'), $(this).attr("name-select"), $(this).attr("referencia-select"));
        }
        else {
            setProduto($(this).attr('data-select'), $(this).attr("name-select"));
        }
    });

    dart.dataTables.loadFormFilterEvents($("#form-datatable-modal"));

    var validator = $("#form-datatable-modal").validate({
        ignore: ".ignore"
    });
})();