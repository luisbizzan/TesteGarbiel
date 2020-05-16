(function () {
    $('#modalVisualizar').on('hidden.bs.modal', function () {
        location.reload();
    });

    $.validator.setDefaults({ ignore: null });
    $('.onlyNumber').mask('0#');
    $("dateFormat").mask("99/99/9999");
    $('.hourMinute').mask("99:99", { reverse: true });

    $.validator.addMethod('validateDateOrPrazoInicial', function (value, ele) {
        var dataInicial = $("#Filter_Data_Inicial").val();

        if (value !== "")
            return true;
        else if (dataInicial !== "")
            return true;
        else
            return false;
    }, 'Data Inicial');

    $.validator.addMethod('validateDateOrPrazoFinal', function (value, ele) {
        var dataFinal = $("#Filter_Data_Final").val();

        if (value !== "")
            return true;
        else if (dataFinal !== "")
            return true;
        else
            return false;
    }, 'Data Final Obrigatório');

    var actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        var visivelEstornar = view.estornarSolicitacao && (full.Id_Status !== 24);
        var visivelConferir = view.conferirSolicitacao && (full.Id_Status === 22 || full.Id_Status === 23);

        return [
            {
                text: "Visualizar",
                color: "btn-info",
                attrs: { 'data-id': full.Id, 'action': 'visualizarSolicitacao' },
                icon: 'fa fa-eye',
                visible: view.detailsVisible
            },
            {
                text: "Estornar",
                color: "btn-danger",
                attrs: { 'data-id': full.Id, 'action': 'estornarSolicitacao' },
                icon: 'fa fa-pencil-square',
                visible: visivelEstornar
            },
            {
                text: "Conferir",
                color: "btn-primary",
                attrs: { 'data-id': full.Id, 'action': 'conferirSolicitacao' },
                icon: 'fa fa-check-square-o',
                visible: visivelConferir
            }
        ];
    });

    $("#dataTable").on('click', "[action='visualizarSolicitacao']", visualizarSolicitacao);
    $("#dataTable").on('click', "[action='estornarSolicitacao']", estornarSolicitacao);
    $("#dataTable").on('click', "[action='conferirSolicitacao']", conferirSolicitacao);

    $("#btImportarSolicitacao").click(function () {
        importarSolicitacao();
    });

    var $Data_Inicial = $('#Filter_Data_Inicial').closest('.date');
    var $Data_Final = $('#Filter_Data_Final').closest('.date');

    var createLinkedPickers = function () {
        var dataInicial = $Data_Inicial.datetimepicker({
            locale: moment.locale(),
            format: 'L',
            allowInputToggle: true
        });

        var dataFinal = $Data_Final.datetimepicker({
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
            { data: 'Cli_Cnpj', },
            { data: 'Id', },
            { data: 'Dt_Criacao', },
            { data: 'Tipo', },
            { data: 'Status', },
            actionsColumn
        ]
    });

    $('.btn-row-actions').tooltip();

    dart.dataTables.loadFormFilterEvents();
})();

function visualizarSolicitacao() {
    var id = $(this).data("id");
    let modal = $("#modalVisualizar");

    modal.load(CONTROLLER_PATH + "SolicitacaoVisualizar/" + id, function () {
        modal.modal();

        var botoes = ['selectAll', 'selectNone',
            {
                text: '<i class="fa fa-exchange"></i> Trocar Fornecedor',
                className: 'btn-success ',
                action: function (e, dt, node, config) {
                    var ids = $.map(this.rows('.selected').data(), function (item) {
                        return item[1].split(" - ")[0];
                    });
                    console.log(ids)

                    if (ids.length == 0) {
                        PNotify.error({ text: "Selecione um item." });
                    } else {
                        //criarAcao(ids.join());
                    }
                }
            },
            {
                text: '<i class="fa fa-qrcode"></i> Emitir Etiquetas',
                className: 'btn-primary ',
                action: function (e, dt, node, config) {
                    var ids = $.map(this.rows('.selected').data(), function (item) {
                        return item[1].split(" - ")[0];
                    });
                    console.log(ids)

                    if (ids.length == 0) {
                        PNotify.error({ text: "Selecione um item." });
                    } else {
                        //criarAcao(ids.join());
                    }
                }
            },
            {
                text: '<i class="fa fa-ticket"></i> N.F. de Compra',
                className: 'btn-success',
                action: function (e, dt, node, config) {
                    var ids = $.map(this.rows('.selected').data(), function (item) {
                        return item[1].split(" - ")[0];
                    });
                    console.log(ids)

                    if (ids.length == 0) {
                        PNotify.error({ text: "Selecione um item." });
                    } else {
                        //criarAcao(ids.join());
                    }
                }
            },
        ];

        $('#tbSolicitacaoItens').DataTable({
            destroy: true,
            serverSide: false,
            stateSave: false,
            columnDefs: [
                {
                    targets: [1],
                    visible: false
                },
                {
                    orderable: false,
                    className: 'select-checkbox',
                    targets: [0]
                },
            ],
            select: {
                style: 'os',
                selector: 'td:first-child'
            },
            dom: "Bfrtip",
            bInfo: true,
            buttons: botoes,
        });
    });
}

function conferirSolicitacao() {
    var id = $(this).data("id");
    let modal = $("#modalVisualizar");

    modal.load(CONTROLLER_PATH+"ConferenciaConferir", {
        Id: id,
        Tipo: 'solicitacao'
    }, function () {
        modal.modal();
    });
}

function importarSolicitacao() {
    let modal = $("#modalVisualizar");

    modal.load(CONTROLLER_PATH + "SolicitacaoImportar/", function () {
        modal.modal();
    });
}

function estornarSolicitacao() {
    var id = $(this).data("id");

    $.confirm({
        type: 'red',
        theme: 'material',
        title: 'Estornar Solicitação',
        content: 'Serão excluidos todos os dados da solicitação, e você terá que importar o documento novamente, continuar?',
        typeAnimated: true,
        autoClose: 'cancelar|10000',
        buttons: {
            confirmar: {
                text: 'Estornar',
                btnClass: 'btn-red',
                action: function () {
                    $.ajax({
                        url: HOST_URL + CONTROLLER_PATH + "SolicitacaoEstornar",
                        method: "POST",
                        data: { Id: id },
                        success: function (result) {
                            if (result.Success) {
                                location.reload();
                                PNotify.success({ text: result.Message, delay: 1000 });
                            } else {
                                PNotify.error({ text: result.Message });
                            }
                        },
                        error: function (request, status, error) {
                            PNotify.warning({ text: result.Message });
                        }
                    });
                }
            },
            cancelar: {
                text: 'Cancelar',
                action: function () {
                }
            }
        }
    });
}