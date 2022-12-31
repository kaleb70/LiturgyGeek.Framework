using LiturgyGeek.Framework.Clcs.Enums;
using LiturgyGeek.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ChurchRuleCriteria = LiturgyGeek.Framework.Clcs.Model.ChurchRuleCriteria;

namespace LiturgyGeek.Framework.Calendars
{
    public class ChurchCalendar : Clcs.Model.ChurchCalendar<ChurchRule, ChurchRuleGroup, CustomFlagBehavior, ChurchRuleCriteria, ChurchEventRank, ChurchSeason, ChurchEvent>, ICloneable<ChurchCalendar>
    {
        [JsonConstructor]
        public ChurchCalendar(string name, string traditionKey, string defaultEventRank)
            : base(name, traditionKey, defaultEventRank)
        {
        }

        public ChurchCalendar(string name, string traditionKey, string defaultEventRank, CalendarReckoning solarReckoning, CalendarReckoning paschalReckoning)
            :  base(name, traditionKey, defaultEventRank)
        {
            SolarReckoning = solarReckoning;
            PaschalReckoning = paschalReckoning;
        }

        public ChurchCalendar(ChurchCalendar other)
            : base(other.Name, other.TraditionKey, other.DefaultEventRank)
        {
            SolarReckoning = other.SolarReckoning;
            PaschalReckoning = other.PaschalReckoning;
            RuleGroups = other.RuleGroups.Clone();
            EventRanks = other.EventRanks.Clone();
            CustomFlagBehaviors = other.CustomFlagBehaviors.Clone();
            Seasons = other.Seasons.Clone();
            Events = other.Events.Clone();
        }

        public ChurchCalendar Clone() => new ChurchCalendar(this);

        object ICloneable.Clone() => Clone();

        public ChurchCalendar CloneAndMerge(IChurchCalendarProvider provider)
        {
            var result = Clone();

            foreach (var churchEvent in result.Events)
                churchEvent.Merge(result, provider);

            return result;
        }
    }
}
