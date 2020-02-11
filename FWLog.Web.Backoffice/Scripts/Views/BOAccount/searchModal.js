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

    $('#dataTableModalUsu').DataTable({
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
            dart.dataTables.addEventsForDropdownAutoposition($('#dataTableModalUsu'));

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


      function setUsuario_Click() {
       setUsuario($(this).attr('data-select'), $(this).attr("name-select"), view_modal.origem);
       }
       
    $('#dataTableModalUsu').off('click', '[data-select]', setUsuario_Click);
    $('#dataTableModalUsu').on('click', '[data-select]', setUsuario_Click);

    dart.dataTables.loadFormFilterEvents($("#form-datatable-modal"));

    var validator = $("#form-datatable-modal").validate({
        ignore: ".ignore"
    });
})();