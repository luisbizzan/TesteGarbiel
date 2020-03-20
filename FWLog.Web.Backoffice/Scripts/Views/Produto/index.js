﻿(function () {

    var actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                text: "Detalhes",
                attrs: { 'data-id': full.IdProduto, 'action': 'detailsUrl' },
                icon: 'fa fa-eye',
                visible: view.detalhesVisivel
            },
            {
                text: "Inserir/Editar",
                action: 'edit',
                href: view.editarProdutoUrl + '/' + full.IdProduto,
                visible: view.edicaoEInsercaoVisivel
            },
        ];
    });

    $("#dataTable").on('click', "[action='detailsUrl']", detalhesEntradaConferencia);

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
   
    dart.dataTables.loadFormFilterEvents();

})();

function detalhesEntradaConferencia() {
    var id = $(this).data("id");
    let modalDetalhesProduto = $("#modalDetalhesProduto");

    modalDetalhesProduto.load(CONTROLLER_PATH + "DetalhesProduto/" + id, function () {
        modalDetalhesProduto.modal();
    });
}
