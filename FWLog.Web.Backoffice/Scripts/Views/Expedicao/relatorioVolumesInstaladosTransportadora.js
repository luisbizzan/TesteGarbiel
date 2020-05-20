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
            { defaultContent: '' },
            { data: 'CodigoEndereco' },
            { data: 'NumeroPedido' },
            { data: 'NumeroVolume' }
        ],
        rowGroup: {
            dataSrc: ['Transportadora']
        }
    });

    $('#dataTable').dataTable.error = function (settings, helpPage, message) {
        console.log(message)
    };

    dart.dataTables.loadFormFilterEvents();

    $("#pesquisarTransportadora").click(function () {
        limpaModais();

        $("#modalPesquisaTransportadora").load(HOST_URL + "Transportadora/SearchModal", function () {
            $("#modalPesquisaTransportadora").modal();
        });
    });

    $("#limparTransportadora").click(function () {
        $("#Filter_NomeTransportadora").val("");
        $("#Filter_IdTransportadora").val("");
    });

    $("#pesquisarTransportadoraEndereco").click(function () {
        limpaModais();

        $("#modalPesquisaTransportadoraEndereco").load(HOST_URL + "TransportadoraEndereco/PesquisaModal", function () {
            $("#modalPesquisaTransportadoraEndereco").modal();
        });
    });

    $("#limparTransportadoraEndereco").click(function () {
        $("#Filter_TransportadoraEndereco").val("");
        $("#Filter_EnderecoCodigo").val("");
    });

    $("#pesquisarPedidoVenda").click(function () {
        limpaModais();

        $("#modalPesquisaPedidoVenda").load(HOST_URL + "Expedicao/PedidoVendaPesquisaModal/", function () {
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

})();

function limpaModais() {
    $("#modalPesquisaTransportadora").empty();
    $("#modalPesquisaTransportadoraEndereco").empty();
    $("#modalPesquisaPedidoVenda").empty();
}

function setTransportadora(idTransportadora, nome) {
    $("#Filter_NomeTransportadora").val(nome);
    $("#Filter_IdTransportadora").val(idTransportadora);
    $("#modalPesquisaTransportadora").modal("hide");
    $("#modalPesquisaTransportadora").empty();
}

function selecionarTransportadoraEndereco(idEnderecoTransportadora, enderecoCodigo) {
    $("#Filter_TransportadoraEndereco").val(enderecoCodigo);
    $("#Filter_EnderecoCodigo").val(enderecoCodigo);
    $("#modalPesquisaTransportadoraEndereco").modal("hide");
    $("#modalPesquisaTransportadoraEndereco").empty();
}

function selecionarPedidoVenda(idPedidoVenda, numeroPedidoVenda) {
    $("#Filter_NumeroPedidoVenda").val(numeroPedidoVenda);
    $("#Filter_IdPedidoVenda").val(idPedidoVenda);
    $("#modalPesquisaPedidoVenda").modal("hide");
    $("#modalPesquisaPedidoVenda").empty();
}