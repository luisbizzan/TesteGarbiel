(function () {
    loadButtonsEvents();

    $('#ddlImpressaoItem').on('change', function () {
        $(".validationImpressaoItem").text("");
    });

    $('#btnAddImpressaoItem').on('click', function () {
        var id = $('#ddlImpressaoItem').val() === "" ? 0 : $('#ddlImpressaoItem').val();

        if (id === 0) {
            $(".validationImpressaoItem").text("Selecione um tipo de impressão para adiconar ao perfil.");

            return;
        }

        var bok = false;

        $(".idImpressaoItem").each(function (i, e) {
            if (id === $(this).val()) {
                $(".validationImpressaoItem").text("O tipo de impressão escolhido já foi adicionado ao perfil.");
                bok = true;
                return;
            }
        });

        if (bok) {
            return;
        }

        $.post(HOST_URL + "PerfilImpressora/AdicionarTipoImpressao/" + id, function (result) {
            $("#TiposImpressaoSelecionados").append(result);
            loadButtonsEvents();
        });
    });

    $('.btnSalvar').on('click', function () {
        $("#TiposImpressaoSelecionados").find(".panelImpressaoItem").each(function (i, e) {
            $(this).find(".idImpressaoItem").attr('id', `TiposImpressao_${i}__IdImpressaoItem`);
            $(this).find(".idImpressaoItem").attr('name', `TiposImpressao[${i}].IdImpressaoItem`);

            $(this).find(".descricao").attr('id', `TiposImpressao_${i}__Descricao`);
            $(this).find(".descricao").attr('name', `TiposImpressao[${i}].Descricao`);

            $(this).find("[data-group]").each(function (ii, ee) {
                $(this).attr('id', `TiposImpressao_${i}__Impressoras_${ii}__Selecionado`);
                $(this).attr('name', `TiposImpressao[${i}].Impressoras[${ii}].Selecionado`);
            });

            $(this).find(".impressoras").find('input[type=hidden]').not(".idImpressora, .nomeImpressora").each(function (ii, ee) {
                $(this).attr('name', `TiposImpressao[${i}].Impressoras[${ii}].Selecionado`);
            });

            $(this).find(".impressoras").find("input:hidden.idImpressora").each(function (ii, ee) {
                $(this).attr('name', `TiposImpressao[${i}].Impressoras[${ii}].IdImpressora`);
            });

            $(this).find(".impressoras").find("input:hidden.nomeImpressora").each(function (ii, ee) {
                $(this).attr('name', `TiposImpressao[${i}].Impressoras[${ii}].Nome`);
            });
        });
    });
})();

function loadButtonsEvents() {
    $(".btn-danger").off();
    $(".btn-danger").on('click', function () {
        $(this).parents(".panelImpressaoItem").remove();
    });

    $(".checkAllGroups").off();
    $('.checkAllGroups').on('click', function () {
        $(this).parents(".panelImpressaoItem").find("[data-group]").prop('checked', true);
    });

    $(".uncheckAllGroups").off();
    $('.uncheckAllGroups').on('click', function () {
        $(this).parents(".panelImpressaoItem").find("[data-group]").prop('checked', false);
    });

    $("[data-group]").off();
    $("[data-group]").on('click', function () { $(".validationImpressaoItem").text(""); });
}