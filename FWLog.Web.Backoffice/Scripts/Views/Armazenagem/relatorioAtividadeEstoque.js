(function () {
    $('.onlyNumber').mask('0#');

    var $DataInicialSolicitacao = $('#Filter_DataInicialSolicitacao').closest('.date');
    var $DataFinalSolicitacao = $('#Filter_DataFinalSolicitacao').closest('.date');

    var createLinkedPickers = function () {
        var dataInicialSolicitacao = $DataInicialSolicitacao.datetimepicker({
            locale: moment.locale(),
            format: 'L',
            allowInputToggle: true
        });

        var dataFinalSolicitacao = $DataFinalSolicitacao.datetimepicker({
            locale: moment.locale(),
            useCurrent: false,
            format: 'L',
            allowInputToggle: true
        });

        new dart.DateTimePickerLink(dataInicialSolicitacao, dataFinalSolicitacao, { ignoreTime: true });
    };

    createLinkedPickers();

    var $DataInicialExecucao = $('#Filter_DataInicialExecucao').closest('.date');
    var $DataFinalExecucao = $('#Filter_DataFinalExecucao').closest('.date');

    var createLinkedPickers2 = function () {
        var dataInicialExecucao = $DataInicialExecucao.datetimepicker({
            locale: moment.locale(),
            format: 'L',
            allowInputToggle: true
        });

        var dataFinalExecucao = $DataFinalExecucao.datetimepicker({
            locale: moment.locale(),
            useCurrent: false,
            format: 'L',
            allowInputToggle: true
        });

        new dart.DateTimePickerLink(dataInicialExecucao, dataFinalExecucao, { ignoreTime: true });
    };

    createLinkedPickers2();

    dart.dataTables.loadFormFilterEvents();

    $(document.body).on('click', "#pesquisar", function () {
        $("#tabela").show();
    });

    $("#downloadRelatorio").click(function () {
        $.ajax({
            url: `/${CONTROLLER_PATH}/DownloadRelatorioAtividadeEstoque`,
            method: "POST",
            cache: false,
            xhrFields: {
                responseType: 'blob'
            },
            data: {
                IdProduto: $("#Filter_IdProduto").val(),
                IdAtividadeEstoqueTipo: $("#Filter_IdAtividadeEstoqueTipo").val(),
                QuantidadeInicial: $("#Filter_QuantidadeInicial").val(),
                QuantidadeFinal: $("#Filter_QuantidadeFinal").val(),
                DataInicialSolicitacao: $("#Filter_DataInicialSolicitacao").val(),
                DataFinalSolicitacao: $("#Filter_DataFinalSolicitacao").val(),
                DataInicialExecucao: $("#Filter_DataInicialExecucao").val(),
                DataFinalExecucao: $("#Filter_DataFinalExecucao").val(),
                IdUsuarioExecucao: $("#Filter_IdUsuarioExecucao").val()
            },
            success: function (data) {
                var a = document.createElement('a');
                var url = window.URL.createObjectURL(data);

                a.href = url;
                a.download = 'Relatório Atividades de Estoque.pdf';
                document.body.append(a);
                a.click();
                a.remove();
                window.URL.revokeObjectURL(url);
            }
        });
    });

    $("#imprimirRelatorio").click(function () {
        if ($("#relatorioAtividadeEstoqueForm").valid()) {
            $("#modalImpressoras").load(HOST_URL + "BOPrinter/Selecionar?idImpressaoItem=1&acao=atividadeEstoque", function () {
                $("#modalImpressoras").modal();
            });
        }
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
            { data: 'TipoAtividade' },
            { data: 'ReferenciaProduto' },
            { data: 'DescricaoProduto' },
            { data: 'CodigoEndereco' },
            { data: 'QuantidadeInicial' },
            { data: 'QuantidadeFinal' },
            { data: 'PorcentagemDivergencia', orderable: false },
            { data: 'DataSolicitacao' },
            { data: 'DataExecucao' },
            { data: 'UsuarioExecucao' },
            { data: 'Finalizado' }
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

    $("#pesquisarUsuarioExecucao").click(function () {
        $("#modalUsuarioExecucao").load(HOST_URL + "BOAccount/SearchModal/Recebimento", function () {
            $("#modalUsuarioExecucao").modal();
        });
    });

    function limparUsuarioExecucao() {
        $("#Filter_UserNameExecucao").val("");
        $("#Filter_IdUsuarioExecucao").val("");
    }

    $("#limparUsuarioExecucao").click(function () {
        limparUsuarioExecucao();
    });
})();

function setProduto(idProduto, descricao) {
    $("#Filter_IdProduto").val(idProduto);
    $("#Filter_DescricaoProduto").val(descricao);
    $("#modalProduto").modal("hide");
    $("#modalProduto").empty();
}

function setUsuario(idUsuario, nomeUsuario, origem) {
    $("#Filter_UserNameExecucao").val(nomeUsuario);
    $("#Filter_IdUsuarioExecucao").val(idUsuario);
    $("#modalUsuarioExecucao").modal("hide");
    $("#modalUsuarioExecucao").empty();
}

function imprimir(acao, id) {
    switch (acao) {
        case 'atividadeEstoque':
            $.ajax({
                url: "/Armazenagem/ImprimirRelatorioAtividadeEstoque",
                method: "POST",
                data: {
                    IdImpressora: $("#IdImpressora").val(),
                    IdProduto: $("#Filter_IdProduto").val(),
                    IdAtividadeEstoqueTipo: $("#Filter_IdAtividadeEstoqueTipo").val(),
                    QuantidadeInicial: $("#Filter_QuantidadeInicial").val(),
                    QuantidadeFinal: $("#Filter_QuantidadeFinal").val(),
                    DataInicialSolicitacao: $("#Filter_DataInicialSolicitacao").val(),
                    DataFinalSolicitacao: $("#Filter_DataFinalSolicitacao").val(),
                    DataInicialExecucao: $("#Filter_DataInicialExecucao").val(),
                    DataFinalExecucao: $("#Filter_DataFinalExecucao").val(),
                    IdUsuarioExecucao: $("#Filter_IdUsuarioExecucao").val()
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