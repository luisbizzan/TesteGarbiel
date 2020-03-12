(function () {
    $('.onlyNumber').mask('0#');
    $('.codigoConfirmacao').css({ 'display': 'none' });
    $('.chaveAcesso').css({ 'display': 'none' });

    //Método feito quando seria necessário informar a chave de acesso para finalizar quarentena de devolução total - Verificar com Vitão
    /*$("#status").change(function () {
        var status = this.value;
        var loteStatus = $("#LoteStatus").val();

        if (status == 2 && loteStatus != "Finalizado(Dev. Total)") {
            $(".codigoConfirmacao").css({ 'display': 'block' });
        }
        else if (status == 2 && loteStatus == "Finalizado(Dev. Total)") {
            $(".chaveAcesso").css({ 'display': 'block' });
        }
        else {
            $(".codigoConfirmacao").css({ 'display': 'none' });
            $(".chaveAcesso").css({ 'display': 'none' });
        }
    });*/

    $("#status").change(function () {
        var status = this.value;
        var loteStatus = $("#LoteStatus").val();

        if (status == 2) {
            $(".codigoConfirmacao").css({ 'display': 'block' });
        }
        else {
            $(".codigoConfirmacao").css({ 'display': 'none' });
        }
    });

    /*
    $("#submit").click(function (e) {
        e.preventDefault();

        var model       = $("#detalhesQuarentenaForm").serializeArray();

        $.ajax({
            url: HOST_URL + CONTROLLER_PATH + "validarChaveAcessoIgualDaNFe",
            cache: false,
            method: "POST",
            data: model,
            success: function (result) {
                if (!!result.Success) {
                    $.ajax({
                        url: "/BOQuarentena/DetalhesQuarentena",
                        method: "POST",
                        data: model,
                        success: function (result) {
                            if (!!result.Success) {
                                PNotify.success({ text: result.Message });
                                fechaModal();
                            } else {
                                PNotify.warning({ text: result.Message });
                            }
                        }
                    });
                } else {
                    PNotify.warning({ text: result.Message });
                }
            }
        });
    });*/


    $("#submit").click(function (e) {
        e.preventDefault();

        var model = $("#detalhesQuarentenaForm").serializeArray();

        $.ajax({
            url: "/BOQuarentena/DetalhesQuarentena",
            method: "POST",
            data: model,
            success: function (result) {
                if (!!result.Success) {
                    PNotify.success({ text: result.Message });
                    fechaModal();
                } else {
                    PNotify.warning({ text: result.Message });
                }
            }
        });
    });
    function fechaModal() {
        var $modal = $("#modalAlterarStatus");

        $modal.modal("hide");
        $modal.empty();

        $('#dataTable').DataTable().ajax.reload(null, false);
    }
})();