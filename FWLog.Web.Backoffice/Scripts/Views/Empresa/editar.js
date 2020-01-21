(function () {
    $.validator.setDefaults({ ignore: null });
    $.validator.addMethod("CNPJValido",
        function (value, element, params) {
            if (!value) {
                return true;
            }

            let noMaskValue = value.replace("/", String.empty).replace(".", String.empty).replace("-", String.empty);
            let isValid = dart.validations.isValidCnpj(noMaskValue);

            return isValid;
        }, "Deve ser um CNPJ válido."
    );

    $.validator.addClassRules({
        CNPJValido: { CNPJValido: true }
    });

    $("#pesquisarEmpresaGarantia").click(function () {
        $("#modalEmpresaGarantia").load(HOST_URL + "Empresa/SearchModal?CampoSelecionado=EmpresaGarantia", function () {
            $("#modalEmpresaGarantia").modal();
        });
    });

    $("#limparEmpresaMatriz").click(function () {
        limparEmpresa("EmpresaMatriz");
    });

    $("#limparEmpresaGarantia").click(function () {
        limparEmpresa("EmpresaGarantia");
    });

})();

function setEmpresa(idEmpresa, razaoSocial, campo) {
    let razao = $("#" + campo).find(".razaoSocial");
    let empresa = $("#" + campo).find(".idEmpresa");
    razao.val(razaoSocial);
    empresa.val(idEmpresa);
    $("#modal" + campo).modal("hide");
    $("#modal" + campo).empty();
}

function limparEmpresa(campo) {
    let razao = $("#" + campo).find(".razaoSocial");
    let empresa = $("#" + campo).find(".idEmpresa");
    razao.val("");
    empresa.val("");
}


