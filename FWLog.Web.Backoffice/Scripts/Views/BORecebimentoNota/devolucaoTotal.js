(function () {
    $("#finalizarDevolucaoTotal").click(function () {
        ValidarPermissaoDevolucaoTotal();
    });
})();


/*function confirmarfinalizarConferencia() {
    $(".close").click();

    let $modal = $("#modalConferencia");
    let idLote = 801;

    $modal.load(HOST_URL + CONTROLLER_PATH + "ResumoFinalizarConferencia/" + idLote, function () {
        $modal.modal();
    });
}*/

function ExibirModalAcessoCoordenadorDevolucaoTotal() {
    $(".close").click();

    let $modal = $("#modalAcessoCoordenadorDevolucaoTotal");

    $modal.load(HOST_URL + CONTROLLER_PATH + "ValidarAcessoDevolucaoTotal/", function () {
        $modal.modal();
    });
}



function ValidarAcessoDevolucaoTotal() {
    var usuario = $("#Usuario").val();
    var senha   = $("#Senha").val();

    $(".close").click();
    let $modal = $("#modalAcessoCoordenadorDevolucaoTotal");

    $modal.load(HOST_URL + CONTROLLER_PATH + "ValidarAcessoDevolucaoTotal/" + idLote, function () {
        $modal.modal();

    $.ajax({
        url: HOST_URL + CONTROLLER_PATH + "ValidarAcessoDevolucaoTotal",
        cache: false,
        global: false,
        method: "POST",
        data: {
            usuario: usuario,
            senha  : senha
        },
        success: function (result) {
            if (result.Success) {

                FinalizarDevolucaoTotal();

                $('#modalAcessoCoordenador').modal('toggle');

                $("#Usuario").val('');
                $("#Senha").val('');

                //$("#Referencia").focus();
            }
            else {
                PNotify.warning({ text: result.Message });
            }
        },
        error: function (request, status, error) {
            PNotify.error({ text: request.Message });
        }
    });
}

function ValidarPermissaoDevolucaoTotal() {
    $.ajax({
        url: HOST_URL + CONTROLLER_PATH + "ValidarPermissaoDevolucaoTotal",
        method: "POST",
        cache: false,
        success: function (result) {
            if (result.Success) {
                FinalizarDevolucaoTotal();
            } else {
                ValidarAcessoDevolucaoTotal();
                //PNotify.warning({ text: result.Message });
            }
        }
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