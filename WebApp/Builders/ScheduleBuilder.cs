using Microsoft.SqlServer.Server;
using GridLibrary;
using System.Globalization;
using WebApp.Entities;
using WebApp.Enums.Schedule;
using WebApp.Models.Schedule;

namespace WebApp.Builders
{
    public class ScheduleBuilder
    {
        private readonly ScheduleBuilderModel _model;
        private readonly DateTimeFormatInfo _dateFormat = CultureInfo.CurrentCulture.DateTimeFormat;

        /// <summary>
        /// Get events based on start and end utc dates
        /// </summary>
        private Func<DateTime, DateTime, IEnumerable<Schedule>> GetEvents => (startUtc, endUtc) => _model.Schedules.Where(o => o.StartDate < endUtc && o.EndDate >= startUtc);

        public ScheduleBuilder(ScheduleBuilderModel model)
        {
            _model = model;
        }

        /// <summary>
        /// get view types for the button group
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<KeyContent> GetViewTypes()
        {
            var viewTypes = new Dictionary<ScheduleViewType, string>
            {
                {ScheduleViewType.Day, "День"},
                {ScheduleViewType.Week, "Неделя"},
                {ScheduleViewType.Month, "Месяц"}
            };

            return viewTypes.Select(o => new KeyContent(o.Key.ToString(), o.Value));
        }

        /// <summary>
        /// hours step dropdown items
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<KeyContent> GetHourSteps()
        {
            var hoursTypes = new Dictionary<int, string>
            {
                { 30, "30 мин" },
                { 60, "1 час" },
                { 120, "2 часа" },
                { 180, "3 часа" },
                { 300, "5 часов" },
                { 720, "12 часов" },
            };

            return hoursTypes.Select(o => new KeyContent(o.Key, o.Value));
        }

        /// <summary>
        /// build the scheduler view grid model
        /// </summary>
        /// <returns></returns>
        public GridModelDto<ScheduleRow> Build()
        {
            var clientDate = _model.DateSelected ?? DateTime.Now;

            GetPeriodForViewType(clientDate, out var periodStartClient, out var periodEndClient);

            var periodStartUtc = periodStartClient;
            var periodEndUtc = periodEndClient;

            var columns = GenerateColumnsForViewType(periodStartClient, periodEndClient);

            _model.GridParams.Columns = columns.ToArray();
            _model.GridParams.Paging = false;

            var events = GetEvents(periodStartUtc, periodEndUtc).ToArray();

            var rows = new List<ScheduleRow>();

            if (IsDayLikeView())
            {
                var allDayRow = new ScheduleRow { Time = "Весь день", IsAllDay = true };

                var allDayCells = new List<ScheduleCell>();
                var daysCount = GetDaysCount();

                for (var i = 0; i < daysCount; i++)
                {
                    var dayStartUtc = periodStartUtc.AddDays(i);
                    var allDayEvents = GetAllDayEvents(events, dayStartUtc);
                    var cell = BuildCell(allDayEvents, dayStartUtc);
                    allDayCells.Add(cell);
                }

                allDayRow.Cells = allDayCells;

                rows.Add(allDayRow);
            }

            if (_model.ViewType == ScheduleViewType.Month)
            {
                var weeksInPeriod = Math.Ceiling((periodEndUtc - periodStartUtc).TotalDays / 7);
                for (var week = 0; week < weeksInPeriod; week++)
                {
                    var row = new ScheduleRow();
                    var cells = new List<ScheduleCell>();

                    for (var day = 0; day < 7; day++)
                    {
                        var dayEvents = GetDayEvents(events, periodStartUtc.AddDays(day + week * 7));
                        var cell = BuildCell(dayEvents, periodStartUtc.AddDays(day + week * 7));
                        cells.Add(cell);
                    }

                    row.Cells = cells;
                    rows.Add(row);
                }
            }
            else if (IsDayLikeView())
            {
                var minutes = 0;
                var minutesUpperBound = 24 * 60;
                var stepMinutes = _model.HourStep;

                while (minutes < minutesUpperBound)
                {
                    var row = new ScheduleRow();
                    var partHour = minutes / 60;
                    var partMinute = minutes % 60;

                    var timePartStartUtc =
                        new DateTime(
                            periodStartClient.Year,
                            periodStartClient.Month,
                            periodStartClient.Day,
                            partHour,
                            partMinute,
                            0);

                    row.Time = (DateTime.Now.Date + new TimeSpan(partHour, partMinute, 0)).ToString(_dateFormat.ShortTimePattern);

                    var days = GetDaysCount();
                    var cells = new List<ScheduleCell>();

                    for (var day = 0; day < days; day++)
                    {
                        var startUtc = timePartStartUtc.AddDays(day);

                        var allDayIds = rows[0].Cells[day].Events.Select(o => o.Id);
                        var timePartEvents = GetTimePartEvents(
                            events, startUtc, startUtc.AddMinutes(stepMinutes), allDayIds);

                        var cell = BuildCell(timePartEvents, startUtc);

                        cells.Add(cell);
                    }

                    minutes += stepMinutes;
                    row.Cells = cells;
                    rows.Add(row);
                }
            }

            var model = new GridModelBuilder<ScheduleRow>(rows.AsQueryable(), _model.GridParams)
            {
                FrozenRows = IsDayLikeView() ? 1 : 0,
                Tag = new
                {
                    View = _model.ViewType.ToString(),
                    Date = clientDate.ToString(_dateFormat.ShortDatePattern),
                    DateLabel = GetDateToString(clientDate, periodStartClient, periodEndClient)
                }
            }.Build();

            return model;
        }

        private bool IsDayLikeView()
        {
            return _model.ViewType == ScheduleViewType.Day || _model.ViewType == ScheduleViewType.Week;
        }

        /// <summary>
        /// Получаем период в строке.
        /// </summary>
        /// <param name="date">Дата.</param>
        /// <param name="periodStartClient">Начало периода.</param>
        /// <param name="periodEndClient">Конец периода.</param>
        /// <returns>Строку периода.</returns>
        /// <exception cref="InvalidOperationException">Если данного типа не существует.</exception>
        private string GetDateToString(DateTime date, DateTime periodStartClient, DateTime periodEndClient)
        {
            return _model.ViewType switch
            {
                ScheduleViewType.Day => date.ToString(_dateFormat.LongDatePattern),
                ScheduleViewType.Week => periodStartClient.ToString(_dateFormat.MonthDayPattern) + " - " + periodEndClient.ToString(_dateFormat.MonthDayPattern),
                ScheduleViewType.Month => date.ToString(_dateFormat.YearMonthPattern),
                _ => throw new InvalidOperationException(nameof(_model.ViewType)),
            };
        }

        private List<Column> GenerateColumnsForViewType(DateTime periodStartClient, DateTime periodEndClient)
        {
            var columns = new List<Column>();

            if (_model.ViewType == ScheduleViewType.Day || _model.ViewType == ScheduleViewType.Week)
            {
                columns.Add(new Column { ClientFormat = "<div class='timeLabel'>.(Time)</div>", Width = 90 });
            }

            var index = 0;
            for (var day = periodStartClient; day < periodEndClient && index < 7; day = day.AddDays(1))
            {
                var header = _model.ViewType == ScheduleViewType.Month
                                 ? _dateFormat.GetDayName(day.DayOfWeek)
                                 : _dateFormat.GetAbbreviatedDayName(day.DayOfWeek) + " "
                                   + _dateFormat.GetAbbreviatedMonthName(day.Date.Month) + " " + day.Day;

                if (_model.ViewType == ScheduleViewType.Week)
                {
                    header = string.Format("<span class='day' data-date='{1}'>{0}</a>", header, day.ToShortDateString());
                }

                var column = new Column { ClientFormatFunc = "scheduler.buildCell(" + index + ")", Header = header };
                columns.Add(column);
                index++;
            }

            return columns;
        }

        /// <summary>
        /// Возвращает начальную дату и конечную дату от текущей даты.
        /// </summary>
        /// <param name="date">Дата.</param>
        /// <param name="periodStartClient">Начало периода.</param>
        /// <param name="periodEndClient">Конец периода.</param>
        /// <exception cref="InvalidOperationException">Если передан неизвестный тип.</exception>
        private void GetPeriodForViewType(DateTime date, out DateTime periodStartClient, out DateTime periodEndClient)
        {
            switch (_model.ViewType)
            {
                case ScheduleViewType.Day:
                    periodStartClient = date.Date;
                    periodEndClient = periodStartClient.AddDays(1);
                    break;
                case ScheduleViewType.Week:
                    periodStartClient = StartOfWeek(date, _dateFormat.FirstDayOfWeek).Date;
                    periodEndClient = periodStartClient.AddDays(7);
                    break;
                case ScheduleViewType.Month:
                    periodStartClient = StartOfWeek(new DateTime(date.Year, date.Month, 1), _dateFormat.FirstDayOfWeek).Date;
                    periodEndClient = StartOfWeek(new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month)), _dateFormat.FirstDayOfWeek).AddDays(7);
                    break;
                default:
                    throw new InvalidOperationException(nameof(_model.ViewType));
            }
        }

        private static IEnumerable<Schedule> GetDayEvents(IEnumerable<Schedule> events, DateTime dayStartUtc)
        {
            var dayEndUtc = dayStartUtc.AddDays(1);

            var dayEvents = events.Where(o => o.StartDate < dayEndUtc && o.EndDate >= dayStartUtc);

            return dayEvents;
        }

        private static IEnumerable<Schedule> GetAllDayEvents(Schedule[] events, DateTime dayStartUtc)
        {
            var dayEndUtc = dayStartUtc.AddDays(1);

            var result = new List<Schedule>();
            result.AddRange(events.Where(o => o.IsAllDay
                && o.StartDate < dayEndUtc
                && o.EndDate >= dayStartUtc));

            var nonMarked = events.Where(o => !o.IsAllDay && ((dayEndUtc < o.EndDate ? dayEndUtc : o.EndDate) - (dayStartUtc > o.StartDate ? dayStartUtc : o.StartDate)).TotalHours >= 7);

            result.AddRange(nonMarked);

            return result;
        }

        private static IEnumerable<Schedule> GetTimePartEvents(IEnumerable<Schedule> events, DateTime startUtc, DateTime endUtc, IEnumerable<Guid> allDayIds)
        {
            var timePartEvents = events.Where(o => !o.IsAllDay && o.StartDate < endUtc && o.EndDate > startUtc && !allDayIds.Contains(o.Id));

            return timePartEvents;
        }

        private ScheduleCell BuildCell(IEnumerable<Schedule> schedules, DateTime startUtc)
        {
            var cell = new ScheduleCell { Ticks = startUtc.Ticks };

            if (_model.ViewType == ScheduleViewType.Month)
            {
                cell.Day = startUtc.ToString("d MMM");
                cell.Date = startUtc.Date.ToString(_dateFormat.ShortDatePattern);
            }

            var values = new List<ScheduleDisplay>();

            foreach (var schedule in schedules)
            {
                var display = new ScheduleDisplay 
                { 
                    Id = schedule.Id, 
                    Title = schedule.Name, 
                    Color = schedule.Color ?? "White", 
                    Time = ""
                };

                if (!schedule.IsAllDay)
                {
                    if (startUtc.Date == startUtc.Date || startUtc.Date == startUtc.Date.AddDays(1))
                    {
                        display.Time = $"{startUtc.ToString(_dateFormat.ShortTimePattern)} - {startUtc.ToString(_dateFormat.ShortTimePattern)}";
                    }
                    else
                    {
                        display.Time = startUtc.Date == schedule.StartDate.Date
                            ? $"{startUtc.ToString(_dateFormat.ShortTimePattern)} - ..."
                            : $"... - {startUtc.ToString(_dateFormat.ShortTimePattern)}";
                    }
                }

                values.Add(display);
            }

            cell.Events = values.ToArray();

            return cell;
        }

        /// <summary>
        /// Возвращает день равный началу недели.
        /// </summary>
        /// <param name="date">Дата.</param>
        /// <param name="startDay">Начало недели.</param>
        /// <returns>День равный началу недели</returns>
        private static DateTime StartOfWeek(DateTime date, DayOfWeek startDay)
        {
            return date.AddDays(-(date.DayOfWeek - startDay));
        }

        /// <summary>
        /// Возвращает кол-во дней.
        /// </summary>
        /// <returns>Кол-во дней.</returns>
        private int GetDaysCount()
        {
            return _model.ViewType == ScheduleViewType.Day ? 1 : 7;
        }
    }
}
