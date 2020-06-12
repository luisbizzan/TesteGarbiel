(function () {
    dart.dataTables.loadFormFilterEvents();

    $("#pesquisarProduto").click(function () {
        $("#modalProduto").load(HOST_URL + "Produto/SearchModal", function () {
            $("#modalProduto").modal();
        });
    });

    function limparProduto() {
        $("#Filtros_DescricaoProduto").val("");
        $("#Filtros_IdProduto").val("");
    }

    $("#limparProduto").click(function () {
        limparProduto();
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

    $("#downloadRelatorio").click(function () {
        $.ajax({
            url: `/${CONTROLLER_PATH}/DownloadRelatorioResumoEtiquetagem`,
            method: "POST",
            cache: false,
            xhrFields: {
                responseType: 'blob'
            },
            data: {
                IdProduto: $("#Filter_IdProduto").val(),
                DataInicial: $("#Filter_DataInicial").val(),
                DataFinal: $("#Filter_DataFinal").val(),
                QuantidadeInicial: $("#Filter_QuantidadeInicial").val(),
                QuantidadeFinal: $("#Filter_QuantidadeFinal").val(),
                IdUsuarioEtiquetagem: $("#Filter_IdUsuarioEtiquetagem").val()
            },
            success: function (data) {
                var a = document.createElement('a');
                var url = window.URL.createObjectURL(data);

                a.href = url;
                a.download = 'Relatório Resumo Etiquetagem.pdf';
                document.body.append(a);
                a.click();
                a.remove();
                window.URL.revokeObjectURL(url);
            }
        });
    });

    var $DataInicial = $('#Filtros_DataInicial').closest('.date');
    var $DataFinal = $('#Filtros_DataFinal').closest('.date');

    var createLinkedPickers = function () {
        var dataInicial = $DataInicial.datetimepicker({
            locale: moment.locale(),
            format: 'L',
            allowInputToggle: true
        });

        var dataFinal = $DataFinal.datetimepicker({
            locale: moment.locale(),
            useCurrent: false,
            format: 'L',
            allowInputToggle: true
        });

        new dart.DateTimePickerLink(dataInicial, dataFinal, { ignoreTime: true });
    };

    createLinkedPickers();

    $('#dataTable').DataTable({
        ajax: {
            "url": view.pageDataUrl,
            "type": "POST",
            "data": function (data) {
                dart.dataTables.saveFilterToData(data);
            },
            "error": function (data) {
                if (!!(data.statusText)) {
                    PNotify.error({ text: data.statusText });
                    NProgress.done();
                }
            }
        },
        deferLoading: 0,
        initComplete: function (settings, json) {
            dart.dataTables.addEventsForDropdownAutoposition($('#dataTable'));
        },
        columns: [
            { data: 'IdLote' },
            { data: 'ReferenciaProduto' },
            { data: 'DescricaoProduto' },
            { data: 'Saldo' },
            { data: 'DataValidade' }
        ]
    });

    $('#dataTable').dataTable.error = function (settings, helpPage, message) {
        console.log(message)
    };
})();

function setProduto(idProduto, descricao) {
    $("#Filtros_DescricaoProduto").val(descricao);
    $("#Filtros_IdProduto").val(idProduto);
    $("#modalProduto").modal("hide");
    $("#modalProduto").empty();
}

function setLote(idLote) {
    $("#Fitros_IdLote").val(idLote);
    $("#modalLote").modal("hide");
    $("#modalLote").empty();
}

function limparLote() {
    $("#Filtros_IdLote").val("");
}