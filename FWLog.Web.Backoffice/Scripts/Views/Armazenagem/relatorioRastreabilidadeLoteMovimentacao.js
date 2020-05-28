(function () {
    $('.onlyNumber').mask('0#');

    $.validator.addMethod('validateQtdFinal', function (value, ele) {
        var qtdeInicial = $("#Filter_QuantidadeInicial").val();

        if (!value || !qtdeInicial) {
            return true;
        }

        value = parseInt(value);
        qtdeInicial = parseInt(qtdeInicial);

        if (value >= qtdeInicial)
            return true;
        else
            return false;
    }, 'Quantidade final deve ser maior que a quantidade inicial');

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

    var options = {
        stateSave: false,
        info: false,
        ajax: {
            "url": view.pageDataUrl,
            "type": "POST",
            "data": function (data) {
                dart.dataTables.saveFilterToData(data);
            }
        },
        initComplete: function (settings, json) {
            dart.dataTables.addEventsForDropdownAutoposition($('#dataTable'));
        },
        order: [[5, "ASC"]],
        columns: [
            { data: 'ReferenciaProduto' },
            { data: 'DescricaoProduto' },
            { data: 'NroVolume' },
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
    };

    $("#pesquisarUsuarioMovimentacao").click(function () {
        $("#modalUsuarioMovimentacao").load(HOST_URL + "BOAccount/SearchModal/Recebimento", function () {
            $("#modalUsuarioMovimentacao").modal();
        });
    });

    function limparUsuarioMovimentacao() {
        $("#Filter_UserNameMovimentacao").val("");
        $("#Filter_IdUsuarioMovimentacao").val("");
    }

    $("#limparUsuarioMovimentacao").click(function () {
        limparUsuarioMovimentacao();
    });

    //Necessário pois o idLote e o idProduto não podem ser limpados.
    function limparFiltros() {
        var $form = $('[data-filter="form"]');
        var $dataTable = $('#' + $form.attr('data-filter-for'));

        $form[0].reset();
        $form.find('input#Filter_UserNameMovimentacao').val('');
        $form.find('input#Filter_IdUsuarioMovimentacao').val('');
        $form.find('input#Filter_QuantidadeInicial').val('');
        $form.find('input#Filter_QuantidadeFinal').val('');
        $form.find('input#Filter_DataHoraInicial').val('');
        $form.find('input#Filter_DataHoraFinal').val('');

        // Necessário dar trigger no evento 'change' dos inputs para que force a atualização,
        // de componentes de data e outros se necessário.
        $form.find('input').trigger('change');

        if (!$form.valid())
            return;

        $dataTable.DataTable().search('').draw();
        NProgress.start();
    }

    $("#limparFiltros").click(function () {
        limparFiltros();
    });

    $('#dataTable').dataTable(options);
    dart.dataTables.loadFormFilterEvents();
})();

function setUsuario(idUsuario, nomeUsuario, origem) {
    $("#Filter_UserNameMovimentacao").val(nomeUsuario);
    $("#Filter_IdUsuarioMovimentacao").val(idUsuario);
    $("#modalUsuarioMovimentacao").modal("hide");
    $("#modalUsuarioMovimentacao").empty();
}