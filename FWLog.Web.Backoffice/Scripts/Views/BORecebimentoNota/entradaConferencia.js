(function () {
    $('.onlyNumber').mask('0#');

    $("#QuantidadePorCaixa").mask("S#############", {
        translation: {
            'S': {
                pattern: /-/,
                optional: true
            }
        }
    });

    $('#modalRegistroConferencia').keypress(function (e) {
        if (e.keyCode == '13') {
            validarDiferencaMultiploConferencia();
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
        validarDiferencaMultiploConferencia();
    });

    $("#finalizarConferencia").click(function () {
        confirmarfinalizarConferencia();
    });

    $("#exibirDivergencia").click(function () {
        exibirDivergencia();
    });

    $("#validarAcessoCoordenador").click(function () {
        let usuario = $("#Usuario").val();
        let senha = $("#Senha").val();

        $.ajax({
            url: HOST_URL + CONTROLLER_PATH + "ValidarAcessoCoordenadorConferencia",
            cache: false,
            method: "POST",
            data: {
                usuario: usuario,
                senha: senha
            },
            success: function (result) {
                if (result.Success) {
                    let referencia = $("#Referencia").val();
                    let multiplo = $("#Multiplo").val();
                    let quantidadePorCaixa = $("#QuantidadePorCaixa").val();
                    let quantidadeCaixa = $("#QuantidadeCaixa").val();
                    let inicioConferencia = $("#InicioConferencia").val();

                    registrarConferencia(referencia, quantidadePorCaixa, quantidadeCaixa, inicioConferencia, multiplo);

                    $('#modalAcessoCoordenador').modal('toggle');

                    $("#Usuario").val('');
                    $("#Senha").val('');

                    setTimeout(function () {
                        $("#Referencia").focus();
                    }, 150);
                }
                else {
                    PNotify.info({ text: result.Message });
                }
            },
            error: function (request, status, error) {
                PNotify.warning({ text: result.Message });
            }
        });
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
                $("#Multiplo").focus();

            } else {
                PNotify.info({ text: result.Message });

                waitingDialog.hide();
                $("#Referencia").focus();
            }
        }
    });
}

function validarDiferencaMultiploConferencia() {
    let referencia = $("#Referencia").val();
    let multiplo = $("#Multiplo").val();
    let quantidadePorCaixa = $("#QuantidadePorCaixa").val();
    let quantidadeCaixa = $("#QuantidadeCaixa").val();
    let inicioConferencia = $("#InicioConferencia").val();

    $.ajax({
        url: HOST_URL + CONTROLLER_PATH + "VerificarDiferencaMultiploConferencia",
        cache: false,
        method: "POST",
        data: {
            codigoBarrasOuReferencia: referencia,
            multiplo: multiplo
        },
        success: function (result) {
            if (result.Success) {
                waitingDialog.hide();

                $('#modalAcessoCoordenador').modal('show');

                setTimeout(function () {
                    $("#Usuario").focus();
                }, 150);
            } else {
                registrarConferencia(referencia, quantidadePorCaixa, quantidadeCaixa, inicioConferencia, multiplo);
            }
        },
        error: function (request, status, error) {
            PNotify.info({ text: request.responseText });
        }
    });
}

function registrarConferencia(referencia, quantidadePorCaixa, quantidadeCaixa, inicioConferencia, multiplo) {
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
            quantidadeCaixa: quantidadeCaixa,
            inicioConferencia: inicioConferencia,
            multiplo: multiplo
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
    $(".close").click();
    var $modal = $("#modalConferencia");
    $modal.load(HOST_URL + CONTROLLER_PATH + "ResumoFinalizarConferencia/" + view_modal.idLote, function () {
        $modal.modal();
    });
}

function exibirDivergencia() {
    let $modal = $("#modalConferencia");

    $(".close").click();

    $modal.load(HOST_URL + CONTROLLER_PATH + "ResumoDivergenciaConferencia/" + view_modal.idLote, function () {
        $modal.modal();
    });
}

function finalizarConferencia() {
    $(".close").click();
    waitingDialog.hide();
    $("#dataTable").DataTable().ajax.reload();
}

function resetarCamposConferencia() {
    $("#Referencia").val('');
    $("#DescricaoReferencia").val('');
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