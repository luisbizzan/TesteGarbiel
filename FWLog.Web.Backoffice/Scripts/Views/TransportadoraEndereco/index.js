(function () {

    let actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                action: 'details',
                href: view.urlDetalhes + '/' + full.IdTransportadoraEndereco,
                visible: view.destalhesVisivel
            },
            {
                action: 'edit',
                href: view.urlEditar + '/' + full.IdTransportadoraEndereco,
                visible: view.editarVisivel
            },
            {
                action: 'delete',
                attrs: { 'data-delete-url': view.urlExcluir + '/' + full.IdTransportadoraEndereco },
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

    $('#dataTable').on('click', '[data-delete-url]', function () {
        var table = $(this).closest('table');

        dart.modalAjaxDelete.open({
            title: 'Excluir Corredor x Impressora',
            message: 'Somente Transportadora x Endereço não utilizados podem ser excluídos. Deseja continuar?',
            deleteUrl: $(this).attr('data-delete-url'),
            onConfirm: function () {
                table.DataTable().ajax.reload(null, false);
            }
        });
    });

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
    let razaoSocial = $("#Filtros_RazaoSocialTransportadora");
    let cliente = $("#Filtros_IdTransportadora");
    razaoSocial.val("");
    cliente.val("");
}

function setTransportadora(idTransportadora, nomeFantasia) {
    $("#Filtros_RazaoSocialTransportadora").val(nomeFantasia);
    $("#Filtros_IdTransportadora").val(idTransportadora);
    $("#modalTransportadora").modal("hide");
    $("#modalTransportadora").empty();
}

function selecionarEnderecoArmazenagem(IdEnderecoArmazenagem, codigo) {
    $("#Filtros_CodigoEnderecoArmazenagem").val(codigo);
    $("#Filtros_IdEnderecoArmazenagem").val(IdEnderecoArmazenagem);
    $("#modalPesquisaEnderecoArmazenagem").modal("hide");
    $("#modalPesquisaEnderecoArmazenagem").empty();
}


