(function () {
    let $referencia = $("#Referencia");
    let $quantidade = $("#Quantidade");
    let $idGarantia = $("#IdGarantia").val();
    let $idNotaFiscal = $("#IdNotaFiscal").val();
    let $modalConferencia = $("#modalConferencia");

    let listaReferencia = new Array;

    let permiteRegistrar = false;

    $quantidade.mask("S#############", {
        translation: {
            'S': {
                pattern: /-/,
                optional: true
            }
        }
    });


    function adicionaEventos() {
        $modalConferencia.on('hidden.bs.modal', removeEventos);

        $referencia.on('blur', referencia_Blur);
        $referencia.on('focus', referencia_Focus);
        $referencia.on('keydown', referencia_Keydown);

        $quantidade.on('focus', quantidade_Focus);
        $quantidade.on('keydown', quantidade_Keydown);

        onScan.attachTo($quantidade[0], {
            onScan: function (sScancode, iQuatity) {
                if (listaReferencia[0] != referenciaFormatada(sScancode)) {
                    PNotify.warning({ text: 'Referência inválida, não corresponde à referência iniciada!' });
                } else {
                    //Altera a cor de fundo do campo para dar um destaque.
                    setTimeout(alterarBackgroundInput, 400);

                    //Captura o valor da quantidade por Caixa.
                    var contador = Number($quantidade.val()) + iQuatity;

                    //Atribui o valor somado.
                    $quantidade.val(contador);

                    //Atualiza o valor no array
                    if (typeof listaReferencia[1] === 'undefined') {
                        listaReferencia.push(contador);
                    }
                    else {
                        listaReferencia[1] = contador;
                    }

                    //Retorna a cor de fundo original do campo.
                    $quantidade.css({ 'background': '#eee' });
                }
            }
        });
    }

    function removeEventos() {
        $referencia.off('blur', referencia_Blur);
        $referencia.off('focus', referencia_Focus);
        $referencia.off('keydown', referencia_Keydown);
        $quantidade.off('focus', quantidade_Focus);
        $quantidade.off('keydown', quantidade_Keydown);


        try {
            onScan.detachFrom($quantidade[0]);
        } catch (e) { }

        $modalConferencia.off('hidden.bs.modal', removeEventos);
    }

    removeEventos();
    adicionaEventos();


    function referencia_Blur() {
        if (!$(this).val()) {
            $referencia.focus();
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
                $.when(carregarDadosReferenciaConferenciaGarantia()).then(function () {
                    listaReferencia.push($referencia.val());
                    $quantidade.focus();
                });
            }
        }
        //else {
        //    if ($tipoConferencia.text() == "Por Quantidade") {
        //        resetarCamposConferencia(false);
        //    }
        //}
    }

    function quantidade_Focus() {
        $quantidade.css({ 'background': '#98fb9873' });
    }

    function quantidade_Keydown(e) {
        if (e.keyCode == 9) {
            if (!e.target.value) {
            } else {
                $referencia.focus();
            }

            return false;
        }
    }

    function carregarDadosReferenciaConferenciaGarantia() {
        var referencia = $referencia.val();

        overlay(true);

        $.ajax({
            url: HOST_URL + CONTROLLER_PATH + "ObterDadosReferenciaConferenciaGarantia",
            global: false,
            async: false,
            cache: false,
            method: "POST",
            data: {
                codigoBarrasOuReferencia: referencia,
                idGarantia: $idGarantia,
                idNotaFiscal: $idNotaFiscal
            },
            success: function (result) {
                if (result.Success) {
                    var model = JSON.parse(result.Data);

                    $("#Descricao").val(model.Descricao);
                    $("#Unidade").val(model.Unidade);
                    $("#Fabricante").val(model.Fabricante);


                    if (result.Message != null) {
                        PNotify.warning({ text: result.Message });
                    }

                    //Reset array.
                    listaReferencia = new Array;

                    overlay(false);

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

                permiteRegistrar = false;
            }
        });
    }

    function overlay(show) {
        var $overlay = $('#overlay');
        if (!!show) {
            $referencia.attr('disabled', true);
            $overlay.show();
            $("#fecharConferencia").hide();
        } else {
            $referencia.attr('disabled', false);
            $overlay.hide();
            $("#fecharConferencia").show();
        }
    }

    function alterarBackgroundInput() {
        $quantidade.css({ 'background': '#98fb9873' });
    }

    function referenciaFormatada(sScancode) {
        var sScancodeString = String(sScancode);

        var pattern = /[^\w]+/g;

        var sScancodeFormated = sScancodeString.replace(pattern, "");

        return sScancodeFormated
    }

})();

