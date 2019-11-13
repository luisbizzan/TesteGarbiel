(function () { 
    $("#imprimirEtiquetaConferencia").click(function () {
        $("#modalEtiquetaConferencia").load("BORecebimentoNota/DetalhesEtiquetaConferencia", function () {
            $("#modalEtiquetaConferencia").modal();
        });
    });

    $("#RegistrarRecebimentoNota").click(function () {
        $("#modalEtiquetaConferencia").load("BORecebimentoNota/RegistrarRecebimentoNota", function () {
           //Definir
        });
    });

    //Alterar chamada
    $("#Registro").click(function () {
        $("#modalRegistroRecebimento").load("BORecebimentoNota/ExibirModalRegistroRecebimento", function () {
            $("#modalRegistroRecebimento").modal();

            $('#ChaveAcesso').keypress(function (event) {
                var keycode = (event.keyCode ? event.keyCode : event.which);
                if (keycode === 13) {
                    $("#RegistroRecebimentoDetalhes").load("BORecebimentoNota/CarregarDadosNotaFiscalRegistro?chaveAcesso=" + $('#ChaveAcesso').val(), function () { });
                }
            });
        });
    });

    $("#imprimirRelatorio").click(function () {
        $("#modalImpressoras").load("BOPrinter/Selecionar", function () {
            $("#modalImpressoras").modal();
        });
    });

    $("#downloadRelatorio").click(function () {
        $.ajax({
            url: "/BORecebimentoNota/DownloadRelatorioNotas",
            method: "POST",
            xhrFields: {
                responseType: 'blob'
            },
            data: {
                Lote: $("#Filter_Lote").val(),
                Nota: $("#Filter_Nota").val(),
                DANFE: $("#Filter_DANFE").val(),
                IdStatus: $("#Filter_ListaStatus").val(),
                DataInicial: $("#Filter_DataInicial").val(),
                DataFinal: $("#Filter_DataFinal").val(),
                PrazoInicial: $("#Filter_PrazoInicial").val(),
                PrazoFinal: $("#Filter_PrazoFinal").val(),
                IdFornecedor: $("#Filter_IdFornecedor").val(),
                Atraso: $("#Filter_Atraso").val(),
                QuantidadePeca: $("#Filter_QuantidadePeca").val(),
                Volume: $("#Filter_Volume").val()
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

    var actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                action: 'details',
                href: view.detailsUrl + '?id=' + full.Id,
                visible: view.detailsVisible
            },
            {
                action: 'edit',
                href: view.editUrl + '?id=' + full.Id,
                visible: view.editVisible
            },
            {
                action: 'delete',
                attrs: { 'data-delete-url': view.deleteUrl + '?id=' + full.Id },
                visible: view.deleteVisible
            },
        ];
    });

    var $DataInicial = $('#Filter_DataInicial').closest('.date');
    var $DataFinal = $('#Filter_DataFinal').closest('.date');
    var $PrazoInicial = $('#Filter_PrazoInicial').closest('.date');
    var $PrazoFinal = $('#Filter_PrazoFinal').closest('.date');

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

        var prazoInicial = $PrazoInicial.datetimepicker({
            locale: moment.locale(),
            useCurrent: false,
            format: 'L',
            allowInputToggle: true
        });

        var prazoFinal = $PrazoFinal.datetimepicker({
            locale: moment.locale(),
            useCurrent: false,
            format: 'L',
            allowInputToggle: true
        });

        new dart.DateTimePickerLink(dataInicial, dataFinal, prazoInicial, prazoFinal, { ignoreTime: true });
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
        initComplete: function (settings, json) {
            dart.dataTables.addEventsForDropdownAutoposition($('#dataTable'));
        },
        stateSaveParams: function (settings, data) {
            dart.dataTables.saveFilterToData(data);
        },
        stateLoadParams: function (settings, data) {
            dart.dataTables.loadFilterFromData(data);
        },
        columns: [
            { data: 'Lote' },
            { data: 'Nota' },
            { data: 'QuantidadePeca' },
            { data: 'QuantidadeVolume' },
            { data: 'RecebidoEm' },
            { data: 'Atraso' },
            { data: 'Prazo' },
            { data: 'Fornecedor' },
            { data: 'Status' },
            actionsColumn
        ]
    });

    $('#dataTable').DataTable.ext.errMode = function (settings, helpPage, message) {
        let jqXHR = settings.jqXHR;
        if (jqXHR) {
            let responseJSON = jqXHR.responseJSON;
            if (responseJSON) {
                if (responseJSON.showAsToast)
                    PNotify.error({ text: responseJSON.error });
                return;
            }
        }
        alert(message);
    };

    dart.dataTables.loadFormFilterEvents();

    $("#pesquisarFornecedor").click(function () {
        $("#modalFornecedor").load(HOST_URL + "BOFornecedor/SearchModal", function () {
            $("#modalFornecedor").modal();
        });
    });

    function limparFornecedor() {
        let razao = $("#Filter_RazaoSocialFornecedor");
        let fornecedor = $("#Filter_IdFornecedor");
        razao.val("");
        fornecedor.val("");
    }

    $("#limparFornecedor").click(function () {
        limparFornecedor();
    });

    $("#pesquisarUsuarioRecebimento").click(function () {
        $("#modalUsuarioRecebimento").load(HOST_URL + "BOAccount/SearchModal", function () {
            $("#modalUsuarioRecebimento").modal();
        });
    });

    function limparUsuarioRecebimento() {
        let userName = $("#Filter_UserNameRecebimento");
        let usuarioId = $("#Filter_IdUsuarioRecebimento");
        userName.val("");
        usuarioId.val("");
    }

    $("#limparUsuarioRecebimento").click(function () {
        limparUsuarioRecebimento();
    });
})();

function Imprimir() {
    $.ajax({
        url: "/BORecebimentoNota/ImprimirRelatorioNotas",
        method: "POST",
        data: {
            IdImpressora: $("#IdImpressora").val(),
            Lote: $("#Filter_Lote").val(),
            Nota: $("#Filter_Nota").val(),
            DANFE: $("#Filter_DANFE").val(),
            IdStatus: $("#Filter_ListaStatus").val(),
            DataInicial: $("#Filter_DataInicial").val(),
            DataFinal: $("#Filter_DataFinal").val(),
            PrazoInicial: $("#Filter_PrazoInicial").val(),
            PrazoFinal: $("#Filter_PrazoFinal").val(),
            IdFornecedor: $("#Filter_IdFornecedor").val(),
            Atraso: $("#Filter_Atraso").val(),
            QuantidadePeca: $("#Filter_QuantidadePeca").val(),
            Volume: $("#Filter_Volume").val()
        },
        success: function () {
            $("#btnFechar").click();
        }
    });
}

function setFornecedor(idFornecedor, razaoSocial) {
    let razao = $("#Filter_RazaoSocialFornecedor");
    let fornecedor = $("#Filter_IdFornecedor");
    razao.val(razaoSocial);
    fornecedor.val(idFornecedor);
    $("#modalFornecedor").modal("hide");
    $("#modalFornecedor").empty();
}

function setUsuarioRecebimento(idUsuario, nomeUsuario) {
    let userName = $("#Filter_UserNameRecebimento");
    let usuarioId = $("#Filter_IdUsuarioRecebimento");
    userName.val(nomeUsuario);
    usuarioId.val(idUsuario);
    $("#modalUsuarioRecebimento").modal("hide");
    $("#modalUsuarioRecebimento").empty();
}