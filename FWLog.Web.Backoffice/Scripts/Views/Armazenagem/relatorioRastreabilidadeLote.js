(function () {
    $.validator.setDefaults({ ignore: null });
    $('.onlyNumber').mask('0#');

    $.validator.addMethod('validateLoteOrProduto', function (value, ele) {
        var produto = $("#Filter_DescricaoProduto").val();

        if (value != "")
            return true
        else if (produto != "")
            return true
        else
            return false;
    }, 'Informe o Lote ou Produto para prosseguir');

    var actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                attrs: { 'data-id': full.IdLote, 'action': 'produtosLote' },
                //href: view.urlLoteProduto + '/' + full.IdLote,
                visible: view.loteProdutoVisivel,
                icon: 'fa fa-cubes',
                text: "Produtos do Lote",
            }
        ];
    });

    dart.dataTables.loadFormFilterEvents();

    $(document.body).on('click', "#pesquisar", function () {
        $("#tabela").show();
    });

    $("#downloadRelatorio").click(function () {
        $.ajax({
            url: `/${CONTROLLER_PATH}/DownloadRelatorioRastreabilidadeLote`,
            method: "POST",
            cache: false,
            xhrFields: {
                responseType: 'blob'
            },
            data: {
                IdLote: $("#Filter_IdLote").val(),
                NroNota: $("#Filter_NroNota").val(),
                IdProduto: $("#Filter_IdProduto").val(),
                ReferenciaPronduto: $("#Filter_DescricaoProduto").val(),
                DataCompraMinima: $("#Filter_DataCompraMinima").val(),
                DataCompraMaxima: $("#Filter_DataCompraMaxima").val(),
                DataRecebimentoMinima: $("#Filter_DataRecebimentoMinima").val(),
                DataRecebimentoMaxima: $("#Filter_DataRecebimentoMaxima").val(),
                QtdCompraMinima: $("#Filter_QtdCompraMinima").val(),
                QtdCompraMaxima: $("#Filter_QtdCompraMaxima").val(),
                QtdRecebidaMinima: $("#Filter_QtdRecebidaMinima").val(),
                QtdRecebidaMaxima: $("#Filter_QtdRecebidaMaxima").val()
            },
            success: function (data) {
                var a = document.createElement('a');
                var url = window.URL.createObjectURL(data);

                a.href = url;
                a.download = 'Relatório Rastreio de Peça.pdf';
                document.body.append(a);
                a.click();
                a.remove();
                window.URL.revokeObjectURL(url);
            }
        });
    });

    $('#dataTable').DataTable({
        ajax: {
            "url": view.pageDataUrl,
            "type": "POST",
            "data": function (data) {
                dart.dataTables.saveFilterToData(data);
            },
            "error": function (data) {
                if (!!(data.statusText)) {
                    NProgress.done();
                }
            }
        },
        deferLoading: 0,
        initComplete: function (settings, json) {
            dart.dataTables.addEventsForDropdownAutoposition($('#dataTable'));
        },
        stateSaveParams: function (settings, data) {
            dart.dataTables.saveFilterToData(data);
        },
        order: [[0, "ASC"]],
        columns: [
            { data: 'IdLote' },
            { data: 'Status' },
            { data: 'DataRecebimento' },
            { data: 'DataConferencia' },
            { data: 'QuantidadeVolume' },
            { data: 'QuantidadePeca' },
            actionsColumn
        ]
    });

    $('#dataTable').dataTable.error = function (settings, helpPage, message) {
        console.log(message)
    };

    $("#dataTable").on('click', "[action='produtosLote']", produtosLote);

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

    $("#modalLote").on("hidden.bs.modal", function () {
        $("#modalLote").text('');
    });

    $("#pesquisarLote").on('click', function () {
        if (!$(this).attr('disabled')) {
            $("#modalLote").load(HOST_URL + "BORecebimentoNota/PesquisaLote", function () {
                $("#modalLote").modal();
            });
        }
    });

    $("#limparLote").click(function () {
        if (!$(this).attr('disabled')) {
            limparLote();
        }
    });


})();

function setProduto(idProduto, descricao) {
    $("#Filter_IdProduto").val(idProduto);
    $("#Filter_DescricaoProduto").val(descricao);
    $("#modalProduto").modal("hide");
    $("#modalProduto").empty();
}

function produtosLote() {
    var id = $(this).data("id");

    $(location).attr('href', view.urlLoteProduto + '?idLote=' + id);
}

function setLote(idLote) {
    $("#Filter_IdLote").val(idLote);
    $("#modalLote").modal("hide");
    $("#modalLote").empty();
}

function limparLote() {
    $("#Filter_IdLote").val("");
}