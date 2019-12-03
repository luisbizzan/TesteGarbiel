(function () {
    $('.onlyNumber').mask('0#');

    var actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        debugger

        return [
            {
                text: "Atualizar Quarentena",
                attrs: { 'data-id': full.IdNotaFiscal, 'action': 'alterarStatus' },
                icon: 'fa fa-pencil-square',
                visible: view.registrarRecebimento
            },
            //{
            //    text: "Registrar Recebimento",
            //    attrs: { 'data-id': full.IdNotaFiscal, 'action': 'click' },
            //    icon: 'fa fa-pencil-square',
            //    visible: view.registrarRecebimento
            //},
            //{
            //    text: "Registrar Conferência",
            //    attrs: { 'data-id': full.IdNotaFiscal, 'action': 'click' },
            //    icon: 'fa fa-check-square',
            //    visible: view.registrarRecebimento
            //},
            //{
            //    text: "Tratar Divergência",
            //    attrs: { 'data-id': full.IdNotaFiscal, 'action': 'click' },
            //    icon: 'fa fa-warning',
            //    visible: view.registrarRecebimento
            //}
        ];
    });

    var $DataAberturaInicial = $('#Filter_DataAberturaInicial').closest('.date');
    var $DataAberturaFinal = $('#Filter_DataAberturaFinal').closest('.date');
    var $DataEncerramentoInicial = $('#Filter_DataEncerramentoInicial').closest('.date');
    var $DataEncerramentoFinal = $('#Filter_DataEncerramentoFinal').closest('.date');

    var createLinkedPickers = function () {
        var dataAberturaInicial = $DataAberturaInicial.datetimepicker({
            locale: moment.locale(),
            format: 'L',
            allowInputToggle: true
        });

        var dataAberturaFinal = $DataAberturaFinal.datetimepicker({
            locale: moment.locale(),
            useCurrent: false,
            format: 'L',
            allowInputToggle: true
        });

        new dart.DateTimePickerLink(dataAberturaInicial, dataAberturaFinal, { ignoreTime: true });
    };

    var createLinkedPickes2 = function () {
        var dataEncerramentoInicial = $DataEncerramentoInicial.datetimepicker({
            locale: moment.locale(),
            format: 'L',
            allowInputToggle: true
        });

        var dataEncerramentoFinal = $DataEncerramentoFinal.datetimepicker({
            locale: moment.locale(),
            useCurrent: false,
            format: 'L',
            allowInputToggle: true
        });

        new dart.DateTimePickerLink(dataEncerramentoInicial, dataEncerramentoFinal, { ignoreTime: true });
    }

    createLinkedPickers();
    createLinkedPickes2();


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
            { data: 'Fornecedor' },
            { data: 'DataAbertura' },
            { data: 'DataEncerramento' },
            { data: 'Atraso' },
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

    $(document.body).on('click', "[action='alterarStatus']", alterarStatus);

    function alterarStatus() {
        let id = $(this).data("id");
        let $modal = $("#alterarStatus");
    }

})();

function setFornecedor(idFornecedor, razaoSocial) {
    let razao = $("#Filter_RazaoSocialFornecedor");
    let fornecedor = $("#Filter_IdFornecedor");
    razao.val(razaoSocial);
    fornecedor.val(idFornecedor);
    $("#modalFornecedor").modal("hide");
    $("#modalFornecedor").empty();
}
