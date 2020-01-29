(function () {
    $("#downloadDetalhesNotaEntradaConferencia").click(function () {
        $.ajax({
            url: '/BORecebimentoNota/DownloadDetalhesNotaEntradaConferencia/' + view_modal.idNotaFiscal,
            method: "GET",
            xhrFields: {
                responseType: 'blob'
            },
            data: { },
            success: function (data) {
                var a = document.createElement('a');
                var url = window.URL.createObjectURL(data);
                a.href = url;
                a.download = 'Detalhes Nota Entrada Conferencia ' + view_modal.idNotaFiscal +'.pdf';
                document.body.append(a);
                a.click();
                a.remove();
                window.URL.revokeObjectURL(url);
            }
        });
    });

    $("#imprimirDetalhesNotaEntradaConferencia").click(function () {
        $("#modalImpressoras").load("BOPrinter/Selecionar?idImpressaoItem=1&acao=entconf&id=" + view_modal.idNotaFiscal, function () {
            $("#btnFecharModal").click();
            $("#modalImpressoras").modal();
        });
    });

    $(".bar_tabs").click(function () {
        if (!$("#entradaConferencia").is(":visible")) {
            $(".impressao").show();
        }
        else {
            $(".impressao").hide();
        }
    });

    $(".informacoes").find("label").css("margin-top", "3px");
})();