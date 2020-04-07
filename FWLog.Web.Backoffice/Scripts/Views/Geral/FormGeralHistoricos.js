(function () {
    $('#formHist').submit(function (e) {
        e.preventDefault();

        $.ajax({
            type: "POST",
            url: "/Geral/GravarHistorico",
            data: {
                Historico: $("#Historico").val(),
                Id_Ref: $("#Id_Ref").val(),
                Id_Categoria: $("#Id_Categoria").val(),
            },
            success: function (result) {
                if (result.Success) {
                    ListarHistoricos($("#Id_Categoria").val(), $("#Id_Ref").val());
                    PNotify.success({ text: result.Message });
                } else {
                    PNotify.error({ text: result.Message });
                }
            },
            error: function (request, status, error) {
                PNotify.warning({ text: result.Message });
            }
        });
    });
})();

function ListarHistoricos(Id_Categoria, Id_Ref) {
    $("#modalPadrao").load("/Geral/ListarHistoricos", {
        Id_Categoria: Id_Categoria,
        Id_Ref: Id_Ref,
    }, function () {
        $("#modalPadrao").modal();
    });
}