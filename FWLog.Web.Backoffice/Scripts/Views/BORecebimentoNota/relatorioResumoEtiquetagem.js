(function () {
    $('.onlyNumber').mask('0#');

    dart.dataTables.loadFormFilterEvents();

    $("#pesquisarProduto").click(function () {
        $("#modalProduto").load(HOST_URL + "Produto/SearchModal", function () {
            $("#modalProduto").modal();
        });
    });

    function limparProduto() {
        $("#Filter_DescricaoProduto").val("");
        $("#Filter_IdProduto").val("");
    }

    $("#limparProduto").click(function () {
        limparProduto();
    });

    $("#pesquisarUsuarioEtiquetagem").click(function () {
        $("#modalUsuarioEtiquetagem").load(HOST_URL + "BOAccount/SearchModal/Etiquetagem", function () {
            $("#modalUsuarioEtiquetagem").modal();
        });
    });

    function limparUsuarioEtiquetagem() {
        $("#Filter_UsuarioEtiquetagem").val("");
        $("#Filter_IdUsuarioEtiquetagem").val("");
    }

    $("#limparUsuarioEtiquetagem").click(function () {
        limparUsuarioEtiquetagem();
    });

    $(document.body).on('click', "#pesquisar", function () {
        $("#tabela").removeClass("hidden");
    });

    $.validator.addMethod("validateQtdeFinal", function (value, element, params) {
        //Capture valor da quantidade inicial.
        var qtdeInicial = $("#Filter_QuantidadeInicial").val();
        //Atribuo 0 caso não tenha nada preenchido.
        if (!qtdeInicial)
            qtdeInicial = 0;
        if (!value)
            return true;
        //Retorno verdadeiro se for maior que a quantidade inicial.
        return parseInt(value) >= parseInt(qtdeInicial);
    }, "O valor deve ser um número maior que a Quantidade Inicial.");

    $.validator.addClassRules({
        validateQtdeFinal: { validateQtdeFinal: true }
    });

    $.validator.addMethod("validateQtdeInicial", function (value, element, params) {
        //Capture valor da quantidade inicial.
        var qtdeFinal = $("#Filter_QuantidadeFinal").val();
        //Atribuo 0 caso não tenha nada preenchido.
        if (qtdeFinal && parseInt(value) > parseInt(qtdeFinal))
            return false;
        if (!value)
            value = 0;
        return true;
    }, "O valor deve ser um número menor que a Quantidade Final.");

    $.validator.addClassRules({
        validateQtdeInicial: { validateQtdeInicial: true }
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

    var $DataInicial = $('#Filter_DataInicial').closest('.date');
    var $DataFinal = $('#Filter_DataFinal').closest('.date');

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
            { data: 'IdLogEtiquetagem' },
            { data: 'Referencia' },
            { data: 'Descricao' },
            { data: 'TipoEtiquetagem', width: 100 },
            { data: 'Quantidade', width: 100 },
            { data: 'DataHora', width: 100 },
            { data: 'Usuario', width: 100 }
        ]
    });

    $('#dataTable').dataTable.error = function (settings, helpPage, message) {
        console.log(message)
    };
})();

function setProduto(idProduto, descricao) {
    $("#Filter_DescricaoProduto").val(descricao);
    $("#Filter_IdProduto").val(idProduto);
    $("#modalProduto").modal("hide");
    $("#modalProduto").empty();
}

function setUsuario(idUsuario, nomeUsuario, origem) {
    if (origem === "Etiquetagem") {
        $("#Filter_UsuarioEtiquetagem").val(nomeUsuario);
        $("#Filter_IdUsuarioEtiquetagem").val(idUsuario);
        $("#modalUsuarioEtiquetagem").modal("hide");
        $("#modalUsuarioEtiquetagem").empty();
    }
}