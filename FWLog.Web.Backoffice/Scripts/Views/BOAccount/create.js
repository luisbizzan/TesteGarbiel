(function () {
    loadButtons();

    $(".isEmpresaPrincial").off();
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
            $(".isEmpresaPrincial").off();
            loadButtons();
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

            $(this).find("[data-group]").each(function (ii, ee) {
                $(this).attr('id', `EmpresasGrupos_${i}__Grupos_${ii}__IsSelected`);
                $(this).attr('name', `EmpresasGrupos[${i}].Grupos[${ii}].IsSelected`);
            });

            $(this).find(".empresasGrupo").find('input[type=hidden]').not(".NomeGrupo").each(function (ii, ee) {
                $(this).attr('name', `EmpresasGrupos[${i}].Grupos[${ii}].IsSelected`);
            });

            $(this).find(".empresasGrupo").find("input:hidden.NomeGrupo").each(function (ii, ee) {
                $(this).attr('name', `EmpresasGrupos[${i}].Grupos[${ii}].Name`);
            });
        });
    });

    $("#pesquisarEmpresa").click(function () {
        $("#modalEmpresaPrincipal").load(HOST_URL + "Empresa/SearchModal?CampoSelecionado=EmpresaPrincipal", function () {
            $("#modalEmpresaPrincipal").modal();
        });
    });

    $("#limparEmpresa").click(limparEmpresa);
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

function setEmpresa(idEmpresa, razaoSocial, campo) {
    $("#" + campo).find("#razaoSocial").val(razaoSocial);
    $("#" + campo).find("#empresaId").val(idEmpresa);

    $("#modal" + campo).modal("hide");
    $("#modal" + campo).empty();
}

function limparEmpresa() {
    $("#EmpresaPrincipal").find("#razaoSocial").val("");
    $("#EmpresaPrincipal").find("#empresaId").val("");
}