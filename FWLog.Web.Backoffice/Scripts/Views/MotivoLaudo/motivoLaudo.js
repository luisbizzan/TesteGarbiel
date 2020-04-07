(function () {
    var actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                attrs: { 'data-id': full.IdMotivoLaudo, 'action': 'editarMotivoLaudo' },
                icon: 'fa fa-edit',
                visible: view.editVisible
            },
            {
                attrs: { 'data-id': full.IdMotivoLaudo, 'action': 'teste' },
                icon: 'fa fa-edit',
                visible: view.editVisible
            },
        ];
    });

    $("#btAdicionar").on('click', adicionarMotivoLaudo);

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
        order: [[1, "desc"]],
        columns: [
            { data: 'IdMotivoLaudo', width: 60 },
            { data: 'Descricao' },
            { data: 'Status', width: 100 },
            actionsColumn
        ]
    });

    $("#dataTable").on('click', "[action='editarMotivoLaudo']", editarMotivoLaudo);

    $("#dataTable").on('click', "[action='teste']", teste);

    $('#modalEditarMotivoLaudo').on('hidden.bs.modal', function () {
        $("#modalEditarMotivoLaudo").text('');
        $("#dataTable").DataTable().ajax.reload();
    })

    dart.dataTables.loadFormFilterEvents();
})();

function editarMotivoLaudo() {
    let id = $(this).data("id");

    $("#modalEditarMotivoLaudo").load(HOST_URL + CONTROLLER_PATH + "Selecionar/" + id, function () {
        $("#modalEditarMotivoLaudo").modal();
        $('input').iCheck({ checkboxClass: 'icheckbox_flat-green' });
        var $confirmarEdicao = $("#confirmarEdicao");
        $confirmarEdicao.on('click', function () {
            gravarMotivoLaudo();
        });
    });
}

function adicionarMotivoLaudo() {
    $("#modalEditarMotivoLaudo").load(HOST_URL + CONTROLLER_PATH + "Selecionar/" + 0, function () {
        $("#modalEditarMotivoLaudo").modal();
        $('input').iCheck({ checkboxClass: 'icheckbox_flat-green' });
        var $confirmarEdicao = $("#confirmarEdicao");
        $confirmarEdicao.on('click', function () {
            gravarMotivoLaudo();
        });
    });
}

function gravarMotivoLaudo() {
    var descricao = $("#DescricaoEdicao").val();
    var id = $("#IdMotivoLaudo").val();
    var ativo = $('#Ativo').is(':checked');

    $.ajax({
        url: HOST_URL + CONTROLLER_PATH + "Gravar",
        method: "POST",
        data: {
            descricao: descricao,
            ativo: ativo,
            idMotivoLaudo: id
        },
        success: function (result) {
            if (result.Success) {
                $("#modalEditarMotivoLaudo").modal('hide');
                PNotify.success({ text: result.Message });
            } else {
                PNotify.error({ text: result.Message });
            }
        },
        error: function (request, status, error) {
            PNotify.warning({ text: result.Message });
        }
    });
}

function teste() {
    let Id_Categoria = 1;
    let Id_Ref = $(this).data("id");

    $("#modalPadrao").load("/Geral/ListarHistoricos", {
        Id_Categoria: Id_Categoria,
        Id_Ref: Id_Ref,
    }, function () {
        $("#modalPadrao").modal();
    });
}