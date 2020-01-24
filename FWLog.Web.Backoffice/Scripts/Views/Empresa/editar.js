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

function setEmpresa(idEmpresa, nomeFantasia, campo) {
    debugger
    let _nomeFantasia = $("#" + campo).find(".nomeFantasia");
    let empresa = $("#" + campo).find(".idEmpresa");
    _nomeFantasia.val(nomeFantasia);
    empresa.val(idEmpresa);
    $("#modal" + campo).modal("hide");
    $("#modal" + campo).empty();
}

function limparEmpresa(campo) {
    let _nomeFantasia = $("#" + campo).find(".nomeFantasia");
    let empresa = $("#" + campo).find(".idEmpresa");
    _nomeFantasia.val("");
    empresa.val("");
}


