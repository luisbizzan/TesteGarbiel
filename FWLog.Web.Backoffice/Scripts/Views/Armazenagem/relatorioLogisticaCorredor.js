(function () {
    $.validator.setDefaults({ ignore: null });
    $('.onlyNumber').mask('0#');
    var $DataInicial = $('#Filter_DataInicial').closest('.date');
    var $DataFinal = $('#Filter_DataFinal').closest('.date');

    $.validator.addMethod('validatePontoArmazenagem', function (value, ele) {
        var prontoArmazenagem = $("#Filter_DescricaoPontoArmazenagem").val();

        if (value != "")
            return true
        else if (prontoArmazenagem != "")
            return true
        else
            return false;
      
    }, 'Informe o ponto de armazenagem para prosseguir');

    $.validator.addMethod('validateNivelArmazenagem', function (value, ele) {
        var nivelArmazenagem = $("#Filter_DescricaoNivelArmazenagem").val();

        if (value != "")
            return true
        else if (nivelArmazenagem != "")
            return true
        else
            return false;

    }, 'Informe o nível de armazenagem para prosseguir');

    $.validator.addMethod('validateCorredorInicial', function (value, ele) {
        var corredorInicial = $("#Filter_CorredorInicial").val();
        var corredorFinal = $("#Filter_CorredorFinal").val();

        if (corredorInicial == "" && corredorFinal == "")
            return true
        else if (corredorInicial == "" && corredorFinal != "")
            return false
        else
            return true;
    }, 'Informe o corredor inicial para prosseguir');

    $.validator.addMethod('validateCorredorFinal', function (value, ele) {
        var corredorInicial = $("#Filter_CorredorInicial").val();
        var corredorFinal = $("#Filter_CorredorFinal").val();

        if (corredorInicial == "" && corredorFinal == "")
            return true
        else if (corredorInicial != "" && corredorFinal == "")
            return false
        else
            return true
    }, 'Informe o corredor final para prosseguir');
   

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
            { data: 'Codigo' },
            { data: 'Referencia' },
            { data: 'Descricao' },
            { data: 'Tipo' },
            { data: 'Comprimento' },
            { data: 'Largura' },
            { data: 'Altura' },
            { data: 'Cubagem' },
            { data: 'Giro6m' },
            { data: 'GiroDD' },
            { data: 'ItLoc' },
            { data: 'Saldo' },
            { data: 'DuraDD' },
            { data: 'DtRepo' },

        ],
    });

    dart.dataTables.loadFormFilterEvents();

    $('#dataTable').dataTable.error = function (settings, helpPage, message) {
        console.log(message)
    };

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

    $("#pesquisarNivelArmazenagem").click(function () {
        $("#modalPesquisaNivelArmazenagem").load(HOST_URL + "NivelArmazenagem/PesquisaModal", function () {
            $("#modalPesquisaNivelArmazenagem").modal();
        });
    });

    $("#limparNivelArmazenagem").click(function () {
        $("#Filter_DescricaoNivelArmazenagem").val("");
        $("#Filter_IdNivelArmazenagem").val("");
    });

    $("#pesquisarPontoArmazenagem").click(function () {
        let id = $("#Filtros_IdNivelArmazenagem").val();
        $("#modalPesquisaPontoArmazenagem").load(HOST_URL + "PontoArmazenagem/PesquisaModal/" + id, function () {
            $("#modalPesquisaPontoArmazenagem").modal();
        });
    });

    $("#limparPontoArmazenagem").click(function () {
        $("#Filter_DescricaoPontoArmazenagem").val("");
        $("#Filter_IdPontoArmazenagem").val("");
    });

    $("#downloadRelatorioLogisticaCorredor").click(function () {
        if ($("#relatorioLogisticaCorredorForm").valid()) {
            $.ajax({
                url: "/Armazenagem/DownloadRelatorioLogisticaCorredor",
                method: "POST",
                data: {
                    IdNivelArmazenagem: $("#Filter_IdNivelArmazenagem").val(),
                    IdPontoArmazenagem: $("#Filter_IdPontoArmazenagem").val(),
                    CorredorInicial: $("#Filter_CorredorInicial").val(),
                    CorredorFinal: $("#Filter_CorredorFinal").val(),
                    DataInicial: $("#Filter_DataInicial").val(),
                    DataFinal: $("#Filter_DataFinal").val(),
                    Ordenacao: $("#Filter_Ordenacao").val()
                },
                xhrFields: {
                    responseType: 'blob'
                },
                success: function (data) {
                    var a = document.createElement('a');
                    var url = window.URL.createObjectURL(data);

                    a.href = url;
                    a.download = 'Relatório - Logística por Corredor.pdf';
                    document.body.append(a);
                    a.click();
                    a.remove();
                    window.URL.revokeObjectURL(url);
                }
            });
        }
    });

    $("#imprimirRelatorioLogisticaCorredor").click(function () {
        if ($("#relatorioLogisticaCorredorForm").valid()) {
            $("#modalImpressoras").load(HOST_URL + "BOPrinter/Selecionar?idImpressaoItem=1&acao=logisticaCorredor", function () {
                $("#modalImpressoras").modal();
            });
        }
    });

})();

function selecionarNivelArmazenagem(idNivelArmazenagem, descricao) {
    $("#Filter_DescricaoNivelArmazenagem").val(descricao);
    $("#Filter_IdNivelArmazenagem").val(idNivelArmazenagem);
    $("#modalPesquisaNivelArmazenagem").modal("hide");
    $("#modalPesquisaNivelArmazenagem").empty();
}

function selecionarPontoArmazenagem(idPontoArmazenagem, descricao) {
    $("#Filter_DescricaoPontoArmazenagem").val(descricao);
    $("#Filter_IdPontoArmazenagem").val(idPontoArmazenagem);
    $("#modalPesquisaPontoArmazenagem").modal("hide");
    $("#modalPesquisaPontoArmazenagem").empty();
}

function imprimir(acao, id) {
    switch (acao) {
        case 'logisticaCorredor':
            $.ajax({
                url: "/Armazenagem/ImprimirRelatorioLogisticaCorredor",
                method: "POST",
                data: {
                    IdImpressora: $("#IdImpressora").val(),
                    IdNivelArmazenagem: $("#Filter_IdNivelArmazenagem").val(),
                    IdPontoArmazenagem: $("#Filter_IdPontoArmazenagem").val(),
                    CorredorInicial: $("#Filter_CorredorInicial").val(),
                    CorredorFinal: $("#Filter_CorredorFinal").val(),
                    DataInicial: $('#Filter_DataInicial').val(),
                    DataFinal: $('#Filter_DataFinal').val(),
                    Ordenacao: $("#Filter_Ordenacao").val()
                },
                success: function (result) {
                    if (result.Success) {
                        PNotify.success({ text: result.Message });
                    } else {
                        PNotify.error({ text: result.Message });
                    }
                    $('#modalImpressoras').modal('toggle');
                    waitingDialog.hide();
                }
            });
            break;
    }
}

