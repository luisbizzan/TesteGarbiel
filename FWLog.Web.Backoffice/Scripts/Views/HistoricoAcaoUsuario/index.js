(function () {
    $.validator.setDefaults({ ignore: null });
    $('.onlyNumber').mask('0#');
    $("dateFormat").mask("99/99/9999");
    $('.hourMinute').mask("99:99", { reverse: true });

    var $DataInicial = $('#Filter_DataInicial').closest('.date');
    var $DataFinal = $('#Filter_DataFinal').closest('.date');
    

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

    $('#dataTable').DataTable({
        ajax: {
            "url": view.pageDataUrl,
            "type": "POST",
            "data": function (data) {
                dart.dataTables.saveFilterToData(data);
            }
        },
        columns: [
            { data: 'Usuario', },
            { data: 'Descricao', },
            { data: 'ColetorAplicacaoDescricao', },
            { data: 'HistoricoColetorTipoDescricao', },
            { data: 'DataHora', },
        ]
    });

    dart.dataTables.loadFormFilterEvents();

    $("#pesquisarUsuario").click(function () {
        debugger
        $("#modalUsuario").load(HOST_URL + "BOAccount/SearchModal/Usuario", function () {
            $("#modalUsuario").modal();
        });
    });

    $("#limparUsuario").click(function () {
        limparUsuario();
    });

})();

function setUsuario(idUsuario, nomeUsuario, origem) {
    if (origem === "Usuario") {
        $("#Filter_UserNameUsuario").val(nomeUsuario);
        $("#Filter_IdUsuario").val(idUsuario);
        $("#modalUsuario").modal("hide");
        $("#modalUsuario").empty();
    }
}

function limparUsuario() {
    $("#Filter_UserNameUsuario").val("");
    $("#Filter_IdUsuario").val("");
}
