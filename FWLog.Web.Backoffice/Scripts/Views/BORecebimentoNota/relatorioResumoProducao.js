(function () {
    let $tableConferencia = $('#dataTableConferencia > table');
    let $tableRecebimento = $('#dataTableRecebimento > table');

    let $divTableConferencia = $('#dataTableConferencia');
    let $divTableRecebimento = $('#dataTableRecebimento');

    let $divTables = $('#tabela');

    $(document.body).on('click', "#pesquisar", function (e) {
        e.preventDefault();
        debugger

        var tipo = $("input[name='TipoRelatorio']:checked").val();

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
            { data: 'LotesRecebidosUsuario' },
            { data: 'PecasRecebidasUsuario' },
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
            { data: 'NotasRecebidasUsuario' },
            { data: 'VolumesRecebidosUsuario' },
            { data: 'NotasRecebidas', width: 100 },
            { data: 'VolumesRecebidos', width: 100 },
            { data: 'Percentual', width: 100 },
            { data: 'Ranking', width: 100 }
        ]
    });

    $tableRecebimento.dataTable.error = function (settings, helpPage, message) {
        console.log(message)
    };
})();