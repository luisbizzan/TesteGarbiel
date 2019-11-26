(function () {
    loadButtons();

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

        $.post(HOST_URL + "BOAccount/AdicionarEmpresa?idEmpresa=" + id, function (result) {
            $("#EmpresasSelecionadas").append(result);

            loadButtons();
        });
    });

    $('.btnSalvar').on('click', function () {
        $("#EmpresasSelecionadas").find(".panelEmpresa").each(function (i, e) {
            $(this).find(".idEmpresa").attr('id', `EmpresasGrupos_${i}__IdEmpresa`);
            $(this).find(".idEmpresa").attr('name', `EmpresasGrupos[${i}].IdEmpresa`);

            $(this).find(".empresaNome").attr('id', `EmpresasGrupos_Name_${i}`);
            $(this).find(".empresaNome").attr('name', `EmpresasGrupos[${i}].Name`);

            $(this).find("[data-group]").each(function (ii, ee) {
                $(this).attr('id', `EmpresasGrupos_${i}__Grupos_${ii}__IsSelected`);
                $(this).attr('name', `EmpresasGrupos[${i}].Grupos[${ii}].IsSelected`);
            });

            $(this).find(".row").find('input[type=hidden]').not(".NomeGrupo").each(function (ii, ee) {
                $(this).attr('name', `EmpresasGrupos[${i}].Grupos[${ii}].IsSelected`);
            });

            $(this).find(".row").find("input:hidden.NomeGrupo").each(function (ii, ee) {
                $(this).attr('name', `EmpresasGrupos[${i}].Grupos[${ii}].Name`);
            });
        });

        loadButtons();
    });

})();

function loadButtons() {
    $(".btn-danger").on('click', function () {
        $(this).parents(".panelEmpresa").remove();
    });

    $('.checkAllGroups').on('click', function () {
        $(this).parents(".panelEmpresa").find("[data-group]").prop('checked', true);
    });

    $('.uncheckAllGroups').on('click', function () {
        $(this).parents(".panelEmpresa").find("[data-group]").prop('checked', false);
    });

    $("[data-group]").on('click', function () { $(".validationEmpresa").text(""); });
}
