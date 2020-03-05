(function () {
    $('.onlyNumber').mask('0#');
    $('.money').mask("#.##0,00", { reverse: true });

    $("#RegistrarNotaRecebimento").click(function () {
        validarNotaRecebimentoDiv();
    });

    $("#pesquisarFornecedorModal").click(function () {
        $("#modalFornecedorNotaRecebimento").load(HOST_URL + "BOFornecedor/SearchModal/NotaRecebimentoDiv", function () {
            $("#modalFornecedorNotaRecebimento").modal();
        });
    });

    function limparFornecedorNotaRecebimento() {
        $("#NomeFornecedor").val("");
        $("#IdFornecedor").val("");
    }

    $("#limparFornecedorNotaRecebimento").click(function () {
        limparFornecedorNotaRecebimento();
    });
})();


function validarNotaRecebimentoDiv() {
    var chaveAcesso       = $('#ChaveAcesso').val();
    var idFornecedor      = $('#IdFornecedor').val();
    var numeroNF          = $('#NumeroNF').val();
    var serie             = $('#Serie').val();
    var valor             = $('#Valor').val();
    var quantidadeVolumes = $('#QuantidadeVolumes').val();

    $.ajax({
        url: HOST_URL + CONTROLLER_PATH + "validarNotaRecebimento",
        method: "POST",
        cache: false,
        data: {
            chaveAcesso      : chaveAcesso,
            idFornecedor     : idFornecedor,
            numeroNF         : numeroNF,
            serie            : serie,
            valor            : valor,
            quantidadeVolumes: quantidadeVolumes
        },
        success: function (result) {
            if (result.Success) {
                registrarNotaRecebimentoDiv();
            } else {
                PNotify.warning({ text: result.Message });
            }
        }
    });
}

function registrarNotaRecebimentoDiv() {
    var chaveAcesso       = $('#ChaveAcesso').val();
    var idFornecedor      = $('#IdFornecedor').val();
    var numeroNF          = $('#NumeroNF').val();
    var serie             = $('#Serie').val();
    var valor             = $('#Valor').val();
    var quantidadeVolumes = $('#QuantidadeVolumes').val();

    $.ajax({
        url: HOST_URL + CONTROLLER_PATH + "salvarNotaRecebimentoDiv",
        method: "POST",
        cache: false,
        data: {
            chaveAcesso       : chaveAcesso,
            idFornecedor      : idFornecedor,
            numeroNF          : numeroNF,
            serie             : serie,
            valor             : valor,
            quantidadeVolumes : quantidadeVolumes
        },
        success: function (result) {
            if (result.Success) {
                PNotify.success({ text: result.Message });
                $('#modalNotaRecebimento').modal('toggle');
                $("#modalImpressoras").load("BOPrinter/Selecionar?idImpressaoItem=9&acao=etqrecebimentosemnota&id=" + result.Data, function () {
                    $("#modalImpressoras").modal();
                });
                $("#dataTable").DataTable().ajax.reload();
            } else {
                PNotify.warning({ text: result.Message });
            }
        }
    });
}

