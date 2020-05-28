(function () {
    $('.onlyNumber').mask('0#');

    $("#finalizarDevolucaoTotal").click(function () {
        ValidarPermissaoDevolucaoTotal();
    });

    $("#validarAcessoCoordenadorDevolucaoTotal").click(function () {
        ValidarAcessoDevolucaoTotal();
    });
})();

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
                VerificarStatusLote();

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
                VerificarStatusLote();
            }
            else {
                ExibirModalAcessoCoordenadorDevolucaoTotal();
            }
        }
    });
};

function VerificarStatusLote() {
    debugger
    let id = $("#IdNotaFiscal").val()
    let quantidadeEtiqueta = $('#QuantidadeEtiqueta').val();

    $.ajax({
        url: HOST_URL + "BORecebimentoNota/ContinuarProcessamentoLote/" + id,
        cache: false,
        method: "POST",
        success: function (result) {
            if (!result.Success) {
                PNotify.error({ text: result.Message });
            } else {
                $(".fecharModal").click();

                if (result.Data !== "True") {
                    return;
                }

                let $modal = $("#modalProcessamentoDevolucaoTotal");
                $modal.load(HOST_URL + CONTROLLER_PATH + "ResumoProcessamentoDevolucaoTotal?id=" + id + "&quantidadeEtiqueta=" + quantidadeEtiqueta, function () {
                    $modal.modal();
                    $('input').iCheck({ checkboxClass: 'icheckbox_flat-green' });
                });
            }
        }
    });
}




