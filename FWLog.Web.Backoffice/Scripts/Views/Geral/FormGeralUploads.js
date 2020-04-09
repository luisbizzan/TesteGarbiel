(function () {
    $('#Arquivo').on('change', function (e) {
        var files = e.target.files;
        if (files.length > 0) {
            if (window.FormData !== undefined) {
                var data = new FormData();
                for (var x = 0; x < files.length; x++) {
                    data.append("file" + x, files[x]);
                }

                $.ajax({
                    type: "POST",
                    url: '/Geral/GravarUpload?Id_Categoria=' + $("#Id_Categoria").val() + '&Id_Ref=' + $("#Id_Ref").val(),
                    contentType: false,
                    processData: false,
                    data: data,
                    success: function (result) {
                        var resultObject = JSON.parse(result);

                        if (resultObject.isUploaded == true)
                            PNotify.success({ text: "Upload Concluido Com Sucesso." });
                        else
                            PNotify.error({ text: JSON.stringify(resultObject.message) });

                        ListarUploads($("#Id_Categoria").val(), $("#Id_Ref").val());
                    },
                    error: function (xhr, status, p3, p4) {
                        $('#Imagem').val("");
                        $.LoadingOverlay("hide");
                        PNotify.error({ text: "Falha No Upload." });
                    }
                });
            } else {
                PNotify.error({ text: "Sem suporte a HTML5!" });
            }
        }
    });

    $('.btn-excluir-upload').on('click', function (e) {
        ExcluirUpload($(this).attr("data-id"), $(this).attr("data-arquivo"));
    });
})();

function ListarUploads(Id_Categoria, Id_Ref) {
    $("#modalPadrao").load("/Geral/ListarUploads", {
        Id_Categoria: Id_Categoria,
        Id_Ref: Id_Ref,
    }, function () {
        $("#modalPadrao").modal();
    });
}

function ExcluirUpload(Id, Arquivo) {
    $.confirm({
        title: 'Confirm!',
        content: 'Simple confirm!',
        buttons: {
            confirm: function () {
                $.alert('Confirmed!');
            },
            cancel: function () {
                $.alert('Canceled!');
            },
            somethingElse: {
                text: 'Something else',
                btnClass: 'btn-blue',
                keys: ['enter', 'shift'],
                action: function () {
                    $.alert('Something else?');
                }
            }
        }
    });

    //$.ajax({
    //    url: "/Geral/ExcluirUpload",
    //    type: "GET",
    //    data: {
    //        Id: Id,
    //        Arquivo: Arquivo
    //        Id_Categoria: $("#Id_Categoria").val(),
    //        Id_Ref: $("#Id_Ref").val()
    //    },
    //    success: function (result) {
    //        if (result.success == true) {
    //            PNotify.success({ text: "Item excluido com sucesso!" });
    //            ListarUploads($("#Id_Categoria").val(), $("#Id_Ref").val());
    //        }

    //    },
    //    error: function (data) {
    //        $.toaster({ priority: "danger", title: "Erro", message: "Falha ao salvar!" });
    //    }
    //});
}