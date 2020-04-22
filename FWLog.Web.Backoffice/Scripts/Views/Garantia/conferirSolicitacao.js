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

function conferir() {
    var id = $("#Solicitacao_Id").val();
    let div = $("#tabConferir");

    div.load(CONTROLLER_PATH + "ConferenciaForm/" + id, function () {
        $("#btFinalizarConferencia").click(function () {
            finalizarConferencia();
        });

        $("#btItensPendentes").click(function () {
            itensPendentes();
        });
    });
}

function divergencia() {
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
    let modal = $("#modalItensPendentes .modal-body");
    $("#modalItensPendentes .modal-title").html("Itens Pendentes");

    modal.load(CONTROLLER_PATH + "ConferenciaItensPendentes/" + id, function () {
        $("#modalItensPendentes").modal("show");
    });
}