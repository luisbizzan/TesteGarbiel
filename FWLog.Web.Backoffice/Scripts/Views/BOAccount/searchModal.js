(function () {
    let actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                action: 'select',
                visible: true,
                attrs: { 'data-select': full.UsuarioId, 'name-select': full.Nome }
            }
        ];
    });

    debugger

    //$('#dataTableModal').DataTable().clear();

    //if (!$.fn.dataTable.isDataTable('#dataTableModal')) {
    $('.dataTableModal').DataTable({
        destroy: true,
        ajax: {
            "url": view_modal.pageDataUrl,
            "type": "POST",
            "data": function (data) {
                dart.dataTables.saveFilterToData(data, $("#form-datatable-modal"));
            }
        },
        lengthChange: false,
        initComplete: function (settings, json) {
            dart.dataTables.addEventsForDropdownAutoposition($('.dataTableModal'));

        },
        columns: [
            { data: 'UserName' },
            { data: 'Nome' },
            { data: 'Departamento' },
            { data: 'Cargo' },
            actionsColumn
        ],
        "bInfo": false
    });
    //}

    function setUsuario_Click() {
        setUsuario($(this).attr('data-select'), $(this).attr("name-select"), view_modal.origem);
    }

    $('.dataTableModal').off('click', '[data-select]', setUsuario_Click);
    $('.dataTableModal').on('click', '[data-select]', setUsuario_Click);

    dart.dataTables.loadFormFilterEvents($("#form-datatable-modal"));
})();