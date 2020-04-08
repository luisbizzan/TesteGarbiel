(function () {
    $.validator.setDefaults({ ignore: null });
    $('.onlyNumber').mask('0#');


    $.validator.addMethod('validateCorredorInicial', function (value, ele) {
        var corredorInicial = $("#Filter_CorredorInicial").val();
        var corredorFinal = $("#Filter_CorredorFinal").val();

        if (corredorInicial == "" && corredorFinal == "")
            return true
        else if (corredorInicial == "" && corredorFinal != "")
            return false
        else
            return true;
    }, 'Informe o corredor inicial para prosseguir');

    $.validator.addMethod('validateCorredorFinal', function (value, ele) {
        var corredorInicial = $("#Filter_CorredorInicial").val();
        var corredorFinal = $("#Filter_CorredorFinal").val();

        if (corredorInicial == "" && corredorFinal == "")
            return true
        else if (corredorInicial != "" && corredorFinal == "")
            return false
        else
            return true
    }, 'Informe o corredor final para prosseguir');


    $(document.body).on('click', "#pesquisar", function () {
        $("#tabelaTotalPorAlas").show();
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
            { data: 'CodigoEndereco' },
            { data: 'IdUsuarioInstalacao' },
            { data: 'ReferenciaProduto' },
            { data: 'IdLote' },
            { data: 'DataInstalacao' },
            { data: 'PesoProduto' },
            { data: 'QuantidadeProdutoPorEndereco' },
            { data: 'PesoTotalDeProduto' }
        ],
        order: [[2, 'asc']],
        rowGroup: {
            dataSrc: 'NumeroCorredor'
        }
    });

    $('#dataTable').dataTable.error = function (settings, helpPage, message) {
        console.log(message)
    };

    dart.dataTables.loadFormFilterEvents();

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

    $("#downloadRelatorioTotalEnderecoPorAla").click(function () {
        $.ajax({
            url: "/Armazenagem/DownloadRelatorioTotalPorAla",
            method: "POST",
            data: {
                IdNivelArmazenagem: $("#Filter_IdNivelArmazenagem").val(),
                IdPontoArmazenagem: $("#Filter_IdPontoArmazenagem").val(),
                CorredorInicial: $("#Filter_CorredorInicial").val(),
                CorredorFinal: $("#Filter_CorredorFinal").val(),
                ImprimirVazia: $("#Filter_ImprimirVazia").val(),
            },
            xhrFields: {
                responseType: 'blob'
            },
            success: function (data) {
                var a = document.createElement('a');
                var url = window.URL.createObjectURL(data);

                a.href = url;
                a.download = 'Relatório total por alas.pdf';
                document.body.append(a);
                a.click();
                a.remove();
                window.URL.revokeObjectURL(url);
            }
        });
    });

    $("#imprimirRelatorioTotalEnderecoPorAla").click(function () {
        $("#modalImpressoras").load(HOST_URL + "BOPrinter/Selecionar?idImpressaoItem=1&acao=totaPorlAla", function () {
            $("#modalImpressoras").modal();
        });
    });

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


function imprimir(acao, id) {
    switch (acao) {
        case 'totaPorlAla':
            $.ajax({
                url: "/Armazenagem/ImprimirRelatorioTotalPorAla",
                method: "POST",
                data: {
                    IdImpressora: $("#IdImpressora").val(),
                    IdNivelArmazenagem: $("#Filter_IdNivelArmazenagem").val(),
                    IdPontoArmazenagem: $("#Filter_IdPontoArmazenagem").val(),
                    CorredorInicial: $("#Filter_CorredorInicial").val(),
                    CorredorFinal: $("#Filter_CorredorFinal").val(),
                    ImprimirVazia: $("#Filter_ImprimirVazia").val(),
                },
                success: function (result) {
                    mensagemImpressao(result);
                    $('#modalImpressoras').modal('toggle');
                    waitingDialog.hide();
                }
            });
            break;
    }
}
