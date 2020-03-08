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
                $('#modalDevolucaoTotal').modal('toggle');
                PNotify.error({ text: result.Message });
            } else {
                $(".close").click();
                $('#modalDevolucaoTotal').modal('toggle');
                $("#dataTable").DataTable().ajax.reload();
                PNotify.success({ text: result.Message });
                //confirmarfinalizarConferencia();   //Se não vai haver conferencia. não tem necessidade de exibir a tela de resumo.
            }
        }
    });
}