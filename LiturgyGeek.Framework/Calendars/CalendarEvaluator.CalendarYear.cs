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

            private readonly ChurchEventInstance[] fixedEventsByDay;

            private readonly ChurchEventInstance[] movableEventsByDay;

            private readonly int[] eventCountsByDay;

            private readonly ChurchEventInstance[][] eventSources;

            private readonly List<ChurchSeasonInstance> seasonInstances = new List<ChurchSeasonInstance>
            {
                default(ChurchSeasonInstance), // placeholder for index 0
            };

            private readonly int[] seasonsByDay;

            private readonly List<DateTime> dateInstances = new List<DateTime>
            {
                default(DateTime), // placeholder for index 0
            };

            private readonly List<ChurchRuleCriteriaInstance> criteriaInstances = new List<ChurchRuleCriteriaInstance>
            {
                default(ChurchRuleCriteriaInstance), // placeholder for index 0
            };

            private readonly Dictionary<RuleKey, int> ruleVisibilityInstances = new Dictionary<RuleKey, int>();

            public CalendarYear(int year, ChurchCalendar churchCalendar, ChurchCalendarSystem calendarSystem)
            {
                Year = year;
                var firstOfYear = new DateTime(year, 1, 1);
                int daysInYear = firstOfYear.AddYears(1).Subtract(firstOfYear).Days;
                eventCountsByDay = new int[daysInYear + 1];
                eventSources = new ChurchEventInstance[2][];
                eventSources[0] = movableEventsByDay = new ChurchEventInstance[daysInYear + 1];
                eventSources[1] = fixedEventsByDay = new ChurchEventInstance[daysInYear + 1];

                seasonsByDay = new int[daysInYear + 1];

                AddRuleVisibilityInstances(calendarSystem, churchCalendar);
                AddEventInstances(calendarSystem, churchCalendar);
                AddSeasonInstances(calendarSystem, churchCalendar);
            }

            private void AddRuleVisibilityInstances(ChurchCalendarSystem calendarSystem, ChurchCalendar churchCalendar)
            {
                foreach (var group in churchCalendar.RuleGroups)
                {
                    foreach (var rule in group.Value.Rules.Where(r => r.Value._VisibilityCriteria != null))
                    {
                        ruleVisibilityInstances[new RuleKey(group.Key, rule.Key)] =
                            AddCriteriaInstance(calendarSystem, Year, rule.Value._VisibilityCriteria!);
                    }
                }
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

                                    ChurchEventInstance eventInstance = new ChurchEventInstance(churchEvent)
                                    {
                                        nextEventInstance = target[dayOfYear],
                                    };

                                    AddCriteriaInstances(calendarSystem, RuleCriteriaFlags.Event,
                                                            churchEvent.RuleCriteria, basisYear,
                                                            instanceDate.Value, instanceDate.Value,
                                                            out eventInstance.ruleCriteriaIndex,
                                                            out eventInstance.ruleCriteriaCount);

                                    target[dayOfYear] = eventInstance;
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
                                                    .Select((s, i) => new { s, i })
                                                    .Skip(1)
                                                    .OrderByDescending(s => s.s.season.IsDefault)
                                                    .ThenByDescending(s => s.s.DaysInSeason))
                    {
                        var minDayOfYear = seasonInstance.s.startDate.Year < Year
                                            ? 1
                                            : seasonInstance.s.startDate.DayOfYear;
                        var maxDayOfYear = seasonInstance.s.endDate.Year > Year
                                            ? seasonsByDay.Length
                                            : seasonInstance.s.endDate.DayOfYear + 1;

                        Array.Fill(seasonsByDay, seasonInstance.i, minDayOfYear, maxDayOfYear - minDayOfYear);
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
                        AddCriteriaInstance(calendarSystem, flags, basisYear, startDate, endDate,
                                            criteriaGroup.Key, criteria);
                    }
                }
                if (criteriaInstances.Count == startIndex)
                    startIndex = count = 0;
                else
                    count = criteriaInstances.Count - startIndex;
            }

            private int AddCriteriaInstance(ChurchCalendarSystem calendarSystem,
                                                int basisYear,
                                                GeneralCriteria criteria)
                => AddCriteriaInstance(calendarSystem, 0, basisYear,
                                        new DateTime(basisYear, 1, 1), new DateTime(basisYear + 1, 1, 1).AddDays(-1),
                                        "", criteria);

            private int AddCriteriaInstance(ChurchCalendarSystem calendarSystem,
                                                RuleCriteriaFlags flags,
                                                int basisYear,
                                                DateTime startDate,
                                                DateTime endDate,
                                                string ruleGroupKey,
                                                ChurchRuleCriteria criteria)
            {
                bool isGeneral = criteria is GeneralCriteria;

                var criteriaInstance = new ChurchRuleCriteriaInstance
                {
                    ruleGroupKey = ruleGroupKey,
                    criteria = criteria,
                };

                criteriaInstance.startDate = criteria.StartDate?.GetInstance(calendarSystem, basisYear);
                criteriaInstance.endDate = criteria.EndDate?.GetInstance(calendarSystem, basisYear);

                if (isGeneral ||
                    ((criteriaInstance.startDate ?? startDate) <= endDate
                        && (criteriaInstance.endDate ?? endDate) >= startDate))
                {
                    AddDateInstances(calendarSystem, criteria.IncludeDates, basisYear, startDate, endDate,
                        out criteriaInstance.includeDatesIndex, out criteriaInstance.includeDatesCount);
                    if (isGeneral)
                    {
                        AddDateInstances(calendarSystem, criteria.IncludeDates, basisYear - 1, startDate, endDate,
                            out _, out int spillForwardCount);
                        AddDateInstances(calendarSystem, criteria.IncludeDates, basisYear + 1, startDate, endDate,
                            out _, out int spillBackwardCount);
                        criteriaInstance.includeDatesCount += spillForwardCount + spillBackwardCount;
                    }

                    if (isGeneral || criteria.IncludeDates.Count == 0 || criteriaInstance.includeDatesCount > 0)
                    {
                        AddDateInstances(calendarSystem, criteria.ExcludeDates, basisYear, startDate, endDate,
                            out criteriaInstance.excludeDatesIndex, out criteriaInstance.excludeDatesCount);
                        if (isGeneral)
                        {
                            AddDateInstances(calendarSystem, criteria.ExcludeDates, basisYear - 1, startDate, endDate,
                                out _, out int spillForwardCount);
                            AddDateInstances(calendarSystem, criteria.ExcludeDates, basisYear + 1, startDate, endDate,
                                out _, out int spillBackwardCount);
                            criteriaInstance.excludeDatesCount += spillForwardCount + spillBackwardCount;
                        }

                        criteriaInstance.flags = flags;

                        if (criteria.StartDate != null
                                || criteria.EndDate != null
                                || criteria.ExcludeDates.Count > 0)
                        {
                            criteriaInstance.flags |= RuleCriteriaFlags.ExcludeDates;
                        }
                        if (criteria.ExcludeCustomFlags.Count > 0)
                            criteriaInstance.flags |= RuleCriteriaFlags.ExcludeCustomFlags;

                        if (criteria.IncludeDates.Count > 0)
                            criteriaInstance.flags |= RuleCriteriaFlags.IncludeDates;
                        if (criteria.IncludeRanks.Count > 0)
                            criteriaInstance.flags |= RuleCriteriaFlags.IncludeRanks;
                        if (criteria.IncludeCustomFlags.Count > 0)
                            criteriaInstance.flags |= RuleCriteriaFlags.IncludeCustomFlags;

                        int result = criteriaInstances.Count;
                        criteriaInstances.Add(criteriaInstance);
                        return result;
                    }
                }
                return 0;
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

            public ChurchRuleCriteriaInstance? GetRuleVisibility(string group, string rule)
            {
                return ruleVisibilityInstances.TryGetValue(new RuleKey(group, rule), out int index)
                        ? criteriaInstances[index]
                        : null;
            }

            public ChurchEventInstance[] GetEventInstances(DateTime date)
            {
                if (date.Year != Year)
                    throw new ArgumentOutOfRangeException(nameof(date));

                var result = new ChurchEventInstance[eventCountsByDay[date.DayOfYear]];
                int resultIndex = 0;

                foreach (var source in eventSources)
                {
                    for (var eventInstance = source[date.DayOfYear];
                            eventInstance != null;
                            eventInstance = eventInstance.nextEventInstance)
                    {
                        result[resultIndex++] = eventInstance;
                    }
                }

                return result;
            }

            public ChurchSeasonInstance GetSeasonInstance(DateTime date)
            {
                return date.Year == Year
                        ? seasonInstances[seasonsByDay[date.DayOfYear]]
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

                    if (criteria.IncludeCustomFlags.Count > 0
                            && !criteria.IncludeCustomFlags
                                .Intersect(events.SelectMany(e => e.churchEvent.CustomFlags))
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
                    if (criteria.ExcludeCustomFlags.Count > 0
                            && criteria.ExcludeCustomFlags
                                .Intersect(events.SelectMany(e => e.churchEvent.CustomFlags))
                                .Any())
                    {
                        return false;
                    }

                    return true;
                }
            }

            public class ChurchEventInstance
            {
                public ChurchEventInstance? nextEventInstance;

                public int ruleCriteriaIndex;

                public int ruleCriteriaCount;

                public ChurchEvent churchEvent;

                public ChurchEventInstance(ChurchEvent churchEvent)
                {
                    this.churchEvent = churchEvent;
                }

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

            public struct RuleKey
            {
                public string Group { get; private init; }

                public string Rule { get; private init; }

                public RuleKey(string group, string rule)
                {
                    Group = group;
                    Rule = rule;
                }
            }
        }
    }
}
