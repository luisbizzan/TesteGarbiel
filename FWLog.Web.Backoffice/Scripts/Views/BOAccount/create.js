(function () {
    loadButtonsEvents();

    $('#ddlEmpresas').on('change', function () {
        $(".validationEmpresa").text("");
    });

    $('#btnAddEmpresa').on('click', function () {
        var id = $('#ddlEmpresas').val() === "" ? 0 : $('#ddlEmpresas').val();

        if (id === 0) {
            $(".validationEmpresa").text("Selecione uma empresa para adiconar ao usuário.");

            return;
        }

        var bok = false;

        $(".idEmpresa").each(function (i, e) {
            if (id === $(this).val()) {
                $(".validationEmpresa").text("A empresa escolhida já foi adicionada ao usuário.");
                bok = true;
                return;
            }
        });

        if (bok) {
            return;
        }

        $.post(HOST_URL + "BOAccount/AdicionarEmpresa/" + id, function (result) {
            $("#EmpresasSelecionadas").append(result);
            loadButtonsEvents();
        });
    });

    $('.btnSalvar').on('click', function () {
        $("#EmpresasSelecionadas").find(".panelEmpresa").each(function (i, e) {
            $(this).find(".idEmpresa").attr('id', `EmpresasGrupos_${i}__IdEmpresa`);
            $(this).find(".idEmpresa").attr('name', `EmpresasGrupos[${i}].IdEmpresa`);

            $(this).find(".empresaNome").attr('id', `EmpresasGrupos_${i}__Nome`);
            $(this).find(".empresaNome").attr('name', `EmpresasGrupos[${i}].Nome`);

            $(this).find(".isEmpresaPrincial").attr('id', `EmpresasGrupos_${i}__IsEmpresaPrincipal`);
            $(this).find(".isEmpresaPrincial").attr('name', `EmpresasGrupos[${i}].IsEmpresaPrincipal`);

            $(this).find(".idPerfilImpressoraPadrao").attr('id', `EmpresasGrupos_${i}__IdPerfilImpressoraPadrao`);
            $(this).find(".idPerfilImpressoraPadrao").attr('name', `EmpresasGrupos[${i}].IdPerfilImpressoraPadrao`);

            $(this).find("[data-group]").each(function (ii, ee) {
                $(this).attr('id', `EmpresasGrupos_${i}__Grupos_${ii}__IsSelected`);
                $(this).attr('name', `EmpresasGrupos[${i}].Grupos[${ii}].IsSelected`);
            });

            $(this).find(".empresaprincipal").find('input[type=hidden]').each(function (ii, ee) {
                $(this).attr('name', `EmpresasGrupos_${i}__IsEmpresaPrincipal`);
            });

            $(this).find(".empresasGrupo").find('input[type=hidden]').not(".NomeGrupo").each(function (ii, ee) {
                $(this).attr('name', `EmpresasGrupos[${i}].Grupos[${ii}].IsSelected`);
            });

            $(this).find(".empresasGrupo").find("input:hidden.NomeGrupo").each(function (ii, ee) {
                $(this).attr('name', `EmpresasGrupos[${i}].Grupos[${ii}].Name`);
            });
        });
    });
})();

function loadButtonsEvents() {
    $(".btn-danger").off();
    $(".btn-danger").on('click', function () {
        $(this).parents(".panelEmpresa").remove();
    });

    $(".checkAllGroups").off();
    $('.checkAllGroups').on('click', function () {
        $(this).parents(".panelEmpresa").find("[data-group]").prop('checked', true);
    });

    $(".uncheckAllGroups").off();
    $('.uncheckAllGroups').on('click', function () {
        $(this).parents(".panelEmpresa").find("[data-group]").prop('checked', false);
    });

    $("[data-group]").off();
    $("[data-group]").on('click', function () { $(".validationEmpresa").text(""); });

    $(".isEmpresaPrincial").off();

    if ($('.isEmpresaPrincial').length === 1) {
        $('.isEmpresaPrincial').prop('checked', true);
    }

    $('.isEmpresaPrincial').on('click', function () {
        if ($(this).is(":checked")) {
            $("#EmpresasSelecionadas").find(".panelEmpresa").each(function (i, e) {
                $(this).find(".isEmpresaPrincial").prop('checked', false);
            });

            $(this).prop('checked', true);
        } else {
            $(this).prop('checked', true);
        }
    });
}