(function () {
    $("#FinalizarDevolucaoTotal").click(function () {
        FinalizarDevolucaoTotal();
    });
})();

function FinalizarDevolucaoTotal() {
    debugger;
    let idLote = $("#IdLote").val();
    let quantidadeEtiqueta = $('#QuantidadeEtiqueta').val();

    $.ajax({
        url: HOST_URL + CONTROLLER_PATH + "/FinalizarProcessamentoDevolucaoTotal",
        cache: false,
        method: "POST",
        data: {
            idLote: idLote,
            quantidadeEtiqueta: quantidadeEtiqueta
        },
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