(function () {
    $.validator.setDefaults({ ignore: null });
    $('.onlyNumber').mask('0#');

    $('#dataTable').DataTable({
        ajax: {
            "url": view.pageDataUrl,
            "type": "POST",
            "data": function (data) {
                dart.dataTables.saveFilterToData(data);
            },
        },
        //deferLoading: 0,
        //initComplete: function (settings, json) {
        //    dart.dataTables.addEventsForDropdownAutoposition($('#dataTable'));
        //},
        //stateSaveParams: function (settings, data) {
        //    dart.dataTables.saveFilterToData(data);
        //},
        columns: [
            { "defaultContent": "" },
            { data: 'CodigoEndereco' },
            { data: 'IdUsuarioInstalacao' },
            { data: 'ReferenciaProduto' },
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

})();
