(function () {
    let permiteRegistrar = false;
    let $referencia = $("#Referencia");
    var $quantidadePorCaixa = $("#QuantidadePorCaixa");
    var $quantidadeCaixa = $("#QuantidadeCaixa");

    $('.onlyNumber').mask('0#');

    $quantidadePorCaixa.mask("S#############", {
        translation: {
            'S': {
                pattern: /-/,
                optional: true
            }
        }
    });

    $quantidadePorCaixa.on('keydown', function (e) {
        if (e.keyCode == 9 && !e.target.value) {
            return false;
        }
    });

    $quantidadeCaixa.on('keydown', function (e) {
        if (e.keyCode == 9) {
            if (!e.target.value) {
            } else {
                $referencia.focus();
            }

            return false;
        }
    });

    $('#modalRegistroConferencia').keydown(function (e) {
        if (e.keyCode == 13 && permiteRegistrar) {
            registrarConferencia();
        }
    });

    $referencia.focus(function () {
        permiteRegistrar = false;
    });

    $referencia.on('keypress keydown', function (e) {
        if (e.keyCode == 13) {
            if (!e.target.value) {
                PNotify.info({ text: 'Referência está vazio. Por favor, entre com um valor!' });

                $referencia.focus();

                return false;
            } else {
                carregarDadosReferenciaConferencia();
            }
        } else {
            resetarCamposConferencia(false);
        }
    });

    $referencia.blur(function () {
        if (!$(this).val()) {
            $referencia.focus();
        }
        else {
            carregarDadosReferenciaConferencia();
        }
    });

    $("#confirmarConferencia").click(function () {
        registrarConferencia();
    });

    $("#finalizarConferencia").click(function () {
        confirmarfinalizarConferencia();
    });

    $("#exibirDivergencia").click(function () {
        exibirDivergencia();
    });

    function carregarDadosReferenciaConferencia() {
        let referencia = $referencia.val();

        $("#QuantidadePorCaixa").focus();

        overlay(true);

        $.ajax({
            url: HOST_URL + CONTROLLER_PATH + "ObterDadosReferenciaConferencia",
            global: false,
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

                    overlay(false);

                    $("#QuantidadePorCaixa").focus();

                    permiteRegistrar = true;
                } else {
                    PNotify.info({ text: result.Message });

                    overlay(false);

                    $referencia.focus();

                    permiteRegistrar = false;
                }
            },
            error: function () {
                PNotify.info({ text: 'Não foi possível obter dados. Por favor, tente novamente!' });

                overlay(false);

                $referencia.focus();

                permiteRegistrar = false;
            }
        });
    }

    function registrarConferencia() {
        let referencia = $("#Referencia").val();
        let quantidadePorCaixa = $("#QuantidadePorCaixa").val();
        let quantidadeCaixa = $("#QuantidadeCaixa").val();
        let inicioConferencia = $("#InicioConferencia").val();

        overlay(true);

        if (quantidadePorCaixa === '')
            quantidadePorCaixa = 0;

        if (quantidadeCaixa === '')
            quantidadeCaixa = 0;

        $.ajax({
            url: HOST_URL + CONTROLLER_PATH + "RegistrarConferencia",
            global: false,
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

                    permiteRegistrar = false;

                    overlay(false);

                    $referencia.focus();

                } else {
                    PNotify.info({ text: result.Message });

                    overlay(false);

                    permiteRegistrar = true;
                }
            }
        });
    }

    function overlay(show) {
        var $overlay = $('#overlay');

        if (!!show) {
            $referencia.attr('disabled', true);
            $overlay.show();
        } else {
            $referencia.attr('disabled', false);
            $overlay.hide();
        }
    }
})();

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

function resetarCamposConferencia(limpaReferencia = true) {
    if (!!limpaReferencia) {
        $("#Referencia").val('');
    }

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