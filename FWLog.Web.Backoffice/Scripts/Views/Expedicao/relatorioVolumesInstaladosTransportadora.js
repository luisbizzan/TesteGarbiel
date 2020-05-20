(function () {
    $.validator.setDefaults({ ignore: null });
    $('.onlyNumber').mask('0#');

    $(document.body).on('click', "#pesquisar", function () {
        $("#tabelaResultado").show();
    });

    $('#dataTable').DataTable({
        ajax: {
            "url": view.pageDataUrl,
            "type": "POST",
            "data": function (data) {
                dart.dataTables.saveFilterToData(data);
            },
            "error": function (data) {
                if (!!(data.statusText)) {
                    NProgress.done();
                }
            }
        },
        deferLoading: 0,
        initComplete: function (settings, json) {
            dart.dataTables.addEventsForDropdownAutoposition($('#dataTable'));
        },
        stateSaveParams: function (settings, data) {
            dart.dataTables.saveFilterToData(data);
        },
        columns: [
            { data: 'Transportadora' },
            { data: 'CodigoEndereco' },
            { data: 'NumeroPedido' },
            { data: 'NumeroVolume' }
        ]
    });

    $('#dataTable').dataTable.error = function (settings, helpPage, message) {
        console.log(message)
    };

    dart.dataTables.loadFormFilterEvents();

    $("#pesquisarTransportadora").click(function () {
        $("#modalPesquisaTransportadora").load(HOST_URL + "Transportadora/SearchModal", function () {
            $("#modalPesquisaTransportadora").modal();
        });
    });

    $("#limparTransportadora").click(function () {
        $("#Filter_NomeTransportadora").val("");
        $("#Filter_IdTransportadora").val("");
    });

    $("#pesquisarPedidoVenda").click(function () {
        $("#modalPesquisaPedidoVenda").load(HOST_URL + "PedidoVenda/PesquisaModal/", function () {
            $("#modalPesquisaPedidoVenda").modal();
        });
    });

    $("#limparPedidoVenda").click(function () {
        $("#Filter_NumeroPedidoVenda").val("");
        $("#Filter_IdPedidoVenda").val("");
    });

    //$("#downloadRelatorioVolumesInstaladosTransportadora").click(function () {

    //    if ($("#relatorioVolumesInstaladosTransportadoraForm").valid()) {
    //        $.ajax({
    //            url: "/Expedicao/DownloadRelatorioVolumesInstaladosTransportadora",
    //            method: "POST",
    //            data: {
    //                IdTransportadora: $("#Filter_IdTransportadora").val(),
    //                IdPedidoVenda: $("#Filter_IdPedidoVenda").val(),
    //                CorredorInicial: $("#Filter_CorredorInicial").val(),
    //                CorredorFinal: $("#Filter_CorredorFinal").val()
    //            },
    //            xhrFields: {
    //                responseType: 'blob'
    //            },
    //            success: function (data) {
    //                var a = document.createElement('a');
    //                var url = window.URL.createObjectURL(data);

    //                a.href = url;
    //                a.download = 'Relatório Totalização por Localização.pdf';
    //                document.body.append(a);
    //                a.click();
    //                a.remove();
    //                window.URL.revokeObjectURL(url);
    //            }
    //        });
    //    }
    //});

    //$("#imprimirRelatorioTotalizacaoLocalizacao").click(function () {
    //    if ($("#relatorioTotalizacaoLocalizacaoForm").valid()) {
    //        $("#modalImpressoras").load(HOST_URL + "BOPrinter/Selecionar?idImpressaoItem=1&acao=totalLocalizacao", function () {
    //            $("#modalImpressoras").modal();
    //        });
    //    }
    //});

})();

function setTransportadora(idTransportadora, nome) {
    $("#Filter_NomeTransportadora").val(nome);
    $("#Filter_IdTransportadora").val(idTransportadora);
    $("#modalPesquisaTransportadora").modal("hide");
    $("#modalPesquisaTransportadora").empty();
}

function selecionarPedidoVenda(idPedidoVenda, numero) {
    $("#Filter_NumeroPedidoVenda").val(numero);
    $("#Filter_IdPedidoVenda").val(idPedidoVenda);
    $("#modalPesquisaPedidoVenda").modal("hide");
    $("#modalPesquisaPedidoVenda").empty();
}

//function imprimir(acao, id) {
//    switch (acao) {
//        case 'totalLocalizacao':
//            $.ajax({
//                url: "/Armazenagem/ImprimirRelatorioVolumesInstaladosTransportadora",
//                method: "POST",
//                data: {
//                    IdImpressora: $("#IdImpressora").val(),
//                    IdTransportadora: $("#Filter_IdTransportadora").val(),
//                    IdPedidoVenda: $("#Filter_IdPedidoVenda").val(),
//                    CorredorInicial: $("#Filter_CorredorInicial").val(),
//                    CorredorFinal: $("#Filter_CorredorFinal").val()
//                },
//                success: function (result) {
//                    if (result.Success) {
//                        PNotify.success({ text: result.Message });
//                    } else {
//                        PNotify.error({ text: result.Message });
//                    }
//                    $('#modalImpressoras').modal('toggle');
//                    waitingDialog.hide();
//                }
//            });
//            break;
//    }
//}