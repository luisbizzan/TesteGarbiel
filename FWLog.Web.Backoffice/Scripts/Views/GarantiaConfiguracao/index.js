
/* gravar novo fornecedor quebra */
$("#btnGravar").click(function (e) {
    var _fornecedor = new Object();
    _fornecedor.Id = 0;
    _fornecedor.Cod_Fornecedor = $("#Cod_Fornecedor").val();

    $.post("/GarantiaConfiguracao/IncluirFornecedorQuebra", { fornecedor: _fornecedor }, function (s) {
        if (s.Success) {
            PNotify.success({ text: s.Message });
            $("#Cod_Fornecedor").val("");
        }
        else {
            PNotify.error({ text: s.Message });
        }
        console.log(s);
    }).fail(function (f) {
        //console.log("[Fail]");
        //console.log(f);
    }).done(function (d) {
        //console.log("[Done]");
        //console.log(d);
    });
});
