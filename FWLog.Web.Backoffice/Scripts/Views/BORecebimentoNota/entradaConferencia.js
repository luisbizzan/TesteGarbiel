(function () {
    $('#modalConferencia').keypress(function (e) {
        if (e.keyCode == '13') {
            registrarConferencia();
        }
    });

    $("#Referencia").blur(function () {
        carregarDadosReferenciaConferencia();
    });

    $("#confirmarConferencia").click(function () {
        registrarConferencia();
    });

    $("#finalizarConferencia").click(function () {
        confirmarfinalizarConferencia();
    });
})();

function carregarDadosReferenciaConferencia() {
    let referencia = $("#Referencia").val();
   
    $.ajax({
        url: HOST_URL + CONTROLLER_PATH + "ObterDadosReferenciaConferencia",
        cache: false,
        method: "POST",
        data: {
            idLote: view_modal.idLote,
            codigoBarrasOuReferencia: referencia
        },
        success: function (result) {
            if (result.Success) {
                var model = JSON.parse(result.Data);

                $("#Embalagem").val(model.Embalagem);
                $("#Unidade").val(model.Unidade);
                $("#Multiplo").val(model.Multiplo);
                $("#QuantidadeEstoque").val(model.QuantidadeEstoque);
                $("#Localizacao").val(model.Localizacao);
                $("#QuantidadeNaoConferida").val(model.QuantidadeNaoConferida);
                $("#QuantidadeConferida").val(model.QuantidadeConferida);

                if (model.EnviarPicking) {
                    $("#msgEnviarPicking").removeClass("hidden");
                }

                waitingDialog.hide();
                $("#QuantidadePorCaixa").focus();

            } else {
                PNotify.info({ text: result.Message });

                waitingDialog.hide();
                $("#Referencia").focus();
            }
        }
    });
}

function registrarConferencia() {
    let referencia = $("#Referencia").val();
    let quantidadePorCaixa = $("#QuantidadePorCaixa").val();
    let quantidadeCaixa = $("#QuantidadeCaixa").val();

    if (quantidadePorCaixa === '')
        quantidadePorCaixa = 0;

    if (quantidadeCaixa === '')
        quantidadeCaixa = 0;

    $.ajax({
        url: HOST_URL + CONTROLLER_PATH + "RegistrarConferencia",
        cache: false,
        method: "POST",
        data: {
            idLote: view_modal.idLote,
            codigoBarrasOuReferencia: referencia,
            quantidadePorCaixa: quantidadePorCaixa,
            quantidadeCaixa: quantidadeCaixa
        },
        success: function (result) {
            if (result.Success) {

                PNotify.info({ text: result.Message });

                resetarCamposConferencia();

                if (!$('#msgEnviarPicking').hasClass('hidden')) {
                    $("#msgEnviarPicking").addClass("hidden");
                }

                waitingDialog.hide();
                $("#Referencia").focus();

            } else {
                PNotify.info({ text: result.Message });
            }
        }
    });
}

function confirmarfinalizarConferencia() {

    dart.modalAjaxConfirm.open({
        title: 'Lote',
        message: "Deseja realmente finalizar a conferência do lote?",  
        url: HOST_URL + CONTROLLER_PATH + "FinalizarConferencia/" + view_modal.idLote,
        onConfirm: finalizarConferencia
    });   
}

function finalizarConferencia() {
    $(".close").click();
    waitingDialog.hide();
    $("#dataTable").DataTable().ajax.reload();
}

function resetarCamposConferencia() {
    $("#Referencia").val('');
    $("#Embalagem").val('');
    $("#Unidade").val('');
    $("#Multiplo").val('');
    $("#QuantidadePorCaixa").val('');
    $("#QuantidadeCaixa").val('');
    $("#QuantidadeEstoque").val('');
    $("#Localizacao").val('');
    $("#QuantidadeNaoConferida").val('');
    $("#QuantidadeConferida").val('');
}