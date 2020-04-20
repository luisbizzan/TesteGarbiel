(function () {
    var actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                action: 'imprimir',
                icon: 'fa fa-print',
                text: "Imprimir Etiqueta Endereço",
                attrs: { 'data-id': full.IdEnderecoArmazenagem, 'action': 'imprimir' },
                visible: view.imprimirVisivel
            },
            {
                action: 'details',
                href: view.urlDetalhes + '/' + full.IdEnderecoArmazenagem,
                visible: view.destalhesVisivel
            },
            {
                action: 'edit',
                href: view.urlEditar + '/' + full.IdEnderecoArmazenagem,
                visible: view.editarVisivel
            },
            {
                action: 'delete',
                attrs: { 'data-delete-url': view.urlExcluir + '/' + full.IdEnderecoArmazenagem },
                visible: view.excluirVisivel
            }
        ];
    });

    var iconeStatus = function (data, type, row) {

        if (type === 'display') {
            var nomeCor,
                codigo = row.Codigo || '',
                tooltipText;

            if (row.Ocupado === false) {
                nomeCor = 'verde';
                tooltipText = 'Endereço disponível';
            }
            else {
                nomeCor = 'vermelho',
                    codigo = row.Codigo || '',
                    tooltipText = 'Endereço ocupado';
            }

            return `<i class="fa fa-circle icone-status-${nomeCor}" title = "${tooltipText}" data-toggle = "tooltip"></i>${codigo}`;
        }

        return data;
    };

    var options = {
        ajax: {
            "url": view.urlDadosLista,
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
        order: [[2, "DESC"]],
        columns: [
            { data: 'NivelArmazenagem' },
            { data: 'PontoArmazenagem' },
            { data: 'Codigo', render: iconeStatus },
            { data: 'Fifo' },
            { data: 'PontoSeparacao' },
            { data: 'EstoqueMinimo' },
            { data: 'Quantidade' },
            { data: 'Status' },
            actionsColumn
        ]
    };

    $('#dataTable').DataTable(options);
    dart.dataTables.loadFormFilterEvents();

    $('#dataTable').on('click', '[data-delete-url]', function () {
        var table = $(this).closest('table');

        dart.modalAjaxDelete.open({
            title: 'Excluir Endereco de Armazenagem',
            message: 'Somente Enderecos de Armazenagem não utilizados podem ser excluídos. Deseja continuar?',
            deleteUrl: $(this).attr('data-delete-url'),
            onConfirm: function () {
                table.DataTable().ajax.reload(null, false);
            }
        });
    });

    $("#pesquisarNivelArmazenagem").click(function () {
        $("#modalPesquisaNivelArmazenagem").load("NivelArmazenagem/PesquisaModal", function () {
            $("#modalPesquisaNivelArmazenagem").modal();
        });
    });

    $("#limparNivelArmazenagem").click(function () {
        $("#Filtros_DescricaoNivelArmazenagem").val("");
        $("#Filtros_IdNivelArmazenagem").val("");
    });

    $("#pesquisarPontoArmazenagem").click(function () {
        $("#modalPesquisaPontoArmazenagem").load("PontoArmazenagem/PesquisaModal", function () {
            $("#modalPesquisaPontoArmazenagem").modal();
        });
    });

    $("#limparPontoArmazenagem").click(function () {
        $("#Filtros_DescricaoPontoArmazenagem").val("");
        $("#Filtros_IdPontoArmazenagem").val("");
    });

    $("#dataTable").on('click', "[action='imprimir']", imprimirEtiquetaEndereco);

    function imprimirEtiquetaEndereco() {
        let id = $(this).data("id");

        let $modal = $("#confirmarImpressao");

        $modal.load(HOST_URL + CONTROLLER_PATH + "ConfirmarImpressao?IdEnderecoArmazenagem=" + id, function () {
            $modal.modal();
        });
    }

})();

function selecionarNivelArmazenagem(idNivelArmazenagem, descricao) {
    $("#Filtros_DescricaoNivelArmazenagem").val(descricao);
    $("#Filtros_IdNivelArmazenagem").val(idNivelArmazenagem);
    $("#modalPesquisaNivelArmazenagem").modal("hide");
    $("#modalPesquisaNivelArmazenagem").empty();
}

function selecionarPontoArmazenagem(idPontoArmazenagem, descricao) {
    $("#Filtros_DescricaoPontoArmazenagem").val(descricao);
    $("#Filtros_IdPontoArmazenagem").val(idPontoArmazenagem);
    $("#modalPesquisaPontoArmazenagem").modal("hide");
    $("#modalPesquisaPontoArmazenagem").empty();
}

//Recebendo o id do endereço no parâmetro 'acao'.
function imprimir(acao, id) {
    var idImpressora = $("#IdImpressora").val();

    var dados = $("#recebimentoEtiquetaIndividualPersonalizada").serializeArray();
    dados.push({ name: "IdImpressora", value: idImpressora });

    $.ajax({
        url: HOST_URL + CONTROLLER_PATH + "ImprimirEtiqueta",
        method: "POST",
        cache: false,
        data: {
            IdImpressora: idImpressora,
            IdEnderecoArmazenagem: acao
        },
        success: function (result) {
            if (result.Success) {
                PNotify.success({ text: result.Message });

                fechaModal();
            } else {
                PNotify.error({ text: result.Message });
            }
        },
        error: function (data) {
            PNotify.error({ text: "Ocorreu um erro na impressão." });
            NProgress.done();
        }
    });
}

function fechaModal() {
    var $modal = $("#modalImpressoras");

    $modal.modal("hide");
    $modal.empty();
}