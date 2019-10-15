(function () {

    var $executionStart = $('#Filter_ExecutionDateStart').closest('.date');
    var $executionEnd = $('#Filter_ExecutionDateEnd').closest('.date');

    var createLinkedPickers = function () {

        var pickerStart = $executionStart.datetimepicker({
            locale: moment.locale(),
            format: 'L',
            allowInputToggle: true
        });

        var pickerEnd = $executionEnd.datetimepicker({
            locale: moment.locale(),
            useCurrent: false,
            format: 'L',
            allowInputToggle: true
        });

        new dart.DateTimePickerLink(pickerStart, pickerEnd, { ignoreTime: true });
    };

    createLinkedPickers();

    var actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                action: 'details',
                href: view.detailsUrl + '?id=' + full.IdBOLogSystem,
                visible: view.detailsVisible
            }
        ];
    });

    var options = {
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
        stateSaveParams: function (settings, data) {
            dart.dataTables.saveFilterToData(data);
        },
        stateLoadParams: function (settings, data) {
            dart.dataTables.loadFilterFromData(data);
        },
        order: [[4, "desc"]],
        columns: [
            { data: 'UserName' },
            { data: 'ActionType' },
            { data: 'Entity' },
            { data: 'IP' },
            {
                data: "ExecutionDate",
                type: 'date',
                render: function (data, type, full, meta) {
                    return moment(full.ExecutionDate).format('L LTS');
                }
            },
            actionsColumn
        ]
    };

    $('#dataTable').dataTable(options);

    dart.dataTables.loadFormFilterEvents();

})();
