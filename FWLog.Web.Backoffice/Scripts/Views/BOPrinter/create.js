(function () {
    $("#pesquisarEmpresa").click(function () {
        $("#modalEmpresaPrincipal").load(HOST_URL + "Empresa/SearchModal?CampoSelecionado=EmpresaPrincipal", function () {
            $("#modalEmpresaPrincipal").modal();
        });
    });

    $("#limparEmpresa").click(limparEmpresa);
})();

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