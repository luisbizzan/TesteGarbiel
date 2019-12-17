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
        $("#modalImpressoras").load("BOPrinter/Selecionar?tipo=laser&acao=entconf&id=" + view_modal.idNotaFiscal, function () {
            $("#btnFecharModal").click();
            $("#modalImpressoras").modal();
        });
    });
})();