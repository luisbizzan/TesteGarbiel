// Utilidades para o componente datatables.
(function (dart, resources, undefined) {

    dart.dataTables = (function () {

        var windowLoadDate = null;

        // Para que a barra de progresso não apareça várias vezes seguida ao carregar a página,
        // guardamos o momento que a página terminou de carregar e usamos para comparar com o tempo que passou.
        $(window).on('load', function () {
            windowLoadDate = new Date();
        });

        // Essa função faz com o que o dropdown abra para cima ou para baixo de acordo com o espaço disponível.
        var addEventsForDropdownAutoposition = function (dataTable) {
            $(dataTable).find('tbody').on("shown.bs.dropdown", ".btn-group", function () {
                var $ul = $(this).children(".dropdown-menu");
                var $button = $(this).children(".dropdown-toggle");
                var ulOffset = $ul.offset();
                var spaceUp = (ulOffset.top - $button.height() - $ul.height()) - $(window).scrollTop();
                var spaceDown = $(window).scrollTop() + $(window).height() - (ulOffset.top + $ul.height());
                if (spaceDown < 0 && (spaceUp >= 0 || spaceUp > spaceDown)) {
                    $(this).addClass("dropup");
                }
            }).on("hidden.bs.dropdown", ".btn-group", function () {
                $(this).removeClass("dropup");
            });
        };

        var loadFormFilterEvents = function ($form, validate) {

            if ($form == null) {
                $form = $('[data-filter="form"]');
            }
            var $clear = $form.find('[data-filter="clear"]');
            var $search = $form.find('[data-filter="search"]');
            var $dataTable = $('#' + $form.attr('data-filter-for'));


            $clear.on('click', function () {
                $form[0].reset();
                $form.find('input').val('');

                // Necessário dar trigger no evento 'change' dos inputs para que force a atualização,
                // de componentes de data e outros se necessário.
                $form.find('input').trigger('change');

                $search.trigger('click');
            });

            $search.on('click', function () {
                $dataTable.DataTable().search('').draw();
                NProgress.start();
            });

            $form.on('keydown', function (e) {
                var keycode = (e.keyCode ? e.keyCode : e.which);
                var enterKey = 13;
                if (e.keyCode === enterKey) {
                    $search.trigger('click');

                    // Queremos impedir que o formulário seja enviado.
                    e.preventDefault();
                }
            });

            $dataTable.on('preXhr.dt', function () {
                if (windowLoadDate === null) {
                    return;
                }

                var timeSinceWindowLoad = new Date() - windowLoadDate;
                if (timeSinceWindowLoad > 1000) {
                    NProgress.start();
                }
            });

            $dataTable.on('xhr.dt', function () {
                NProgress.done();
            });
        };

        var showFilter = function ($form) {
            var $collapse = $form.find('.collapse-link');
            var $BOX_PANEL = $($collapse).closest('.x_panel'),
                $ICON = $($collapse).find('i'),
                $BOX_CONTENT = $BOX_PANEL.find('.x_content');

            // fix for some div with hardcoded fix class
            if ($BOX_PANEL.attr('style')) {
                $BOX_CONTENT.css('display', 'block');
            } else {
                $BOX_CONTENT.css('display', 'block');
                $BOX_PANEL.css('height', 'auto');
            }

            $ICON.toggleClass('fa-chevron-up fa-chevron-down');
        };

        var loadFilterFromData = function (data) {

            if (!data.hasOwnProperty('CustomFilter')) {
                return;
            }

            var hasFilter = false;
            var $form = $('[data-filter="form"]');
            $form.find('[data-field]').each(function () {
                var field = $(this).attr('data-field');
                if (data.CustomFilter.hasOwnProperty(field)) {
                    $(this).val(data.CustomFilter[field]).trigger('change');

                    if (data.CustomFilter[field] !== '' && data.CustomFilter[field] !== null) {
                        hasFilter = true;
                    }
                }
            });

            if (hasFilter) {
                showFilter($form);
            }
        };

        var saveFilterToData = function (data, $form) {

            data.CustomFilter = {};

            if ($form == null) {
                $form = $('[data-filter="form"]');
            }
            $form.find('[data-field]').each(function () {
                var field = $(this).attr('data-field');
                data.CustomFilter[field] = $(this).val();
            });

            return data;
        };


        return {
            loadFormFilterEvents: loadFormFilterEvents,
            loadFilterFromData: loadFilterFromData,
            saveFilterToData: saveFilterToData,
            addEventsForDropdownAutoposition: addEventsForDropdownAutoposition
        };


    })();

})(window.dart = window.dart || {}, dart.resources);

// Render Actions Column
(function (dart, resources, undefined) {

    dart.dataTables = dart.dataTables || {};

    dart.dataTables.renderActionsColumn = function (actionFn) {

        var minActionsForDropdown = 4;

        var isDropdownMode = function (actions) {

            var visibleActions = 0;

            for (var i = 0; i < actions.length; i++) {
                if (actions[i].visible) {
                    visibleActions++;
                }
            }

            return visibleActions >= minActionsForDropdown;
        };

        var presetActions = [
            { name: 'details', element: 'a', icon: 'fa fa-eye', text: resources.get("ViewAction") },
            { name: 'edit', element: 'a', icon: 'fa fa-edit', text: resources.get("EditAction") },
            { name: 'delete', element: 'button', icon: 'fa fa-trash-o', text: resources.get("DeleteAction") },
            { name: 'select', element: 'button', icon: 'fa fa-check-circle', text: "Selecionar" },
        ];

        var getPresetActionFor = function (action) {
            for (var i = 0; i < presetActions.length; i++) {
                if (action.hasOwnProperty('action') && action.action === presetActions[i].name) {
                    return presetActions[i];
                }
            }
            return null;
        };

        var generateButtonElementForAction = function (action) {

            action.attrs.class = 'btn btn-row-actions btn-default';
            action.attrs.title = action.text;
            action.attrs.text = action.text;

            var element = 'button';
            if (action.hasOwnProperty('href')) {
                action.attrs.href = action.href;
                element = 'a';
            }

            action.attrs.html = $('<i/>', { 'class': action.icon })[0].outerHTML;
            return $('<' + element + '/>', action.attrs);
        };

        var generateLiElementForAction = function (action) {

            if (action.hasOwnProperty('href')) {
                action.attrs.href = action.href;
            } else {
                action.attrs.href = 'javascript:void(0);'
            }

            action.attrs.text = ' ' + action.text;

            var $li = $('<li/>');
            var $a = $('<a/>', action.attrs).prepend($('<i/>', { 'class': action.icon + ' fa-fw' }));

            return $li.append($a);
        };

        var generateContainerForActions = function (actions, isDropdownModeResult) {
            if (isDropdownModeResult) {
                var $html = $('<div/>', { 'class': 'btn-group' });

                var $button = $('<button/>', {
                    'class': 'btn btn-row-actions btn-default dropdown-toggle',
                    'data-toggle': 'dropdown',
                    'aria-haspopup': 'true',
                    'text': 'Ações'
                }).append(' <span class="caret"></span>');

                var $ul = $('<ul/>', { 'class': 'dropdown-menu dropdown-menu-right' });

                return $html.append($button, $ul);
            } else {
                return $('<div/>', { 'style': 'display:inline' });
            }
        };

        var getHtml = function (actions, isDropdownModeResult) {

            var $html = generateContainerForActions(actions, isDropdownModeResult);

            for (var i = 0; i < actions.length; i++) {

                if (!actions[i].visible) {
                    continue;
                }

                var presetAction = getPresetActionFor(actions[i]);
                var attrs = actions[i].attrs || {};

                if (presetAction !== null) {
                    action = {
                        href: actions[i].href,
                        text: presetAction.text,
                        icon: presetAction.icon,
                        visible: actions[i].visible,
                        attrs: attrs
                    };
                } else {
                    action = actions[i];
                }

                if (isDropdownModeResult) {
                    var element = generateLiElementForAction(action);
                    $html.find('ul').append(element);
                } else {
                    var element = generateButtonElementForAction(action);
                    $html.append(element);
                }
            }

            return $html[0].outerHTML;
        };

        return {
            sortable: false,
            searchable: false,
            className: "row-actions",
            width: 1,
            render: function (data, type, full, meta) {
                var actions = actionFn(data, type, full, meta);
                return getHtml(actions, isDropdownMode(actions));
            }
        };
    };

})(window.dart = window.dart || {}, dart.resources);

// Render Multiple Actions Column
(function (dart, resources, undefined) {

    dart.dataTables = dart.dataTables || {};

    //{
    //    dataName: string,
    //    dataTableIdentifier: string,
    //    actions: [
    //        {
    //            icon: string,
    //            text: string,
    //            visible: boolean,
    //            onClick: function
    //        }
    //    ],
    //    options: dataTableOptions
    //}
    dart.dataTables.multipleAction = function (dataName, dataTableIdentifier, actions, options) {
        var response = options;

        var buttons = [];

        var anyVisible = false;

        $.each(actions, function () {
            var actionItem = this;

            if (actionItem.visible) {
                anyVisible = true;

                buttons.push({
                    text: '<i class="' + actionItem.icon + '"></i> ' + actionItem.text,
                    action: function (e, dt, node, config) {
                        var items = dart.dataTables.getAllChecked();
                        actionItem.onClick(items);
                    }
                });
            }
        });

        if (anyVisible) {
            dart.dataTables.createCheckColumn(dataTableIdentifier);

            var dom = "<'row'<'col-xs-12 col-sm-6'l><'col-xs-12 col-sm-6'B>>" +
                "<'row'<'col-xs-12 col-sm-12'tr>>" +
                "<'row'<'col-xs-12 col-sm-5'i><'col-xs-12 col-sm-7'p>>";

            response.dom = dom;

            response.buttons = {
                dom: {
                    container: {
                        tag: 'div',
                        className: ''
                    },
                },
                buttons: [
                    {
                        extend: 'collection',
                        text: resources.get("MultipleActions") + ' <span class="caret"></span>',
                        className: 'btn btn-default dropdown-toggle pull-right hidden-xs',
                        background: false,
                        autoClose: true,
                        buttons: buttons
                    },
                    {
                        extend: 'collection',
                        text: resources.get("MultipleActions") + ' <span class="caret"></span>',
                        className: 'btn btn-default dropdown-toggle pull-left hidden-sm hidden-md hidden-lg',
                        background: false,
                        autoClose: true,
                        buttons: buttons
                    }
                ],
            };

            response.drawCallback = function (settings) {
                dart.dataTables.resizeCheckColumn();
            };

            if (!response.order) {
                response.order = [[1, 'asc']];
            }

            if (response.columns) {
                response.columns.splice(0, 0, {
                    width: 1,
                    data: null,
                    sortable: false,
                    searchable: false,
                    orderable: false,
                    className: "text-center",
                    render: function (data, type, full) {
                        if (type === 'display') {
                            return '<input type="checkbox" data-id="' + full[dataName] + '" class="check-dataTable" />';
                        }
                        return data;
                    },
                });
            }
        }

        return response;
    };

    dart.dataTables.getAllChecked = function () {
        var checkList = $('.check-dataTable:checked');

        var response = [];

        if (checkList.length == 0) {
            PNotify.warning({
                text: resources.get("NoItemsSelected")
            })
        }
        else {
            $.each(checkList, function () {
                response.push($(this).attr('data-id'));
            });
        }

        return response;
    };

    dart.dataTables.createCheckColumn = function (dataTableIdentifier) {
        var $tr = $(dataTableIdentifier).parents().find('tr');

        var $th = $('<th/>', { 'class': 'check-column' });

        var $input = $('<input/>', {
            'type': 'checkbox',
            'class': 'all-check-dataTable'
        });

        $input.appendTo($th);

        $tr.prepend($th);

        $('.all-check-dataTable').change(function (e) {
            $(this).parents().find('.check-dataTable').prop('checked', $(this).is(':checked'));
        });
    };

    dart.dataTables.resizeCheckColumn = function () {
        var paddingLeft = $('.check-dataTable').parent().css('padding-left');
        $('.check-column').css('padding-left', paddingLeft);
    };

    $(window).resize(function () {
        setTimeout(function () {
            dart.dataTables.resizeCheckColumn();
        }, 1500)
    });

})(window.dart = window.dart || {}, dart.resources);