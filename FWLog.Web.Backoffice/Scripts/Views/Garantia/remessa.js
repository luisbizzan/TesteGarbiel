document.addEventListener("DOMContentLoaded", function (event) {
    validarRemessaAutomatica();
});

(function () {
    $('#modalVisualizar').on('hidden.bs.modal', function () {
        location.reload();
    });

    $.validator.setDefaults({ ignore: null });
    $('.onlyNumber').mask('0#');
    $("dateFormat").mask("99/99/9999");
    $('.hourMinute').mask("99:99", { reverse: true });

    $("#btCriarRemessa").click(function () {
        criarRemessa();
    });

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
        var visivelEstornar = view.estornarRemessa && (full.Id_Status === 38) && full.Do_Usr_Logado == 1;
        var visivelVisualizar = view.listarRemessa;
        var visivelConferir = view.conferirRemessa && (full.Id_Status === 38) && full.Do_Usr_Logado == 1;

        return [
            {
                text: "Visualizar",
                color: "btn-info",
                attrs: { 'data-id': full.Id, 'action': 'visualizarRemessa' },
                icon: 'fa fa-eye',
                visible: visivelVisualizar
            },
            {
                text: "Estornar",
                color: "btn-danger",
                attrs: { 'data-id': full.Id, 'action': 'estornarRemessa' },
                icon: 'fa fa-pencil-square',
                visible: visivelEstornar
            },

            {
                text: "Conferir",
                color: "btn-primary",
                attrs: { 'data-id': full.Id, 'action': 'conferirRemessa' },
                icon: 'fa fa-check-square-o',
                visible: visivelConferir
            }
        ];
    });

    $("#dataTable").on('click', "[action='visualizarRemessa']", visualizarRemessa);
    $("#dataTable").on('click', "[action='conferirRemessa']", conferirRemessa);
    $("#dataTable").on('click', "[action='estornarRemessa']", estornarRemessa);

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
            { data: 'Cod_Fornecedor', },
            { data: 'Id', },
            { data: 'Dt_Criacao', },
            { data: 'Status', },
            actionsColumn
        ]
    });

    $('.btn-row-actions').tooltip();

    dart.dataTables.loadFormFilterEvents();
})();

function validarRemessaAutomatica() {
    $.ajax({
        url: "/Garantia/RemessaAutomaticaCriarUsr",
        method: "POST",
        success: function (result) {
            console.log(result);
            if (!result.Success) {
                if (result.DataObject.Id_Remessa == 0) {
                    $.confirm({
                        type: 'red',
                        theme: 'material',
                        closeIcon: function () {
                            return false;
                        },
                        title: 'Remessa Automática',
                        content: result.Message,
                        typeAnimated: true,

                        buttons: {
                            confirmar: {
                                text: 'Criar Conferência',
                                btnClass: 'btn-red',
                                action: function () {
                                    $.ajax({
                                        url: "/Garantia/RemessaCriarGravar",
                                        method: "POST",
                                        data: { Cod_Fornecedor: result.DataObject.Cod_Fornecedor },
                                        success: function (result2) {
                                            if (result2.Success) {
                                                //ABRIR CONFERENCIA DA REMESSA
                                                let modal = $("#modalVisualizar");
                                                modal.load("ConferenciaConferir", {
                                                    Id: result2.Data,
                                                    Tipo: 'remessa'
                                                }, function () {
                                                    modal.modal();
                                                });
                                                PNotify.success({ text: result2.Message });
                                            } else {
                                                PNotify.error({ text: result2.Message });
                                            }
                                        },
                                        error: function (request, status, error) {
                                            PNotify.warning({ text: result2.Message });
                                        }
                                    });
                                }
                            }
                        }
                    });
                } else {
                    //ABRIR CONFERENCIA DA REMESSA
                    let modal = $("#modalVisualizar");
                    modal.load("ConferenciaConferir", {
                        Id: result.DataObject.Id_Remessa,
                        Tipo: 'remessa'
                    }, function () {
                        modal.modal();
                    });
                }
            }
        },
        error: function (request, status, error) {
            PNotify.warning({ text: result.Message });
        }
    });
}

function visualizarRemessa() {
    var id = $(this).data("id");
    let modal = $("#modalVisualizar");

    modal.load("/Garantia/RemessaVisualizar/" + id, function () {
        modal.modal();
        $("#modalVisualizar .modal-title").html("Detalhes da Remessa");

        var botoes = ['selectAll', 'selectNone',
            {
                text: '<i class="fa fa-list"></i> Detalhado',
                className: 'btn-success ',
                action: function (e, dt, node, config) {
                    visualizarRemessaDetalhado(id);
                }
            },
            {
                text: '<i class="fa fa-gears"></i> Peças Conferidas',
                className: 'btn-primary ',
                action: function (e, dt, node, config) {
                    visualizarRemessaItensConferidos(id);
                }
            },

        ];

        $('#tbRemessaItens').DataTable({
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

function visualizarRemessaItensConferidos(id) {
    $("#modalItensPendentes").modal("hide");
    let modal = $("#modalItensPendentes .modal-body");
    $("#modalItensPendentes .modal-title").html("Peças Conferidas");

    modal.load("/Garantia/ConferenciaItemConferidoRemessa", {
        Id_Remessa: id
    }, function () {
        $("#modalItensPendentes").modal("show");
        $('#tbItensPendentes').DataTable({
            destroy: true,
            serverSide: false,
            stateSave: false,
            dom: "Bfrtip",
            order: [],
            buttons: [],
            searching: true,
            bInfo: true
        });
    });
}

function visualizarRemessaDetalhado(id) {
    $("#modalItensPendentes").modal("hide");
    let modal = $("#modalItensPendentes .modal-body");
    $("#modalItensPendentes .modal-title").html("Itens Conferidos");

    modal.load("/Garantia/RemessaDetalhadoVisualizar", {
        Id: id
    }, function () {
        $("#modalItensPendentes").modal("show");
        $("#modalItensPendentes .modal-title").html("Remessa Detalhado");

        var botoes = ['selectAll', 'selectNone',
            {
                text: '<i class="fa fa-qrcode"></i> Emitir Etiquetas',
                className: 'btn-primary ',
                action: function (e, dt, node, config) {
                    var ids = $.map(this.rows('.selected').data(), function (item) {
                        var s = item[2].split(" - ")[0] + ";" + item[4].split(" - ")[0];
                        return s;
                    });

                    if (ids.length == 0) {
                        PNotify.error({ text: "Selecione um item." });
                    } else {
                        /* # Imprimir Etiqueta # */
                        IdsSelecionados = new Object();
                        IdsSelecionados = ids;
                        listarImpressoras();
                    }
                }
            },
        ];

        $('#tbRemessaItensDetalhado').DataTable({
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

function conferirRemessa() {
    var id = $(this).data("id");
    let modal = $("#modalVisualizar");

    modal.load("ConferenciaConferir", {
        Id: id,
        Tipo: 'remessa'
    }, function () {
        modal.modal();
    });
}

function criarRemessa() {
    let modal = $("#modalVisualizar");

    modal.load("RemessaCriar/", function () {
        modal.modal();
    });
}

function estornarRemessa(idParam) {
    var id = typeof idParam === 'undefined' ? $(this).data("id") : idParam;
    console.log(id);

    $.confirm({
        type: 'red',
        theme: 'material',
        title: 'Estornar Remessa',
        content: 'Serão excluidos todos os dados da remessa, e você terá que cria-la novamente, continuar?',
        typeAnimated: true,
        autoClose: 'cancelar|10000',
        buttons: {
            confirmar: {
                text: 'Estornar',
                btnClass: 'btn-red',
                action: function () {
                    $.ajax({
                        url: "RemessaEstornar/",
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

/* [GENÉRICO] mostrar mensagens */
function Mensagem(sucesso, mensagem) {
    if (sucesso) {
        PNotify.success({ text: mensagem, delay: 1000 });
    }
    else {
        PNotify.error({ text: mensagem, delay: 5000 });
    }
}

/* Selecionar Impressora */
var Impressora = new Object();
$(function () {
    $(document).ready(function () {
        Impressora = new Object();
        var ddlImpressoras = $('#ddlPerfilImpressora option:selected')[0];
        var ddlEmpresa = $('#ddlEmpresa option:selected')[0];
        Impressora.IdPerfilImpressora = ddlImpressoras.value;
        Impressora.IdEmpresa = ddlEmpresa.value;
    });

    $('#ddlPerfilImpressora').on('change', function () {
        Impressora = new Object();
        var ddlImpressoras = $('#ddlPerfilImpressora option:selected')[0];
        var ddlEmpresa = $('#ddlEmpresa option:selected')[0];
        Impressora.IdPerfilImpressora = ddlImpressoras.value;
        Impressora.IdEmpresa = ddlEmpresa.value;
    })


})

/* Listar Impressoras do Usuario  */
function listarImpressoras() {
    $.post("/GarantiaEtiqueta/ImpressoraUsuario", { IdEmpresa: Impressora.IdEmpresa, IdPerfilImpressora: Impressora.IdPerfilImpressora }, function (s) {
        $("#ddlImpressorasUsuario").empty();
        if (s.Success) {
            $("#ddlImpressorasUsuario").html(s.Data);
            if (s.RecordQuantity == 1) {
                Imprimir();
            }
            else {
                $("#modalImprimir").modal("show");
            }
        }
    }).fail(function (f) {
        console.log(f);
    }).done(function (d) {
    });
}

/*  Imprimiri Registros  */
var IdsSelecionados = new Object();
function Imprimir() {
    var ddlImpressoraSelecionada = $('#ddlImpressorasUsuario option:selected')[0];

    var registro = new Object();
    registro.Impressora = ddlImpressoraSelecionada.value;
    registro.EtiquetaImpressaoIds = IdsSelecionados;

    $.post("/GarantiaEtiqueta/ProcessarImpressaoEtiqueta", { EtiquetaImpressao: registro }, function (s) {
        Mensagem(s.Success, s.Message);
        if (s.Success) {
            location.reload();
        }
    }).fail(function (f) {
        console.log(f);
    }).done(function (d) {
    });

}
