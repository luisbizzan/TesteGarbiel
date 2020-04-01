(function () {

    var actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                attrs: { 'data-id': full.IdMotivoLaudo, 'action': 'editarMotivoLaudo' },
                icon: 'fa fa-edit',
                visible: view.editVisible
            },
        ];
    });

    $('#dataTable').DataTable({
        ajax: {
            "url": view.pageDataUrl,
            "type": "POST",
            "data": function (data) {
                dart.dataTables.saveFilterToData(data);
            }
        },
        initComplete: function (settings, json) {
            dart.dataTables.addEventsForDropdownAutoposition($('#dataTable'));
        },
        //stateSaveParams: function (settings, data) {
        //    dart.dataTables.saveFilterToData(data);
        //},
        //stateLoadParams: function (settings, data) {
        //    dart.dataTables.loadFilterFromData(data);
        //},
        order: [[1, "desc"]],
        columns: [
            { data: 'IdMotivoLaudo', width: 60 },
            { data: 'Descricao' },
            { data: 'Status', width: 100 },
            actionsColumn
        ]
    });

    $("#dataTable").on('click', "[action='editarMotivoLaudo']", editarMotivoLaudo);

    $('#modalEditarMotivoLaudo').on('hidden.bs.modal', function () {
        $("#modalEditarMotivoLaudo").text('');
        $("#dataTable").DataTable().ajax.reload();
    })

    dart.dataTables.loadFormFilterEvents

})();

function editarMotivoLaudo() {
    let id = $(this).data("id");

    $("#modalEditarMotivoLaudo").load(HOST_URL + CONTROLLER_PATH + "ExibirModalDeEdicaoMotivoLaudo/" + id, function () {
        $("#modalEditarMotivoLaudo").modal();
        $('input').iCheck({ checkboxClass: 'icheckbox_flat-green' });
        var $confirmarEdicao = $("#confirmarEdicao");

        $confirmarEdicao.on('click', function () {
            var descricao = $("#DescricaoEdicao").val();
            var ativo = $('#Ativo').is(':checked');  

            $.ajax({
                url: HOST_URL + CONTROLLER_PATH + "Edit",
                method: "POST",
                data: {
                    descricao: descricao,
                    ativo: ativo,
                    idMotivoLaudo: id
                },
                success: function (result) {
                    $("#modalEditarMotivoLaudo").modal('hide');
                    PNotify.success({ text: result.Message });
                },
                error: function (request, status, error) {
                    PNotify.warning({ text: result.Message });
                }
            });
        });
    });
}
