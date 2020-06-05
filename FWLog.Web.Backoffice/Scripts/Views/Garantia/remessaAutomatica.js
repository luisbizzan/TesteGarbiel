listarRemessa();

function listarRemessa() {
    let div = $("#listaRemessa");
    div.html("");

    div.load("/Garantia/RemessaAutomaticaListar", {
    }, function () {
        $('#tbRemessaLista').DataTable({
            destroy: true,
            serverSide: false,
            stateSave: false,

            dom: "Bfrtip",
            bInfo: true,
            buttons: [],
        });

        $('.chkStatus').iCheck({ checkboxClass: 'icheckbox_flat-green' });

        $('.chkStatus').on('ifChanged', function (event) {
            var id_status = $(this).prop("checked") ? 42 : 43;
            var id = $(this).data("id");

            $.ajax({
                type: "POST",
                url: "/Garantia/RemessaAutomaticaAlterarStatus",
                data: { Id: id, Id_Status: id_status },
                cache: false,
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        PNotify.success({ text: "Status salvo com sucesso!", delay: 1000 });
                        listarRemessa();
                    }
                },
                error: function (data) {
                    PNotify.warning({ text: result.Message });
                }
            });
        });
    });
}