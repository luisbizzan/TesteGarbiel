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

    debugger
    $('#modalRegistroConferencia').on('keydown', function (e) {
        if (e.keyCode == 27 && permiteRegistrar) {
            validarDiferencaMultiploConferencia();
        }

        if (e.keyCode == 115) {
            if ($tipoConferencia.val() != "Por Quantidade") {

                $('#modalAcessoCoordenadorTipoConferencia').modal('show');

                $('#MensagemTipoConferencia').text('Solicite a liberação do Coordenador para alterar o tipo de conferência.');

                setTimeout(function () {
                    $("#UsuarioTipoConferencia").focus();
                }, 150);
            }
        }
    });

    onScan.attachTo($quantidadePorCaixa[0]);

    $quantidadePorCaixa[0].addEventListener('scan', function (sScancode, iQuatity) {
        if ($tipoConferencia.val() != "Por Quantidade") {
            if (listaReferencia[0] != sScancode.detail.scanCode) {
                PNotify.warning({ text: 'Referência inválida, não corresponde a referência iniciada!' });
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
                PNotify.warning({ text: 'Referência está vazio. Por favor, informe o código de barras ou a referência!' });

                $referencia.focus();

                return false;
            } else {
                $.when(carregarDadosReferenciaConferencia()).then(function () {
                    listaReferencia.push($referencia.val())
                }
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
        if ($tipoConferencia != 'Por Quantidade') {
            debugger
            $quantidadePorCaixa.css({ 'background': '#eee' });
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
                    PNotify.warning({ text: result.Message });
                }
            },
            error: function (request, status, error) {
                PNotify.warning({ text: result.Message });
            }
        });
    });

    $("#validarAcessoCoordenadorTipoConferencia").click(function () {
        let usuario = $("#UsuarioTipoConferencia").val();
        let senha = $("#SenhaTipoConferencia").val();

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
    });

    function carregarDadosReferenciaConferencia() {
        let referencia = $referencia.val();

        //$("#QuantidadePorCaixa").focus();

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

                    PNotify.success({ text: result.Message });

                    resetarCamposConferencia();

                    if (!$('#msgEnviarPicking').hasClass('hidden')) {
                        $("#msgEnviarPicking").addClass("hidden");
                    }

                    permiteRegistrar = false;

                    overlay(false);

                    $referencia.focus();

                } else {
                    PNotify.warning({ text: result.Message });

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

debugger
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
                }
            }
        },
        error: function (request, status, error) {
            PNotify.error({ text: request.responseText });
        }
    });
}

