(function () {
    $("#finalizarDevolucaoTotal").click(function () {
        ValidarPermissaoDevolucaoTotal();
    });

    $("#validarAcessoCoordenadorDevolucaoTotal").click(function () {
        ValidarAcessoDevolucaoTotal();
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
    $('#modalAcessoCoordenadorDevolucaoTotal').modal('show');
    $('#UsuarioDevolucaoTotal').val('');
    $('#SenhaDevolucaoTotal').val('');

    $('#MensagemDevolucaoTotal').text('Solicite a liberação do Coordenador para devolução total.');

    setTimeout(function () {
        $("#UsuarioDevolucaoTotal").focus();
    }, 150);
}

function ValidarAcessoDevolucaoTotal() {
    debugger
    var usuario = $("#UsuarioDevolucaoTotal").val();
    var senha = $("#SenhaDevolucaoTotal").val();

    $.ajax({
        url: HOST_URL + CONTROLLER_PATH + "ValidarAcessoDevolucaoTotal",
        cache: false,
        global: false,
        method: "POST",
        data: {
            usuario: usuario,
            senha: senha
        },
        success: function (result) {
            if (result.Success) {
                FinalizarDevolucaoTotal();

                $('#modalAcessoCoordenadorDevolucaoTotal').modal('toggle');

                $('#UsuarioDevolucaoTotal').val('');
                $('#SenhaDevolucaoTotal').val('');

                $("#UsuarioDevolucaoTotal").focus();
            }
            else {
                PNotify.warning({ text: result.Message });
            }
        },
        error: function (request, status, error) {
            PNotify.error({ text: request.Message });
        }
    });
};


function ValidarPermissaoDevolucaoTotal() {
    $.ajax({
        url: HOST_URL + CONTROLLER_PATH + "validarPermissaoDevolucaoTotal",
        cache: false,
        method: "POST",
        success: function (result) {
            if (result.Success) {
                FinalizarDevolucaoTotal();
            }
            else {
                ExibirModalAcessoCoordenadorDevolucaoTotal();
            }
        }
    });
};


function FinalizarDevolucaoTotal() {
    var idLote = $('#IdLote').val();

    $.ajax({
        url: HOST_URL + CONTROLLER_PATH + "/finalizarDevolucaoTotal",
        cache: false,
        method: "POST",
        data: {
            idLote: idLote
        },
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






