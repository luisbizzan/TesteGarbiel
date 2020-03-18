(function () {

    var actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                text: "Detalhes",
                attrs: { 'data-id': full.IdNotaFiscal, 'action': 'detailsUrl' },
                icon: 'fa fa-eye',
                visible: view.detailsVisible
            },
            {
                text: "Editar",
                attrs: { 'data-id': full.IdMotivoLaudo, 'action': 'editarMotivoLaudo' },
                icon: 'fa fa-edit',
                visible: view.editVisible
            },
        ];
    });

    $("#dataTable").on('click', "[action='editarMotivoLaudo']", editarMotivoLaudo);

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
        stateSaveParams: function (settings, data) {
            dart.dataTables.saveFilterToData(data);
        },
        stateLoadParams: function (settings, data) {
            dart.dataTables.loadFilterFromData(data);
        },
        order: [[1, "desc"]],
        columns: [
            { data: 'Referencia', },
            { data: 'Descricao', },
            { data: 'Peso', },
            { data: 'Largura', },
            { data: 'Altura', },
            { data: 'Comprimento', },
            { data: 'Unidade', },
            { data: 'Multiplo', },
            { data: 'Endereco', },
            { data: 'Status', },
            actionsColumn
        ]
    });

    $("#pesquisarEnderecoArmazenagem").click(function () {
        $("#modalEnderecoArmazenagem").load(HOST_URL + "EnderecoArmazenagem/PesquisaModal", function () {
            $("#modalEnderecoArmazenagem").modal();
        });
    });
   
    dart.dataTables.loadFormFilterEvents();

})();

function detalhesEntradaConferencia() {
}

function editarMotivoLaudo() {
}
