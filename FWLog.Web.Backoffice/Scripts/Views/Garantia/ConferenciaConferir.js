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

    $('#modalLaudoDetalhes').on('hidden.bs.modal', function () {
        itensLaudo();
    });
})();

function conferir() {
    let div = $("#tabConferir");

    div.load("/Garantia/ConferenciaForm", {
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

        $("#btConferirManual").click(function () {
            conferirManual();
        });

        $("#btAtualizarItemRemessa").click(function () {
            atualizarItemRemessa();
        });

        onScan.attachTo($('#Form_Refx')[0], {
            onScan: function (sScancode, iQuatity) {
                console.log(sScancode);
                //TODO PROCESSAR LEITOR E PEGAR REFX
                $('#Form_Refx').val(sScancode);

                conferirItem();
            }
        });

        $('#Form_Refx').keypress(function (e) {
            if (e.which == 13) {
                e.preventDefault();
                conferirItem();
            }
        });
    });
}

function conferirItem() {
    $.ajax({
        url: "/Garantia/AtualizarItemConferencia",
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

function divergencia() {
    let div = $("#tabDivergencia");

    div.load("/Garantia/ConferenciaDivergencia", {
        Id_Conferencia: $("#Conferencia_Id").val()
    }, function () {
        $('#tbDivergenciaItens').DataTable({
            destroy: true,
            serverSide: false,
            stateSave: false,
            pageLength: 200,
            dom: "Bfrtip",
            buttons: [],
            searching: true,
            bInfo: true
        });
    });
}

function finalizarConferencia() {
    $.ajax({
        url: "/Garantia/ConferenciaVerificacaoFinalizar",
        method: "POST",
        data: { Id_Conferencia: $("#Conferencia_Id").val() },
        success: function (result) {
            if (result.Success) {
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
                                finalizarConferenciaConfirmar();
                            }
                        },
                        cancelar: {
                            text: 'Cancelar',
                            action: function () {
                            }
                        }
                    }
                });
            } else {
                PNotify.error({ text: result.Message });
            }
        },
        error: function (request, status, error) {
            PNotify.warning({ text: result.Message });
        }
    });
}

function finalizarConferenciaConfirmar() {
    $.confirm({
        type: 'red',
        theme: 'material',
        title: 'Finalizar',
        content: 'Tem Certeza?',
        typeAnimated: true,
        autoClose: 'cancelar|10000',
        buttons: {
            confirmar: {
                text: 'Finalizar',
                btnClass: 'btn-red',
                action: function () {
                    $.ajax({
                        url: "/Garantia/ConferenciaFinalizar",
                        method: "POST",
                        data: { Id_Conferencia: $("#Conferencia_Id").val() },
                        success: function (result) {
                            if (result.Success) {
                                $("#modalVisualizar").modal("hide");
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

function itensLaudo() {
    var id = 0;
    let modal = $("#modalLaudo .modal-body");
    $("#modalLaudo").modal("show");
    $("#modalLaudo .modal-title").html("Itens do Laudo");

    modal.load("/Garantia/ConferenciaLaudo", {
        Id_Conferencia: $("#Conferencia_Id").val()
    }, function () {
        $('.btn-row-actions').tooltip();

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
            pageLength: 200,
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

function itensLaudoDetalhe(refx) {
    var id = 0;
    let modal = $("#modalLaudoDetalhes .modal-body");
    $("#modalLaudoDetalhes .modal-title").html(refx + " - Itens");

    modal.load("/Garantia/ConferenciaLaudoDetalhe", {
        Id_Conferencia: $("#Conferencia_Id").val(),
        Refx: refx
    }, function () {
        $("#modalLaudoDetalhes").modal("show");
        $("#modalLaudo").modal("hide");
        $('#tbLaudoItensDetalhe').DataTable({
            destroy: true,
            serverSide: false,
            stateSave: false,
            pageLength: 200,
            dom: "Bfrtip",
            order: [],
            buttons: [],
            searching: true,
            bInfo: true
        });

        $('.onlyNumber').mask('0#');

        let quantMax = $("#formLaudoItensDetalhe #Quant_Max").val();
        if (quantMax <= 0)
            $("#divFormLaudoItensDetalhe").hide();

        $("#formLaudoItensDetalhe #Quant").val(quantMax);

        $('#formLaudoItensDetalhe').submit(function (e) {
            e.preventDefault();
            var formulario = $("form#formLaudoItensDetalhe").serialize();

            $.ajax({
                url: "/Garantia/ConferenciaLaudoDetalheGravar",
                method: "POST",
                data: formulario,
                success: function (result) {
                    if (result.Success) {
                        itensLaudoDetalhe(refx);
                        PNotify.success({ text: result.Message, delay: 1000 });
                    } else {
                        PNotify.error({ text: result.Message });
                    }
                },
                error: function (request, status, error) {
                    PNotify.warning({ text: result.Message });
                }
            });
        });
    });
}

function excluirLaudo(id) {
    let refx = $("#formLaudoItensDetalhe #Refx").val();

    $.confirm({
        type: 'red',
        theme: 'material',
        title: 'Excluir Laudo',
        content: 'Tem Certeza?',
        typeAnimated: true,
        autoClose: 'cancelar|10000',
        buttons: {
            confirmar: {
                text: 'Excluir',
                btnClass: 'btn-red',
                action: function () {
                    $.ajax({
                        url: "/Garantia/ConferenciaLaudoDetalheExcluir",
                        method: "POST",
                        data: { Id: id },
                        success: function (result) {
                            if (result.Success) {
                                itensLaudoDetalhe(refx);
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

function itensPendentes() {
    var id = 0;
    $("#modalItensPendentes").modal("hide");
    let modal = $("#modalItensPendentes .modal-body");
    $("#modalItensPendentes .modal-title").html("Itens Pendentes");

    modal.load("/Garantia/ConferenciaItemPendente", {
        Id_Conferencia: $("#Conferencia_Id").val()
    }, function () {
        $("#modalItensPendentes").modal("show");
        $('#tbItensPendentes').DataTable({
            destroy: true,
            serverSide: false,
            stateSave: false,
            pageLength: 200,
            dom: "Bfrtip",
            order: [],
            buttons: [],
            searching: true,
            bInfo: true
        });
    });
}

function conferirManual() {
    $("#modalItensPendentes").modal("hide");
    let modal = $("#modalItensPendentes .modal-body");
    $("#modalItensPendentes .modal-title").html("Conferência Manual");

    modal.load("/Garantia/ConferenciaConferirManual", {
        Id_Conferencia: $("#Conferencia_Id").val()
    }, function () {
        $("#modalItensPendentes").modal("show");

        $('.select2').select2();

        $('.onlyNumber').mask("Z0999999.00", {
            translation: {
                '0': { pattern: /\d/ },
                '9': { pattern: /\d/, optional: true },
                'Z': { pattern: /[\-\+]/, optional: true }
            }
        });

        $('#formConferirManual').submit(function (e) {
            e.preventDefault();
            conferirItemManual();
        });

        $("#formConferirManual #Refx").change(function () {
            var $select = $("#formConferirManual #Id_Solicitacao");
            $select.empty().select2();

            $.ajax({
                url: "/Garantia/ConferenciaListarRemessaSolicitacaoAjax",
                type: "GET",
                data: {
                    Id_Conferencia: $("#Conferencia_Id").val(),
                    Refx: $(this).val()
                },
                success: function (data) {
                    $select.select2("destroy");

                    $select.append("<option value=''></option>");
                    $.each(data, function (obj) {
                        $select.append("<option  value=" + data[obj].Id_Solicitacao + ">" + data[obj].Id_Solicitacao + "</option>");
                    });
                    $select.select2();
                },
                error: function (data) {
                }
            });
        });
    });
}

function conferirItemManual() {
    var formulario = $("form#formConferirManual").serialize();

    $.ajax({
        url: "/Garantia/AtualizarItemConferencia",
        method: "POST",
        data: formulario,
        success: function (result) {
            if (result.Success) {
                $("#modalItensPendentes").modal('hide');
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

function itensConferidos() {
    var id = 0;
    $("#modalItensPendentes").modal("hide");
    let modal = $("#modalItensPendentes .modal-body");
    $("#modalItensPendentes .modal-title").html("Peças Conferidas");

    modal.load("/Garantia/ConferenciaItemConferido", {
        Id_Conferencia: $("#Conferencia_Id").val()
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

function alterarQuantidade() {
    $.confirm({
        title: 'Alterar Quantidade',
        content: '' +
            '<form action="" class="formName">' +
            '<div class="form-group">' +
            '<label>Digite a Quantidade</label>' +
            '<input type="text" placeholder="Quantidade" class="qtd form-control onlyNumber" required />' +
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
            $('.onlyNumber').mask("Z0999999.00", {
                translation: {
                    '0': { pattern: /\d/ },
                    '9': { pattern: /\d/, optional: true },
                    'Z': { pattern: /[\-\+]/, optional: true }
                }
            });
            this.$content.find('.qtd').focus();
            this.$content.find('form').on('submit', function (e) {
                e.preventDefault();
                jc.$$gravar.trigger('click');
            });
        }
    });
}

function atualizarItemRemessa() {
    $.ajax({
        url: "/Garantia/RemessaAtualizarItemGravar",
        method: "POST",
        data: {
            Id_Conferencia: $("#Conferencia_Id").val()
        },
        success: function (result) {
            if (result.Success) {
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