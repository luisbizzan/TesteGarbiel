(function () {
    $.validator.setDefaults({ ignore: null });
    $('.onlyNumber').mask('0#');

    $.validator.addMethod('validateFilters', function (value, ele) {
        var produto = $("#Filter_DescricaoProduto").val();
        var dataHoraInicial = $("#Filter_DataHoraInicial").val(); 
        var dataHoraFinal = $("#Filter_DataHoraFinal").val(); 
        var userNameMovimentacao = $("#Filter_UserNameMovimentacao").val(); 
        var codigoEnderecoArmazenagem = $("#Filter_CodigoEnderecoArmazenagem").val(); 
        
        if (value != "")
            return true
        else if (produto != "")
            return true
        else if (dataHoraInicial != "" && dataHoraFinal != "")
            return true
        else if (userNameMovimentacao != "")
            return true
        else if (codigoEnderecoArmazenagem != "")
            return true
        else
            return false;
    }, 'Informe ao menos um filtro para prosseguir');

    var $DataInicial = $('#Filter_DataHoraInicial').closest('.date');
    var $DataFinal = $('#Filter_DataHoraFinal').closest('.date');

    var createLinkedPickers = function () {
        var dataInicial = $DataInicial.datetimepicker({
            locale: moment.locale(),
            format: 'L',
            allowInputToggle: true
        });

        var dataFinal = $DataFinal.datetimepicker({
            locale: moment.locale(),
            useCurrent: false,
            format: 'L',
            allowInputToggle: true
        });

        new dart.DateTimePickerLink(dataInicial, dataFinal, { ignoreTime: true });
    };

    createLinkedPickers();

    dart.dataTables.loadFormFilterEvents();

    $(document.body).on('click', "#pesquisar", function () {
        $("#tabela").show();
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
        order: [[6, "ASC"]],
        columns: [
            { data: 'IdLote' },
            { data: 'ReferenciaProduto' },
            { data: 'DescricaoProduto' },
            { data: 'Tipo' },
            { data: 'Quantidade' },
            { data: 'Endereco' },
            { data: 'DataHora' },
            { data: 'UsuarioMovimentacao' }
        ],
        createdRow: function (row, data, dataIndex) {
            if (data.Tipo == 'Entrada') {
                $(row).addClass('row-green');
            }
            else if (data.Tipo == 'Saida') {
                $(row).addClass('row-red');
            }
            else if (data.Tipo == 'Ajuste') {
                $(row).addClass('row-purple');
            }
            else if (data.Tipo == 'Abastecimento') {
                $(row).addClass('row-dark-green');
            }
        }
    });

    $('#dataTable').dataTable.error = function (settings, helpPage, message) {
        console.log(message)
    };

    $("#modalLote").on("hidden.bs.modal", function () {
        $("#modalLote").text('');
    });

    $("#pesquisarLote").on('click', function () {
        if (!$(this).attr('disabled')) {
            $("#modalLote").load(HOST_URL + "BORecebimentoNota/PesquisaLote", function () {
                $("#modalLote").modal();
            });
        }
    });

    $("#limparLote").click(function () {
        if (!$(this).attr('disabled')) {
            limparLote();
        }
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

    $("#pesquisarEnderecoArmazenagem").click(function () {
        let id = $("#IdPontoArmazenagem").val();
        $("#modalPesquisaEnderecoArmazenagem").load(HOST_URL + "EnderecoArmazenagem/PesquisaModal/" + id, function () {
            $("#modalPesquisaEnderecoArmazenagem").modal();
        });
    });

    $("#limparEnderecoArmazenagem").click(function () {
        $("#CodigoEnderecoArmazenagem").val("");
        $("#IdEnderecoArmazenagem").val("");
    });

    $("#pesquisarUsuarioMovimentacao").click(function () {
        $("#modalUsuarioMovimentacao").load(HOST_URL + "BOAccount/SearchModal/Recebimento", function () {
            $("#modalUsuarioMovimentacao").modal();
        });
    });

    $("#limparUsuarioMovimentacao").click(function () {
        limparUsuarioMovimentacao();
    });
})();

function setLote(idLote) {
    $("#Filter_IdLote").val(idLote);
    $("#modalLote").modal("hide");
    $("#modalLote").empty();
}

function limparLote() {
    $("#Filter_IdLote").val("");
}

function setProduto(idProduto, descricao) {
    $("#Filter_IdProduto").val(idProduto);
    $("#Filter_DescricaoProduto").val(descricao);
    $("#modalProduto").modal("hide");
    $("#modalProduto").empty();
}

function limparProduto() {
    $("#Filter_IdProduto").val("");
    $("#Filter_DescricaoProduto").val("");
}

function setUsuario(idUsuario, nomeUsuario, origem) {
    $("#Filter_UserNameMovimentacao").val(nomeUsuario);
    $("#Filter_IdUsuarioMovimentacao").val(idUsuario);
    $("#modalUsuarioMovimentacao").modal("hide");
    $("#modalUsuarioMovimentacao").empty();
}

function limparUsuarioMovimentacao() {
    $("#Filter_UserNameMovimentacao").val("");
    $("#Filter_IdUsuarioMovimentacao").val("");
}

function selecionarEnderecoArmazenagem(IdEnderecoArmazenagem, codigo) {
    $("#CodigoEnderecoArmazenagem").val(codigo);
    $("#IdEnderecoArmazenagem").val(IdEnderecoArmazenagem);
    $("#modalPesquisaEnderecoArmazenagem").modal("hide");
    $("#modalPesquisaEnderecoArmazenagem").empty();
}