(function () {
    let permiteRegistrar = false;
    let $referencia = $("#Referencia");
    var $quantidadePorCaixa = $("#QuantidadePorCaixa");
    var $quantidadeCaixa = $("#QuantidadeCaixa");
    var $multiplo = $("#Multiplo");
    var $tipoConferencia = $("#TipoConferencia");
    var listaReferencia = new Array; 

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

    $quantidadePorCaixa.on('focus', function () {
        if ($tipoConferencia.val() != "Por Quantidade") {
            $quantidadePorCaixa.css({ 'background': '#98fb9873' });
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

    $multiplo.on('keydown', function (e) {
        if (e.keyCode == 9 && !e.target.value) {
            return false;
        }
    });

    $('#modalRegistroConferencia').keydown(function (e) {
        if (e.keyCode == 13 && permiteRegistrar) {
            if ($tipoConferencia.val() == "Por Quantidade") {
                validarDiferencaMultiploConferencia();
            }
        }

        if (e.keyCode == 27 && permiteRegistrar) {
            if ($tipoConferencia.val() != "Por Quantidade") {
                validarDiferencaMultiploConferencia();
            }
        }
    });    

    onScan.attachTo($quantidadePorCaixa[0]);
    
    $quantidadePorCaixa[0].addEventListener('scan', function (sScancode, iQuatity) {
        if ($tipoConferencia.val() != "Por Quantidade") {
            if (listaReferencia[0] != sScancode.detail.scanCode) {
                PNotify.info({ text: 'Referência inválida, não corresponde a referência iniciada!' });
            }
            else {
                //Altera a cor de fundo do campo para dar um destaque.
                setTimeout(alterarBackgroundInput, 400);

                //Captura o valor da quantidade por Caixa.
                let contador = Number($quantidadePorCaixa.val()) + 1;

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
    });

    onScan.attachTo($multiplo[0]);

    $multiplo[0].addEventListener('scan', function (sScancode, iQuatity) {
        $multiplo.val('');
    });

    $referencia.focus(function () {
        permiteRegistrar = false;
    });
    
    $referencia.on('keypress keydown', function (e) {
        if (e.keyCode == 13) {
            if (!e.target.value) {
                PNotify.info({ text: 'Referência está vazio. Por favor, informe o código de barras ou a referência!' });

                $referencia.focus();

                return false;
            } else {
                debugger;
                $.when(carregarDadosReferenciaConferencia()).promise(
                    listaReferencia.push($referencia.val())
                );
            }
        } else {
            if ($tipoConferencia == "Por Quantidade") {
                resetarCamposConferencia(false);
            }
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

    $quantidadePorCaixa.blur(function () {
        $quantidadePorCaixa.css({ 'background': '#eee' });
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

                    registrarConferencia(referencia, quantidadePorCaixa, quantidadeCaixa, inicioConferencia, multiplo);

                    $('#modalAcessoCoordenador').modal('toggle');

                    $("#Usuario").val('');
                    $("#Senha").val('');

                    $referencia.focus();
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

    function carregarDadosReferenciaConferencia() {
        let referencia = $referencia.val();

        //$("#QuantidadePorCaixa").focus();

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
                    PNotify.info({ text: result.Message });

                    overlay(false);

                    $referencia.focus();

                    permiteRegistrar = false;
                }
            },
            error: function () {
                PNotify.info({ text: 'Não foi possível obter dados. Por favor, tente novamente!' });

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

    if (!multiplo)
        multiplo = 0;

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
                registrarConferencia(referencia, quantidadePorCaixa, quantidadeCaixa, inicioConferencia, multiplo);
            }
        },
        error: function (request, status, error) {
            PNotify.info({ text: request.responseText });
        }
    });
}

function registrarConferencia(referencia, quantidadePorCaixa, quantidadeCaixa, inicioConferencia, multiplo) {
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
                multiplo: multiplo
            },
            success: function (result) {
                if (result.Success) {

                    PNotify.success({ text: result.Message });

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

    function alterarBackgroundInput() {
        $quantidadePorCaixa.css({ 'background': '#98fb9873' });
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

