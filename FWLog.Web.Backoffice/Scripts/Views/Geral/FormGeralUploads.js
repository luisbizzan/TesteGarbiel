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
        let id = $(this).attr("data-id");
        let arquivo = $(this).attr("data-arquivo");
        let tabela = $(this).attr("data-tabela");

        $.confirm({
            type: 'red',
            theme: 'material',
            title: 'Excluir',
            content: 'Tem Certeza?',
            typeAnimated: true,
            autoClose: 'cancelar|10000',
            buttons: {
                confirmar: {
                    text: 'Excluir',
                    btnClass: 'btn-red',
                    action: function () {
                        ExcluirUpload(id, arquivo, tabela);
                    }
                },
                cancelar: {
                    text: 'Cancelar',
                    action: function () {
                    }
                }
            }
        });
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

function ExcluirUpload(Id, Arquivo, Tabela) {
    $.ajax({
        url: "/Geral/ExcluirUpload",
        type: "POST",
        data: {
            Id: Id,
            Arquivo: Arquivo,
            Id_Categoria: $("#Id_Categoria").val(),
            Tabela: Tabela,
            Id_Ref: $("#Id_Ref").val()
        },
        success: function (result) {
            if (result.Success) {
                PNotify.success({ text: result.Message });
                ListarUploads($("#Id_Categoria").val(), $("#Id_Ref").val());
            }
        },
        error: function (result) {
            PNotify.warning({ text: result.Message });
        }
    });
}