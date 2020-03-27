(function () {
    var actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                attrs: { 'data-id': full.IdLote, 'data-id-produto' : full.IdProduto, 'action': 'movimentacoes' },
                visible: view.loteMovimentacaoVisivel,
                icon: 'fa fa-list',
                text: "Movimentações",
            }
        ];
    });

    var options = {
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
        order: [[1, "ASC"]],
        columns: [
            { data: 'ReferenciaProduto' },
            { data: 'DescricaoProduto' },
            { data: 'QuantidadeRecebida' },
            { data: 'Saldo' },
            actionsColumn
        ]
    };

    $("#dataTable").on('click', "[action='movimentacoes']", movimentacoes);

    $("#pesquisarProduto").on('click', function () {
        if (!$(this).attr('disabled')) {
            $("#modalProduto").load(HOST_URL + "Produto/SearchModal", function () {
                $("#modalProduto").modal();
            });
        }
    });

    function limparProduto() {
        $("#Filter_IdProduto").val("");
        $("#Filter_DescricaoProduto").val("");
    }

    $("#limparProduto").click(function () {
        if (!$(this).attr('disabled')) {
            limparProduto();
        }
    });

    //Necessário pois o idLote não pode ser limpado.
    function limparFiltros() {
        var $form = $('[data-filter="form"]');
        var $dataTable = $('#' + $form.attr('data-filter-for'));

        $form[0].reset();
        $form.find('input#Filter_DescricaoProduto').val('');
        $form.find('input#Filter_IdProduto').val('');

        // Necessário dar trigger no evento 'change' dos inputs para que force a atualização,
        // de componentes de data e outros se necessário.
        $form.find('input').trigger('change');

        if (!$form.valid())
            return;

        $dataTable.DataTable().search('').draw();
        NProgress.start();
    }

    $("#limparFiltros").click(function () {
        limparFiltros();
    });

    $('#dataTable').dataTable(options);
    dart.dataTables.loadFormFilterEvents();
})();

function setProduto(idProduto, descricao) {
    $("#Filter_IdProduto").val(idProduto);
    $("#Filter_DescricaoProduto").val(descricao);
    $("#modalProduto").modal("hide");
    $("#modalProduto").empty();
}

function movimentacoes() {
    var id = $(this).data("id");
    var idProduto = $(this).data("id-produto");

    $(location).attr('href', view.urlLoteMovimentacao + '?idLote=' + id + '&idProduto=' + idProduto);
}