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
    partial class CalendarEvaluator
    {
        private class CalendarYear
        {
            public int Year { get; init; }

            private readonly List<ChurchEventInstance> eventInstances = new List<ChurchEventInstance>();

            private readonly int[] fixedEventsByDay;

            private readonly int[] movableEventsByDay;

            private readonly int[] eventCountsByDay;

            private readonly int[][] eventSources;

            private readonly List<ChurchSeasonInstance> seasonInstances = new List<ChurchSeasonInstance>();

            private readonly ChurchSeasonInstance[] seasonsByDay;

            private readonly List<DateTime> dateInstances = new List<DateTime>();

            private readonly List<ChurchRuleCriteriaInstance> criteriaInstances = new List<ChurchRuleCriteriaInstance>();

            public CalendarYear(int year, ChurchCalendar churchCalendar, ChurchCalendarSystem calendarSystem)
            {
                Year = year;
                var firstOfYear = new DateTime(year, 1, 1);
                int daysInYear = firstOfYear.AddYears(1).Subtract(firstOfYear).Days;
                eventCountsByDay = new int[daysInYear + 1];
                eventSources = new int[2][];
                eventSources[0] = movableEventsByDay = new int[daysInYear + 1];
                eventSources[1] = fixedEventsByDay = new int[daysInYear + 1];

                seasonsByDay = new ChurchSeasonInstance[daysInYear + 1];

                AddEventInstances(calendarSystem, churchCalendar);
                AddSeasonInstances(calendarSystem, churchCalendar);
            }

            private void AddEventInstances(ChurchCalendarSystem calendarSystem, ChurchCalendar churchCalendar)
            {
                foreach (var churchEvent in churchCalendar.Events.AsEnumerable().Reverse())
                {
                    foreach (var date in churchEvent.Dates)
                    {
                        var target = date.IsMovable ? movableEventsByDay : fixedEventsByDay;

                        for (int basisYear = Year - 1; basisYear <= Year + 1; basisYear++)
                        {
                            for (var instanceDate = date.GetInstance(calendarSystem, basisYear);
                                    instanceDate.HasValue;
                                    instanceDate = date.GetInstance(calendarSystem, basisYear, instanceDate))
                            {
                                if (instanceDate?.Year == Year)
                                {
                                    var dayOfYear = instanceDate.Value.DayOfYear;
                                    var newIndex = eventInstances.Count;

                                    ChurchEventInstance eventInstance = new ChurchEventInstance
                                    {
                                        nextEventInstance = target[dayOfYear],
                                        churchEvent = churchEvent,
                                    };

                                    AddCriteriaInstances(calendarSystem, churchEvent.RuleCriteria, basisYear,
                                                            instanceDate.Value, instanceDate.Value,
                                                            out eventInstance.ruleCriteriaIndex,
                                                            out eventInstance.ruleCriteriaCount);

                                    eventInstances.Add(eventInstance);
                                    target[dayOfYear] = newIndex;
                                    ++eventCountsByDay[dayOfYear];
                                }
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
                        var instanceEndDate = season.EndDate.GetInstanceFollowing(season.StartDate, calendarSystem, basisYear);

                        if (instanceStartDate?.Year <= Year
                            && instanceEndDate?.Year >= Year
                            && instanceStartDate.Value <= instanceEndDate.Value)
                        {
                            ChurchSeasonInstance seasonInstance = new ChurchSeasonInstance
                            {
                                startDate = instanceStartDate.Value,
                                endDate = instanceEndDate.Value,
                                season = season,
                            };

                            AddCriteriaInstances(calendarSystem, season.RuleCriteria, basisYear,
                                                    seasonInstance.startDate, seasonInstance.endDate,
                                                    out seasonInstance.ruleCriteriaIndex,
                                                    out seasonInstance.ruleCriteriaCount);

                            seasonInstances.Add(seasonInstance);
                        }
                    }

                    foreach (var seasonInstance in seasonInstances
                                                    .OrderByDescending(s => s.season.IsDefault)
                                                    .ThenByDescending(s => s.DaysInSeason))
                    {
                        var minDayOfYear = seasonInstance.startDate.Year < Year
                                            ? 1
                                            : seasonInstance.startDate.DayOfYear;
                        var maxDayOfYear = seasonInstance.endDate.Year > Year
                                            ? seasonsByDay.Length
                                            : seasonInstance.endDate.DayOfYear + 1;

                        Array.Fill(seasonsByDay, seasonInstance, minDayOfYear, maxDayOfYear - minDayOfYear);
                    }
                }
            }

            private void AddCriteriaInstances(ChurchCalendarSystem calendarSystem,
                                                IDictionary<string, ChurchRuleCriteria[]> criteriaGroups,
                                                int basisYear,
                                                DateTime startDate,
                                                DateTime endDate,
                                                out int startIndex,
                                                out int count)
            {
                startIndex = criteriaInstances.Count;
                foreach (var criteriaGroup in criteriaGroups)
                {
                    foreach (var criteria in criteriaGroup.Value)
                    {
                        var criteriaInstance = new ChurchRuleCriteriaInstance
                        {
                            ruleGroupKey = criteriaGroup.Key,
                            criteria = criteria,
                        };

                        criteriaInstance.startDate = criteria.StartDate?.GetInstance(calendarSystem, basisYear);
                        criteriaInstance.endDate = criteria.EndDate?.GetInstance(calendarSystem, basisYear);

                        AddDateInstances(calendarSystem, criteria.IncludeDates, basisYear, startDate, endDate,
                            out criteriaInstance.includeDatesIndex, out criteriaInstance.includeDatesCount);

                        AddDateInstances(calendarSystem, criteria.ExcludeDates, basisYear, startDate, endDate,
                            out criteriaInstance.excludeDatesIndex, out criteriaInstance.excludeDatesCount);
                    }
                }
                if (criteriaInstances.Count == startIndex)
                    startIndex = count = 0;
                else
                    count = criteriaInstances.Count - startIndex;
            }

            private void AddDateInstances(ChurchCalendarSystem calendarSystem,
                                            IReadOnlyList<ChurchDate> dates,
                                            int basisYear,
                                            DateTime startDate,
                                            DateTime endDate,
                                            out int startIndex,
                                            out int count)
            {
                startIndex = dateInstances.Count;

                foreach (var date in dates)
                {
                    for (var dateInstance = date.GetInstance(calendarSystem, basisYear);
                            dateInstance.HasValue;
                            dateInstance = date.GetInstance(calendarSystem, basisYear, dateInstance))
                    {
                        if (dateInstance >= startDate && dateInstance <= endDate)
                            dateInstances.Add(dateInstance.Value);
                    }
                }

                if (dateInstances.Count > startIndex)
                    count = dateInstances.Count - startIndex;
                else
                    startIndex = count = 0;
            }

            public ChurchEvent[] GetEvents(DateTime date)
            {
                if (date.Year != Year)
                    throw new ArgumentOutOfRangeException(nameof(date));

                var result = new ChurchEvent[eventCountsByDay[date.DayOfYear]];
                int resultIndex = 0;

                foreach (var source in eventSources)
                {
                    int instanceIndex = source[date.DayOfYear];
                    while (instanceIndex != 0)
                    {
                        var instance = eventInstances[instanceIndex];
                        result[resultIndex++] = instance.churchEvent;
                        instanceIndex = instance.nextEventInstance;
                    }
                }

                return result;
            }

            public ChurchSeason GetSeason(DateTime date)
            {
                return date.Year == Year
                        ? seasonsByDay[date.DayOfYear].season
                        : throw new ArgumentOutOfRangeException(nameof(date));
            }

            private struct ChurchRuleCriteriaInstance
            {
                public string ruleGroupKey;

                public DateTime? startDate;

                public DateTime? endDate;

                public int includeDatesIndex;

                public int includeDatesCount;

                public int excludeDatesIndex;

                public int excludeDatesCount;

                public ChurchRuleCriteria criteria;
            }

            private struct ChurchEventInstance
            {
                public int nextEventInstance;

                public int ruleCriteriaIndex;

                public int ruleCriteriaCount;

                public ChurchEvent churchEvent;
            }

            private struct ChurchSeasonInstance
            {
                public DateTime startDate;

                public DateTime endDate;

                public int DaysInSeason => endDate.Subtract(startDate).Days + 1;

                public int ruleCriteriaIndex;

                public int ruleCriteriaCount;

                public ChurchSeason season;
            }
        }
    }
}
