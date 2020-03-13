(function () {
    let $referencia = $("#Referencia");
    let $quantidade = $("#Quantidade");
    let $idGarantia = $("#IdGarantia").val();
    let $idNotaFiscal = $("#IdNotaFiscal").val();
    let $modalConferenciaGarantia = $("#modalConferenciaGarantia");
    let $btnTipoGarantia = $("#btnTipoGarantia");
    let $motivoLaudo = $("#MotivoLaudo");
    let $btnGroupTipo = $("#tipo .btn");
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
        $modalConferenciaGarantia.on('hidden.bs.modal', removeEventos);

        $(document).on('keydown', document_Keydown);

        $referencia.on('blur', referencia_Blur);
        $referencia.on('focus', referencia_Focus);
        $referencia.on('keydown', referencia_Keydown);


        $quantidade.on('focus', quantidade_Focus);
        $quantidade.on('keydown', quantidade_Keydown);
        $quantidade.on('blur', quantidade_Blur)
        $quantidade.on('keyup', quantidade_KeyUp);

        $btnGroupTipo.on('click', tipo_Click);


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

        $(document).off('keydown', document_Keydown);

        $quantidade.off('focus', quantidade_Focus);
        $quantidade.off('keydown', quantidade_Keydown);
        $quantidade.off('blur', quantidade_Blur)
        $quantidade.off('keyup', quantidade_KeyUp);

        $btnGroupTipo.off('click', tipo_Click);

        try {
            onScan.detachFrom($quantidade[0]);
        } catch (e) { }

        $modalConferenciaGarantia.off('hidden.bs.modal', removeEventos);
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
                    permiteRegistrar = true;
                });
            }
        }
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

    function quantidade_Blur() {
        if ($referencia.val() && !$(this).val()) {
            $quantidade.focus();
        }
    }

    function quantidade_KeyUp() {
        if ($(this).val() > 0) {
            $btnGroupTipo.prop('disabled', false);
            $btnTipoGarantia.addClass("btn-secondary-selected");
        }
        else {
            $btnGroupTipo.prop('disabled', true);
            $btnTipoGarantia.addClass("btn-secondary-selected");
            $btnTipoGarantia.removeClass('btn-secondary-selected');
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

    function tipo_Click(e) {
        $btnGroupTipo.each(function (index) {
            $(this).removeClass('btn-secondary-selected');
        });

        $(e.target).addClass("btn-secondary-selected");

        var bla = $motivoLaudo;

        if ($(e.target).attr("id") === 'btnTipoLaudo') {
            $motivoLaudo.removeAttr("readonly");
            $motivoLaudo.focus();
        }
        else {
            $motivoLaudo.attr("readonly", "readonly");
        }
    }

    function document_Keydown(e) {
        //Verifica se o modal de conferência está aberto.
        if (permiteRegistrar) {
            if ($modalConferenciaGarantia.is(':visible')) {
                var modalConferenciaAberta = VerificaModalConferenciaAberta();
                if (modalConferenciaAberta) {
                    switch (e.keyCode) {
                        //Verifica se a tela pressionada é F4 (Permite inserir manualmente a quantidade)
                        case 115: {
                            $quantidade.removeAttr("readonly");
                            $quantidade.css({ 'background': '#eee' });
                            break;
                        }
                    }
                }
            }
        }
    }

    // Solução paliativa permanete onde verifica qual modal está na frente
    function VerificaModalConferenciaAberta() {
        var $listaModais = $('.modal:visible');

        var listaModais = $listaModais.map(function (a, b) {
            return { zIndex: parseInt($(b).css('z-index')), elem: b };
        }).sort((a, b) => (a.zIndex < b.zIndex) ? 1 : -1);

        if (listaModais.length == 0) {
            return false;
        }

        return $(listaModais[0].elem).is($modalConferenciaGarantia);
    }
})();

