using LiturgyGeek.Framework.Clcs.Dates;
using LiturgyGeek.Framework.Clcs.Enums;
using LiturgyGeek.Framework.Globalization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Calendars
{
    public partial class CalendarEvaluator
    {
        private readonly IChurchCalendarProvider calendarProvider;

        private readonly Dictionary<string, ChurchCalendar> calendars = new Dictionary<string, ChurchCalendar>();

        public CalendarEvaluator(IChurchCalendarProvider calendarProvider)
        {
            this.calendarProvider = calendarProvider;
        }

        public ChurchCalendarSystem GetCalendarSystem(CalendarReckoning solarReckoning, CalendarReckoning paschalReckoning)
        {
            Calendar fixedCalendar = solarReckoning switch
            {
                CalendarReckoning.Julian => new JulianCalendar(),
                CalendarReckoning.Gregorian => new GregorianCalendar(),
                CalendarReckoning.RevisedJulian => new RevisedJulianCalendar(),
                _ => throw new NotSupportedException($"{solarReckoning} not supported for solar calendar"),
            };
            PaschalCalendar paschalCalendar = paschalReckoning switch
            {
                CalendarReckoning.Julian => new JulianPaschalCalendar(),
                CalendarReckoning.Gregorian => new GregorianPaschalCalendar(),
                _ => throw new NotSupportedException($"{paschalReckoning} not supported for paschal calendar"),
            };
            return new ChurchCalendarSystem(fixedCalendar, paschalCalendar);
        }

        public ChurchCalendarSystem GetCalendarSystem(ChurchCalendar calendar)
            => GetCalendarSystem(calendar.SolarReckoning, calendar.PaschalReckoning);

        public ChurchCalendarSystem GetCalendarSystem(string calendarKey)
            => GetCalendarSystem(calendarProvider.GetCalendar(calendarKey));

        public ChurchCalendar GetCalendar(string calendarKey)
        {
            return calendars.TryGetValue(calendarKey, out var result)
                    ? result
                    : calendars[calendarKey] = calendarProvider.GetCalendar(calendarKey).CloneAndMerge(calendarProvider);
        }

        public CalendarDay[] Evaluate(string calendarKey, DateTime minDate, DateTime maxDate)
        {
            minDate = minDate.Date;
            maxDate = maxDate.Date;
            var churchCalendar = GetCalendar(calendarKey);
            var calendarSystem = GetCalendarSystem(churchCalendar);

            var calendarYear = new CalendarYear(minDate.Year, churchCalendar, calendarSystem);
            return Enumerable.Range(0, (maxDate - minDate).Days)
                                .Select(i => minDate.AddDays(i))
                                .Select(d =>
                                {
                                    if (calendarYear.Year != d.Year)
                                        calendarYear = new CalendarYear(d.Year, churchCalendar, calendarSystem);
                                    return d;
                                })
                                .Select(d => new CalendarDay(d, calendarYear.GetSeason(d))
                                {
                                    Events = calendarYear.GetEvents(d).ToList(),
                                })
                                .ToArray();
        }
    }
}
