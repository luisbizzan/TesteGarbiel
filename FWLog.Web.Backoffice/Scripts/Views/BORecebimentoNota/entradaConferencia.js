﻿(function () {
    let permiteRegistrar = false;

    let listaReferencia = new Array;

    let $confirmarConferencia = $("#confirmarConferencia");
    let $confirmarRegistroConferencia = $("#confirmarRegistroConferencia");
    let $exibirDivergencia = $("#exibirDivergencia");
    let $finalizarConferencia = $("#finalizarConferencia");
    let $modalConferencia = $("#modalConferencia");
    let $multiplo = $("#Multiplo");
    let $quantidadeCaixa = $("#QuantidadeCaixa");
    let $quantidadePorCaixa = $("#QuantidadePorCaixa");
    let $referencia = $("#Referencia");
    let $tipoConferencia = $("#TipoConferencia");
    let $validarAcessoCoordenador = $("#validarAcessoCoordenador");
    let $validarAcessoCoordenadorTipoConferencia = $("#validarAcessoCoordenadorTipoConferencia");

    $('.onlyNumber').mask('0#');

    $quantidadePorCaixa.mask("S#############", {
        translation: {
            'S': {
                pattern: /-/,
                optional: true
            }
        }
    });

    function adicionaEventos() {
        $modalConferencia.on('hidden.bs.modal', removeEventos);

        //Captura todas as teclas da tela.
        $(document).on('keydown', document_Keydown);

        $confirmarConferencia.on('click', confirmarConferencia_Click);

        $confirmarRegistroConferencia.on('click', confirmarRegistroConferencia_Click);

        $exibirDivergencia.on('click', exibirDivergencia);

        $referencia.on('blur', referencia_Blur);
        $referencia.on('focus', referencia_Focus);
        $referencia.on('keydown', referencia_Keydown);

        $finalizarConferencia.on('click', confirmarfinalizarConferencia);

        $multiplo.on('keydown', multiplo_Keydown);

        $quantidadeCaixa.on('keydown', quantidadeCaixa_Keydown);

        $quantidadePorCaixa.on('focus', quantidadeDeCaixa_Focus);
        $quantidadePorCaixa.on('keydown', quantidadeDeCaixa_Keydown);

        $validarAcessoCoordenador.on('click', validarAcessoCoordenador_Click);

        $validarAcessoCoordenadorTipoConferencia.on('click', validarAcessoCoordenadorTipoConferencia_Click);

        onScan.attachTo($multiplo[0], {
            onScan: function (sScancode, iQuatity) {
                $multiplo.val('');
            }
        });

        onScan.attachTo($quantidadePorCaixa[0], {
            onScan: function (sScancode, iQuatity) {
                if ($tipoConferencia.val() != "Por Quantidade") {
                    if (listaReferencia[0] != sScancode.detail.scanCode) {
                        PNotify.warning({ text: 'Referência inválida, não corresponde à referência iniciada!' });
                    } else {
                        //Altera a cor de fundo do campo para dar um destaque.
                        setTimeout(alterarBackgroundInput, 400);

                        //Captura o valor da quantidade por Caixa.
                        var contador = Number($quantidadePorCaixa.val()) + 1;

                        //Atribui o valor somado.
                        $quantidadePorCaixa.val(contador);

                        //Atualiza o valor no array
                        if (typeof listaReferencia[1] === 'undefined') {
                            listaReferencia.push(contador);
                        }
                        else {
                            listaReferencia[1] = contador;
                        }

                        //Retorna a cor de fundo original do campo.
                        $quantidadePorCaixa.css({ 'background': '#eee' });
                    }
                }
            }
        });
    }

    function removeEventos() {
        $(document).off('keydown', document_Keydown);

        $confirmarConferencia.off('click', confirmarConferencia_Click);

        $confirmarRegistroConferencia.off('click', confirmarRegistroConferencia_Click);

        $exibirDivergencia.off('click', exibirDivergencia);

        $referencia.off('blur', referencia_Blur);
        $referencia.off('focus', referencia_Focus);
        $referencia.off('keydown', referencia_Keydown);

        $finalizarConferencia.off('click', confirmarfinalizarConferencia);

        $multiplo.off('keydown', multiplo_Keydown);

        $quantidadeCaixa.off('keydown', quantidadeCaixa_Keydown);

        $quantidadePorCaixa.off('focus', quantidadeDeCaixa_Focus);
        $quantidadePorCaixa.off('keydown', quantidadeDeCaixa_Keydown);

        removeEventosCoordenador();

        try {
            onScan.detachFrom($multiplo[0]);
            onScan.detachFrom($quantidadePorCaixa[0]);
        } catch (e) { }

        $modalConferencia.off('hidden.bs.modal', removeEventos);
    }

    function removeEventosCoordenador() {
        $validarAcessoCoordenador.off('click', validarAcessoCoordenador_Click);

        $validarAcessoCoordenadorTipoConferencia.off('click', validarAcessoCoordenadorTipoConferencia_Click);
    }

    removeEventos();
    adicionaEventos();

    function carregarDadosReferenciaConferencia() {
        var referencia = $referencia.val();

        overlay(true);

        $.ajax({
            url: HOST_URL + CONTROLLER_PATH + "ObterDadosReferenciaConferencia",
            global: false,
            async: false,
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
                    $("#QuantidadePorCaixa").val(model.quantidadePorCaixa);
                    $("#QuantidadeCaixa").val(model.QuantidadeCaixa);

                    if (model.EnviarPicking) {
                        $("#msgEnviarPicking").removeClass("hidden");
                    }

                    //Reset array.
                    listaReferencia = new Array;

                    overlay(false);

                    $("#Multiplo").focus();

                    permiteRegistrar = true;
                } else {
                    PNotify.warning({ text: result.Message });

                    overlay(false);

                    $referencia.focus();

                    permiteRegistrar = false;
                }
            },
            error: function () {
                PNotify.warning({ text: 'Não foi possível obter dados. Por favor, tente novamente!' });

                overlay(false);

                $("#Multiplo").focus();

                permiteRegistrar = false;
            }
        });
    }

    function validarDiferencaMultiploConferencia() {
        let referencia = $("#Referencia").val();
        let multiplo = $("#Multiplo").val();
        let quantidadePorCaixa = $("#QuantidadePorCaixa").val();
        let quantidadeCaixa = $("#QuantidadeCaixa").val();
        let inicioConferencia = $("#InicioConferencia").val();
        let idTipoConferencia = $("#IdTipoConferencia").val();

        if (!multiplo)
            multiplo = 0;

        if (!quantidadePorCaixa)
            quantidadePorCaixa = 0;

        $.ajax({
            url: HOST_URL + CONTROLLER_PATH + "VerificarDiferencaMultiploConferencia",
            cache: false,
            global: false,
            method: "POST",
            data: {
                codigoBarrasOuReferencia: referencia,
                quantidadePorCaixa: quantidadePorCaixa,
                multiplo: multiplo
            },
            success: function (result) {
                if (result.Success) {

                    $('#modalAcessoCoordenador').modal('show');

                    $('#Mensagem').text(result.Message);

                    setTimeout(function () {
                        $("#Usuario").focus();
                    }, 150);
                } else {
                    registrarConferencia(referencia, quantidadePorCaixa, quantidadeCaixa, inicioConferencia, multiplo, idTipoConferencia);
                }
            },
            error: function (request, status, error) {
                PNotify.error({ text: request.responseText });
            }
        });
    }

    function registrarConferencia(referencia, quantidadePorCaixa, quantidadeCaixa, inicioConferencia, multiplo, idTipoConferencia) {
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
                inicioConferencia: inicioConferencia,
                multiplo: multiplo,
                idTipoConferencia: idTipoConferencia
            },
            success: function (result) {
                if (result.Success) {
                    permiteRegistrar = false;

                    PNotify.success({ text: result.Message });

                    resetarCamposConferencia();

                    $("#msgEnviarPicking").addClass("hidden");

                    overlay(false);

                    $referencia.focus();

                } else {
                    permiteRegistrar = true;

                    PNotify.warning({ text: result.Message });

                    overlay(false);
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

    function alterarBackgroundInput() {
        $quantidadePorCaixa.css({ 'background': '#98fb9873' });
    }

    function quantidadeDeCaixa_Keydown(e) {
        if (e.keyCode == 9 && !e.target.value) {
            return false;
        }
    }

    function quantidadeDeCaixa_Focus() {
        if ($tipoConferencia.val() != "Por Quantidade") {
            $quantidadePorCaixa.css({ 'background': '#98fb9873' });
        } else {
            $quantidadePorCaixa.css({ 'background': '#fff' });
        }
    }

    function quantidadeCaixa_Keydown(e) {
        if (e.keyCode == 9) {
            if (!e.target.value) {
            } else {
                $referencia.focus();
            }

            return false;
        }
    }

    function multiplo_Keydown(e) {
        if (e.keyCode == 9 && !e.target.value) {
            return false;
        }
    }

    function referencia_Focus() {
        permiteRegistrar = false;
    }

    function referencia_Keydown(e) {
        if (e.keyCode == 13) {
            if (!e.target.value) {
                PNotify.warning({ text: 'Referência está vazio. Por favor, informe o código de barras ou a referência!' });

                $referencia.focus();

                return false;
            } else {
                $.when(carregarDadosReferenciaConferencia()).then(function () {
                    listaReferencia.push($referencia.val())
                });
            }
        } else {
            if ($tipoConferencia == "Por Quantidade") {
                resetarCamposConferencia(false);
            }
        }
    }

    function referencia_Blur() {
        if (!$(this).val()) {
            $referencia.focus();
        }
        else {
            carregarDadosReferenciaConferencia();
        }
    }

    function confirmarConferencia_Click() {
        //Se o tipo da conferência é 100%. 
        //Caso seja, solicita confirmação do usuário. 
        //Caso contrário, chama o método para validar o múltiplo da conferência e posteriormente o registro da conferência.
        if ($tipoConferencia.val() != "Por Quantidade") {
            $.when(consultarPecasHaMaisConferencia()).then(function (qtdePecasHaMais) {
                if (!qtdePecasHaMais)
                    return;
                else {
                    $('#modalRegistrarConferencia').modal('show');

                    $('#MensagemRegistrarConferencia').text('Deseja realmente registrar a quantidade ' + $quantidadePorCaixa.val() + '? É importante saber que após a confirmação, as etiquetas de volume e PC A+ serão impressas.');

                    if (qtdePecasHaMais > 0)
                        $('#MensagemPecasHaMais').text('Atenção! Foi identificado divergência com o pedido de compra. Separar ' + qtdePecasHaMais + ' peças A+.');
                }
            });
        }
        else {
            validarDiferencaMultiploConferencia();
        }
    }

    function confirmarRegistroConferencia_Click() {
        $('#modalRegistrarConferencia').modal('toggle');
        validarDiferencaMultiploConferencia();
    }

    function validarAcessoCoordenador_Click() {
        var usuario = $("#Usuario").val();
        var senha = $("#Senha").val();

        $.ajax({
            url: HOST_URL + CONTROLLER_PATH + "ValidarAcessoCoordenadorConferencia",
            cache: false,
            global: false,
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
                    let idTipoConferencia = $("#IdTipoConferencia").val();

                    registrarConferencia(referencia, quantidadePorCaixa, quantidadeCaixa, inicioConferencia, multiplo, idTipoConferencia);

                    $('#modalAcessoCoordenador').modal('toggle');

                    $("#Usuario").val('');
                    $("#Senha").val('');

                    $referencia.focus();
                }
                else {
                    PNotify.warning({ text: result.Message });
                }
            },
            error: function (request, status, error) {
                PNotify.warning({ text: result.Message });
            }
        });
    }

    function validarAcessoCoordenadorTipoConferencia_Click() {
        var usuario = $("#UsuarioTipoConferencia").val();
        var senha = $("#SenhaTipoConferencia").val();

        $.ajax({
            url: HOST_URL + CONTROLLER_PATH + "ValidarAcessoCoordenadorConferencia",
            cache: false,
            global: false,
            method: "POST",
            data: {
                usuario: usuario,
                senha: senha
            },
            success: function (result) {
                if (result.Success) {
                    $("#IdTipoConferencia").val('1');
                    $("#TipoConferencia").val('Por Quantidade');
                    $("#QuantidadePorCaixa").attr("readonly", false);
                    $("#QuantidadeCaixa").attr("readonly", false);
                    $("#QuantidadeCaixa").val('');

                    $('#modalAcessoCoordenadorTipoConferencia').modal('toggle');

                    $quantidadePorCaixa.focus();

                    $("#UsuarioTipoConferencia").val('');
                    $("#SenhaTipoConferencia").val('');
                }
                else {
                    PNotify.warning({ text: result.Message });
                }
            },
            error: function (request, status, error) {
                PNotify.warning({ text: result.Message });
            }
        });
    }

    function document_Keydown(e) {
        //Verifica se o modal de conferência está aberto.
        if ($('#modalConferencia').is(':visible')) {
            if (permiteRegistrar) {
                switch (e.keyCode) {
                    //Verifica se a tecla pressionada é ESC (Registrar Conferência).
                    case 27: {

                        //Se o tipo da conferência é 100%. 
                        //Caso seja, solicita confirmação do usuário. 
                        //Caso contrário, chama o método para validar o múltiplo da conferência e posteriormente o registro da conferência.
                        if ($tipoConferencia.val() != "Por Quantidade") {
                            $.when(consultarPecasHaMaisConferencia()).then(function (qtdePecasHaMais) {
                                if (!qtdePecasHaMais)
                                    return
                                else {
                                    $('#modalRegistrarConferencia').modal('show');

                                    $('#MensagemRegistrarConferencia').text('Deseja realmente registrar a quantidade ' + $quantidadePorCaixa.val() + '? É importante saber que após a confirmação, as etiquetas de volume e PC A+ serão impressas.');

                                    if (qtdePecasHaMais > 0)
                                        $('#MensagemPecasHaMais').text('Atenção! Foi identificado divergência com o pedido de compra. Separar ' + qtdePecasHaMais + ' peças A+.');
                                }
                            }
                            );
                        }
                        else {
                            validarDiferencaMultiploConferencia();
                        }

                        break;
                    }
                    //Verifica se a tela pressionada é F4 (Alterar Tipo da Conferência)
                    case 115: {
                        if ($tipoConferencia.val() != "Por Quantidade") {

                            $('#modalAcessoCoordenadorTipoConferencia').modal('show');
                            $('#UsuarioTipoConferencia').val('');
                            $('#SenhaTipoConferencia').val('');

                            $('#MensagemTipoConferencia').text('Solicite a liberação do Coordenador para alterar o tipo de conferência.');

                            setTimeout(function () {
                                $("#UsuarioTipoConferencia").focus();
                            }, 150);
                        }

                        break;
                    }
                }
            }
        }
    }

    function removerMsgConferenciaManual() {
        if ($tipoConferencia.val() == "Por Quantidade") {

            $("#legendaTipoConferencia").addClass("hidden");
            $("#legendaRegistroConferencia").removeClass("col-lg-2");
            $("#legendaRegistroConferencia").addClass("col-lg-6");
        }
        else {
            $("#legendaTipoConferencia").removeClass("hidden");
            $("#legendaRegistroConferencia").removeClass("col-lg-6");
            $("#legendaRegistroConferencia").addClass("col-lg-2");
        }
    }

    removerMsgConferenciaManual();
})();

var confirmRegistrarConferencia = Confirm;

function confirmarfinalizarConferencia() {
    $(".close").click();

    let $modal = $("#modalConferencia");

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

    resetarTipoConferencia();

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

function consultarPecasHaMaisConferencia() {
    let referencia = $("#Referencia").val();
    let quantidadePorCaixa = $("#QuantidadePorCaixa").val() || 0;
    let idLote = $("#IdLote").val();
    let retorno = 0;

    $.ajax({
        url: HOST_URL + CONTROLLER_PATH + "ConsultarPecasHaMaisConferencia",
        cache: false,
        global: false,
        async: false,
        method: "POST",
        data: {
            codigoBarrasOuReferencia: referencia,
            idLote: idLote,
            quantidadePorCaixa: quantidadePorCaixa
        },
        success: function (result) {
            if (result.Success) {
                retorno = result.Data;
            }
            else {
                PNotify.warning({ text: result.Message });
                retorno = undefined;
            }
        },
        error: function (request, status, error) {
            PNotify.warning({ text: result.Message });
        }
    });

    return retorno;
}

function resetarTipoConferencia() {
    $.ajax({
        url: HOST_URL + CONTROLLER_PATH + "ObterTipoConferencia",
        cache: false,
        global: false,
        method: "POST",
        success: function (result) {
            if (result.Success) {
                if (result.Message != 'Por Quantidade') {
                    $("#IdTipoConferencia").val(result.Data);
                    $("#TipoConferencia").val(result.Message);
                    $("#QuantidadePorCaixa").attr("readonly", true);
                    $("#QuantidadeCaixa").attr("readonly", true);

                    $("#QuantidadePorCaixa").css({ 'background': '#eee' });
                }
            }
        },
        error: function (request, status, error) {
            PNotify.error({ text: request.responseText });
        }
    });
}

