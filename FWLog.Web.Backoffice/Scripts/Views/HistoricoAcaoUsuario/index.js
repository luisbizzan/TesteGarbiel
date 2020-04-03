(function () {
    $.validator.setDefaults({ ignore: null });
    $('.onlyNumber').mask('0#');
    $("dateFormat").mask("99/99/9999");
    $('.hourMinute').mask("99:99", { reverse: true });

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
            }
        },
        columns: [
            { data: 'Usuario', },
            { data: 'Descricao', },
            { data: 'ColetorAplicacaoDescricao', },
            { data: 'HistoricoColetorTipoDescricao', },
            { data: 'DataHora', },
        ]
    });

    $("#downloadHistoricoAcaoUsuario").click(function () {
        $.ajax({
            url: "/HistoricoAcaoUsuario/DownloadHistorico",
            method: "POST",
            data: {
                IdUsuario: $("#Filter_IdUsuario").val(),
                UsuarioSelecionado: $("#Filter_UserNameUsuario").val(),
                DataInicial: $("#Filter_DataInicial").val(),
                DataFinal: $("#Filter_DataFinal").val(),
                IdColetorAplicacao: $("#Filter_IdColetorAplicacao").val(),
                ColetorAplicacao: $("#Filter_IdColetorAplicacao option:selected").text(),
                IdHistoricoColetorTipo: $("#Filter_IdHistoricoColetorTipo").val(),
                HistoricoColetorTipo: $("#Filter_IdHistoricoColetorTipo option:selected").text(),
            },
            xhrFields: {
                responseType: 'blob'
            },
            success: function (data) {
                var a = document.createElement('a');
                var url = window.URL.createObjectURL(data);

                a.href = url;
                a.download = 'Relatório Histórico do Usuário.pdf';
                document.body.append(a);
                a.click();
                a.remove();
                window.URL.revokeObjectURL(url);
            }
        });
    });

    $("#imprimirHistoricoAcaoUsuario").click(function () {
        $("#modalImpressoras").load("BOPrinter/Selecionar?idImpressaoItem=1&acao=historico", function () {
            $("#modalImpressoras").modal();
        });
    });

    dart.dataTables.loadFormFilterEvents();

    $("#pesquisarUsuario").click(function () {
        $("#modalUsuario").load(HOST_URL + "BOAccount/SearchModal/Usuario", function () {
            $("#modalUsuario").modal();
        });
    });

    $("#limparUsuario").click(function () {
        limparUsuario();
    });

})();

function setUsuario(idUsuario, nomeUsuario, origem) {
    if (origem === "Usuario") {
        $("#Filter_UserNameUsuario").val(nomeUsuario);
        $("#Filter_IdUsuario").val(idUsuario);
        $("#modalUsuario").modal("hide");
        $("#modalUsuario").empty();
    }
}

function limparUsuario() {
    $("#Filter_UserNameUsuario").val("");
    $("#Filter_IdUsuario").val("");
}


function imprimir(acao, id) {
    switch (acao) {
        case 'historico':
            $.ajax({
                url: "/HistoricoAcaoUsuario/ImprimirHistorico",
                method: "POST",
                data: {
                    IdImpressora: $("#IdImpressora").val(),
                    IdUsuario: $("#Filter_IdUsuario").val(),
                    UsuarioSelecionado: $("#Filter_UserNameUsuario").val(),
                    DataInicial: $("#Filter_DataInicial").val(),
                    DataFinal: $("#Filter_DataFinal").val(),
                    IdColetorAplicacao: $("#Filter_IdColetorAplicacao").val(),
                    ColetorAplicacao: $("#Filter_IdColetorAplicacao option:selected").text(),
                    IdHistoricoColetorTipo: $("#Filter_IdHistoricoColetorTipo").val(),
                    HistoricoColetorTipo: $("#Filter_IdHistoricoColetorTipo option:selected").text(),
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