(function () {
    conferir();

    $("#conferir-tab").click(function () {
        conferir();
    });

    $("#divergencia-tab").click(function () {
        divergencia();
    });

    $("#btFinalizarConferencia").click(function () {
        finalizarConferencia();
    });
})();

function conferir() {
    let div = $("#tabConferir");

    div.load(CONTROLLER_PATH + "ConferenciaForm", {
        Id_Conferencia: $("#Conferencia_Id").val(),
    }, function () {
        $("#Form_Quant").val("1");

        $("#btItensPendentes").click(function () {
            itensPendentes();
        });

        $("#btItensConferidos").click(function () {
            itensConferidos();
        });

        $("#btAlterarQuantidade").click(function () {
            alterarQuantidade();
        });

        $('#Form_Refx').keypress(function (e) {
            if (e.which == 13) {
                e.preventDefault();

                $.ajax({
                    url: HOST_URL + CONTROLLER_PATH + "AtualizarItemConferencia",
                    method: "POST",
                    data: {
                        Id_Conf: $("#Conferencia_Id").val(),
                        Refx: $("#Form_Refx").val(),
                        Quant_Conferida: $("#Form_Quant").val(),
                    },
                    success: function (result) {
                        if (result.Success) {
                            $("#Form_Quant").val("1");
                            $("#Form_Refx").val("");
                            PNotify.success({ text: result.Message });
                        } else {
                            PNotify.error({ text: result.Message });
                        }
                    },
                    error: function (request, status, error) {
                        PNotify.warning({ text: result.Message });
                    }
                });
            }
        });
    });
}

function divergencia() {
    let div = $("#tabDivergencia");

    div.load(CONTROLLER_PATH + "ConferenciaDivergencia", {
        Id_Conferencia: $("#Conferencia_Id").val()
    }, function () {
        $('#tbDivergenciaItens').DataTable({
            destroy: true,
            serverSide: false,
            stateSave: false,
            dom: "Bfrtip",
            buttons: [],
            searching: true,
            bInfo: true
        });
    });
}

function finalizarConferencia() {
    $.confirm({
        type: 'red',
        theme: 'material',
        title: 'Finalização da Conferência',
        content: 'Será finalizado a conferência e após este procedimento será abastecido o(s) estoque(s) e gerado(s) devido(s) documentos.',
        typeAnimated: true,
        autoClose: 'cancelar|10000',
        buttons: {
            laudo: {
                text: 'Lançar Itens de Laudo',
                btnClass: 'btn-blue',
                action: function () {
                    itensLaudo();
                }
            },
            finalizar: {
                text: 'Finalizar',
                btnClass: 'btn-red',
                action: function () {
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

function itensLaudo() {
    var id = 0;
    let modal = $("#modalLaudo .modal-body");
    $("#modalLaudo .modal-title").html("Itens do Laudo");

    modal.load(CONTROLLER_PATH + "ConferenciaLaudo/" + id, function () {
        $("#modalLaudo").modal("show");

        var botoes = ['selectAll', 'selectNone',
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
        ];

        $('#tbLaudoItens').DataTable({
            destroy: true,
            serverSide: false,
            stateSave: false,
            columnDefs: [
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

function itensLaudoDetalhe() {
    var id = 0;
    let modal = $("#modalLaudoDetalhes .modal-body");
    $("#modalLaudoDetalhes .modal-title").html("zzz - Itens");

    modal.load(CONTROLLER_PATH + "ConferenciaLaudoDetalhe/" + id, function () {
        $("#modalLaudoDetalhes").modal("show");
    });
}

function itensPendentes() {
    var id = 0;
    $("#modalItensPendentes").modal("hide");
    let modal = $("#modalItensPendentes .modal-body");
    $("#modalItensPendentes .modal-title").html("Itens Pendentes");

    modal.load(CONTROLLER_PATH + "ConferenciaItensPendentes/" + id, function () {
        $("#modalItensPendentes").modal("show");
    });
}

function itensConferidos() {
    var id = 0;
    $("#modalItensPendentes").modal("hide");
    let modal = $("#modalItensPendentes .modal-body");
    $("#modalItensPendentes .modal-title").html("Itens Conferidos");

    modal.load(CONTROLLER_PATH + "ConferenciaItensConferidos/" + id, function () {
        $("#modalItensPendentes").modal("show");
    });
}

function alterarQuantidade() {
    $.confirm({
        title: 'Alterar Quantidade',
        content: '' +
            '<form action="" class="formName">' +
            '<div class="form-group">' +
            '<label>Digite a Quantidade</label>' +
            '<input type="text" placeholder="Quantidade" class="qtd form-control" required />' +
            '</div>' +
            '</form>',
        buttons: {
            gravar: {
                text: 'Gravar',
                btnClass: 'btn-blue',
                action: function () {
                    var qtd = this.$content.find('.qtd').val();
                    if (!qtd) {
                        $.alert('Digite uma quantidade valida.');
                        return false;
                    }
                    $("#Form_Quant").val(qtd);
                }
            },
            cancelar: {
                text: 'Cancelar',
                action: function () {
                }
            }
        },
        onContentReady: function () {
            var jc = this;
            this.$content.find('.qtd').focus();
            this.$content.find('form').on('submit', function (e) {
                e.preventDefault();
                jc.$$gravar.trigger('click');
            });
        }
    });
}