(function () {
    $('.onlyNumber').mask('0#');

    $(document.body).on('click', "#pesquisar", function () {
        $.when(validarOsFiltros()).then(function (success) {
            if (success) {
                $("#tabelaPosicaoInventario").show();
            }
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
                { "defaultContent": "" },
                { data: 'Codigo', width: '30px' },
                { data: 'IdLote', width: '30px' },
                { data: 'QuantidadeProdutoPorEndereco', width: '30px' },
            ],
            order: [[2, 'asc']],
            rowGroup: {
                dataSrc: ['Referencia']
            }
        });

        $('#dataTable').dataTable.error = function (settings, helpPage, message) {
            console.log(message)
        };

        dart.dataTables.loadFormFilterEvents();
    });

    $("#pesquisarNivelArmazenagem").click(function () {
        $("#modalPesquisaNivelArmazenagem").load(HOST_URL + "NivelArmazenagem/PesquisaModal", function () {
            $("#modalPesquisaNivelArmazenagem").modal();
        });
    });

    $("#limparNivelArmazenagem").click(function () {
        $("#Filter_DescricaoNivelArmazenagem").val("");
        $("#Filter_IdNivelArmazenagem").val("");
    });

    $("#pesquisarPontoArmazenagem").click(function () {
        let id = $("#Filtros_IdNivelArmazenagem").val();
        $("#modalPesquisaPontoArmazenagem").load(HOST_URL + "PontoArmazenagem/PesquisaModal/" + id, function () {
            $("#modalPesquisaPontoArmazenagem").modal();
        });
    });

    $("#limparPontoArmazenagem").click(function () {
        $("#Filter_DescricaoPontoArmazenagem").val("");
        $("#Filter_IdPontoArmazenagem").val("");
    });

    $("#pesquisarProduto").on('click', function () {
        if (!$(this).attr('disabled')) {
            $("#modalProduto").load(HOST_URL + "Produto/SearchModal", function () {
                $("#modalProduto").modal();
            });
        }
    });

    $("#limparProduto").click(function () {
        if (!$(this).attr('disabled')) {
            limparProduto();
        }
    });

    function limparProduto() {
        $("#Filter_IdProduto").val("");
        $("#Filter_DescricaoProduto").val("");
    }

    //$("#downloadRelatorioTotalEnderecoPorAla").click(function () {
    //    $.ajax({
    //        url: "/Armazenagem/DownloadRelatorioTotalPorAla",
    //        method: "POST",
    //        data: {
    //            IdNivelArmazenagem: $("#Filter_IdNivelArmazenagem").val(),
    //            IdPontoArmazenagem: $("#Filter_IdPontoArmazenagem").val(),
    //            CorredorInicial: $("#Filter_CorredorInicial").val(),
    //            CorredorFinal: $("#Filter_CorredorFinal").val(),
    //            ImprimirVazia: $("#Filter_ImprimirVazia").val(),
    //        },
    //        xhrFields: {
    //            responseType: 'blob'
    //        },
    //        success: function (data) {
    //            var a = document.createElement('a');
    //            var url = window.URL.createObjectURL(data);

    //            a.href = url;
    //            a.download = 'Relatório total por alas.pdf';
    //            document.body.append(a);
    //            a.click();
    //            a.remove();
    //            window.URL.revokeObjectURL(url);
    //        }
    //    });
    //});

    //$("#imprimirRelatorioTotalEnderecoPorAla").click(function () {
    //    $("#modalImpressoras").load(HOST_URL + "BOPrinter/Selecionar?idImpressaoItem=1&acao=totaPorlAla", function () {
    //        $("#modalImpressoras").modal();
    //    });
    //});

})();

function selecionarNivelArmazenagem(idNivelArmazenagem, descricao) {
    $("#Filter_DescricaoNivelArmazenagem").val(descricao);
    $("#Filter_IdNivelArmazenagem").val(idNivelArmazenagem);
    $("#modalPesquisaNivelArmazenagem").modal("hide");
    $("#modalPesquisaNivelArmazenagem").empty();
}

function selecionarPontoArmazenagem(idPontoArmazenagem, descricao) {
    $("#Filter_DescricaoPontoArmazenagem").val(descricao);
    $("#Filter_IdPontoArmazenagem").val(idPontoArmazenagem);
    $("#modalPesquisaPontoArmazenagem").modal("hide");
    $("#modalPesquisaPontoArmazenagem").empty();
}

function setProduto(idProduto, descricao) {
    $("#Filter_IdProduto").val(idProduto);
    $("#Filter_DescricaoProduto").val(descricao);
    $("#modalProduto").modal("hide");
    $("#modalProduto").empty();
}

function validarOsFiltros() {

    let success = true;

    $.ajax({
        url: HOST_URL + CONTROLLER_PATH + "ValidarPesquisaRelatorioPosicaoInventario",
        global: false,
        async: false,
        cache: false,
        method: "POST",
        data: {
            idProduto: $("#Filter_IdProduto").val(),
            idNivelArmazenagem: $("#Filter_IdNivelArmazenagem").val(),
            idPontoArmazenagem: $("#Filter_PontoArmazenagem").val()
        },
        success: function (result) {
            if (!result.Success) {
                PNotify.warning({ text: result.Message });
                success = result.Success;
            }
        },
        error: function () {
            PNotify.warning({ text: 'Não foi validar os filtros. Por favor, tente novamente!' });
        }
    });

    return success;
}


//function imprimir(acao, id) {
//    switch (acao) {
//        case 'totaPorlAla':
//            $.ajax({
//                url: "/Armazenagem/ImprimirRelatorioTotalPorAla",
//                method: "POST",
//                data: {
//                    IdImpressora: $("#IdImpressora").val(),
//                    IdNivelArmazenagem: $("#Filter_IdNivelArmazenagem").val(),
//                    IdPontoArmazenagem: $("#Filter_IdPontoArmazenagem").val(),
//                    CorredorInicial: $("#Filter_CorredorInicial").val(),
//                    CorredorFinal: $("#Filter_CorredorFinal").val(),
//                    ImprimirVazia: $("#Filter_ImprimirVazia").val(),
//                },
//                success: function (result) {
//                    mensagemImpressao(result);
//                    $('#modalImpressoras').modal('toggle');
//                    waitingDialog.hide();
//                }
//            });
//            break;
//    }
//}

