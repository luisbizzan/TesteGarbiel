(function () {
    $('.onlyNumber').mask('0#');
    $.validator.setDefaults({ ignore: null });

    $.validator.addMethod('validateCorredorInicial', function (value, ele) {
        var corredorFinal = $("#Filtros_CorredorFinal").val();

        if (!corredorFinal || !value) {
            return true;
        }

        value = parseInt(value);
        corredorFinal = parseInt(corredorFinal);

        if (value <= corredorFinal)
            return true;
        else
            return false;
    }, 'Corredor inicial deve ser menor que o corredor final.');

    

    var actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                action: 'details',
                href: view.urlDetalhes + '/' + full.IdGrupoCorredorArmazenagem,
                visible: view.destalhesVisivel
            },
            {
                action: 'edit',
                href: view.urlEditar + '/' + full.IdGrupoCorredorArmazenagem,
                visible: view.editarVisivel
            },
            {
                action: 'delete',
                attrs: { 'data-delete-url': view.urlExcluir + '/' + full.IdGrupoCorredorArmazenagem },
                visible: view.excluirVisivel
            }
        ];
    });

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
        columns: [
            { data: 'CorredorInicial' },
            { data: 'CorredorFinal' },
            { data: 'DescricaoPontoArmazenagem' },
            { data: 'Impressora' },
            { data: 'Status' },
            actionsColumn
        ]
    };

    $('#dataTable').DataTable(options);
    dart.dataTables.loadFormFilterEvents();

    $('#dataTable').on('click', '[data-delete-url]', function () {
        var table = $(this).closest('table');

        dart.modalAjaxDelete.open({
            title: 'Excluir Corredor x Impressora',
            message: 'Somente corredores x impressora não utilizados podem ser excluídos. Deseja continuar?',
            deleteUrl: $(this).attr('data-delete-url'),
            onConfirm: function () {
                table.DataTable().ajax.reload(null, false);
            }
        });
    });

    $("#pesquisarPontoArmazenagem").click(function () {
        $("#modalPesquisaPontoArmazenagem").empty();

        let id = $("#Filtros_IdNivelArmazenagem").val();
        $("#modalPesquisaPontoArmazenagem").load(HOST_URL + "PontoArmazenagem/PesquisaModal/" + id, function () {
            $("#modalPesquisaPontoArmazenagem").modal();
        });
    });

    $("#limparPontoArmazenagem").click(function () {
        $("#modalPesquisaPontoArmazenagem").empty();

        $("#Filtros_DescricaoPontoArmazenagem").val("");
        $("#Filtros_IdPontoArmazenagem").val("");
    });
})();

function selecionarPontoArmazenagem(idPontoArmazenagem, descricao) {
    $("#Filtros_DescricaoPontoArmazenagem").val(descricao);
    $("#Filtros_IdPontoArmazenagem").val(idPontoArmazenagem);
    $("#modalPesquisaPontoArmazenagem").modal("hide");
    $("#modalPesquisaPontoArmazenagem").empty();
}