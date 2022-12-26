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

            private readonly List<ChurchEventInstance> eventInstances = new List<ChurchEventInstance>
            {
                default(ChurchEventInstance) // placeholder for index 0
            };

            private readonly int[] fixedEventsByDay;

            private readonly int[] movableEventsByDay;

            private readonly int[] eventCountsByDay;

            private readonly int[][] eventSources;

            private readonly List<ChurchSeasonInstance> seasonInstances = new List<ChurchSeasonInstance>
            {
                default(ChurchSeasonInstance), // placeholder for index 0
            };

            private readonly ChurchSeasonInstance[] seasonsByDay;

            private readonly List<DateTime> dateInstances = new List<DateTime>
            {
                default(DateTime), // placeholder for index 0
            };

            private readonly List<ChurchRuleCriteriaInstance> criteriaInstances = new List<ChurchRuleCriteriaInstance>
            {
                default(ChurchRuleCriteriaInstance), // placeholder for index 0
            };

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

                                    AddCriteriaInstances(calendarSystem, RuleCriteriaFlags.Event,
                                                            churchEvent.RuleCriteria, basisYear,
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

                            AddCriteriaInstances(calendarSystem, RuleCriteriaFlags.Season,
                                                    season.RuleCriteria, basisYear,
                                                    seasonInstance.startDate, seasonInstance.endDate,
                                                    out seasonInstance.ruleCriteriaIndex,
                                                    out seasonInstance.ruleCriteriaCount);

                            seasonInstances.Add(seasonInstance);
                        }
                    }

                    foreach (var seasonInstance in seasonInstances
                                                    .Skip(1)
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
                                                RuleCriteriaFlags flags,
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

                        if ((criteriaInstance.startDate ?? startDate) <= endDate
                            && (criteriaInstance.endDate ?? endDate) >= startDate)
                        {
                            AddDateInstances(calendarSystem, criteria.IncludeDates, basisYear, startDate, endDate,
                                out criteriaInstance.includeDatesIndex, out criteriaInstance.includeDatesCount);

                            if (criteria.IncludeDates.Count == 0 || criteriaInstance.includeDatesCount > 0)
                            {
                                AddDateInstances(calendarSystem, criteria.ExcludeDates, basisYear, startDate, endDate,
                                    out criteriaInstance.excludeDatesIndex, out criteriaInstance.excludeDatesCount);

                                criteriaInstance.flags = flags;
                                if (criteria.StartDate != null
                                        || criteria.EndDate != null
                                        || criteria.ExcludeDates.Count > 0)
                                {
                                    criteriaInstance.flags |= RuleCriteriaFlags.ExcludeDates;
                                }
                                if (criteria.IncludeDates.Count > 0)
                                    criteriaInstance.flags |= RuleCriteriaFlags.IncludeDates;
                                if (criteria.IncludeRanks.Count > 0)
                                    criteriaInstance.flags |= RuleCriteriaFlags.IncludeRanks;

                                criteriaInstances.Add(criteriaInstance);
                            }
                        }
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

            public ChurchEventInstance[] GetEventInstances(DateTime date)
            {
                if (date.Year != Year)
                    throw new ArgumentOutOfRangeException(nameof(date));

                var result = new ChurchEventInstance[eventCountsByDay[date.DayOfYear]];
                int resultIndex = 0;

                foreach (var source in eventSources)
                {
                    int instanceIndex = source[date.DayOfYear];
                    while (instanceIndex != 0)
                    {
                        var instance = eventInstances[instanceIndex];
                        result[resultIndex++] = instance;
                        instanceIndex = instance.nextEventInstance;
                    }
                }

                return result;
            }

            public ChurchSeasonInstance GetSeasonInstance(DateTime date)
            {
                return date.Year == Year
                        ? seasonsByDay[date.DayOfYear]
                        : throw new ArgumentOutOfRangeException(nameof(date));
            }

            public struct ChurchRuleCriteriaInstance
            {
                public string ruleGroupKey;

                public DateTime? startDate;

                public DateTime? endDate;

                public int includeDatesIndex;

                public int includeDatesCount;

                public int excludeDatesIndex;

                public int excludeDatesCount;

                public ChurchRuleCriteria criteria;

                public RuleCriteriaFlags flags;

                public bool MeetsCriteria(CalendarYear calendarYear, DateTime date, IEnumerable<ChurchEventInstance> events)
                {
                    if (startDate.HasValue && startDate > date)
                        return false;

                    if (endDate.HasValue && endDate < date)
                        return false;

                    if (includeDatesCount > 0
                            && !Enumerable.Range(includeDatesIndex, includeDatesCount)
                                            .Any(i => calendarYear.dateInstances[i] == date))
                    {
                        return false;
                    }

                    if (criteria.IncludeRanks.Count > 0
                            && !criteria.IncludeRanks
                                .Intersect(events.Select(e => e.churchEvent.EventRankKey))
                                .Any())
                    {
                        return false;
                    }

                    if (excludeDatesCount > 0
                            && Enumerable.Range(excludeDatesIndex, excludeDatesCount)
                                            .Any(i => calendarYear.dateInstances[i] == date))
                    {
                        return false;
                    }

                    return true;
                }
            }

            public struct ChurchEventInstance
            {
                public int nextEventInstance;

                public int ruleCriteriaIndex;

                public int ruleCriteriaCount;

                public ChurchEvent churchEvent;

                public IEnumerable<ChurchRuleCriteriaInstance> GetRules
                    (
                        CalendarYear calendarYear,
                        DateTime date,
                        IEnumerable<ChurchEventInstance> events
                    )
                {
                    return Enumerable.Range(ruleCriteriaIndex, ruleCriteriaCount)
                            .Select(i => calendarYear.criteriaInstances[i])
                            .Where(c => c.MeetsCriteria(calendarYear, date, events));
                }
            }

            public struct ChurchSeasonInstance
            {
                public DateTime startDate;

                public DateTime endDate;

                public int DaysInSeason => endDate.Subtract(startDate).Days + 1;

                public int ruleCriteriaIndex;

                public int ruleCriteriaCount;

                public ChurchSeason season;

                public IEnumerable<ChurchRuleCriteriaInstance> GetRules
                    (
                        CalendarYear calendarYear,
                        DateTime date,
                        IEnumerable<ChurchEventInstance> events
                    )
                {
                    return Enumerable.Range(ruleCriteriaIndex, ruleCriteriaCount)
                            .Select(i => calendarYear.criteriaInstances[i])
                            .Where(c => c.MeetsCriteria(calendarYear, date, events));
                }
            }
        }
    }
}
