using LiturgyGeek.Framework.Clcs.Enums;
using LiturgyGeek.Framework.Globalization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Calendars
{
    public class CalendarEvaluator
    {
        private readonly IChurchCalendarProvider calendarProvider;

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

        public ChurchCalendarSystem GetCalendarSystem(string churchCalendarCode)
            => GetCalendarSystem(calendarProvider.GetCalendar(churchCalendarCode));

        public CalendarDay[] Evaluate(string churchCalendarCode, DateTime minDate, DateTime maxDate)
        {
            minDate = minDate.Date;
            maxDate = maxDate.Date;
            var calendar = calendarProvider.GetCalendar(churchCalendarCode).CloneAndResolve(calendarProvider);
            var calendarSystem = GetCalendarSystem(calendar);
            var events = calendarSystem.ResolveAll
                            (
                                minDate,
                                maxDate,
                                calendar.Events.SelectMany(Event => Event.Dates.Select(Date => (Event, Date)))
                            )
                            .OrderBy(e => e.Date);
            var seasons = calendarSystem.ResolveAll
                            (
                                minDate,
                                maxDate,
                                calendar.Seasons.Select(s => (Season: s, s.StartDate, s.EndDate))
                            )
                            .OrderBy(s => (s.EndDate - s.StartDate).TotalDays);
            var defaultSeason = seasons.Single(s => s.Season.IsDefault).Season;
            return Enumerable.Range(0, (maxDate - minDate).Days)
                                .Select(i => minDate.AddDays(i))
                                .Select(d => new CalendarDay(d, seasons.FirstOrDefault(s => s.StartDate <= d && s.EndDate >= d).Season)
                                {
                                    Events = events.Where(e => e.Date == d).Select(e => e.Event).ToList(),
                                }).ToArray();
        }
    }
}
