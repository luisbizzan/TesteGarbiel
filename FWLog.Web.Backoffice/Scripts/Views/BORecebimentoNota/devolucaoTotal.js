(function () {

    $("#finalizarConferencia").click(function () {
        confirmarfinalizarConferencia();
    });

    $("#finalizarDevolucaoTotal").click(function () {
        FinalizarDevolucaoTotal();
    });
})();


function confirmarfinalizarConferencia() {
    $(".close").click();

    let $modal = $("#modalConferencia");
    let idLote = 801;

    $modal.load(HOST_URL + CONTROLLER_PATH + "ResumoFinalizarConferencia/" + idLote, function () {
        $modal.modal();
    });
}


function FinalizarDevolucaoTotal() {
    $.ajax({
        url: HOST_URL + CONTROLLER_PATH + "/FinalizarDevolucaoTotal/",
        cache: false,
        method: "POST",
        success: function (result) {
            if (!result.Success) {
                PNotify.error({ text: result.Message });
                $('#modalDevolucaoTotal').modal('toggle');
               /* var model = JSON.parse(result.Data);

                $('.flat[id="Processamento_CriacaoQuarentena"]').iCheck(model.CriacaoQuarentena ? "check" : "uncheck");
                $('.flat[id="Processamento_CriacaoNFDevolucao"]').iCheck(model.CriacaoNFDevolucao ? "check" : "uncheck");
                $('.flat[id="Processamento_ConfirmacaoNFDevolucao"]').iCheck(model.ConfirmacaoNFDevolucao ? "check" : "uncheck");
                $('.flat[id="Processamento_AutorizacaoNFDevolucaoSefaz"]').iCheck(model.AutorizacaoNFDevolucaoSefaz ? "check" : "uncheck");

                $('input').iCheck({ checkboxClass: 'icheckbox_flat-green' });*/
            } else {
                PNotify.success({ text: result.Message });
                $(".close").click();
                $('#modalDevolucaoTotal').modal('toggle');
                $("#dataTable").DataTable().ajax.reload();
                confirmarfinalizarConferencia();
              //  $("#dataTable").DataTable().ajax.reload();
            }
        }
    });
}