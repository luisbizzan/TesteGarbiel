(function () {

    var $createdStart = $('#Filter_CreatedStart').closest('.date');
    var $createdEnd = $('#Filter_CreatedEnd').closest('.date');

    var createLinkedPickers = function () {

        var pickerStart = $createdStart.datetimepicker({
            locale: moment.locale(),
            format: 'L',
            allowInputToggle: true
        });

        var pickerEnd = $createdEnd.datetimepicker({
            locale: moment.locale(),
            useCurrent: false,
            format: 'L',
            allowInputToggle: true
        });

        new dart.DateTimePickerLink(pickerStart, pickerEnd, { ignoreTime: true });
    };
    
    var actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                action: 'details',
                href: view.detailsUrl + '?id=' + full.IdApplicationLog,
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
        order: [[3, "desc"]],
        columns: [
            { data: 'Level' },
            { data: 'Message' },
            { data: 'ApplicationName' },
            {
                data: "Created",
                render: function (data, type, full, meta) {
                    return moment(full.Created).format('L LTS');
                }
            },
            actionsColumn
        ]
    };

    // DataTable
    $('#dataTable').DataTable(options);

    var load = function () {
        dart.dataTables.loadFormFilterEvents();
        createLinkedPickers();
    };

    load();

})();