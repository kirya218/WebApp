var scheduler = function ($) {
    function buildCell(i) {
        return function (row) {
            var cell = row.cells[i];
            if (!cell) return "";
            var result = "<div data-ticks='" + cell.ticks + "' data-allday='" + row.isAllDay + "' class='timePart' >";

            if (cell.day) {
                result += "<div class='awe-il'><span class='day' data-date='" + cell.date + "'>" + cell.day + "</span></div>";
            }

            if (cell.events) {
                awef.loop(cell.events, function (value) {
                    var style = '';
                    if (value.color) {
                        var color = 'fff';
                        var bcolor = value.color.replace('#', '');

                        if (hexToVal(bcolor) > 530) {
                            color = darken(bcolor);
                        }

                        style = 'style="background-color:' + value.color + '; color:#' + color + ';"';
                    }
                    result += '<div class="schEvent" data-id="' + value.id + '" ' + style + '>'
                        + (value.Time ? '<div class="schTime">' + value.time + '</div>' : "")
                        + '<button type="button" class="delEvent">&times;</button>'
                        + '<div class="eventTitle">' + value.title + '</div>'
                        + '</div>';
                });
            }

            result += '</div>';
            return result;
        };
    }

    function hexToVal(hex) {
        var r = parseInt(hex.substr(0, 2), 16),
            g = parseInt(hex.substr(2, 2), 16),
            b = parseInt(hex.substr(4, 2), 16);
        return r + g + b;
    }

    function darken(hex) {
        var r = parseInt(hex.substr(0, 2), 16),
            g = parseInt(hex.substr(2, 2), 16),
            b = parseInt(hex.substr(4, 2), 16);

        function f(c) {
            var h = Math.max(0, c - 170).toString(16);
            if (!h) h = '00';
            if (h.length == 1) h = '0' + h;
            return h;
        }

        return f(r) + f(g) + f(b);
    }

    return {
        buildCell: buildCell,
        init: function (gridId, popupName) {
            var g = $('#' + gridId);
            var o = g.data('o');
            var sched = g.closest('.scheduler');
            var api = o.api;
            var $viewType = sched.find('.viewType .awe-val');
            var fzOpt = { left: 1 };
            var bar = $('.schedBotBar[data-g='+gridId+']');

            awem.gridFreezeColumns(fzOpt)(o);

            g.on('awebfren', function () {
                var tag = o.lrs.tg;

                if (tag.View === 'Month') {
                    fzOpt.left = 0;
                    bar.hide();

                } else {
                    fzOpt.left = 1;
                    bar.show();
                }
            });

            g.on('aweload', function (e) {
                var data = o.lrs;
                var tag = data.tg;

                if ($viewType.val() !== tag.View) {
                    $viewType.val(tag.View).data('api').render();
                }

                sched.find('.schDate .awe-val').val(tag.Date);
                sched.find('.dateLabel').html(tag.DateLabel);
            })
                .on('click', '.eventTitle', function () {
                    awe.open('edit' + popupName, { params: { id: $(this).parent().data('id') } });
                })
                .on('click', '.delEvent', function () {
                    awe.open('delete' + popupName, { params: { id: $(this).closest('.schEvent').data('id') } });
                })
                .on('dblclick', 'td', function (e) {
                    var schdev = $(e.target).closest('.schEvent');
                    if (!schdev.length) {
                        var tp = $(this).find('.timePart');
                        awe.open('create' + popupName,
                            { params: { ticks: tp.data('ticks'), allDay: tp.data('allday') } });
                    } else {
                        if (!$(e.target).is('.delEvent'))
                            awe.open('edit' + popupName, { params: { id: schdev.data('id') } });
                    }
                })
                .on('click', '.day', function (e) {
                    if ($(e.target).is('.day')) {
                        api.load({ oparams: { viewType: 'Day', date: $(this).data('date') } });
                    }
                });
        }
    };
}(jQuery);