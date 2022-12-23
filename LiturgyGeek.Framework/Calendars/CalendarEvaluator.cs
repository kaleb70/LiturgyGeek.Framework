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
    public class CalendarEvaluator
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

        private class CalendarYear
        {
            public int Year { get; init; }

            private readonly List<ChurchEventInstance> eventInstances = new List<ChurchEventInstance>();

            private readonly int[] fixedEvents;

            private readonly int[] movableEvents;

            private readonly int[] eventCounts;

            private readonly int[][] eventSources;

            private readonly List<ChurchSeasonInstance> seasonInstances = new List<ChurchSeasonInstance>();

            private readonly ChurchSeasonInstance[] seasons;

            public CalendarYear(int year, ChurchCalendar churchCalendar, ChurchCalendarSystem calendarSystem)
            {
                Year = year;
                var firstOfYear = new DateTime(year, 1, 1);
                int daysInYear = firstOfYear.AddYears(1).Subtract(firstOfYear).Days;
                eventCounts = new int[daysInYear + 1];
                eventSources = new int[2][];
                eventSources[0] = movableEvents = new int[daysInYear + 1];
                eventSources[1] = fixedEvents = new int[daysInYear + 1];

                seasons = new ChurchSeasonInstance[daysInYear + 1];

                AddEventInstances(calendarSystem, churchCalendar);
                AddSeasonInstances(calendarSystem, churchCalendar);
            }

            private void AddEventInstances(ChurchCalendarSystem calendarSystem, ChurchCalendar churchCalendar)
            {
                foreach (var churchEvent in churchCalendar.Events.AsEnumerable().Reverse())
                {
                    foreach (var date in churchEvent.Dates)
                    {
                        var target = (date is MovableDate) ? movableEvents : fixedEvents;

                        for (int basisYear = Year - 1; basisYear <= Year + 1; basisYear++)
                        {
                            var instanceDate = date.GetInstance(calendarSystem, basisYear);
                            if (instanceDate?.Year == Year)
                            {
                                var dayOfYear = instanceDate.Value.DayOfYear;
                                var newIndex = eventInstances.Count;
                                eventInstances.Add(new ChurchEventInstance
                                {
                                    NextIndex = target[dayOfYear],
                                    Event = churchEvent,
                                });
                                target[dayOfYear] = newIndex;
                                ++eventCounts[dayOfYear];
                            }
                        }
                    }
                }
            }

            private void AddSeasonInstances(ChurchCalendarSystem calendarSystem, ChurchCalendar churchCalendar)
            {
                foreach (var season in churchCalendar.Seasons.Values)
                {
                    for (int basisYear = Year - 1; basisYear <= Year + 1; basisYear++)
                    {
                        var instanceStartDate = season.StartDate.GetInstance(calendarSystem, basisYear);
                        var instanceEndDate = season.EndDate.GetInstance(calendarSystem, basisYear);

                        if (instanceStartDate?.Year <= Year
                            && instanceEndDate?.Year >= Year
                            && instanceStartDate.Value <= instanceEndDate.Value)
                        {
                            seasonInstances.Add(new ChurchSeasonInstance
                            {
                                StartDate = instanceStartDate.Value,
                                EndDate = instanceEndDate.Value,
                                Season = season,
                            });
                        }
                    }

                    foreach (var seasonInstance in seasonInstances
                                                    .OrderByDescending(s => s.Season.IsDefault)
                                                    .ThenByDescending(s => s.DaysInSeason))
                    {
                        var minDayOfYear = seasonInstance.StartDate.Year < Year
                                            ? 1
                                            : seasonInstance.StartDate.DayOfYear;
                        var maxDayOfYear = seasonInstance.EndDate.Year > Year
                                            ? seasons.Length
                                            : seasonInstance.EndDate.DayOfYear + 1;

                        Array.Fill(seasons, seasonInstance, minDayOfYear, maxDayOfYear - minDayOfYear);
                    }
                }
            }

            public ChurchEvent[] GetEvents(DateTime date)
            {
                if (date.Year != Year)
                    throw new ArgumentOutOfRangeException(nameof(date));

                var result = new ChurchEvent[eventCounts[date.DayOfYear]];
                int resultIndex = 0;

                foreach (var source in eventSources)
                {
                    int instanceIndex = source[date.DayOfYear];
                    while (instanceIndex != 0)
                    {
                        var instance = eventInstances[instanceIndex];
                        result[resultIndex++] = instance.Event;
                        instanceIndex = instance.NextIndex;
                    }
                }

                return result;
            }

            public ChurchSeason GetSeason(DateTime date)
            {
                return date.Year == Year
                        ? seasons[date.DayOfYear].Season
                        : throw new ArgumentOutOfRangeException(nameof(date));
            }

            private struct ChurchEventInstance
            {
                public int NextIndex { get; set; }

                public ChurchEvent Event { get; set; }
            }

            private struct ChurchSeasonInstance
            {
                public DateTime StartDate { get; set; }

                public DateTime EndDate { get; set; }

                public int DaysInSeason => EndDate.Subtract(StartDate).Days + 1;

                public ChurchSeason Season { get; set; }
            }
        }
    }
}
