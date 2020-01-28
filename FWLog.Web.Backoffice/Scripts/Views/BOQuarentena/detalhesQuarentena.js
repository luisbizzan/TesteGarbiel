(function () {
    $('.onlyNumber').mask('0#');
    $('.codigoConfirmacao').css({'display': 'none'});

    $("#status").change(function () {
        var status = this.value;

        if (status == 2) {
            $(".codigoConfirmacao").css({ 'display': 'block' });
        }
        else {
            $(".codigoConfirmacao").css({ 'display': 'none' });
        }
    });

    $("#submit").click(function (e) {
        e.preventDefault();

        var dados = $("#detalhesQuarentenaForm").serializeArray();

        $.ajax({
            url: "/BOQuarentena/DetalhesQuarentena",
            method: "POST",
            data: dados,
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