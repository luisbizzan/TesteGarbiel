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

    $("#pesquisarTransportadora").click(function () {
        $("#modalTransportadora").load(HOST_URL + "Transportadora/SearchModal?ativo=true", function () {
            $("#modalTransportadora").modal();
        });
    });

    $("#limparTransportadora").click(function () {
        limparTransportadora();
    });

})();

function setEmpresa(idEmpresa, nomeFantasia, campo) {
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

function limparTransportadora() {
    let razaoSocial = $("#RazaoSocialTransportadora");
    let cliente = $("#IdTransportadora");
    razaoSocial.val("");
    cliente.val("");
}

function setTransportadora(idTransportadora, nomeFantasia) {
    $("#RazaoSocialTransportadora").val(nomeFantasia);
    $("#IdTransportadora").val(idTransportadora);
    $("#modalTransportadora").modal("hide");
    $("#modalTransportadora").empty();
}

