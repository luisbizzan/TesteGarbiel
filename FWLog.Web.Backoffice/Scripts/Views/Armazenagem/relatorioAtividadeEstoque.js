﻿(function () {
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
            { data: 'TipoAtividade' },
            { data: 'CodigoEndereco' },
            { data: 'ReferenciaProduto' },
            { data: 'DescricaoProduto' },
            { data: 'QuantidadeInicial' },
            { data: 'DataSolicitacao' },
            { data: 'QuantidadeFinal' },
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