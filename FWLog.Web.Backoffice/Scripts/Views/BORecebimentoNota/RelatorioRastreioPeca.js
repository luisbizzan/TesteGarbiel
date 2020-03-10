(function () {
    dart.dataTables.loadFormFilterEvents();

    $(document.body).on('click', "#pesquisar", function () {
        $("#tabela").show();
    });

    $.validator.addMethod('validateQtdCompraMaxima', function (value, ele) {
        var qtdMinima = $("#Filter_QtdCompraMinima").val();

        if (!value || !qtdMinima) {
            return true;
        }

        value = parseInt(value);
        qtdMinima = parseInt(qtdMinima);

        if (value >= qtdMinima)
            return true;
        else
            return false;
    }, 'Quantidade máxima deve ser maior que a quantidade mínima.');

    $.validator.addMethod('validateQtdRecebidaMaxima', function (value, ele) {
        var qtdMinima = $("#Filter_QtdRecebidaMinima").val();

        if (!value || !qtdMinima) {
            return true;
        }

        value = parseInt(value);
        qtdMinima = parseInt(qtdMinima);

        if (value >= qtdMinima)
            return true;
        else
            return false;
    }, 'Quantidade máxima deve ser maior que a quantidade mínima.');

    $.validator.addMethod('validateQtdCompraMinima', function (value, ele) {
        var qtdMaxima = $("#Filter_QtdCompraMaxima").val();

        if (!qtdMaxima || !value) {
            return true;
        }

        value = parseInt(value);
        qtdMaxima = parseInt(qtdMaxima);

        if (value <= qtdMaxima)
            return true;
        else
            return false;
    }, 'Quantidade mínima deve ser menor que a quantidade máxima.');

    $.validator.addMethod('validateQtdRecebidaMinima', function (value, ele) {
        var qtdMaxima = $("#Filter_QtdRecebidaMaxima").val();

        if (!qtdMaxima || !value) {
            return true;
        }

        value = parseInt(value);
        qtdMaxima = parseInt(qtdMaxima);

        if (value <= qtdMaxima)
            return true;
        else
            return false;
    }, 'Quantidade mínima deve ser menor que a quantidade máxima.');

    $("#downloadRelatorio").click(function () {
        $.ajax({
            url: `/${CONTROLLER_PATH}/DownloadRelatorioRastreioPeca`,
            method: "POST",
            cache: false,
            xhrFields: {
                responseType: 'blob'
            },
            data: {
                IdLote: $("#Filter_IdLote").val(),
                NroNota: $("#Filter_NroNota").val(),
                ReferenciaPronduto: $("#Filter_ReferenciaPronduto").val(),
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

    var $CompraInicial = $('#Filter_DataCompraMinima').closest('.date');
    var $CompraFinal = $('#Filter_DataCompraMaxima').closest('.date');
    var $RecebidoInicial = $('#Filter_DataRecebimentoMinima').closest('.date');
    var $RecebidoFinal = $('#Filter_DataRecebimentoMaxima').closest('.date');

    var createLinkedPickersCompra = function () {
        var dataInicial = $CompraInicial.datetimepicker({
            locale: moment.locale(),
            format: 'L',
            allowInputToggle: true
        });

        var dataFinal = $CompraFinal.datetimepicker({
            locale: moment.locale(),
            useCurrent: false,
            format: 'L',
            allowInputToggle: true
        });

        new dart.DateTimePickerLink(dataInicial, dataFinal, { ignoreTime: true });
    };

    var createLinkedPickesRecebido = function () {
        var dataInicial = $RecebidoInicial.datetimepicker({
            locale: moment.locale(),
            format: 'L',
            allowInputToggle: true
        });

        var dataFinal = $RecebidoFinal.datetimepicker({
            locale: moment.locale(),
            useCurrent: false,
            format: 'L',
            allowInputToggle: true,
        });

        new dart.DateTimePickerLink(dataInicial, dataFinal, { ignoreTime: true });
    }

    createLinkedPickersCompra();
    createLinkedPickesRecebido();

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
        columns: [
            { data: 'IdLote' },
            { data: 'NroNota' },
            { data: 'ReferenciaProduto' },
            { data: 'DescricaoProduto' },
            { data: 'DataRecebimento', width: 100 },
            { data: 'QtdCompra', width: 100 },
            { data: 'QtdRecebida', width: 100 }
        ]
    });

    $('#dataTable').dataTable.error = function (settings, helpPage, message) {
        console.log(message)
    };

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

})();

function setProduto(idProduto, descricao) {
    $("#Filter_IdProduto").val(idProduto);
    $("#Filter_DescricaoProduto").val(descricao);
    $("#modalProduto").modal("hide");
    $("#modalProduto").empty();
}