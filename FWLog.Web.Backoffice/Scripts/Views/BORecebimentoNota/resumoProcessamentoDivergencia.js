(function () {
    $("#FinalizarTratativaDivergencia").click(function () {
        FinalizarTratativa();
    });
})();

function FinalizarTratativa() {
    let idLote = $("#IdLote").val();
    $.ajax({
        url: HOST_URL + CONTROLLER_PATH + "/FinalizarProcessamentoTratativaDivergencia/" + idLote,
        cache: false,
        method: "POST",
        success: function (result) {
            if (!result.Success) {
                PNotify.error({ text: result.Message });

                var model = JSON.parse(result.Data);

                $('.flat[id="Processamento_CriacaoQuarentena"]').iCheck(model.CriacaoQuarentena ? "check" : "uncheck");
                $('.flat[id="Processamento_CriacaoNFDevolucao"]').iCheck(model.CriacaoNFDevolucao ? "check" : "uncheck");
                $('.flat[id="Processamento_ConfirmacaoNFDevolucao"]').iCheck(model.ConfirmacaoNFDevolucao ? "check" : "uncheck");
                $('.flat[id="Processamento_AutorizacaoNFDevolucaoSefaz"]').iCheck(model.AutorizacaoNFDevolucaoSefaz ? "check" : "uncheck");

                $('input').iCheck({ checkboxClass: 'icheckbox_flat-green' });
            } else {
                PNotify.success({ text: result.Message });
                $(".close").click();
                $("#dataTable").DataTable().ajax.reload();
            }
        }
    });
}