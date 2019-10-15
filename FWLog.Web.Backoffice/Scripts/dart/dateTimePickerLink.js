// Cria um link entre dois datepickers, especificando um date picker para data mínima e outro para data máxima.
(function (dart, resources, undefined) {

    // options.ignoreTime:
    //      Define se o horário deve ser levado em consideração ao setar a hora máxima e mínima.
    //      Utilizado para datepickers sem horário.
    dart.DateTimePickerLink = function (datePickerStart, datePickerEnd, options) {      

        options = options || {};
        options.ignoreTime = options.ignoreTime || false;

        var $datePickerStart = $(datePickerStart);
        var $datePickerEnd = $(datePickerEnd);

        var getMaxDate = function (endDate, ignoreTime) {
            if (endDate === null) {
                return false;
            }
            if (ignoreTime) {
                return moment(endDate).endOf('day');
            }
            return endDate;
        };

        var getMinDate = function (startDate, ignoreTime) {
            if (startDate === null) {
                return false;
            }
            if (ignoreTime) {
                return moment(startDate).startOf('day');
            }
            return startDate;
        };

        var updateMinAndMaxDates = function (elementStart, elementEnd) {
            var startData = elementStart.data('DateTimePicker');
            var endData = elementEnd.data('DateTimePicker');

            var startDateBefore = startData.date();

            startData.maxDate(getMaxDate(endData.date(), options.ignoreTime));
            endData.minDate(getMinDate(startData.date(), options.ignoreTime));

            // Necessário setar a data inicial, porque quando a data inicial está em branco e
            // a data final é setada, o valor da data inicial é setado pelo plugin.
            if (!startDateBefore) {
                startData.date(startDateBefore);
            }
        };

        $datePickerStart.on("dp.change", function (e) {
            updateMinAndMaxDates($datePickerStart, $datePickerEnd, options.ignoreTime);
        });

        $datePickerEnd.on("dp.change", function (e) {
            updateMinAndMaxDates($datePickerStart, $datePickerEnd, options.ignoreTime);
        });

        // Update inicial para possível valor já existente no carregamento da página.
        updateMinAndMaxDates($datePickerStart, $datePickerEnd, options.ignoreTime);
    };

})(window.dart = window.dart || {}, dart.resources);