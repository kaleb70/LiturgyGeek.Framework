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

        public CalendarDayResult[] Evaluate(string calendarKey, DateTime minDate, DateTime maxDate)
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
                                    return EvaluateDay(churchCalendar, calendarYear, d);
                                })
                                .ToArray();
        }

        private static CalendarDayResult EvaluateDay(ChurchCalendar churchCalendar, CalendarYear calendarYear, DateTime d)
        {
            var seasonInstance = calendarYear.GetSeasonInstance(d);
            CalendarYear.ChurchEventInstance[] eventInstances = calendarYear.GetEventInstances(d);
            var ruleInstances = seasonInstance.GetRules(calendarYear, d, eventInstances)
                                .Concat(eventInstances.SelectMany(e => e.GetRules(calendarYear, d, eventInstances)))
                .ToArray();

            var rules = ruleInstances.GroupBy(r => r.ruleGroupKey)
                            .Select(rg => rg.OrderByDescending(c => c.flags).First())
                            .Select(r =>
                            {
                                var ruleGroup = churchCalendar.RuleGroups[r.ruleGroupKey];
                                return new ChurchRuleResult
                                {
                                    RuleGroup = new KeyValuePair<string, ChurchRuleGroup>(r.ruleGroupKey, ruleGroup),
                                    Rule = new KeyValuePair<string, ChurchRule>
                                    (
                                        r.criteria.RuleKey,
                                        ruleGroup.Rules[r.criteria.RuleKey]
                                    ),
                                    Show = calendarYear.GetRuleVisibility(r.ruleGroupKey, r.criteria.RuleKey)
                                                ?.MeetsCriteria(calendarYear, d, eventInstances)
                                            ?? true,
                                };
                            })
                            .ToArray();

            var events = eventInstances.Select(e => new ChurchEventResult(e.churchEvent, e.transferredFrom)).ToArray();

            var result = new CalendarDayResult(d, seasonInstance.season, rules, events);

            return result;
        }

        [Flags]
        public enum RuleCriteriaFlags
        {
            None = 0,

            ExcludeDates        = 0b0000_0001,
            ExcludeCustomFlags  = 0b0000_0010,
            IncludeDates        = 0b0000_0100,
            IncludeRanks        = 0b0000_1000,
            IncludeCustomFlags  = 0b0001_0000,
            Season              = 0b0010_0000,
            Event               = 0b0100_0000,
        }

    }
}
