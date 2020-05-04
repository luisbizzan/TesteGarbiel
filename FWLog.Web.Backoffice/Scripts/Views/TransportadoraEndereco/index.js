(function () {

    let actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                text: "Visualizar",
                attrs: { 'data-id': full.IdTransportadoraEndereco, 'action': 'detailsUrl' },
                icon: 'fa fa-eye',
                visible: view.destalhesVisivel
            },
            {
                text: "Editar",
                attrs: { 'data-id': full.IdTransportadoraEndereco, 'action': 'registrarRecebimentoUrl' },
                icon: 'fa fa-edit',
                visible: view.editarVisivel
            },
            {
                text: "Excluir",
                attrs: { 'data-id': full.IdTransportadoraEndereco, 'action': 'registrarRecebimentoUrl' },
                icon: 'fa fa-trash-o',
                visible: view.excluirVisivel
            }
        ];
    });

    $('#dataTable').DataTable({
        ajax: {
            "url": view.pageDataUrl,
            "type": "POST",
            "data": function (data) {
                dart.dataTables.saveFilterToData(data);
            },
        },
        columns: [
            { "defaultContent": "", width: '40%' },
            { data: 'Codigo' },
            actionsColumn
        ],
        order: [[1, 'asc']],
        rowGroup: {
            dataSrc: 'DadosTransportadora'
        }
    });

    $('#dataTable').dataTable.error = function (settings, helpPage, message) {
        console.log(message)
    };

    dart.dataTables.loadFormFilterEvents();

    $("#pesquisarTransportadora").click(function () {
        $("#modalTransportadora").load(HOST_URL + "Transportadora/SearchModal", function () {
            $("#modalTransportadora").modal();
        });
    });


    $("#limparTransportadora").click(function () {
        limparTransportadora();
    });

    $("#pesquisarEnderecoArmazenagem").click(function () {
        //$("#modalPesquisaNivelArmazenagem").empty();
        //$("#modalPesquisaPontoArmazenagem").empty();
        //$("#modalPesquisaEnderecoArmazenagem").empty();

        let buscarTodos = true;

        $("#modalPesquisaEnderecoArmazenagem").load(HOST_URL + "EnderecoArmazenagem/PesquisaModal" + "?id=" + null + "&buscarTodos=" + buscarTodos, function () {
            $("#modalPesquisaEnderecoArmazenagem").modal();
        });
    });

    $("#limparEnderecoArmazenagem").click(function () {
        $("#modalPesquisaEnderecoArmazenagem").empty();

        $("#Filtros_CodigoEnderecoArmazenagem").val("");
        $("#Filtros_IdEnderecoArmazenagem").val("");
    });
  
})();


function limparTransportadora() {
    let razaoSocial = $("#Filter_RazaoSocialTransportadora");
    let cliente = $("#Filter_IdTransportadora");
    razaoSocial.val("");
    cliente.val("");
}


