(function () {
    $('input[type=radio]').iCheck({
        checkboxClass: 'icheckbox_flat-green',
        radioClass: 'iradio_flat-green'
    });

    let $divTables = $('#tabela');

    let $tableConferencia = $('#dataTableConferencia > table');
    let $tableRecebimento = $('#dataTableRecebimento > table');

    let $divTableConferencia = $('#dataTableConferencia');
    let $divTableRecebimento = $('#dataTableRecebimento');

    var $dataInicio = $('#Filter_DataRecebimentoMinima').closest('.date');
    var $dataFim = $('#Filter_DataRecebimentoMaxima').closest('.date');

    var createLinkedPickes = function () {
        var dataInicial = $dataInicio.datetimepicker({
            locale: moment.locale(),
            format: 'L',
            allowInputToggle: true
        });

        var dataFinal = $dataFim.datetimepicker({
            locale: moment.locale(),
            useCurrent: false,
            format: 'L',
            allowInputToggle: true,
        });

        new dart.DateTimePickerLink(dataInicial, dataFinal, { ignoreTime: true });
    }

    createLinkedPickes();

    $("#pesquisarUsuario").click(function () {
        $("#modalUsuario").load(HOST_URL + "BOAccount/SearchModal/Recebimento", function () {
            $("#modalUsuario").modal();
        });
    });

    $(document.body).on('click', "#pesquisar", function (e) {
        e.preventDefault();

        var tipo = $("input[name='TipoRelatorio']:checked").val();
        var data = $('#Filter_DataRecebimentoMinima').val()

        if (!moment(data, "DD/MM/YYYY").isValid()) {
            PNotify.warning({ text: "Selecione uma data inicial." });
        } else {
            switch (tipo) {
                case "R":
                    $tableRecebimento.DataTable().ajax.reload(null, true);

                    $divTableConferencia.hide();
                    $divTableRecebimento.show();

                    $divTables.show();
                    break;
                case "C":
                    $tableConferencia.DataTable().ajax.reload(null, true);

                    $divTableRecebimento.hide();
                    $divTableConferencia.show();

                    $divTables.show();
                    break;
                default:
                    $divTables.hide();
                    $divTableConferencia.hide();
                    $divTableRecebimento.hide();

                    PNotify.warning({ text: "Selecione um tipo de relatório." });
            }
        }

    });

    $tableConferencia.DataTable({
        ajax: {
            "url": view.ConferenciapageDataUrl,
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
            dart.dataTables.addEventsForDropdownAutoposition($tableConferencia);
        },
        stateSaveParams: function (settings, data) {
            dart.dataTables.saveFilterToData(data);
        },
        stateLoadParams: function (settings, data) {
            dart.dataTables.loadFilterFromData(data);
        },
        columns: [
            { data: 'NomeUsuario' },
            { data: 'LotesRecebidosUsuario', width: 100 },
            { data: 'PecasRecebidasUsuario', width: 100 },
            { data: 'LotesRecebidos', width: 100 },
            { data: 'PecasRecebidas', width: 100 },
            { data: 'Percentual', width: 100 },
            { data: 'Ranking', width: 100 }
        ]
    });

    $tableConferencia.dataTable.error = function (settings, helpPage, message) {
        console.log(message)
    };

    $tableRecebimento.DataTable({
        ajax: {
            "url": view.RecebimentopageDataUrl,
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
            dart.dataTables.addEventsForDropdownAutoposition($tableRecebimento);
        },
        stateSaveParams: function (settings, data) {
            dart.dataTables.saveFilterToData(data);
        },
        stateLoadParams: function (settings, data) {
            dart.dataTables.loadFilterFromData(data);
        },
        columns: [
            { data: 'NomeUsuario' },
            { data: 'NotasRecebidasUsuario', width: 100 },
            { data: 'VolumesRecebidosUsuario', width: 100 },
            { data: 'NotasRecebidas', width: 100 },
            { data: 'VolumesRecebidos', width: 100 },
            { data: 'Percentual', width: 100 },
            { data: 'Ranking', width: 100 }
        ]
    });

    $tableRecebimento.dataTable.error = function (settings, helpPage, message) {
        console.log(message)
    };

    $("#limparUsuario").click(function () {
        $("#Filter_NomeUsuario").val("");
        $("#Filter_IdUsuario").val("");
    });
})();

function setUsuario(idUsuario, nomeUsuario, origem) {
    $("#Filter_NomeUsuario").val(nomeUsuario);
    $("#Filter_IdUsuario").val(idUsuario);
    $("#modalUsuario").modal("hide");
    $("#modalUsuario").empty();
}