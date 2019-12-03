(function () {
    $("#pesquisarEmpresaMatriz").click(function () {
        $("#modalEmpresaMatriz").load(HOST_URL + "BOEmpresa/SearchModal?CampoSelecionado=EmpresaMatriz", function () {
            $("#modalEmpresaMatriz").modal();
        });
    });

    $("#pesquisarEmpresaGarantia").click(function () {
        $("#modalEmpresaGarantia").load(HOST_URL + "BOEmpresa/SearchModal?CampoSelecionado=EmpresaGarantia", function () {
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


