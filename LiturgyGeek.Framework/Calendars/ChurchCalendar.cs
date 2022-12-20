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
    public class ChurchCalendar : Clcs.Model.ChurchCalendar<ChurchRule, ChurchRuleGroup, ChurchRuleCriteria, ChurchEventRank, ChurchSeason, ChurchEvent>, ICloneable<ChurchCalendar>
    {
        [JsonConstructor]
        public ChurchCalendar(string name, string traditionKey)
            : base(name, traditionKey)
        {
        }

        public ChurchCalendar(string name, string traditionKey, CalendarReckoning solarReckoning, CalendarReckoning paschalReckoning)
            :  base(name, traditionKey)
        {
            SolarReckoning = solarReckoning;
            PaschalReckoning = paschalReckoning;
        }

        public ChurchCalendar(ChurchCalendar other)
            : base(other.Name, other.TraditionKey)
        {
            SolarReckoning = other.SolarReckoning;
            PaschalReckoning = other.PaschalReckoning;
            RuleGroups = other.RuleGroups.Clone();
            EventRanks = other.EventRanks.Clone();
            Seasons = other.Seasons.Clone();
            Events = other.Events.Clone();
        }

        public ChurchCalendar Clone() => new ChurchCalendar(this);

        object ICloneable.Clone() => Clone();

        public ChurchCalendar CloneAndResolve(IChurchCalendarProvider provider)
        {
            var result = Clone();

            foreach (var churchEvent in result.Events)
                churchEvent.Resolve(this, provider);

            return result;
        }
    }
}
