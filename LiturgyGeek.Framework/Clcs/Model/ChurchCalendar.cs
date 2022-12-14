using LiturgyGeek.Framework.Clcs.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Clcs.Model
{
    public class ChurchCalendar<TRule, TRuleGroup, TCustomFlagBehavior, TRuleCriteria, TEventRank, TSeason, TEvent>
        where TRule : ChurchRule
        where TRuleGroup : ChurchRuleGroup<TRule>
        where TCustomFlagBehavior : CustomFlagBehavior
        where TRuleCriteria : ChurchRuleCriteria
        where TEventRank : ChurchEventRank
        where TSeason : ChurchSeason<TRuleCriteria>
        where TEvent : ChurchEvent<TRuleCriteria>
    {
        public string Name { get; set; }

        public string TraditionKey { get; set; }

        public string CalendarKey { get; set; }

        public CalendarReckoning SolarReckoning { get; set; }

        public CalendarReckoning PaschalReckoning { get; set; }

        public Dictionary<string, TRuleGroup> RuleGroups { get; set; } = new Dictionary<string, TRuleGroup>();

        public Dictionary<string, TEventRank> EventRanks { get; set; } = new Dictionary<string, TEventRank>();

        public string DefaultEventRank { get; set; }

        public Dictionary<string, TCustomFlagBehavior> CustomFlagBehaviors { get; set; }
            = new Dictionary<string, TCustomFlagBehavior>();

        public Dictionary<string, TSeason> Seasons { get; set; } = new Dictionary<string, TSeason>();

        public List<TEvent> Events { get; set; } = new List<TEvent>();

        public ChurchCalendar(string name, string traditionKey, string calendarKey, string defaultEventRank)
        {
            Name = name;
            TraditionKey = traditionKey;
            CalendarKey = calendarKey;
            DefaultEventRank = defaultEventRank;
        }
    }

    public class ChurchCalendar : ChurchCalendar<ChurchRule, ChurchRuleGroup, CustomFlagBehavior, ChurchRuleCriteria, ChurchEventRank, ChurchSeason, ChurchEvent>
    {
        [JsonConstructor]
        public ChurchCalendar(string name, string traditionKey, string calendarKey, string defaultEventRank)
            : base(name, traditionKey, calendarKey, defaultEventRank)
        {
        }
    }
}
