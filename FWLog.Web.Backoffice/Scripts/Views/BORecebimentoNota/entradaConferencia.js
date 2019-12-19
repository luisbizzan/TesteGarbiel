(function () {
    $('.onlyNumber').mask('0#');

    $('#modalRegistroConferencia').keypress(function (e) {
        if (e.keyCode == '13') {
            registrarConferencia();
        }
    });

    $("#Referencia").blur(function () {
        if ($(this).val() == '') {
            waitingDialog.hide();
            $("#Referencia").focus();
        }
        else {
            carregarDadosReferenciaConferencia();
        }
    });

    $("#confirmarConferencia").click(function () {
        registrarConferencia();
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

                $("#DescricaoReferencia").val(model.DescricaoReferencia);
                $("#Embalagem").val(model.Embalagem);
                $("#Unidade").val(model.Unidade);
                $("#Multiplo").val(model.Multiplo);
                $("#QuantidadeEstoque").val(model.QuantidadeEstoque);
                $("#Localizacao").val(model.Localizacao);
                $("#QuantidadeNaoConferida").val(model.QuantidadeNaoConferida);
                $("#QuantidadeConferida").val(model.QuantidadeConferida);
                $("#InicioConferencia").val(model.InicioConferencia);

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
    let inicioConferencia = $("#InicioConferencia").val();

    if (quantidadePorCaixa == '')
        quantidadePorCaixa = 0;

    if (quantidadeCaixa == '')
        quantidadeCaixa = 0;


    $.ajax({
        url: HOST_URL + CONTROLLER_PATH + "RegistrarConferencia",
        cache: false,
        method: "POST",
        data: {
            idLote: view_modal.idLote,
            codigoBarrasOuReferencia: referencia,
            quantidadePorCaixa: quantidadePorCaixa,
            quantidadeCaixa: quantidadeCaixa,
            inicioConferencia: inicioConferencia
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