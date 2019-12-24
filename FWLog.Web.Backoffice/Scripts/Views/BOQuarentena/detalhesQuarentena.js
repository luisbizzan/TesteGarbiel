(function () {
    $("#status").change(function () {
        var status = this.value;

        if (status != 4) {
            $("#codigoConfirmacao").val("");
        }

        $("#codigoConfirmacao").prop("disabled", status != 4);
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
                    PNotify.error({ text: result.Message });
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