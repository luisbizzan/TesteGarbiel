(function () {
    $.validator.setDefaults({ ignore: null });
    $('.onlyNumber').mask('0#');
    $("dateFormat").mask("99/99/9999");
    $('.hourMinute').mask("99:99", { reverse: true });

    $.validator.addMethod('validateDateOrRegistroInicial', function (value, ele) {
        var dataInicial = $("#Filter_DataRegistroInicial").val();

        if (value != "")
            return true
        else if (dataInicial != "")
            return true
        else
            return false;
    }, 'Data de Registro Inicial ou Data de Sincronismo Inicial Obrigatório');

    $.validator.addMethod('validateDateOrRegistroFinal', function (value, ele) {
        var dataFinal = $("#Filter_DataRegistroFinal").val();

        if (value != "")
            return true
        else if (dataFinal != "")
            return true
        else
            return false;
    }, 'Data de registro Final ou Data de Sincronismo Final Obrigatório');

    $.validator.addMethod('validateTime', function (value, ele) {
        if (value === "") {
            return true;
        }
        let regex = /^([0-1]?[0-9]|2[0-4]):([0-5][0-9])(:[0-5][0-9])?$/;
        let validaRegex = regex.test(value);
        if (validaRegex) {
            return true;
        }
        return false;
    }, 'Hora inválida');

    $("#Filter_TempoInicial").blur(function () {
        if ($(this).val() === ":") {
            $(this).val("");
        }
    });

    $("#Filter_InicialInicial").blur(function () {
        if ($(this).val() === ":") {
            $(this).val("");
        }
    });

    $("#imprimirEtiquetaNotaRecebimento").click(function () {
        $("#modalEtiquetaNotaRecebimento").load("BORecebimentoNota/DetalhesEtiquetaNotaRecebimento", function () {
            $("#modalEtiquetaNotaRecebimento").modal();
        });
    });

    $("#imprimirRelatorio").click(function () {
        $("#modalImpressoras").load("BOPrinter/Selecionar?idImpressaoItem=1&acao=notas", function () {
            $("#modalImpressoras").modal();
        });
    });

    $("#downloadRelatorio").click(function () {
        $.ajax({
            url: "/BORecebimentoNota/DownloadRelatorioNotas",
            method: "POST",
            cache: false,
            xhrFields: {
                responseType: 'blob'
            },
            data: {
                NumeroNF: $("#Filter_NumeroNF").val(),
                ChaveAcesso: $("#Filter_ChaveAcesso").val(),
                IdStatus: $("#Filter_IdStatus").val(),
                DataRegistroInicial: $("#Filter_DataRegistroInicial").val(),
                DataRegistroFinal: $("#Filter_DataRegistroFinal").val(),
                DataSincronismoInicial: $("#Filter_DataSincronismoInicial").val(),
                DataSincronismoFinal: $("#Filter_DataSincronismoFinal").val(),
                IdFornecedor: $("#Filter_IdFornecedor").val(),
                DiasAguardando: $("#Filter_DiasAguardando").val(),
                QuantidadePeca: $("#Filter_QuantidadePeca").val(),
                QuantidadeVolume: $("#Filter_QuantidadeVolume").val(),
                IdUsuarioRecebimento: $("#Filter_IdUsuarioRecebimento").val(),
            },
            success: function (data) {
                var a = document.createElement('a');
                var url = window.URL.createObjectURL(data);

                a.href = url;
                a.download = 'Relatório Recebimento Notas.pdf';
                document.body.append(a);
                a.click();
                a.remove();
                window.URL.revokeObjectURL(url);
            }
        });
    });

    var $DataRegistroInicial = $('#Filter_DataRegistroInicial').closest('.date');
    var $DataRegistroFinal = $('#Filter_DataRegistroFinal').closest('.date');
    var $DataSincronismoInicial = $('#Filter_DataSincronismoInicial').closest('.date');
    var $DataSincronismoFinal = $('#Filter_DataSincronismoFinal').closest('.date');

    var createLinkedPickers = function () {
        var dataRegistroInicial = $DataRegistroInicial.datetimepicker({
            locale: moment.locale(),
            format: 'L',
            allowInputToggle: true
        });

        var dataRegistroFinal = $DataRegistroFinal.datetimepicker({
            locale: moment.locale(),
            useCurrent: false,
            format: 'L',
            allowInputToggle: true
        });

        new dart.DateTimePickerLink(dataRegistroInicial, dataRegistroFinal, { ignoreTime: true });
    };

    var createLinkedPickes2 = function () {
        var dataSincronismoInicial = $DataSincronismoInicial.datetimepicker({
            locale: moment.locale(),
            format: 'L',
            allowInputToggle: true
        });

        var dataSincronismoFinal = $DataSincronismoFinal.datetimepicker({
            locale: moment.locale(),
            useCurrent: false,
            format: 'L',
            allowInputToggle: true
        });

        new dart.DateTimePickerLink(dataSincronismoInicial, dataSincronismoFinal, { ignoreTime: true });
    };

    createLinkedPickers();
    createLinkedPickes2();

    var iconeStatus = function (data, type, row) {
        if (type === 'display') {
            var nomeCor,
                lote = row.Lote || '',
                tooltipText;

            if (row.Atraso === 0) {
                nomeCor = 'verde';
                tooltipText = 'Sem dias aguardando';
            }

            if (row.Atraso > 0) {
                if (row.Atraso > 2) {
                    nomeCor = 'vermelho';
                    tooltipText = 'Mais que 2 dias aguardando';
                } else {
                    nomeCor = 'amarelo';
                    tooltipText = 'Até 2 dias aguardando';
                }
            }

            return `<i class="fa fa-circle icone-status-${nomeCor}" title = "${tooltipText}" data-toggle = "tooltip"></i>${lote}`;
        }

        return data;
    };

        $('#dataTable').DataTable({
        ajax: {
            "url": view.pageDataUrl,
            "type": "POST",
            "data": function (data) {
                dart.dataTables.saveFilterToData(data);
            },
            "error": function (data) {
                if (!data.statusText) {
                    PNotify.error({ text: data.statusText });
                    NProgress.done();
                }
            }
        },
        initComplete: function (settings, json) {
            dart.dataTables.addEventsForDropdownAutoposition($('#dataTable'));
        },
        columns: [
            { data: 'NomeFornecedor'                       },
            { data: 'Usuario'                 , width: 100 },
            { data: 'NumeroNF'                             },
            { data: 'Serie'                                },
            { data: 'DiasAguardando'                       },
            { data: 'DataHoraRegistro'                     },
            { data: 'DataHoraSincronismo'                  },
            { data: 'Status'                               }
        ]
    });

    $('#dataTable').dataTable.error = function (settings, helpPage, message) {
        console.log(message);
    };


    dart.dataTables.loadFormFilterEvents();

    $("#pesquisarFornecedor").click(function () {
        $("#modalFornecedor").load(HOST_URL + "BOFornecedor/SearchModal", function () {
            $("#modalFornecedor").modal();
        });
    });

    function limparFornecedor() {
        $("#Filter_NomeFornecedor").val("");
        $("#Filter_IdFornecedor").val("");
    }

    $("#limparFornecedor").click(function () {
        limparFornecedor();
    });

    $("#pesquisarUsuarioRecebimento").click(function () {
        $("#modalUsuarioRecebimento").load(HOST_URL + "BOAccount/SearchModal/Recebimento", function () {
            $("#modalUsuarioRecebimento").modal();
        });
    });

    function limparUsuarioRecebimento() {
        $("#Filter_UserNameRecebimento").val("");
        $("#Filter_IdUsuarioRecebimento").val("");
    }

    $("#limparUsuarioRecebimento").click(function () {
        limparUsuarioRecebimento();
    });

    //Limpando a div da modal de usuários
    $("#modalUsuarioRecebimento").on("hidden.bs.modal", function () {
        $("#modalUsuarioRecebimento").text('');
    });

})();

function imprimir(acao, id) {
    switch (acao) {
        case 'notas':
            $.ajax({
                url: "/BORecebimentoNota/ImprimirRelatorioNotas",
                method: "POST",
                data: {
                    IdImpressora: $("#IdImpressora").val(),
                    NumeroNF: $("#Filter_NumeroNF").val(),
                    ChaveAcesso: $("#Filter_ChaveAcesso").val(),
                    IdStatus: $("#Filter_IdStatus").val(),
                    DataRegistroInicial: $("#Filter_DataRegistroInicial").val(),
                    DataRegistroFinal: $("#Filter_DataRegistroFinal").val(),
                    DataSincronismoInicial: $("#Filter_DataSincronismoInicial").val(),
                    DataSincronismoFinal: $("#Filter_DataSincronismoFinal").val(),
                    IdFornecedor: $("#Filter_IdFornecedor").val(),
                    DiasAguardando: $("#Filter_DiasAguardando").val(),
                    QuantidadePeca: $("#Filter_QuantidadePeca").val(),
                    Volume: $("#Filter_Volume").val(),
                    IdUsuarioRecebimento: $("#Filter_IdUsuarioRecebimento").val()
                },
                success: function (result) {
                    mensagemImpressao(result);
                    $('#modalImpressoras').modal('toggle');
                    waitingDialog.hide();
                }
            });
            break;
        case 'entconf':
            $.ajax({
                url: "/BORecebimentoNota/ImprimirDetalhesEntradaConferencia",
                method: "POST",
                data: {
                    IdImpressora: $("#IdImpressora").val(),
                    IdNotaFiscal: id
                },
                success: function (result) {
                    mensagemImpressao(result);
                    $('#modalImpressoras').modal('toggle');
                    waitingDialog.hide();
                }
            });
            break;
        case 'etqrecebimento':
            $.ajax({
                url: "/BORecebimentoNota/ImprimirEtiquetaRecebimento",
                method: "POST",
                data: {
                    IdImpressora: $("#IdImpressora").val(),
                    IdNotaFiscal: id
                },
                success: function (result) {
                    mensagemImpressao(result);
                    $('#modalImpressoras').modal('toggle');
                    waitingDialog.hide();
                }
            });
            break;
    }
}

function mensagemImpressao(result) {
    if (result.Success) {
        PNotify.success({ text: result.Message });
    } else {
        PNotify.error({ text: result.Message });
    }
}

function detalhesNota() {
    var id = $(this).data("id");
    let $modalDetalhesEntradaConferencia = $("#modalDetalhesEntradaConferencia");

    $modalDetalhesEntradaConferencia.load(CONTROLLER_PATH + "DetalhesEntradaConferencia/" + id, function () {
        $modalDetalhesEntradaConferencia.modal();
    });
}

function setFornecedor(idFornecedor, nomeFornecedor, origem) {
    $("#Filter_NomeFornecedor").val(nomeFornecedor);
    $("#Filter_IdFornecedor").val(idFornecedor);
    $("#modalFornecedor").modal("hide");
    $("#modalFornecedor").empty();
}

function setUsuario(idUsuario, nomeUsuario, origem) {
    $("#Filter_UserNameRecebimento").val(nomeUsuario);
    $("#Filter_IdUsuarioRecebimento").val(idUsuario);
    $("#modalUsuarioRecebimento").modal("hide");
    $("#modalUsuarioRecebimento").empty();
}


