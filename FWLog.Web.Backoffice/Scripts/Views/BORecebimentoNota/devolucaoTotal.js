(function () {
    $("#finalizarDevolucaoTotal").click(function () {
        ValidarPermissaoDevolucaoTotal();
    });

    $("#validarAcessoCoordenadorDevolucaoTotal").click(function () {
        ValidarAcessoDevolucaoTotal();
    });
})();




function ExibirResumoTratativaDivergencia(id) {
    $(".close").click();
    let $modal = $("#modalProcessamentoTratativaDivergencia");


    $modal.load(HOST_URL + CONTROLLER_PATH + "ResumoProcessamentoDivergencia/" + id, function () {
        $modal.modal();
        $('input').iCheck({ checkboxClass: 'icheckbox_flat-green' });
        //FinalizarTratativa();
    });

}

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
                VerificarStatusLoteDev($("#IdNotaFiscal").val());
                ExibirResumoTratativaDivergencia($("#IdNotaFiscal").val());
            }
        }
    });
}


function VerificarStatusLoteDev(id) {
    debugger
    $.ajax({
        url: HOST_URL + "BORecebimentoNota/ContinuarProcessamentoLote/" + id,
        cache: false,
        method: "POST",
        success: function (result) {
            if (!result.Success) {
                PNotify.error({ text: result.Message });
            } else {
                $(".close").click();
                $("#dataTable").DataTable().ajax.reload();

                if (result.Data !== "True") {
                    return;
                }

                PNotify.info({ text: "Continuando processo de finalização da tratativa de divergência..." });
                let $modal = $("#modalProcessamentoTratativaDivergencia");
                $modal.load(HOST_URL + CONTROLLER_PATH + "ResumoProcessamentoDivergencia/" + id, function () {
                    $modal.modal();
                    $('input').iCheck({ checkboxClass: 'icheckbox_flat-green' });
                    FinalizarTratativa();
                });
            }
        }
    });
}




