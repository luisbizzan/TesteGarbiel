(function () {
    conferir();

    $("#conferir-tab").click(function () {
        conferir();
    });

    $("#divergencia-tab").click(function () {
        if (!$(this).parent().hasClass("disabled")) {
            divergencia();           
        }
    });
})();

$("body").keydown(function (e) {
    var keyCode = e.keyCode || e.which;
    //console.log(keyCode);
    if ($("#tabEscolhida").val() == "1") {
        if (keyCode == 115) {
            e.preventDefault();
            if ($("body .jconfirm").length == 0) {
                alterarQuantidade();
            }
        } else if (keyCode == 114) {
            e.preventDefault();
            itensPendentes();
        }
    }
});

function conferir() {
    $("#tabEscolhida").val("1");
    var id = $("#Solicitacao_Id").val();
    let div = $("#tabConferir");

    div.load(CONTROLLER_PATH + "ConferenciaForm/" + id, function () {

        $("#btFinalizarConferencia").click(function () {
            finalizarConferencia();
        });

        $("#btItensPendentes").click(function () {
            itensPendentes();
        });

        $("#btAlterarQuantidade").click(function () {
            alterarQuantidade();
        });

        $('#Form_Refx').keypress(function (e) {
            if (e.which == 13) {
                e.preventDefault();
                $("#Form_Quantidade").val("1");
            }
        });
    });
}

function divergencia() {
    $("#tabEscolhida").val("2");
    var id = $("#Solicitacao_Id").val();
    let div = $("#tabDivergencia");

    div.load(CONTROLLER_PATH + "ConferenciaDivergencia/" + id, function () {
        $("#btFinalizarDivergencia").click(function () {
            //ajax para verificar se esta certo

            //se tiver certo mostra msg de laudo
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
        });
    });
}

function finalizarConferencia() {
    $("#divergencia-tab").parent().removeClass("disabled");
    $("#divergencia-tab").attr("data-toggle", "tab");

    //ajax para verificar se esta certo e retornar a quantida de divergencias
    $.alert({
        type: 'red',
        typeAnimated: true,
        theme: 'material',
        title: 'Atenção!',
        content: 'Foram encontradas <strong>50</strong> divergência(s), verifique na etapa 2!',
    });
    $("#divergencia-tab").click();
}

function itensLaudo() {
    var id = 0;
    let modal = $("#modalLaudo .modal-body");
    $("#modalLaudo .modal-title").html("Itens do Laudo");

    modal.load(CONTROLLER_PATH + "ConferenciaLaudo/" + id, function () {
        $("#modalLaudo").modal("show");
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
                    $("#Form_Quantidade").val(qtd);
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