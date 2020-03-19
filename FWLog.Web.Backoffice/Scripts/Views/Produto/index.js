(function () {

    var actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            //{
            //    text: "Detalhes",
            //    icon: 'fa fa-eye',
            //    action: 'details',
            //    href: view.detalhesProdutoUrl + '/' + full.IdProduto,
            //    visible: view.detalhesVisivel
            //},
            {
                text: "Inserir/Editar",
                action: 'edit',
                href: view.editarProdutoUrl + '/' + full.IdProduto,
                visible: view.edicaoEInsercaoVisivel
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
        //initComplete: function (settings, json) {
        //    dart.dataTables.addEventsForDropdownAutoposition($('#dataTable'));
        //},
        //stateSaveParams: function (settings, data) {
        //    dart.dataTables.saveFilterToData(data);
        //},
        //stateLoadParams: function (settings, data) {
        //    dart.dataTables.loadFilterFromData(data);
        //},
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
